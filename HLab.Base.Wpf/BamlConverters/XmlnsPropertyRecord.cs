using System;

namespace HLab.Base.Wpf.BamlConverters;

internal class XmlnsPropertyRecord : SizedBamlRecord
{
    public override BamlRecordType Type => BamlRecordType.XmlnsProperty;

    public string Prefix { get; set; } = string.Empty;
    public string XmlNamespace { get; set; } = string.Empty;
    public ushort[] AssemblyIds { get; set; } = Array.Empty<ushort>();

    protected override void ReadData(BamlBinaryReader reader, int size)
    {
        Prefix = reader.ReadString();
        XmlNamespace = reader.ReadString();
        AssemblyIds = new ushort[reader.ReadUInt16()];
        for (var i = 0; i < AssemblyIds.Length; i++)
            AssemblyIds[i] = reader.ReadUInt16();
    }

    protected override void WriteData(BamlBinaryWriter writer)
    {
        writer.Write(Prefix);
        writer.Write(XmlNamespace);
        writer.Write((ushort)AssemblyIds.Length);
        foreach (var i in AssemblyIds)
            writer.Write(i);
    }
}