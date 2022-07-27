namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyComplexEndRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.PropertyComplexEnd;

    public override void Read(BamlBinaryReader reader) { }
    public override void Write(BamlBinaryWriter writer) { }
}