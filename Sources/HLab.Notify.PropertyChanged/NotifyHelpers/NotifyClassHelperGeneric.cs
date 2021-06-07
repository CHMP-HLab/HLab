using System.ComponentModel;

namespace HLab.Notify.PropertyChanged.NotifyHelpers
{
    public partial class NotifyClassHelperGeneric : NotifyClassHelperBase
    {
        public NotifyClassHelperGeneric(object target) : base(target)
        {
            if(target is INotifyPropertyChanged tpc)
                tpc.PropertyChanged += TargetPropertyChanged;
            else
            {

            }
        }

        private void TargetPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (Dict.TryGetValue(args.PropertyName, out var propertyEntry))
            {
                propertyEntry.TargetPropertyChanged(Target,args);
            }
        }
    }
}