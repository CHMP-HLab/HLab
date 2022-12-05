using System.IO;

namespace HLab.Base.Wpf.BamlConverters;

internal class BamlBinaryReader : BinaryReader
{
    public BamlBinaryReader(Stream stream)
        : base(stream)
    {
    }

    public int ReadEncodedInt()
    {
        return Read7BitEncodedInt();
    }
}