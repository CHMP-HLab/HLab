namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyStringReferenceRecord : PropertyComplexStartRecord
{
    public override BamlRecordType Type => BamlRecordType.PropertyStringReference;

    public ushort StringId { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        base.Read(reader);
        StringId = reader.ReadUInt16();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        base.Write(writer);
        writer.Write(StringId);
    }
}