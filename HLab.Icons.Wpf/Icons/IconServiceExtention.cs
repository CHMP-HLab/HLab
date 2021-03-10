using System.Threading.Tasks;
using System.Windows;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons
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
