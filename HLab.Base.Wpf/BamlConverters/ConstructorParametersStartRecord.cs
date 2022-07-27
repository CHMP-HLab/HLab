namespace HLab.Base.Wpf.BamlConverters;

internal class ConstructorParametersStartRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.ConstructorParametersStart;

    public override void Read(BamlBinaryReader reader) { }
    public override void Write(BamlBinaryWriter writer) { }
}