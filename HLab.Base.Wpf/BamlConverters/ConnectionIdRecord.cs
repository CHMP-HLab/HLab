namespace HLab.Base.Wpf.BamlConverters;

internal class ConnectionIdRecord : BamlRecord
{
    public override BamlRecordType Type => BamlRecordType.ConnectionId;

    public uint ConnectionId { get; set; }

    public override void Read(BamlBinaryReader reader)
    {
        ConnectionId = reader.ReadUInt32();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        writer.Write(ConnectionId);
    }
}