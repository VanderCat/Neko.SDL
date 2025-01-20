using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Neko.Sdl.CodeGen;

[Generator]
public class EnumGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new EnumSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (!(context.SyntaxContextReceiver is EnumSyntaxReceiver receiver))
            return;

        foreach (var enumDeclaration in receiver.EnumsToAugment)
        {
            ProcessEnum(context, enumDeclaration);
        }
    }

    private void ProcessEnum(GeneratorExecutionContext context, EnumDeclarationSyntax enumDeclaration)
    {
        var semanticModel = context.Compilation.GetSemanticModel(enumDeclaration.SyntaxTree);
        var enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration) as INamedTypeSymbol;

        if (enumSymbol == null)
            return;

        var attributes = enumSymbol.GetAttributes();
        var genEnumAttribute = attributes.FirstOrDefault(a => a.AttributeClass?.Name == "GenEnumAttribute");
        var flagsEnumAttribute = attributes.FirstOrDefault(a => a.AttributeClass?.Name == "FlagsAttribute");

        if (genEnumAttribute == null || genEnumAttribute.ConstructorArguments.Length != 2)
            return;

        var sourceEnumName = genEnumAttribute.ConstructorArguments[0].Value as string;
        var prefix = genEnumAttribute.ConstructorArguments[1].Value as string;

        if (string.IsNullOrEmpty(sourceEnumName) || string.IsNullOrEmpty(prefix))
            return;

        // Find the source enum in referenced assemblies
        var sourceEnum = context.Compilation.GetTypeByMetadataName($"SDL.{sourceEnumName}");
        if (sourceEnum == null)
            return;

        var sourceMembers = sourceEnum.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => f.ConstantValue != null);

        var sb = new StringBuilder();
        var namespaceName = enumSymbol.ContainingNamespace.ToDisplayString();

        sb.AppendLine($@"namespace {namespaceName}
{{
    {(flagsEnumAttribute is not null ? "[Flags]" : "")}
    public enum {enumSymbol.Name}{(sourceEnum.EnumUnderlyingType is not null? (" : " + sourceEnum.EnumUnderlyingType):"")}
    {{");

        foreach (var member in sourceMembers)
        {
            var memberName = member.Name;
            if (memberName.StartsWith(prefix))
                memberName = memberName.Substring(prefix.Length);

            memberName = string.Join("", memberName.Split('_')
                .Select(part => char.ToUpper(part[0]) + part.Substring(1).ToLower()));
            if (new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }.Contains(memberName[0]))
                memberName = "_" + memberName;

            sb.AppendLine($"        {memberName} = {sourceEnum.Name}.{member.Name},");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource($"{enumSymbol.Name}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}

public class EnumSyntaxReceiver : ISyntaxContextReceiver
{
    public List<EnumDeclarationSyntax> EnumsToAugment { get; } = new List<EnumDeclarationSyntax>();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is EnumDeclarationSyntax enumDeclaration)
        {
            var semanticModel = context.SemanticModel;
            var enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration) as INamedTypeSymbol;

            if (enumSymbol == null)
                return;

            var attributes = enumSymbol.GetAttributes();
            if (attributes.Any(a => a.AttributeClass?.Name == "GenEnumAttribute"))
            {
                EnumsToAugment.Add(enumDeclaration);
            }
        }
    }
}