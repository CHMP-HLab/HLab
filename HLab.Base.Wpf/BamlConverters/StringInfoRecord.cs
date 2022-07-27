namespace HLab.Base.Wpf.BamlConverters;

internal class StringInfoRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.StringInfo;

    public ushort StringId { get; set; }
    public string Value { get; set; } = string.Empty;

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        StringId = reader.ReadUInt16();
        Value = reader.ReadString();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(StringId);
        writer.Write(Value);
    }
}