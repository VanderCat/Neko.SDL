using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neko.Sdl;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Rectangle(int x, int y, int width, int height) {
    public int X = x;
    public int Y = y;
    public int Width = width;
    public int Height = height;

    /// <summary>
    /// Calculate the intersection of a rectangle and line segment.
    /// </summary>
    /// <param name="rect">the rectangle to intersect</param>
    /// <param name="start">starting point</param>
    /// <param name="end">end point</param>
    /// <remarks>
    /// This function is used to clip a line segment to a rectangle.
    /// A line segment contained entirely within the rectangle or that does not intersect will remain unchanged.
    /// A line segment that crosses the rectangle at either or both ends will be clipped to the
    /// boundary of the rectangle and the new coordinates saved in start.X, start.Y, end.X and/or end.Y as necessary.
    /// </remarks>
    /// <returns>true if there is an intersection, false otherwise.</returns>
    public static bool IntersectLine(ref Rectangle rect, ref Point start, ref Point end) {
        var startPtr = (int*)Unsafe.AsPointer(ref start);
        var endPtr = (int*)Unsafe.AsPointer(ref end);
        return SDL_GetRectAndLineIntersection((SDL_Rect*)Unsafe.AsPointer(ref rect), startPtr, startPtr + 1, endPtr, endPtr + 1);
    }
    
    /// <summary>
    /// Calculate a minimal rectangle enclosing a set of points.
    /// </summary>
    /// <param name="points">points to be enclosed</param>
    /// <param name="clip">a Rectangle used for clipping</param>
    /// <returns>structure filled in with the minimal enclosing rectangle</returns>
    public static Rectangle? GetEnclosingPoints(Span<Point> points, ref Rectangle clip) {
        var result1 = new Rectangle();
        fixed (Point* pointsPtr = points) {
            if (SDL_GetRectEnclosingPoints((SDL_Point*)pointsPtr, points.Length, (SDL_Rect*)Unsafe.AsPointer(ref clip), (SDL_Rect*)&result1))
                return result1;
            return null;
        }
    }
    
    /// <summary>
    /// Calculate a minimal rectangle enclosing a set of points.
    /// </summary>
    /// <param name="points">points to be enclosed</param>
    /// <param name="result">structure filled in with the minimal enclosing rectangle</param>
    /// <returns>structure filled in with the minimal enclosing rectangle or null if all the points were outside of the clipping rectangle</returns>
    public static Rectangle? GetEnclosingPoints(Span<Point> points) {
        var result1 = new Rectangle();
        fixed (Point* pointsPtr = points) {
            if (SDL_GetRectEnclosingPoints((SDL_Point*)pointsPtr, points.Length, null, (SDL_Rect*)&result1))
                return result1;
            return null;
        }
    }
    
    /// <summary>
    /// Calculate the union of two rectangles
    /// </summary>
    /// <param name="a">structure representing the first rectangle</param>
    /// <param name="b">structure representing the second rectangle</param>
    /// <returns>union of rectangles A and B</returns>
    public static Rectangle Union(ref Rectangle a, ref Rectangle b) {
        var result1 = new Rectangle();
        SDL_GetRectUnion((SDL_Rect*)Unsafe.AsPointer(ref a), (SDL_Rect*)Unsafe.AsPointer(ref b), (SDL_Rect*)&result1).ThrowIfError();
        return result1;
    }

    /// <summary>
    /// Determine whether two rectangles intersect
    /// </summary>
    /// <param name="a">structure representing the first rectangle</param>
    /// <param name="b">structure representing the second rectangle</param>
    /// <returns>true if there is an intersection, false otherwise</returns>
    public static bool HasIntersection(ref Rectangle a, ref Rectangle b) =>
        SDL_HasRectIntersection((SDL_Rect*)Unsafe.AsPointer(ref a), (SDL_Rect*)Unsafe.AsPointer(ref b));

    /// <summary>
    /// Determine whether a point resides inside a rectangle
    /// </summary>
    /// <param name="p"></param>
    /// <param name="r"></param>
    /// <returns>Returns true if p is contained by r, false otherwise</returns>
    /// <remarks>
    /// <para>
    /// A point is considered part of a rectangle if both p and r are not NULL, and p's x and y
    /// coordinates are >= to the rectangle's top left corner, and &lt; the rectangle's x+w and y+h.
    /// So a 1x1 rectangle considers point (0,0) as "inside" and (0,1) as not.
    /// </para>
    /// </remarks>
    public static bool IsPointIn(ref Point p, ref Rectangle r) =>
        SDL_PointInRect((SDL_Point*)Unsafe.AsPointer(ref p), (SDL_Rect*)Unsafe.AsPointer(ref r));

    /// <summary>
    /// Determine whether a rectangle has no area
    /// </summary>
    /// <remarks>
    /// A rectangle is considered "empty" for this function if r is NULL,
    /// or if r's width and/or height are &lt;= 0.
    /// </remarks>
    public bool Empty => SDL_RectEmpty((SDL_Rect*)Unsafe.AsPointer(ref this));

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Rectangle rect && Equals(rect);

    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

    public bool Equals(Rectangle rect) => this == rect;
    
    public static bool operator ==(Rectangle left, Rectangle right) =>
        left.X == right.X && 
        left.Y == right.Y && 
        left.Width == right.Width && 
        left.Height == right.Height;

    public static bool operator !=(Rectangle left, Rectangle right) => !(left == right);
}