﻿/*
  HLab.Base
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of HLab.Base.

    HLab.Base is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HLab.Base is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/
using System;

namespace HLab.Base
{
    public class Token
    {
        readonly object _locked = new object();
        int _count = 0;

        public Token(int nb = 1)
        {
            _count = nb;
        }

        public TokenGetter TryGet(int nb = 1)
        {
            lock (_locked)
            {
                if (nb <= _count)
                {
                    _count -= nb;
                    return new TokenGetter(this,nb);
                }
                return null;
            }
        }
        public TokenGetter Get(int nb = 1)
        {
            TokenGetter r = null;
            while (r == null) r = TryGet(nb);
            return r;
        }

        public void Add(int nb = 1)
        {
            lock (_locked)
            {
                _count += nb;
            }
        }
    }

    public class TokenGetter : IDisposable
    {
        readonly Token _token;
        readonly int _nb;
        public TokenGetter(Token t, int nb)
        {
            _nb = nb;
            _token = t;
        }
        public void Dispose()
        {
            _token.Add(_nb);
        }
    }
}
