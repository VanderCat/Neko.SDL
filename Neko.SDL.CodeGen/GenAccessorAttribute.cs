using System;
using System.Diagnostics;

namespace Neko.Sdl.CodeGen;

[Conditional("DEBUG")]
[AttributeUsage(AttributeTargets.Property)]
public class GenAccessorAttribute(string fieldName, bool cast = false) : Attribute {
    public string FieldName => fieldName;
    public bool Cast => cast;
}