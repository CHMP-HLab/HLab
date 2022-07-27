namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyWithConverterRecord : PropertyRecord
{
    public override BamlRecordType Type => BamlRecordType.PropertyWithConverter;

    public ushort ConverterTypeId { get; set; }

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        base.ReadData(reader, size);
        ConverterTypeId = reader.ReadUInt16();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        base.WriteData(writer);
        writer.Write(ConverterTypeId);
    }
}