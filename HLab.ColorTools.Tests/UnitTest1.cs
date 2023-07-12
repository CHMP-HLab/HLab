namespace HLab.ColorTools.Tests;

public class ColorToolsTests
{
    [Fact]
    public void ToByte()
    {
        var cdouble = HLabColors.RGB(0.5,0.5,0.5);
        var cbyte = cdouble.To<byte>();
        Assert.Equal(127,cbyte.Red); 
        Assert.Equal(127,cbyte.Green); 
        Assert.Equal(127,cbyte.Blue);

        cdouble = HLabColors.RGB(1.0,1.0,1.0);
        cbyte = cdouble.To<byte>();
        Assert.Equal(255,cbyte.Red); 
        Assert.Equal(255,cbyte.Green); 
        Assert.Equal(255,cbyte.Blue);

        cdouble = HLabColors.RGB(0.0,0.0,0.0);
        cbyte = cdouble.To<byte>();
        Assert.Equal(0,cbyte.Red); 
        Assert.Equal(0,cbyte.Green); 
        Assert.Equal(0,cbyte.Blue);
    }

    [Fact]
    public void ToHSL()
    {
        var cdouble = HLabColors.RGB(1.0,1.0,1.0);
        var hsl = cdouble.ToHSL();
        Assert.Equal(0.0,hsl.Hue); 
        Assert.Equal(1.0,hsl.Lightness); 
        Assert.Equal(0.0,hsl.Saturation);

        cdouble = HLabColors.RGB(0.0,0.0,0.0);
        hsl = cdouble.ToHSL();
        Assert.Equal(0.0,hsl.Hue);
        Assert.Equal(0.0,hsl.Lightness);
        Assert.Equal(0.0,hsl.Saturation);

        cdouble = HLabColors.RGB(1.0,0.0,0.0);
        hsl = cdouble.ToHSL();
        Assert.Equal(0.0,hsl.Hue); 
        Assert.Equal(0.5,hsl.Lightness); 
        Assert.Equal(1.0,hsl.Saturation);

        cdouble = HLabColors.RGB(0.0,1.0,0.0);
        hsl = cdouble.ToHSL();
        Assert.Equal(120.0,hsl.Hue); 
        Assert.Equal(0.5,hsl.Lightness); 
        Assert.Equal(1.0,hsl.Saturation);

        cdouble = HLabColors.RGB(0.0,0.0,1.0);
        hsl = cdouble.ToHSL();
        Assert.Equal(240.0,hsl.Hue); 
        Assert.Equal(0.5,hsl.Lightness); 
        Assert.Equal(1.0,hsl.Saturation);

        cdouble = HLabColors.RGB(0.5,0.5,0.5);
        hsl = cdouble.ToHSL();
        Assert.Equal(0.0,hsl.Hue);
        Assert.Equal(0.5,hsl.Lightness);
        Assert.Equal(0.0,hsl.Saturation);

        cdouble = HLabColors.RGB(1.0,1.0,0.0);
        hsl = cdouble.ToHSL();
        Assert.Equal(60.0,hsl.Hue);
        Assert.Equal(0.5,hsl.Lightness);
        Assert.Equal(1.0,hsl.Saturation);

        cdouble = HLabColors.RGB(0.0,1.0,1.0);
        hsl = cdouble.ToHSL();
        Assert.Equal(180.0,hsl.Hue);
        Assert.Equal(0.5,hsl.Lightness);
        Assert.Equal(1.0,hsl.Saturation);

        cdouble = HLabColors.RGB(1.0,0.0,1.0);
        hsl = cdouble.ToHSL();
        Assert.Equal(300.0,hsl.Hue);
        Assert.Equal(0.5,hsl.Lightness);
        Assert.Equal(1.0,hsl.Saturation);

        cdouble = HLabColors.RGB(0.3,0.5,0.7);
        hsl = cdouble.ToHSL();
        Assert.True(Math.Abs(210.0-hsl.Hue)<double.Epsilon);
        Assert.Equal(0.5,hsl.Lightness);
        Assert.InRange(Math.Abs(0.4-hsl.Saturation),0.0,1E-15);
    }

    [Fact]
    public void ToHSLByte()
    {
        var drgb = HLabColors.RGB(1.0,1.0,1.0);
        var dhsl = drgb.ToHSL();
        var bhsl = dhsl.To<byte>();

        Assert.Equal(0,bhsl.Hue); 
        Assert.Equal(255,bhsl.Lightness); 
        Assert.Equal(0,bhsl.Saturation);

        drgb = HLabColors.RGB(0.0,1.0,0.0);
        dhsl = drgb.ToHSL();
        bhsl = dhsl.To<byte>();

        Assert.Equal(85,bhsl.Hue); 
        Assert.Equal(127,bhsl.Lightness); 
        Assert.Equal(255,bhsl.Saturation);
    }

    [Fact]
    public void ByteDoubleAccuracy()
    {
        var cdouble = HLabColors.RGB(0.5,0.5,0.5);
        var cbyte = cdouble.To<byte>();
        var cdouble2 = cbyte.To<double>();
        Assert.InRange(Math.Abs(cdouble.Red-cdouble2.Red), 0, 1.0 / 255.0); 
        Assert.InRange(Math.Abs(cdouble.Green-cdouble2.Green), 0, 1.0 / 255.0); 
        Assert.InRange(Math.Abs(cdouble.Blue-cdouble2.Blue), 0, 1.0 / 255.0);
    }

    [Fact]
    public void ByteAccuracy()
    {
        var c = HLabColors.RGB(1.0,0.5,0.0);
        var hsl1 = c.ToHSL().Saturate(1.0).To<byte>().ToRGB();
        var hsl2 =c.ToHSL().To<byte>().Saturate((byte)255).ToRGB();

        Assert.Equal(hsl1,hsl2);
   }
}
