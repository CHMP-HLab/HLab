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
using System.Threading;

namespace HLab.Base;

public class SuspenderToken : IDisposable
{
    internal readonly ConcurrentQueue<Action> Actions = new();
    public void EnqueueAction(Action action) => Actions.Enqueue(action);

    readonly Suspender _suspender;
#if DEBUG_SUSPENDER
        public readonly StackTrace StackTrace;
#endif

    public SuspenderToken(Suspender suspender)
    {
        _suspender = suspender;
#if DEBUG_SUSPENDER
            StackTrace = new StackTrace(true);
#endif
    }
    public void Dispose()
    {
        _suspender.Resume(this);
    }
}
public class Suspender
{
    readonly ReaderWriterLockSlim _wLock = new();
    readonly HashSet<SuspenderToken> _list = new();
    readonly Action _suspendedAction;
    readonly Action _resumeAction;
    readonly ConcurrentQueue<Action> _resumeActions = new();

    public Suspender(Action suspendedAction=null, Action resumeAction=null)
    {
        _suspendedAction = suspendedAction;
        _resumeAction = resumeAction;
    }
    public SuspenderToken Get()
    {
        _wLock.EnterWriteLock();
        try
        {
            if(_list.Count == 0) _suspendedAction?.Invoke();
            var s = new SuspenderToken(this);
            _list.Add(s);
            return s;
        }
        finally
        {
            _wLock.ExitWriteLock();
        }
    }

    public bool Suspended
    {
        get
        {
            _wLock.EnterReadLock();
            try
            {
                return _list.Count > 0;
            }
            finally
            {
                _wLock.ExitReadLock();
            }

        }            
    }

    public void Resume(SuspenderToken s)
    {
        _wLock.EnterWriteLock();
        try
        {
            _list.Remove(s);
            while (s.Actions.TryDequeue(out var action))
                _resumeActions.Enqueue(action);

            if (_list.Count > 0)
                return;
        }
        finally
        {
            _wLock.ExitWriteLock();
        }

        //            try
        {
            while (_resumeActions.TryDequeue(out var action))
                action();

            _resumeAction?.Invoke();
        }
//            catch (Exception)
        {
                
        }
    }

}