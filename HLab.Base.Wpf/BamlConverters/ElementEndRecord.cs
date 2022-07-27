namespace HLab.Base.Wpf.BamlConverters;

internal class ElementEndRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.ElementEnd;

    public override void Read(BamlBinaryReader reader) { }
    public override void Write(BamlBinaryWriter writer) { }
}