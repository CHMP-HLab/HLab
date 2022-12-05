namespace HLab.Base.Wpf.BamlConverters;

internal class TypeInfoRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.TypeInfo;

    public ushort TypeId { get; set; }
    public ushort AssemblyId { get; set; }

    public string TypeFullName { get; set; } = string.Empty;

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        TypeId = reader.ReadUInt16();
        AssemblyId = reader.ReadUInt16();
        TypeFullName = reader.ReadString();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(TypeId);
        writer.Write(AssemblyId);
        writer.Write(TypeFullName);
    }
}