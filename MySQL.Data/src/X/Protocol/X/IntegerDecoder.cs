// Copyright © 2015, 2016, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL Connector/NET, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA



using Google.Protobuf;
using MySql.Data.MySqlClient.X.XDevAPI.Common;
using System;

namespace MySqlX.Protocol.X
{
  internal class IntegerDecoder : ValueDecoder
  {
    bool _signed;

    public IntegerDecoder(bool signed)
    {
      _signed = signed;
    }

    public override void SetMetadata()
    {
      Column.Type = GetDbType();
      Column.IsNumberSigned = _signed;
      Column.ClrType = _signed ? GetSignedClrType() : GetUnsignedClrType();
      ClrValueDecoder = _signed ? GetSignedValueDecoder() : GetUnsignedValueDecoder();
      Column.IsPadded = _signed ? false : (Flags & 1) != 0;
    }

    private ColumnType GetDbType()
    {
      uint len = Column.Length + (_signed ? 0u : 1u);
      if (len <= 4) return ColumnType.Tinyint;
      else if (len <= 6) return ColumnType.Smallint;
      else if (len <= 9) return ColumnType.Mediumint;
      else if (len <= 11) return ColumnType.Int;
      else return ColumnType.Bigint;
    }

    private Type GetSignedClrType()
    {
      uint len = Column.Length;
      if (len <= 4) return typeof(sbyte);
      else if (len <= 6) return typeof(Int16);
      else if (len <= 9) return typeof(Int32);
      else if (len <= 11) return typeof(Int32);
      else return typeof(Int64);
    }

    private Type GetUnsignedClrType()
    {
      uint len = Column.Length;
      if (len < 4) return typeof(byte);
      else if (len < 6) return typeof(UInt16);
      else if (len < 9) return typeof(UInt32);
      else if (len < 11) return typeof(UInt32);
      else return typeof(UInt64);
    }

    private ClrDecoderDelegate GetSignedValueDecoder()
    {
      uint len = Column.Length;
      if (len <= 4) return SByteValueDecoder;
      else if (len <= 6) return Int16ValueDecoder;
      else if (len <= 9) return Int32ValueDecoder;
      else if (len <= 11) return Int32ValueDecoder;
      else return Int64ValueDecoder;
    }

    private ClrDecoderDelegate GetUnsignedValueDecoder()
    {
      uint len = Column.Length;
      if (len < 4) return ByteValueDecoder;
      else if (len < 6) return UInt16ValueDecoder;
      else if (len < 9) return UInt32ValueDecoder;
      else if (len < 11) return UInt32ValueDecoder;
      else return UInt64ValueDecoder;
    }

    private Int64 ReadInt(byte[] bytes)
    {
      return new CodedInputStream(bytes).ReadSInt64();
    }

    private UInt64 ReadUInt(byte[] bytes)
    {
      return new CodedInputStream(bytes).ReadUInt64();
    }

    public object SByteValueDecoder(byte[] bytes)
    {
      return (sbyte)ReadInt(bytes);
    }

    public object Int16ValueDecoder(byte[] bytes)
    {
      return (Int16)ReadInt(bytes);
    }

    public object Int32ValueDecoder(byte[] bytes)
    {
      return (Int32)ReadInt(bytes);
    }

    public object Int64ValueDecoder(byte[] bytes)
    {
      return ReadInt(bytes);
    }

    public object ByteValueDecoder(byte[] bytes)
    {
      return (byte)ReadUInt(bytes);
    }

    public object UInt16ValueDecoder(byte[] bytes)
    {
      return (UInt16)ReadUInt(bytes);
    }

    public object UInt32ValueDecoder(byte[] bytes)
    {
      return (UInt32)ReadUInt(bytes);
    }

    public object UInt64ValueDecoder(byte[] bytes)
    {
      return (UInt64)ReadUInt(bytes);
    }
  }
}
