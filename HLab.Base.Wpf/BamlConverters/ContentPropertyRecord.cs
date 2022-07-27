namespace HLab.Base.Wpf.BamlConverters;

internal class ContentPropertyRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.ContentProperty;

    public ushort AttributeId { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        AttributeId = reader.ReadUInt16();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(AttributeId);
    }
}