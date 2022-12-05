namespace HLab.Base.Wpf.BamlConverters;

internal class RoutedEventRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.RoutedEvent;

    public string Value { get; set; } = string.Empty;
    public ushort AttributeId { get; set; }

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        AttributeId = reader.ReadUInt16();
        Value = reader.ReadString();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(Value);
        writer.Write(AttributeId);
    }
}