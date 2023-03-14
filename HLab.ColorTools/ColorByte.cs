namespace HLab.ColorTools;

public readonly struct ColorByte
{
    public ColorByte(byte a, byte r, byte g, byte b)
    {
        A = a;
        R = r;
        G = g;
        B = b;
    }
    public ColorByte(ColorDouble c)
    {
        A = (byte)(c.A * 255.0);
        R = (byte)(c.R * 255.0);
        G = (byte)(c.G * 255.0);
        B = (byte)(c.B * 255.0);
    }

    public byte A { get; }
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
}