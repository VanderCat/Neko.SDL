using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Neko.Sdl.CodeGen;

[Generator]
public class SdlHintsSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Find the SDL class
        var sdlClass = context.Compilation.GetTypeByMetadataName("SDL.SDL3");
        if (sdlClass == null) return;

        // Get all SDL_HINT_* constants
        var hintFields = sdlClass.GetMembers()
            .Where(m => m.Kind == SymbolKind.Property)
            .Where(f => f.Name.StartsWith("SDL_HINT_"))
            .Select(f => f.Name);

        // Generate the code
        var sourceBuilder = new StringBuilder();
        sourceBuilder.AppendLine("namespace Neko.Sdl");
        sourceBuilder.AppendLine("{");
        sourceBuilder.AppendLine("    public partial class Hints");
        sourceBuilder.AppendLine("    {");

        foreach (var hintName in hintFields)
        {
            // Convert SDL_HINT_NAME to NameFormat
            var propertyName = string.Join("", 
                hintName.Split('_')
                    .Skip(2) // Skip SDL_HINT
                    .Select(part => char.ToUpper(part[0]) + part.Substring(1).ToLower()));

            sourceBuilder.AppendLine($"        public static readonly Hint {propertyName} = new(SDL.SDL3.{hintName});");
        }

        sourceBuilder.AppendLine("    }");
        sourceBuilder.AppendLine("}");

        // Add the source to the compilation
        context.AddSource("Hints.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }
}