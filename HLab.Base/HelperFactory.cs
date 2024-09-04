/*
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HLab.Base;

public class HelperFactory<T>
    where T : class
{
    readonly ConditionalWeakTable<object, T> _weakTable = new ();

    readonly ConcurrentDictionary<Type, Func<object, T>> _registered = new ();
    readonly ConcurrentDictionary<Type, Func<object, T>> _cache = new ();

    public void Register(Type type, Func<object, T> factory)
    {
        if (!_registered.TryAdd(type, factory)) return;
        foreach (var t in _cache)
        {
            _cache.TryUpdate(t.Key,t.Value, GetFactory(t.Key));
        }
    }

    public void Register<TO>(Func<TO, T> factory)
    {
        Register(typeof(TO),(o) => factory((TO)o));
    }

    public T Get(object target, Action<T> onCreate=null)
    {
        var created = false;

        var obj = _weakTable.GetValue(target, t =>
        {
            created = true;
            return _cache.GetOrAdd(t.GetType(), GetFactory).Invoke(target);
        });

        if (created) onCreate?.Invoke(obj);

        return obj;

    }

    Func<object, T> GetFactory(Type type)
    {
        KeyValuePair<Type, Func<object, T>>? bestMatch=null;

        foreach (var entry in _registered)
        {
            if (!entry.Key.IsAssignableFrom(type)) continue;
            if (!bestMatch.HasValue || bestMatch.Value.Key.IsAssignableFrom(entry.Key))
            {
                bestMatch = entry;
            }
        }
        return bestMatch?.Value;
    }
}