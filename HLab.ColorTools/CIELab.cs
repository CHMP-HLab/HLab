namespace HLab.ColorTools;

/// <summary>
/// Structure to define CIE L*a*b*.
/// </summary>
public struct CIELab
{
    /// <summary>
    /// Gets an empty CIELab structure.
    /// </summary>
    public static readonly CIELab Empty = new();

    public static bool operator ==(CIELab item1, CIELab item2) =>
        Math.Abs(item1.L - item2.L) < double.Epsilon
        && Math.Abs(item1.A - item2.A) < double.Epsilon
        && Math.Abs(item1.B - item2.B) < double.Epsilon;

    public static bool operator !=(CIELab item1, CIELab item2) =>
        Math.Abs(item1.L - item2.L) >= double.Epsilon
        || Math.Abs(item1.A - item2.A) >= double.Epsilon
        || Math.Abs(item1.B - item2.B) >= double.Epsilon;

    /// <summary>
    /// Gets or sets L component.
    /// </summary>
    public double L { get; set; }

    /// <summary>
    /// Gets or sets a component.
    /// </summary>
    public double A { get; set; }

    /// <summary>
    /// Gets or sets b component.
    /// </summary>
    public double B { get; set; }

    public CIELab(double l, double a, double b)
    {
        L = l;
        A = a;
        B = b;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        return (this == (CIELab)obj);
    }

    public readonly override int GetHashCode() => L.GetHashCode() ^ A.GetHashCode() ^ B.GetHashCode();
}