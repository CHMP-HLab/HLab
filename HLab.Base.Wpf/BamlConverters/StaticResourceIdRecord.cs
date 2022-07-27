namespace HLab.Base.Wpf.BamlConverters;

internal class StaticResourceIdRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.StaticResourceId;

    public ushort StaticResourceId { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        StaticResourceId = reader.ReadUInt16();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(StaticResourceId);
    }
}