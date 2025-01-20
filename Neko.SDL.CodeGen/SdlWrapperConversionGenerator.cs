using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neko.Sdl.CodeGen;

[Generator]
public class SdlWrapperConversionGenerator : ISourceGenerator {
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
                var namespaceName = GetNamespace(classDecl);
                var className = classDecl.Identifier.Text;
                var templateName = classDecl.BaseList.Types
                    .First(syntax => syntax.ToString().StartsWith("SdlWrapper<"))
                    .ToString().Split('<')[1].Replace(">", "");
                var sourceBuilder = new StringBuilder();

                sourceBuilder.AppendLine($"namespace {namespaceName};");
                sourceBuilder.AppendLine();
                sourceBuilder.AppendLine($"public unsafe partial class {className} {{");

                //sourceBuilder.AppendLine($"    public {className}() : base() {{ }}");
                sourceBuilder.AppendLine($"    public {className}({templateName}* id) : base(id) {{ }}");
                sourceBuilder.AppendLine($"    public {className}(ref {templateName} id) : base(ref id) {{ }}");

                sourceBuilder.AppendLine($"    public static implicit operator {className}({templateName}* o) => new(o);");
                sourceBuilder.AppendLine("}");

                context.AddSource(
                    $"{className}.g.cs",
                    SourceText.From(sourceBuilder.ToString(), Encoding.UTF8)
                );
            }
        }
    }

    private string GetNamespace(ClassDeclarationSyntax classDecl) {
        if (classDecl.Parent is NamespaceDeclarationSyntax namespaceDecl)
            return namespaceDecl.Name.ToString();

        if (classDecl.Parent is FileScopedNamespaceDeclarationSyntax fileScopedNamespace)
            return fileScopedNamespace.Name.ToString();

        return string.Empty;
    }
}