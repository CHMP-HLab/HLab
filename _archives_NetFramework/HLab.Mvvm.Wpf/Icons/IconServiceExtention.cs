using System.Windows;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf.Icons
{
    public static class IconServiceExtension
    {
        public static object GetIcon(this IIconService service, string id, double height)
        {
            var icon = service.GetIcon(id);

            if (icon is FrameworkElement fe)
            {
                fe.MaxHeight = height;
            }

            return icon;
        }
    }
}
