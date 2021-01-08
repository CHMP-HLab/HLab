using System.ComponentModel;
using System.Linq;

namespace HLab.Notify.PropertyChanged.NotifyParsers
{

    public partial class NotifyClassHelper : NotifyClassHelperBase
    {
        public NotifyClassHelper(object target) : base(target) { }

        public override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
            if (Dict.TryGetValue(args.PropertyName, out var propertyEntry))
            {
                propertyEntry.TargetPropertyChanged(Target,args);
            }
        }
    }
}