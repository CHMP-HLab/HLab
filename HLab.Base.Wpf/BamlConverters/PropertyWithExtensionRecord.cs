namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyWithExtensionRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.PropertyWithExtension;

    public ushort AttributeId { get; set; }
    public ushort Flags { get; set; }
    public ushort ValueId { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        AttributeId = reader.ReadUInt16();
        Flags = reader.ReadUInt16();
        ValueId = reader.ReadUInt16();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(AttributeId);
        writer.Write(Flags);
        writer.Write(ValueId);
    }
}