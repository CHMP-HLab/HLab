using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using HLab.ColorTools;

//[SimpleJob(RuntimeMoniker.Net472, baseline: true)]
//[SimpleJob(RuntimeMoniker.NetCoreApp30)]
[SimpleJob(RuntimeMoniker.NativeAot80)]
[SimpleJob(RuntimeMoniker.Net80)]
//[SimpleJob(RuntimeMoniker.Mono)]
[RPlotExporter]

public class ColorsBenchmark
{
    private ColorRGB<double> _rgb = HLabColors.RGB(0.42,0.314,0.666);
    private ColorRGB<float> _rgbFloat = HLabColors.RGB(0.42f,0.314f,0.666f);
    private ColorHSL<double> _hsl = HLabColors.HSL(0.42,0.314,0.666);
    private ColorHSL<float> _hslFloat = HLabColors.HSL(0.42f,0.314f,0.666f);

    //private MD5 md5 = MD5.Create();
    //private byte[] data;

    //[Params(1000, 10000)]
    //public int N;

    [GlobalSetup]
    public void Setup()
    {
        //data = new byte[N];
        //new Random(42).NextBytes(data);
    }

    [Benchmark]
    public ColorHSL<double> ToHslDouble() => _rgb.ToHslDouble();

    [Benchmark]
    public ColorHSL<double> ToHslDoubleGeneric() => _rgb.ToHSL();

    [Benchmark]
    public ColorHSL<float> ToHslFloatGeneric() => _rgbFloat.ToHSL();

    [Benchmark]
    public ColorRGB<double> ToRgbDouble() => _hsl.ToRgbDouble();

    [Benchmark]
    public ColorRGB<double> ToRgbDoubleGeneric() => _hsl.ToRGB();

    [Benchmark]
    public ColorRGB<float> ToRgbFloatGeneric() => _hslFloat.ToRGB();
}