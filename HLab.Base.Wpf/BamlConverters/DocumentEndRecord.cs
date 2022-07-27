namespace HLab.Base.Wpf.BamlConverters;

internal class DocumentEndRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.DocumentEnd;

    public override void Read(BamlBinaryReader reader) { }
    public override void Write(BamlBinaryWriter writer) { }
}