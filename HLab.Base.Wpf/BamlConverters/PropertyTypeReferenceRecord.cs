namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyTypeReferenceRecord : PropertyComplexStartRecord
{
    public override BamlRecordType Type => BamlRecordType.PropertyTypeReference;

    public ushort TypeId { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        base.Read(reader);
        TypeId = reader.ReadUInt16();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        base.Write(writer);
        writer.Write(TypeId);
    }
}