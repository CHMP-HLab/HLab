namespace HLab.Geo;

public class Line
{
    public static Line fromSegment(Segment s)
    {
        // Line is vertical
        if (s.A.X == s.B.X)
        {
            return new Line(s.A.X);
        }

        double coef = (s.A.Y - s.B.Y) / (s.A.X - s.B.X);
        double origine = s.A.Y - coef * s.A.X;

        return new Line(coef, origine);
    }

    public double Coef { get; }

    readonly double _origine;
    public double OrigineY => double.IsPositiveInfinity(Coef) ? 0 : _origine;

    public double OrigineX => double.IsPositiveInfinity(Coef) ? _origine : (0 - _origine)/Coef;

    public Point Origine => new Point(OrigineX, OrigineY);

    public Line(double coef, double origine)
    {
        Coef = coef;
        _origine = origine;
    }

    // Specific case of vertical line vhere coef is infinit
    public Line(double x)
    {
        Coef = double.PositiveInfinity;
        _origine = x;
    }

    public Point? Intersect(Line l)
    {
        double x;
        double y;

        if (Coef == l.Coef)
        {
            if (OrigineY == l.OrigineY) { return Origine; }
            return null;
        }
                

        if (double.IsPositiveInfinity(Coef))
        {
            if (double.IsPositiveInfinity(l.Coef)) { return null;}
                                
            x = OrigineX;
            y = l.Coef * x + l.OrigineY;
        }
        else
        {
            if (double.IsPositiveInfinity(l.Coef))
            {
                x = l.OrigineX;
                y = Coef * x + OrigineY;
            }
            else
            {
                x = (OrigineY - l.OrigineY) / (l.Coef - Coef);
                y = l.Coef * x + l.OrigineY;
            }
        }

        return new Point(x, y);
    }

    public IEnumerable<Point> Intersect(Segment s)
    {
        Point? p = Intersect(s.Line);
        if (p!=null && s.Rect.Contains(p.Value))
        {
            yield return p.Value;
        }
    }

    public IEnumerable<Point> Intersect(Rect rect)
    {
        foreach (var p in Intersect(rect.Segment(Side.Left))) yield return p;
        foreach (var p in Intersect(rect.Segment(Side.Right))) yield return p;
        foreach (var p in Intersect(rect.Segment(Side.Top))) yield return p;
        foreach (var p in Intersect(rect.Segment(Side.Bottom))) yield return p;
    }
}