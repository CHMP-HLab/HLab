namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.Property;

    public ushort AttributeId { get; set; }

    public string Value { get; set; } = string.Empty;

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        AttributeId = reader.ReadUInt16();
        Value = reader.ReadString();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(AttributeId);
        writer.Write(Value);
    }
}