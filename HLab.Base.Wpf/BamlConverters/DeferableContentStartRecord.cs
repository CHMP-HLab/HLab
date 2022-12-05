using System;
using System.Collections.Generic;
using System.IO;

namespace HLab.Base.Wpf.BamlConverters;

internal class DeferableContentStartRecord : BamlRecord
{
    long pos;
    internal uint size = 0xffffffff;

    public override BamlRecordType Type => BamlRecordType.DeferableContentStart;

    public BamlRecord? Record { get; set; }

    public override void ReadDeferred(IList<BamlRecord> records, int index, IDictionary<long, BamlRecord> recordsByPosition)
    {
        Record = recordsByPosition[pos + size];
    }

    public override void WriteDeferred(IList<BamlRecord> records, int index, BamlBinaryWriter writer)
    {
        if (Record == null)
            throw new InvalidOperationException("Invalid record state");

        writer.BaseStream.Seek(pos, SeekOrigin.Begin);
        writer.Write((uint)(Record.Position - (pos + 4)));
    }

    public override void Read(BamlBinaryReader reader)
    {
        size = reader.ReadUInt32();
        pos = reader.BaseStream.Position;
    }

    public override void Write(BamlBinaryWriter writer)
    {
        pos = writer.BaseStream.Position;
        writer.Write((uint)0);
    }
}