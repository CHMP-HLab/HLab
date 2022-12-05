namespace HLab.Base.Wpf.BamlConverters;

internal class StaticResourceEndRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.StaticResourceEnd;

    public override void Read(BamlBinaryReader reader) { }
    public override void Write(BamlBinaryWriter writer) { }
}