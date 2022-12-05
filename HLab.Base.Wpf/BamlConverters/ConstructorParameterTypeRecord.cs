namespace HLab.Base.Wpf.BamlConverters;

internal class ConstructorParameterTypeRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.ConstructorParameterType;

    public ushort TypeId { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        TypeId = reader.ReadUInt16();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(TypeId);
    }
}