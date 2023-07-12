#nullable enable
using System;
using System.Collections.Generic;

namespace HLab.Base.Disposables;

/// <summary>
/// Helper to dispose a set of objects
///
/// Add to the class:
///
/// readonly DisposeHelper _dispose = new();
///
/// ...
/// 
/// ~ClassWithFinalizer() => Dispose(false);
/// public virtual void Dispose(bool disposing) => _dispose.Dispose(disposing);
/// public void Dispose()
/// {
///     Dispose(true);
///     GC.SuppressFinalize(this);
/// }
/// </summary>
public class DisposeHelper : IDisposable
{
    bool _disposed = false;
    readonly Stack<IDisposable> _toDispose = new();

    Action? _onDispose;

    /// <summary>
    /// dispose managed object.
    /// </summary>
    /// <param name="disposable"></param>
    /// <returns></returns>
    public IDisposable Add(IDisposable disposable)
    {
        _toDispose.Push(disposable);
        return disposable;
    }

    /// <summary>
    /// Add action to free unmanaged resources (unmanaged objects)
    /// or set large fields to null.
    /// </summary>
    /// <param name="action"></param>
    public void OnDispose(Action action)
    {
        _onDispose += action;
    }

    public void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            while (_toDispose.TryPop(out var disposable))
            {
                disposable.Dispose();
            }
        }

        _onDispose?.Invoke();

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~DisposeHelper() => Dispose(false);
}