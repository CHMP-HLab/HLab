namespace HLab.Base.Wpf.BamlConverters;

internal class TextRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.Text;

    public string Value { get; set; } = string.Empty;

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        Value = reader.ReadString();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(Value);
    }
}