using System.ComponentModel;
using System.Dynamic;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf.Icons
{
    public class IconHelper : DynamicObject, INotifyPropertyChanged
    {
        private readonly IIconService _service;
        public IconHelper(IIconService service)
        {
            _service = service;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _service.GetIcon(binder.Name.Replace("_","/"));
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}