// Copyright © 2017, Oracle and/or its affiliates. All rights reserved.
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

namespace MySqlX.Failover
{
  /// <summary>
  /// Depicts a host which can be failed over to.
  /// </summary>
  internal class XServer
  {
    #region Properties

    /// <summary>
    /// Gets and sets the name or address of the host.
    /// </summary>
    internal string Host { get; private set; }
    /// <summary>
    /// Gets and sets the port number.
    /// </summary>
    internal int Port { get; private set; }
    /// <summary>
    /// Gets a value between 0 and 100 which represents the priority of the host.
    /// </summary>
    internal int Priority { get; private set; }
    /// <summary>
    /// Flag to indicate if this host is currently being used.
    /// </summary>
    internal bool IsActive { get; set; }

    #endregion

    internal XServer(string host, int port, int priority)
    {
      this.Host = host;
      this.Port = port;
      this.Priority = priority;
    }
  }
}
