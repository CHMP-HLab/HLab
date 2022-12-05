namespace HLab.Base.Wpf.BamlConverters;

internal class DefAttributeRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.DefAttribute;

    public string Value { get; set; } = string.Empty;
    public ushort NameId { get; set; }

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        Value = reader.ReadString();
        NameId = reader.ReadUInt16();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(Value);
        writer.Write(NameId);
    }
}