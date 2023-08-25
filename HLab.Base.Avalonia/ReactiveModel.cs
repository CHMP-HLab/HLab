#nullable enable
using System.Runtime.CompilerServices;
using HLab.Base.Disposables;
using ReactiveUI;

namespace HLab.Base.Avalonia;

public abstract class ReactiveModel : ReactiveObject, IDisposable
{
    public DisposeHelper Disposer { get; } = new();

    /// <summary>
    /// Object has been saved
    /// </summary>
    public bool Saved
    {
        get => _saved;
        set => SetAndRaise(ref _saved, value);
    }
    bool _saved;


    /// <summary>
    /// Set properties values unsetting saved flag if changed
    /// </summary>
    protected bool SetUnsavedValue<TRet>(ref TRet backingField, TRet value, [CallerMemberName] string propertyName = null)
    {
        using var disposable = DelayChangeNotifications();

        if (EqualityComparer<TRet>.Default.Equals(backingField, value)) return false;

        this.RaisePropertyChanging(propertyName);
        backingField = value;
        Saved = false;
        this.RaisePropertyChanged(propertyName);
        return true;
    }

    public bool SetAndRaise<TRet>(
        ref TRet backingField,
        TRet newValue,
        [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
        {
            return false;
        }

        this.RaisePropertyChanging(propertyName);
        backingField = newValue;
        this.RaisePropertyChanged(propertyName);
        return true;
    }


    public void Dispose()
    {
        Disposer.Dispose();
    }
}