namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyWithStaticResourceIdRecord : StaticResourceIdRecord
{
    public override BamlRecordType Type => BamlRecordType.PropertyWithStaticResourceId;

    public ushort AttributeId { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        AttributeId = reader.ReadUInt16();
        base.Read(reader);
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(AttributeId);
        base.Write(writer);
    }
}