using System;
using System.Diagnostics;

namespace Neko.Sdl.CodeGen;

[Conditional("DEBUG")]
[AttributeUsage(AttributeTargets.Enum)]
public class GenEnumAttribute(string sourceEnum, string prefix) : Attribute {
    public string SourceEnum => sourceEnum;
    public string Prefix => prefix;
}