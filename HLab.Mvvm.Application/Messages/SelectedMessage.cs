using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Application.Messages;

public class SelectedMessage : ISelectedMessage
{
    IView _view;
    IViewModel _viewModel;
    readonly IMvvmService _mvvm;

    public SelectedMessage(object item, IMvvmService mvvm)
    {
        _mvvm = mvvm;
        if (item is IView view)
            View = view;

        if (item is IViewModel vm)
            ViewModel = vm;
    }

    public IView View
    {
        get => _view;
        set
        {
            _view = value;
            if (_mvvm.ViewHelperFactory.Get(value).Linked is IViewModel vm)
            {
                ViewModel = vm;
            }
        }
    }

    public IViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            if (_viewModel.ModelType != null)
                Entity = value.Model;
        }
    }

    public object Entity { get; set; }
}
