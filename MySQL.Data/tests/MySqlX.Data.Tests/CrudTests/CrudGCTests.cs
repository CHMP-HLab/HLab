// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
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

using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using System;
using Xunit;

namespace MySqlX.Data.Tests.ResultTests
{
  public class CrudGCTests : BaseTest
  {
#if !NETCOREAPP2_0
    [Fact]
    public void FetchAllNoReference()
    {
      Collection testColl = CreateCollection("test");
      var stmt = testColl.Add(@"{ ""_id"": 1, ""foo"": 1 }");
      stmt.Add(@"{ ""_id"": 2, ""foo"": 2 }");
      stmt.Add(@"{ ""_id"": 3, ""foo"": 3 }");
      stmt.Add(@"{ ""_id"": 4, ""foo"": 4 }");
      Result result = stmt.Execute();
      Assert.Equal(4, (int)result.RecordsAffected);

      var docResult = testColl.Find().Execute();
      var docs = docResult.FetchAll();
      WeakReference wr = new WeakReference(docResult);
      docResult = null;
      GC.Collect();
      Assert.False(wr.IsAlive);
      Assert.Equal(4, docs.Count);
    }
#endif
  }
}
