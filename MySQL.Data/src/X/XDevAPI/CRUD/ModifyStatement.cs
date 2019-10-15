// Copyright © 2015, 2017, Oracle and/or its affiliates. All rights reserved.
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

using System.Collections.Generic;
using Mysqlx.Crud;
using MySqlX.XDevAPI.Common;
using System;
using MySql.Data;

namespace MySqlX.XDevAPI.CRUD
{
  /// <summary>
  /// Represents a chaining collection modify statement.
  /// </summary>
  public class ModifyStatement : FilterableStatement<ModifyStatement, Collection, Result>
  {
    internal ModifyStatement(Collection collection, string condition) : base(collection, condition)
    {
      Updates = new List<UpdateSpec>();
    }

    internal List<UpdateSpec> Updates;

    /// <summary>
    /// Sets key and value.
    /// </summary>
    /// <param name="docPath">The document path key.</param>
    /// <param name="value">The new value.</param>
    /// <returns>This <see cref="ModifyStatement"/> object.</returns>
    public ModifyStatement Set(string docPath, object value)
    {
      Updates.Add(new UpdateSpec(UpdateOperation.Types.UpdateType.ItemSet, docPath).SetValue(value));
      return this;
    }

    /// <summary>
    /// Changes value for a key.
    /// </summary>
    /// <param name="docPath">The document path key.</param>
    /// <param name="value">The new value.</param>
    /// <returns>This <see cref="ModifyStatement"/> object.</returns>
    public ModifyStatement Change(string docPath, object value)
    {
      Updates.Add(new UpdateSpec(UpdateOperation.Types.UpdateType.ItemReplace, docPath).SetValue(value));
      return this;
    }

    /// <summary>
    /// Removes a key or value from a document.
    /// </summary>
    /// <param name="docPath">The document path key.</param>
    /// <returns>This <see cref="ModifyStatement"/> object.</returns>
    public ModifyStatement Unset(string docPath)
    {
      Updates.Add(new UpdateSpec(UpdateOperation.Types.UpdateType.ItemRemove, docPath));
      return this;
    }

    /// <summary>
    /// Creates a <see cref="ModifyStatement"/> object set with the changes to be applied to all matching documents.
    /// </summary>
    /// <param name="document">The JSON-formatted object describing the set of changes.</param>
    /// <returns>A <see cref="ModifyStatement"/> object set with the changes described in <paramref name="document"/>.</returns>
    /// <remarks><paramref name="document"/> can be a <see cref="DbDoc"/> object, an anonymous object, or a JSON string.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="document"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="document"/> is <c>null</c> or white space.</exception>
    public ModifyStatement Patch(object document)
    {
      if (document == null)
        throw new ArgumentNullException(nameof(document));

      if (document is string && string.IsNullOrWhiteSpace((string) document))
        throw new ArgumentNullException(nameof(document), Resources.ParameterNullOrEmpty);

      DbDoc dbDocument = document is DbDoc ? document as DbDoc : new DbDoc(document);
      Updates.Add(new UpdateSpec(UpdateOperation.Types.UpdateType.MergePatch, string.Empty).SetValue(dbDocument.values));

      return this;
    }

    /// <summary>
    /// Executes the modify statement.
    /// </summary>
    /// <returns>A <see cref="Result"/> object containg the results of the execution.</returns>
    public override Result Execute()
    {
      return Execute(Target.Session.XSession.ModifyDocs, this);
    }
  }
}
