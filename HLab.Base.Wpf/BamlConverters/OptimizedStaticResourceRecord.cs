namespace HLab.Base.Wpf.BamlConverters;

internal class OptimizedStaticResourceRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.OptimizedStaticResource;

    public byte Flags { get; set; }
    public ushort ValueId { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        Flags = reader.ReadByte();
        ValueId = reader.ReadUInt16();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(Flags);
        writer.Write(ValueId);
    }
}