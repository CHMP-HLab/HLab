using System.Collections.Generic;

namespace HLab.Base.Wpf.BamlConverters;

internal abstract class BamlRecord
{
    public abstract BamlRecordType Type { get; }
    public long Position { get; internal set; }
    public abstract void Read(BamlBinaryReader reader);
    public abstract void Write(BamlBinaryWriter writer);

    public virtual void ReadDeferred(IList<BamlRecord> records, int index, IDictionary<long, BamlRecord> recordsByPosition) { }
    public virtual void WriteDeferred(IList<BamlRecord> records, int index, BamlBinaryWriter writer) { }

    protected static void NavigateTree(IList<BamlRecord> records, ref int index)
    {
        while (true)
        {
            switch (records[index].Type)
            {
                case BamlRecordType.DefAttributeKeyString:
                case BamlRecordType.DefAttributeKeyType:
                case BamlRecordType.OptimizedStaticResource:
                    break;

                case BamlRecordType.StaticResourceStart:
                    NavigateTree(records, BamlRecordType.StaticResourceStart, BamlRecordType.StaticResourceEnd, ref index);
                    break;

                case BamlRecordType.KeyElementStart:
                    NavigateTree(records, BamlRecordType.KeyElementStart, BamlRecordType.KeyElementEnd, ref index);
                    break;

                default:
                    return;
            }

            index++;
        }
    }

    static void NavigateTree(IList<BamlRecord> records, BamlRecordType start, BamlRecordType end, ref int index)
    {
        index++;

        while (true) //Assume there always is a end
        {
            var recordType = records[index].Type;

            if (recordType == start)
            {
                NavigateTree(records, start, end, ref index);
            }
            else if (recordType == end)
            {
                return;
            }

            index++;
        }
    }
}