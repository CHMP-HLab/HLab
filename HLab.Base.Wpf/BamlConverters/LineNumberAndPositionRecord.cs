namespace HLab.Base.Wpf.BamlConverters;

internal class LineNumberAndPositionRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.LineNumberAndPosition;

    public uint LineNumber { get; set; }
    public uint LinePosition { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        LineNumber = reader.ReadUInt32();
        LinePosition = reader.ReadUInt32();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(LineNumber);
        writer.Write(LinePosition);
    }
}