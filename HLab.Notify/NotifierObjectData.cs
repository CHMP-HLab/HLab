using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using HLab.Notify.Annotations;

namespace HLab.Notify
{
    [DataContract]
    public class NotifierObjectData : INotifierObject
    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => N.Add(value);
            remove => N.Remove(value);
        }

        //[Import]
        //[Injected]
        //public virtual void SetNotifier(Func<INotifyPropertyChanged, INotifier> n)
        //{
        //    if(N==null)
        //        N = n(this);
        //    else
        //    {
        //        var nn = n(this);
        //    }
        //    //N.Subscribe();
        //}

        protected NotifierObjectData(Func<INotifyPropertyChanged,INotifier> n, bool subscribe = true)
        {
            N = n(this);
            // ReSharper disable once VirtualMemberCallInConstructor
            if(subscribe) N.Subscribe();
        }


        //protected NotifierObject(Func<INotifyPropertyChanged,INotifier> n)
        //{
        //    //_notifier = new Lazy<Notifier>(()=>NotifierService.D.GetNotifier(this)) ;
        //    Notifier = n(this);
        //    //if(init) N.Subscribe();
        //}
        public INotifier GetNotifier() => N;

        [NotMapped]
        protected INotifier N { get; set; }

        public virtual void OnSubscribe(INotifier n) { }
    }
}