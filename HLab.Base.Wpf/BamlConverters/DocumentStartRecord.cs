namespace HLab.Base.Wpf.BamlConverters;

internal class DocumentStartRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.DocumentStart;

    public bool LoadAsync { get; set; }
    public uint MaxAsyncRecords { get; set; }
    public bool DebugBaml { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        LoadAsync = reader.ReadBoolean();
        MaxAsyncRecords = reader.ReadUInt32();
        DebugBaml = reader.ReadBoolean();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(LoadAsync);
        writer.Write(MaxAsyncRecords);
        writer.Write(DebugBaml);
    }
}