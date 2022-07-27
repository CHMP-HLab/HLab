using System;

namespace HLab.Base.Wpf.BamlConverters;

internal class PropertyCustomRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.PropertyCustom;

    public ushort AttributeId { get; set; }
    public ushort SerializerTypeId { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        var pos = reader.BaseStream.Position;
        AttributeId = reader.ReadUInt16();
        SerializerTypeId = reader.ReadUInt16();
        Data = reader.ReadBytes(size - (int)(reader.BaseStream.Position - pos));
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(AttributeId);
        writer.Write(SerializerTypeId);
        writer.Write(Data);
    }
}