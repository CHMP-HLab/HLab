namespace HLab.ColorTools;

public readonly struct ColorDouble
{
    public ColorDouble(double a, double r, double g, double b)
    {
        A = a;
        R = r;
        G = g;
        B = b;
    }

    public ColorDouble(ColorByte c)
    {
        // normalize _r, _g, _b values
        A = c.A / 255.0;
        R = c.R / 255.0;
        G = c.G / 255.0;
        B = c.B / 255.0;

    }

    public double A { get; }
    public double R { get; }
    public double G { get; }
    public double B { get; }
}