using Grace.DependencyInjection.Attributes;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Application
{
    public interface ISelectedMessage
    { }



    [Export(typeof(ISelectedMessage))]
    public class SelectedMessage : ISelectedMessage
    {
        private IView _view;
        private IViewModel _viewModel;
        private readonly IMvvmService _mvvm;

        public SelectedMessage(object item, [Import]IMvvmService mvvm)
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
                if(_viewModel.ModelType != null)
                    Entity = value.Model;
            }
            
        }

        public object Entity {get; set; }
    }
}
