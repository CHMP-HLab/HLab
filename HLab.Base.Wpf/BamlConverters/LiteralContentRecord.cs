namespace HLab.Base.Wpf.BamlConverters;

internal class LiteralContentRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.LiteralContent;

    public string Value { get; set; } = string.Empty;
    public uint Reserved0 { get; set; }
    public uint Reserved1 { get; set; }

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        Value = reader.ReadString();
        Reserved0 = reader.ReadUInt32();
        Reserved1 = reader.ReadUInt32();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(Value);
        writer.Write(Reserved0);
        writer.Write(Reserved1);
    }
}