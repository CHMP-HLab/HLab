using System;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged.NotifyHelpers;

namespace HLab.Notify.PropertyChanged
{
    public class TriggerEntryNotifierWithPath : TriggerEntryNotifier
    {
        private readonly TriggerPath _path;
        private ITriggerEntry _next;

        public TriggerEntryNotifierWithPath(IPropertyEntry propertyEntry, TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler) 
            :base(propertyEntry,handler)
        {
            _path = path;
            propertyEntry.Link(OnPropertyChangedWithPath);
        }

        private void OnPropertyChangedWithPath(object sender, ExtendedPropertyChangedEventArgs e)
        {
            _next?.Dispose();

            if(Handler.TryGetTarget(out var handler))
            {
                if (e.NewValue == null)
                {
                    _next = null;
                }
                else
                {
                    var nextParser = NotifyClassHelperBase.GetHelper(e.NewValue);
                    _next = _path.GetTriggerB(nextParser, handler);
                } 
            }
            else
            {
                _next = null;
            }
        }


        public override void Dispose()
        {
            base.Dispose();
            _next?.Dispose();

            PropertyEntry.Unlink(OnPropertyChangedWithPath);
        }

    }
}