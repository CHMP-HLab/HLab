namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyDictionaryEndRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.PropertyDictionaryEnd;

    public override void Read(BamlBinaryReader reader) { }
    public override void Write(BamlBinaryWriter writer) { }
}