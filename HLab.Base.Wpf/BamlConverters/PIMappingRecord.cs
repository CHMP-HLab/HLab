namespace HLab.Base.Wpf.BamlConverters;

internal class PIMappingRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.PIMapping;

    public string XmlNamespace { get; set; } = string.Empty;
    public string ClrNamespace { get; set; } = string.Empty;
    public ushort AssemblyId { get; set; }

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        XmlNamespace = reader.ReadString();
        ClrNamespace = reader.ReadString();
        AssemblyId = reader.ReadUInt16();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(XmlNamespace);
        writer.Write(ClrNamespace);
        writer.Write(AssemblyId);
    }
}