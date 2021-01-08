using System;

namespace HLab.Mvvm.Annotations
{
    public interface IMvvmContextProvider
    {
        void ConfigureMvvmContext(IMvvmContext ctx);
    }

    public interface IViewModel 
    {
        IMvvmContext MvvmContext { get; set; }
        Type ModelType { get; }

        object Model { get; set; }
    }

    public interface IViewModel<T> : IViewModel, IMvvmLinked<T>
    {
         new T Model { get; set; }
    }
}