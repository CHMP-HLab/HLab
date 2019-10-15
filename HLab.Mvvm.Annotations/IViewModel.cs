using System;
using System.ComponentModel;

namespace HLab.Mvvm.Annotations
{
    public interface IMvvmContextProvider
    {
        //MvvmContext MvvmContext { get; }
        void ConfigureMvvmContext(IMvvmContext ctx);
    }

    public interface IViewModel 
    {
        IMvvmContext MvvmContext { get; set; }
        Type ModelType { get; }

        object Model { get; set; }
    }

    public interface IViewModel<T> : IViewModel
    //where T : INotifyPropertyChanged
    {
        new T Model { get; set; }
    }
}