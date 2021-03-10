using System;
using System.Resources;
using System.Threading.Tasks;
using HLab.Icons.Annotations;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons;

namespace HLab.Icons.Wpf.Providers
{
    public class IconProviderSvg : IIconProvider
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _name;
//        private readonly IIconService _icons;
        public IconProviderSvg(ResourceManager resourceManager, string name)
        { _resourceManager = resourceManager; _name = name; }



        public async Task<object> GetAsync()
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
            if (string.IsNullOrWhiteSpace(_name)) return null;

            await using var svg = _resourceManager.GetStream(_name);
            return await XamlTools.FromSvgStreamAsync(svg).ConfigureAwait(false);
        }

    }
}
