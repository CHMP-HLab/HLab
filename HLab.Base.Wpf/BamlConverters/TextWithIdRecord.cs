namespace HLab.Base.Wpf.BamlConverters;

internal class TextWithIdRecord : TextRecord
{
    public override BamlRecordType Type => BamlRecordType.TextWithId;

    public ushort ValueId { get; set; }

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        ValueId = reader.ReadUInt16();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(ValueId);
    }
}