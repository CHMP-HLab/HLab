namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyListEndRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.PropertyListEnd;

    public override void Read(BamlBinaryReader reader) { }
    public override void Write(BamlBinaryWriter writer) { }
}