namespace Neko.Sdl.Ttf;

public struct SubString {
    /// <summary>
    /// The flags for this substring
    /// </summary>
    public SubStringFlags Flags;
    
    /// <summary>
    /// The byte offset from the beginning of the text
    /// </summary>
    public int Offset;
    
    /// <summary>
    /// The byte length starting at the offset
    /// </summary>
    public int Length;
    
    /// <summary>
    /// The index of the line that contains this substring
    /// </summary>
    public int LineIndex;
    
    /// <summary>
    /// The internal cluster index, used for quickly iterating
    /// </summary>
    public int ClusterIndex;
    
    /// <summary>
    /// The rectangle, relative to the top left of the text, containing the substring
    /// </summary>
    public Rectangle Rect;
}