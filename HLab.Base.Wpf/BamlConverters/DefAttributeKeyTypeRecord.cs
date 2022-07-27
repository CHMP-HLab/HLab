using System;
using System.Collections.Generic;
using System.IO;

namespace HLab.Base.Wpf.BamlConverters;

internal class DefAttributeKeyTypeRecord : ElementStartRecord
{
    uint _position = 0xffffffff;

    public override BamlRecordType Type => BamlRecordType.DefAttributeKeyType;

    public bool Shared { get; set; }
    public bool SharedSet { get; set; }

    public BamlRecord? Record { get; set; }

    public override void ReadDeferred(IList<BamlRecord> records, int index, IDictionary<long, BamlRecord> recordsByPosition)
    {
        NavigateTree(records, ref index);

        Record = recordsByPosition[records[index].Position + _position];
    }

    public override void WriteDeferred(IList<BamlRecord> records, int index, BamlBinaryWriter writer)
    {
        if (Record == null)
            throw new InvalidOperationException("Invalid record state");

        NavigateTree(records, ref index);

        writer.BaseStream.Seek(_position, SeekOrigin.Begin);
        writer.Write((uint)(Record.Position - records[index].Position));
    }

    public override void Read(BamlBinaryReader reader)
    {
        base.Read(reader);
        _position = reader.ReadUInt32();
        Shared = reader.ReadBoolean();
        SharedSet = reader.ReadBoolean();
    }

    public override void Write(BamlBinaryWriter writer)
    {
        base.Write(writer);
        _position = (uint)writer.BaseStream.Position;
        writer.Write((uint)0);
        writer.Write(Shared);
        writer.Write(SharedSet);
    }
}