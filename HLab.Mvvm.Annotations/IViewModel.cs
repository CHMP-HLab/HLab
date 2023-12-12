using System;
using System.Runtime.CompilerServices;

namespace HLab.Mvvm.Annotations;

public interface IMvvmContextProvider
{
    void ConfigureMvvmContext(IMvvmContext ctx) { }
}

internal class ViewModelHelper
{
    readonly IViewModel _viewModel;
    readonly Lazy<Type> _modelType;

    public ViewModelHelper(IViewModel viewModel)
    {
        _viewModel = viewModel;
            
        _modelType = new(() =>
        {
            var type = _viewModel.GetType();
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(IViewModel<>))
            {
                return type.GetGenericArguments()[0];
            }
            return null;

        });
    }

    public IMvvmContext? Context;
    public Type ModelType => _modelType.Value;

    public object? Model;
}

internal static class ViewModelExtensions
{
    public static ConditionalWeakTable<IViewModel,ViewModelHelper> Helpers = new ();

    public static ViewModelHelper GetHelper(IViewModel viewModel) =>
        Helpers.GetValue(viewModel, vm => new ViewModelHelper(vm));
}

public interface IViewModel
{
    public void RaiseAndSetIfChanged<TRet>(
        // ReSharper disable once RedundantAssignment
        ref TRet backingField,
        TRet newValue,
        [CallerMemberName] string? propertyName = null)
    {
        backingField = newValue;
    }


    IMvvmContext? MvvmContext 
    { 
        get => ViewModelExtensions.GetHelper(this).Context; 
        set => RaiseAndSetIfChanged(ref ViewModelExtensions.GetHelper(this).Context, value);
    }

    Type? ModelType => ViewModelExtensions.GetHelper(this).ModelType;

    object? Model
    { 
        get => ViewModelExtensions.GetHelper(this).Model;
        set => RaiseAndSetIfChanged(ref ViewModelExtensions.GetHelper(this).Model, value);
    }
}

public interface IViewModel<T> : IViewModel
where T : class?
{
     new T? Model
     {
         get => Unsafe.As<T?>(Unsafe.As<IViewModel>(this).Model);
         set => Unsafe.As<IViewModel>(this).Model = value;
     }
}