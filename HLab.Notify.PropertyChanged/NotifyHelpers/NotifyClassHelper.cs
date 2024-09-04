using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace HLab.Notify.PropertyChanged.NotifyHelpers;

public partial class NotifyClassHelper : NotifyClassHelperBase
{
    public NotifyClassHelper(object target) : base(target) { }

    public override void OnPropertyChanged([NotNull]PropertyChangedEventArgs args)
    {
        using var s = GetSuspender();
        base.OnPropertyChanged(args);
        if (TryGetPropertyEntry(args.PropertyName, out var propertyEntry))
        {
            s.EnqueueAction(()=> propertyEntry.TargetPropertyChanged(Target,args));
        }
    }
}