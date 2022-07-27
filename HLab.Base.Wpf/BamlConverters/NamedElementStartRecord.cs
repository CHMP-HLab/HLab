namespace HLab.Base.Wpf.BamlConverters;

internal class NamedElementStartRecord : ElementStartRecord
{
    public override BamlRecordType Type => BamlRecordType.NamedElementStart;

    public string? RuntimeName { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        TypeId = reader.ReadUInt16();
        RuntimeName = reader.ReadString();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(TypeId);
        if (RuntimeName != null)
        {
            writer.Write(RuntimeName);
        }
    }
}