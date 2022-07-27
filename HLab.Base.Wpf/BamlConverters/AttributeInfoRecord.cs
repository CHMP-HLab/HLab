namespace HLab.Base.Wpf.BamlConverters;

internal class AttributeInfoRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.AttributeInfo;

    public ushort AttributeId { get; set; }
    public ushort OwnerTypeId { get; set; }
    public byte AttributeUsage { get; set; }
    public string Name { get; set; } = string.Empty;

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        AttributeId = reader.ReadUInt16();
        OwnerTypeId = reader.ReadUInt16();
        AttributeUsage = reader.ReadByte();
        Name = reader.ReadString();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(AttributeId);
        writer.Write(OwnerTypeId);
        writer.Write(AttributeUsage);
        writer.Write(Name);
    }
}