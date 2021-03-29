using System;
using System.Resources;
using System.Threading.Tasks;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderSvg : IIconProvider
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _name;
        private int? _foreground;

//        private readonly IIconService _icons;
        public IconProviderSvg(ResourceManager resourceManager, string name, int? foreground)
        {
            _resourceManager = resourceManager; 
            _name = name;
            _foreground = foreground;
        }



        public async Task<object> GetAsync()
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
            if (string.IsNullOrWhiteSpace(_name)) return null;

            await using var svg = _resourceManager.GetStream(_name);
            return await XamlTools.FromSvgStreamAsync(svg,_foreground).ConfigureAwait(false);
        }

    }
}
