namespace HLab.Base.Wpf.BamlConverters;

internal class KeyElementEndRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.KeyElementEnd;

    public override void Read(BamlBinaryReader reader) { }
    public override void Write(BamlBinaryWriter writer) { }
}