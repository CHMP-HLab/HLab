using System.IO;

namespace HLab.Base.Wpf.BamlConverters;

internal class BamlBinaryWriter : BinaryWriter
{
    public BamlBinaryWriter(Stream stream)
        : base(stream)
    {
    }

    public void WriteEncodedInt(int val)
    {
        Write7BitEncodedInt(val);
    }
}