namespace HLab.Base.Wpf.BamlConverters;

internal class AssemblyInfoRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.AssemblyInfo;

    public ushort AssemblyId { get; set; }
    public string AssemblyFullName { get; set; } = string.Empty;

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        AssemblyId = reader.ReadUInt16();
        AssemblyFullName = reader.ReadString();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(AssemblyId);
        writer.Write(AssemblyFullName);
    }
}