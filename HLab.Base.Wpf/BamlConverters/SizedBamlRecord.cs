using System.Diagnostics;

namespace HLab.Base.Wpf.BamlConverters;

internal abstract class SizedBamlRecord : BamlRecord
{
    public override void Read(BamlBinaryReader reader)
    {
        var pos = reader.BaseStream.Position;
        var size = reader.ReadEncodedInt();

        ReadData(reader, size - (int)(reader.BaseStream.Position - pos));
        Debug.Assert(reader.BaseStream.Position - pos == size);
    }

    static int SizeofEncodedInt(int val)
    {
        if ((val & ~0x7F) == 0)
        {
            return 1;
        }
        if ((val & ~0x3FFF) == 0)
        {
            return 2;
        }
        if ((val & ~0x1FFFFF) == 0)
        {
            return 3;
        }
        if ((val & ~0xFFFFFFF) == 0)
        {
            return 4;
        }
        return 5;
    }

    public override void Write(BamlBinaryWriter writer)
    {
        var pos = writer.BaseStream.Position;
        WriteData(writer);
        var size = (int)(writer.BaseStream.Position - pos);
        size = SizeofEncodedInt(SizeofEncodedInt(size) + size) + size;
        writer.BaseStream.Position = pos;
        writer.WriteEncodedInt(size);
        WriteData(writer);
    }

    protected abstract void ReadData(BamlBinaryReader reader, int size);
    protected abstract void WriteData(BamlBinaryWriter writer);
}