using System.ComponentModel;

namespace HLab.Notify.PropertyChanged.NotifyHelpers;

public class NotifyClassHelperGeneric : NotifyClassHelperBase
{
    public NotifyClassHelperGeneric(object target) : base(target)
    {
        if(target is INotifyPropertyChanged tpc)
            tpc.PropertyChanged += TargetPropertyChanged;
        else
        {

        }
    }

    void TargetPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        if (TryGetPropertyEntry(args.PropertyName, out var propertyEntry))
        {
            propertyEntry.TargetPropertyChanged(Target,args);
        }
    }
}