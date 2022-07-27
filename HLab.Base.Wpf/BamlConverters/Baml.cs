// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable CollectionNeverQueried.Global

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HLab.Base.Wpf.BamlConverters;

internal static class Baml
    {
        static readonly byte[] _signature =
        {
            0x0C, 0x00, 0x00, 0x00, // strlen

            (byte)'M', 0x00,
            (byte)'S', 0x00,
            (byte)'B', 0x00,
            (byte)'A', 0x00,
            (byte)'M', 0x00,
            (byte)'L', 0x00,

            0x00, 0x00, 0x60, 0x00, // reader version
            0x00, 0x00, 0x60, 0x00, // updater version
            0x00, 0x00, 0x60, 0x00, // writer version
        };

        public static IList<BamlRecord> ReadDocument(Stream stream)
        {
            var reader = new BamlBinaryReader(stream);

            var rawSignature = reader.ReadBytes(_signature.Length);

            if (!rawSignature.SequenceEqual(_signature))
                throw new NotSupportedException("Invalid signature");

            var records = new List<BamlRecord>();
            var recordsByPosition = new Dictionary<long, BamlRecord>();

            while (stream.Position < stream.Length)
            {
                var pos = stream.Position;
                var type = (BamlRecordType)reader.ReadByte();

                var record = BamlRecordFromType(type);

                record.Position = pos;
                record.Read(reader);

                records.Add(record);
                recordsByPosition.Add(pos, record);
            }

            for (var i = 0; i < records.Count; i++)
            {
                records[i]?.ReadDeferred(records, i, recordsByPosition);
            }

            return records;
        }

        public static void WriteDocument(IList<BamlRecord> records, Stream stream)
        {
            var writer = new BamlBinaryWriter(stream);

            writer.Write(_signature);

            foreach (var record in records)
            {
                record.Position = stream.Position;
                writer.Write((byte)record.Type);
                record.Write(writer);
            }

            for (var i = 0; i < records.Count; i++)
            {
                var record = records[i];
                record.WriteDeferred(records, i, writer);
            }
        }

        static BamlRecord BamlRecordFromType(BamlRecordType type)
        {
            return type switch
            {
                BamlRecordType.AssemblyInfo => new AssemblyInfoRecord(),
                BamlRecordType.AttributeInfo => new AttributeInfoRecord(),
                BamlRecordType.ConstructorParametersStart => new ConstructorParametersStartRecord(),
                BamlRecordType.ConstructorParametersEnd => new ConstructorParametersEndRecord(),
                BamlRecordType.ConstructorParameterType => new ConstructorParameterTypeRecord(),
                BamlRecordType.ConnectionId => new ConnectionIdRecord(),
                BamlRecordType.ContentProperty => new ContentPropertyRecord(),
                BamlRecordType.DefAttribute => new DefAttributeRecord(),
                BamlRecordType.DefAttributeKeyString => new DefAttributeKeyStringRecord(),
                BamlRecordType.DefAttributeKeyType => new DefAttributeKeyTypeRecord(),
                BamlRecordType.DeferableContentStart => new DeferableContentStartRecord(),
                BamlRecordType.DocumentEnd => new DocumentEndRecord(),
                BamlRecordType.DocumentStart => new DocumentStartRecord(),
                BamlRecordType.ElementEnd => new ElementEndRecord(),
                BamlRecordType.ElementStart => new ElementStartRecord(),
                BamlRecordType.KeyElementEnd => new KeyElementEndRecord(),
                BamlRecordType.KeyElementStart => new KeyElementStartRecord(),
                BamlRecordType.LineNumberAndPosition => new LineNumberAndPositionRecord(),
                BamlRecordType.LinePosition => new LinePositionRecord(),
                BamlRecordType.LiteralContent => new LiteralContentRecord(),
                BamlRecordType.NamedElementStart => new NamedElementStartRecord(),
                BamlRecordType.OptimizedStaticResource => new OptimizedStaticResourceRecord(),
                BamlRecordType.PIMapping => new PIMappingRecord(),
                BamlRecordType.PresentationOptionsAttribute => new PresentationOptionsAttributeRecord(),
                BamlRecordType.Property => new PropertyRecord(),
                BamlRecordType.PropertyArrayEnd => new PropertyArrayEndRecord(),
                BamlRecordType.PropertyArrayStart => new PropertyArrayStartRecord(),
                BamlRecordType.PropertyComplexEnd => new PropertyComplexEndRecord(),
                BamlRecordType.PropertyComplexStart => new PropertyComplexStartRecord(),
                BamlRecordType.PropertyCustom => new PropertyCustomRecord(),
                BamlRecordType.PropertyDictionaryEnd => new PropertyDictionaryEndRecord(),
                BamlRecordType.PropertyDictionaryStart => new PropertyDictionaryStartRecord(),
                BamlRecordType.PropertyListEnd => new PropertyListEndRecord(),
                BamlRecordType.PropertyListStart => new PropertyListStartRecord(),
                BamlRecordType.PropertyStringReference => new PropertyStringReferenceRecord(),
                BamlRecordType.PropertyTypeReference => new PropertyTypeReferenceRecord(),
                BamlRecordType.PropertyWithConverter => new PropertyWithConverterRecord(),
                BamlRecordType.PropertyWithExtension => new PropertyWithExtensionRecord(),
                BamlRecordType.PropertyWithStaticResourceId => new PropertyWithStaticResourceIdRecord(),
                BamlRecordType.RoutedEvent => new RoutedEventRecord(),
                BamlRecordType.StaticResourceEnd => new StaticResourceEndRecord(),
                BamlRecordType.StaticResourceId => new StaticResourceIdRecord(),
                BamlRecordType.StaticResourceStart => new StaticResourceStartRecord(),
                BamlRecordType.StringInfo => new StringInfoRecord(),
                BamlRecordType.Text => new TextRecord(),
                BamlRecordType.TextWithConverter => new TextWithConverterRecord(),
                BamlRecordType.TextWithId => new TextWithIdRecord(),
                BamlRecordType.TypeInfo => new TypeInfoRecord(),
                BamlRecordType.TypeSerializerInfo => new TypeSerializerInfoRecord(),
                BamlRecordType.XmlnsProperty => new XmlnsPropertyRecord(),
                _ => throw new NotSupportedException("Unsupported record type: " + type)
            };
        }
    }