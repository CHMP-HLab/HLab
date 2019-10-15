// Copyright © 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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

using System;
using System.Security;
using System.Security.Permissions;
using System.Net;


namespace MySql.Data.MySqlClient
{
  public sealed class MySqlSecurityPermission : MarshalByRefObject
  {
    private MySqlSecurityPermission()
    {
    }

    public static PermissionSet CreatePermissionSet(bool includeReflectionPermission)
    {
      PermissionSet permissionsSet = new PermissionSet(null);
      permissionsSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
      permissionsSet.AddPermission(new SocketPermission(PermissionState.Unrestricted));
      permissionsSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
      permissionsSet.AddPermission(new DnsPermission(PermissionState.Unrestricted));
      permissionsSet.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
      permissionsSet.AddPermission(new EnvironmentPermission(PermissionState.Unrestricted));

      if (includeReflectionPermission) permissionsSet.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));

      return permissionsSet;
    }
  }
}
