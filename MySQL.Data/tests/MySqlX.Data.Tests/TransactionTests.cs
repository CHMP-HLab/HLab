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

using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.CRUD;
using MySqlX.XDevAPI.Relational;
using System;
using Xunit;

namespace MySqlX.Data.Tests
{
  public class TransactionTests : BaseTest
  {
    [Fact]
    public void Commit()
    {
      Collection coll = CreateCollection("test");
      var docs = new[]
      {
        new {  _id = 1, title = "Book 1", pages = 20 },
        new {  _id = 2, title = "Book 2", pages = 30 },
        new {  _id = 3, title = "Book 3", pages = 40 },
        new {  _id = 4, title = "Book 4", pages = 50 },
      };

      // start the transaction
      coll.Session.StartTransaction();

      Result r = coll.Add(docs).Execute();
      Assert.Equal<ulong>(4, r.RecordsAffected);

      // now roll it back
      coll.Session.Commit();

      DocResult foundDocs = coll.Find().Execute();
      Assert.True(foundDocs.Next());
      Assert.True(foundDocs.Next());
      Assert.True(foundDocs.Next());
      Assert.True(foundDocs.Next());
      Assert.False(foundDocs.Next());
    }

    [Fact]
    public void Rollback()
    {
      Collection coll = CreateCollection("test");
      var docs = new[]
      {
        new {  _id = 1, title = "Book 1", pages = 20 },
        new {  _id = 2, title = "Book 2", pages = 30 },
        new {  _id = 3, title = "Book 3", pages = 40 },
        new {  _id = 4, title = "Book 4", pages = 50 },
      };

      // start the transaction
      coll.Session.StartTransaction();

      Result r = coll.Add(docs).Execute();
      Assert.Equal<ulong>(4, r.RecordsAffected);

      // now roll it back
      coll.Session.Rollback();

      DocResult foundDocs = coll.Find().Execute();
      Assert.False(foundDocs.Next());
    }

    [Fact]
    public void Warnings()
    {
      using(Session session = MySQLX.GetSession(ConnectionString))
      {
        session.SetCurrentSchema(schemaName);
        Schema schema = session.GetSchema(schemaName);
        session.SQL("CREATE TABLE nontrac(id INT primary key) ENGINE=MyISAM").Execute();
        Table table = schema.GetTable("nontrac");
        session.StartTransaction();
        table.Insert().Values(5).Execute();
        Assert.Throws<MySqlException>(() => { table.Insert().Values(5).Execute(); });
        var result = session.Rollback();
        Assert.Equal(1, result.Warnings.Count);
        // warning message: Some non-transactional changed tables couldn't be rolled back
        Assert.Equal(1196u, result.Warnings[0].Code);
      }
    }

    #region Savepoints

    [Fact]
    public void CreateUnnamedSavepoint()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        session.StartTransaction();

        string spName = session.SetSavepoint();
        Assert.False(string.IsNullOrWhiteSpace(spName));

        session.Rollback();
      }
    }

    [Fact]
    public void RollbackToSavepoint()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        var schema = session.GetSchema("test");
        var coll = schema.CreateCollection("collSP");

        session.StartTransaction();

        coll.Add("{ \"test\": \"test\" }").Execute();
        var sp = session.SetSavepoint();
        coll.Add("{ \"test\": \"test\" }").Execute();
        Assert.Equal(2, coll.Find().Execute().FetchAll().Count);
        session.RollbackTo(sp);
        Assert.Equal(1, coll.Find().Execute().FetchAll().Count);

        session.Rollback();
      }
    }

    [Fact]
    public void ReleaseSavepoint()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        var schema = session.GetSchema("test");
        var coll = schema.CreateCollection("collSP");

        session.StartTransaction();

        coll.Add("{ \"test\": \"test\" }").Execute();
        var sp = session.SetSavepoint();
        coll.Add("{ \"test2\": \"test2\" }").Execute();
        Assert.Equal(2, coll.Find().Execute().FetchAll().Count);
        session.ReleaseSavepoint(sp);
        Assert.Equal(2, coll.Find().Execute().FetchAll().Count);

        session.Rollback();
      }
    }

    [Fact]
    public void CreateNamedSavepoint()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        session.StartTransaction();

        string spName = session.SetSavepoint("mySavedPoint");
        Assert.False(string.IsNullOrWhiteSpace(spName));

        session.Rollback();
      }
    }

    [Fact]
    public void RollbackToNamedSavepoint()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        var schema = session.GetSchema("test");
        var coll = schema.CreateCollection("collSP");

        session.StartTransaction();

        coll.Add("{ \"test\": \"test\" }").Execute();
        var sp = session.SetSavepoint("mySavedPoint");
        coll.Add("{ \"test2\": \"test2\" }").Execute();
        Assert.Equal(2, coll.Find().Execute().FetchAll().Count);
        session.RollbackTo(sp);
        Assert.Equal(1, coll.Find().Execute().FetchAll().Count);

        session.Rollback();
      }
    }

    [Fact]
    public void ReleaseNamedSavepoint()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        var schema = session.GetSchema("test");
        var coll = schema.CreateCollection("collSP");

        session.StartTransaction();

        coll.Add("{ \"test\": \"test\" }").Execute();
        var sp = session.SetSavepoint("mySavedPoint");
        coll.Add("{ \"test2\": \"test2\" }").Execute();
        Assert.Equal(2, coll.Find().Execute().FetchAll().Count);
        session.ReleaseSavepoint(sp);
        Assert.Equal(2, coll.Find().Execute().FetchAll().Count);

        session.Rollback();
      }
    }

    [Fact]
    public void NonExistentSavepoint()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        session.StartTransaction();

        Exception exception = Assert.Throws<MySqlException>(() => session.RollbackTo("nonExistentSavePoint"));
        Assert.Equal("SAVEPOINT nonExistentSavePoint does not exist", exception.Message);

        exception = Assert.Throws<MySqlException>(() => session.ReleaseSavepoint("nonExistentSavePoint"));
        Assert.Equal("SAVEPOINT nonExistentSavePoint does not exist", exception.Message);

        session.Rollback();
      }
    }

    [Fact]
    public void CreateSavepointWithWeirdNames()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        string errorMessage = "You have an error in your SQL syntax";
        session.StartTransaction();

        Exception ex = Assert.Throws<MySqlException>(() => session.SetSavepoint(""));
        Assert.StartsWith(errorMessage, ex.Message);
        ex = Assert.Throws<MySqlException>(() => session.SetSavepoint(" "));
        Assert.StartsWith(errorMessage, ex.Message);
        ex = Assert.Throws<MySqlException>(() => session.SetSavepoint(null));
        Assert.StartsWith(errorMessage, ex.Message);
        ex = Assert.Throws<MySqlException>(() => session.SetSavepoint("-"));
        Assert.StartsWith(errorMessage, ex.Message);
        ex = Assert.Throws<MySqlException>(() => session.SetSavepoint("mysp+"));
        Assert.StartsWith(errorMessage, ex.Message);
        ex = Assert.Throws<MySqlException>(() => session.SetSavepoint("3306"));
        Assert.StartsWith(errorMessage, ex.Message);

        var sp = session.SetSavepoint("_");
        session.RollbackTo(sp);        
        sp = session.SetSavepoint("mysql3306");
        session.RollbackTo(sp);

        session.Rollback();
      }
    }

    [Fact]
    public void OverwriteSavepoint()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        var schema = session.GetSchema("test");
        var coll = schema.CreateCollection("collSP");

        session.StartTransaction();

        coll.Add("{ \"test\": \"test\" }").Execute();
        var sp = session.SetSavepoint("mySP");
        coll.Add("{ \"test2\": \"test2\" }").Execute();
        sp = session.SetSavepoint("mySP");
        coll.Add("{ \"test3\": \"test3\" }").Execute();
        sp = session.SetSavepoint("mySP");
        coll.Add("{ \"test4\": \"test4\" }").Execute();
        sp = session.SetSavepoint("mySP");
        session.RollbackTo(sp);
        Assert.Equal(4, coll.Find().Execute().FetchAll().Count);

        session.Rollback();
      }
    }

    [Fact]
    public void MultipleReleasesForSavepoint()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        session.StartTransaction();

        var sp = session.SetSavepoint("mySP");
        session.ReleaseSavepoint(sp);
        Exception exception = Assert.Throws<MySqlException>(() => session.ReleaseSavepoint(sp));
        Assert.Equal(string.Format("SAVEPOINT {0} does not exist", sp), exception.Message);

        session.Rollback();
      }
    }

    [Fact]
    public void RollbackAndReleaseAfterTransactionCommit()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        var schema = session.GetSchema("test");
        var coll = schema.CreateCollection("collSP");

        session.StartTransaction();

        var sp = session.SetSavepoint("mySP");
        coll.Add("{ \"test\": \"test\" }").Execute();

        session.Commit();

        Exception exception = Assert.Throws<MySqlException>(() => session.RollbackTo(sp));
        Assert.Equal(string.Format("SAVEPOINT {0} does not exist", sp), exception.Message);

        exception = Assert.Throws<MySqlException>(() => session.ReleaseSavepoint(sp));
        Assert.Equal(string.Format("SAVEPOINT {0} does not exist", sp), exception.Message);
      }
    }

    [Fact]
    public void RollbackAndReleaseAfterTransactionRollback()
    {
      using(var session = MySQLX.GetSession(ConnectionString))
      {
        var schema = session.GetSchema("test");
        var coll = schema.CreateCollection("collSP");

        session.StartTransaction();

        var sp = session.SetSavepoint("mySP");
        coll.Add("{ \"test\": \"test\" }").Execute();

        session.Rollback();

        Exception exception = Assert.Throws<MySqlException>(() => session.RollbackTo(sp));
        Assert.Equal(string.Format("SAVEPOINT {0} does not exist", sp), exception.Message);

        exception = Assert.Throws<MySqlException>(() => session.ReleaseSavepoint(sp));
        Assert.Equal(string.Format("SAVEPOINT {0} does not exist", sp), exception.Message);
      }
    }

    #endregion
  }
}
