namespace HLab.Geo;

public class Triangle
{
    public Point[] Points { get; } = new Point[3];

    public Point A => Points[0];
    public Point B => Points[1];
    public Point C => Points[2];

    public Segment AB => new Segment(A,B);
    public Segment BC => new Segment(B,C);
    public Segment CA => new Segment(C,A);
  

    public Triangle(Point a, Point b, Point c)
    {
        Points[0] = a;
        Points[1] = b;
        Points[2] = c;
    }

    public Point Gravity => new Point((A.X + B.X + C.X)/3, (A.Y + B.Y + C.Y)/3);

    public Point Inside( Point pCenter, Point pOut)
    {
        Segment s = new Segment(pCenter,pOut);

        Point? pIn = s.Intersect(AB.Line);
        if (pIn != null)
        {
            pOut = pIn.Value;
            s = new Segment(pCenter, pOut);
        }

        pIn = s.Intersect(BC.Line);
        if (pIn != null)
        {
            pOut = pIn.Value;
            s = new Segment(pCenter, pOut);
        }

        pIn = s.Intersect(CA);
        if (pIn != null) 
        {
            pOut = pIn.Value;
            s = new Segment(pCenter, pOut);
        }

        return pOut;
    }
}