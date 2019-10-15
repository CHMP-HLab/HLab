using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using HLab.Base;
using HLab.DependencyInjection.Annotations;
using HLab.DependencyInjection;
using HLab.Notify.Annotations;

namespace HLab.Notify
{
    public class NotifierObject : INotifierObject, IInitializer
    {
        private INotifier _n;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => N.Add(value);
            remove => N.Remove(value);
        }

        INotifier INotifierObject.GetNotifier() => N;
        public INotifier GetNotifier() => N;

        private SuspenderToken _suspender;

        [NotMapped]
        [Import]
        protected INotifier N
        {
            get => _n;
            
            set
            {
//                _suspender = value.Suspend.Get();
                _n = value;
                _n.Subscribe();
            }
        }

        public virtual void OnSubscribe(INotifier n) { }
        public virtual void Initialize(IRuntimeImportContext ctx, object[] args)
        {
            //_suspender?.Dispose();
            //_suspender = null;
            //N.Subscribe();
        }

    }
}
