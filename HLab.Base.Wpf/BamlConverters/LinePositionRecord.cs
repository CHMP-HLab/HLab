namespace HLab.Base.Wpf.BamlConverters;

internal class LinePositionRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.LinePosition;

    public uint LinePosition { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        LinePosition = reader.ReadUInt32();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(LinePosition);
    }
}