using System.ComponentModel;
using System.Dynamic;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons
{
    public class IconHelper : DynamicObject, INotifyPropertyChanged
    {
        readonly IIconService _service;
        public IconHelper(IIconService service)
        {
            _service = service;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _service.GetIconAsync(binder.Name.Replace("_","/"));
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}