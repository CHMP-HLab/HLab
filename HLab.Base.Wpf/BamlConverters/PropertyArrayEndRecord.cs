namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyArrayEndRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.PropertyArrayEnd;

    public override void Read(BamlBinaryReader reader) { }
    public override void Write(BamlBinaryWriter writer) { }
}