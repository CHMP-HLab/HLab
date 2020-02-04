using System.Threading.Tasks;
using System.Windows;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    public static class IconServiceExtension
    {
        public static async Task<object> GetIconAsync(this IIconService service, string id, double height)
        {
            var icon = await service.GetIconAsync(id).ConfigureAwait(true);

            if (icon is FrameworkElement fe)
            {
                fe.MaxHeight = height;
            }

            return icon;
        }
    }
}
