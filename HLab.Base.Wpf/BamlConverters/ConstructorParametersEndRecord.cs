namespace HLab.Base.Wpf.BamlConverters;

internal class ConstructorParametersEndRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.ConstructorParametersEnd;

    public override void Read(BamlBinaryReader reader) { }
    public override void Write(BamlBinaryWriter writer) { }
}