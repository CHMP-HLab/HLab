namespace HLab.Base.Wpf.BamlConverters;

internal class ElementStartRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.ElementStart;

    public ushort TypeId { get; set; }
    public byte Flags { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        TypeId = reader.ReadUInt16();
        Flags = reader.ReadByte();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(TypeId);
        writer.Write(Flags);
    }
}