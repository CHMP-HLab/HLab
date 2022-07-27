using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HLab.Base.Wpf.BamlConverters;

internal class BamlElement
{
    public BamlElement(BamlRecord header)
    {
        Header = header;
    }

    public BamlRecord Header { get; }
    public IList<BamlRecord> Body { get; } = new List<BamlRecord>();
    public IList<BamlElement> Children { get; } = new List<BamlElement>();

    public BamlElement? Parent { get; private set; }
    public BamlRecord? Footer { get; private set; }

    static bool IsHeader(BamlRecord rec)
    {
        return rec.Type switch
        {
            BamlRecordType.ConstructorParametersStart => true,
            BamlRecordType.DocumentStart => true,
            BamlRecordType.ElementStart => true,
            BamlRecordType.KeyElementStart => true,
            BamlRecordType.NamedElementStart => true,
            BamlRecordType.PropertyArrayStart => true,
            BamlRecordType.PropertyComplexStart => true,
            BamlRecordType.PropertyDictionaryStart => true,
            BamlRecordType.PropertyListStart => true,
            BamlRecordType.StaticResourceStart => true,
            _ => false
        };
    }

    static bool IsFooter(BamlRecord rec)
    {
        return rec.Type switch
        {
            BamlRecordType.ConstructorParametersEnd => true,
            BamlRecordType.DocumentEnd => true,
            BamlRecordType.ElementEnd => true,
            BamlRecordType.KeyElementEnd => true,
            BamlRecordType.PropertyArrayEnd => true,
            BamlRecordType.PropertyComplexEnd => true,
            BamlRecordType.PropertyDictionaryEnd => true,
            BamlRecordType.PropertyListEnd => true,
            BamlRecordType.StaticResourceEnd => true,
            _ => false
        };
    }

    static bool IsMatch(BamlRecord header, BamlRecord footer)
    {
        return header.Type switch
        {
            BamlRecordType.ConstructorParametersStart => footer.Type == BamlRecordType.ConstructorParametersEnd,
            BamlRecordType.DocumentStart => footer.Type == BamlRecordType.DocumentEnd,
            BamlRecordType.KeyElementStart => footer.Type == BamlRecordType.KeyElementEnd,
            BamlRecordType.PropertyArrayStart => footer.Type == BamlRecordType.PropertyArrayEnd,
            BamlRecordType.PropertyComplexStart => footer.Type == BamlRecordType.PropertyComplexEnd,
            BamlRecordType.PropertyDictionaryStart => footer.Type == BamlRecordType.PropertyDictionaryEnd,
            BamlRecordType.PropertyListStart => footer.Type == BamlRecordType.PropertyListEnd,
            BamlRecordType.StaticResourceStart => footer.Type == BamlRecordType.StaticResourceEnd,
            BamlRecordType.ElementStart => footer.Type == BamlRecordType.ElementEnd,
            BamlRecordType.NamedElementStart => footer.Type == BamlRecordType.ElementEnd,
            _ => false
        };
    }

    public static BamlElement? Read(IList<BamlRecord> records)
    {
        Debug.Assert(records.Count > 0 && records[0].Type == BamlRecordType.DocumentStart);

        BamlElement? current = null;
        var stack = new Stack<BamlElement>();

        foreach (var record in records)
        {
            if (IsHeader(record))
            {
                var prev = current;

                current = new BamlElement(record);

                if (prev != null)
                {
                    prev.Children.Add(current);
                    current.Parent = prev;
                    stack.Push(prev);
                }
            }
            else if (IsFooter(record))
            {
                if (current == null)
                    throw new InvalidOperationException("Unexpected footer.");

                // ReSharper disable once PossibleNullReferenceException
                while (!IsMatch(current.Header, record))
                {
                    // End record can be omitted (sometimes).
                    if (stack.Count > 0)
                        current = stack.Pop();
                }
                current.Footer = record;
                if (stack.Count > 0)
                    current = stack.Pop();
            }
            else
            {
                if (current == null)
                    throw new InvalidOperationException("Unexpected record.");

                current.Body.Add(record);
            }
        }

        Debug.Assert(stack.Count == 0);
        return current;
    }
}