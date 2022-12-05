namespace HLab.Base.Wpf.BamlConverters;

internal class TypeSerializerInfoRecord : TypeInfoRecord
{
    public override BamlRecordType Type => BamlRecordType.TypeSerializerInfo;

    public ushort SerializerTypeId { get; set; }

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        base.ReadData(reader, size);
        SerializerTypeId = reader.ReadUInt16();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        base.WriteData(writer);
        writer.Write(SerializerTypeId);
    }
}