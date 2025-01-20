using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neko.Sdl.CodeGen;

[Generator]
public class AccessorGenerator : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context) {
        var syntaxTrees = context.Compilation.SyntaxTrees;
        
        foreach (var syntaxTree in syntaxTrees) {
            var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();
            
            var classes = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(c => c.BaseList?.Types.Any(t => 
                    t.ToString().StartsWith("SdlWrapper<")) ?? false);

            foreach (var classDecl in classes) {
                var templateName = classDecl.BaseList.Types
                    .First(syntax => syntax.ToString().StartsWith("SdlWrapper<"))
                    .ToString().Split('<')[1].Replace(">", "");
                var baseStruct = context.Compilation.GetTypeByMetadataName("SDL."+templateName);
                var properties = classDecl.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(p => p.AttributeLists
                        .SelectMany(al => al.Attributes)
                        .Any(a => a.Name.ToString() == "GenAccessor"));

                if (!properties.Any()) continue;

                var namespaceName = GetNamespace(classDecl);
                var className = classDecl.Identifier.Text;
                var sourceBuilder = new StringBuilder();

                sourceBuilder.AppendLine($"namespace {namespaceName};");
                sourceBuilder.AppendLine();
                sourceBuilder.AppendLine($"public unsafe partial class {className} {{");

                foreach (var property in properties) {
                    var attribute = property.AttributeLists
                        .SelectMany(al => al.Attributes)
                        .First(a => a.Name.ToString() == "GenAccessor");

                    var fieldName = attribute.ArgumentList?.Arguments.First().ToString()
                        .Trim('"');
                    var cast = false;
                    if (attribute.ArgumentList?.Arguments.Count > 1)
                        cast = attribute.ArgumentList?.Arguments[1].ToString() == "true";

                    var propertyName = property.Identifier.Text;
                    var propertyType = property.Type.ToString();
                    string? basepropertyType = null;
                    if (cast)
                        basepropertyType = baseStruct.GetMembers().OfType<IFieldSymbol>().First(symbol => symbol.Name == fieldName).Type.ToString();
                    var propertyModifiers = property.Modifiers.Select(token => token.ToString());

                    sourceBuilder.AppendLine($"    {string.Join(" ",propertyModifiers)} {propertyType} {propertyName} {{");
                    sourceBuilder.AppendLine($"        get => {(cast ? $"({propertyType})" : "")}Handle->{fieldName};");
                    sourceBuilder.AppendLine($"        set => Handle->{fieldName} = {(cast ? $"({basepropertyType})" : "")}value;");
                    sourceBuilder.AppendLine("    }");
                    sourceBuilder.AppendLine();
                }

                sourceBuilder.AppendLine("}");

                context.AddSource(
                    $"{className}.g.cs",
                    SourceText.From(sourceBuilder.ToString(), Encoding.UTF8)
                );
            }
        }
    }

    private string GetNamespace(ClassDeclarationSyntax classDecl) {
        var namespaceDecl = classDecl.Parent as NamespaceDeclarationSyntax;
        if (namespaceDecl != null)
            return namespaceDecl.Name.ToString();

        var fileScopedNamespace = classDecl.Parent as FileScopedNamespaceDeclarationSyntax;
        if (fileScopedNamespace != null)
            return fileScopedNamespace.Name.ToString();

        return string.Empty;
    }
}