using System;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    public class IconProviderSvgFromSource : IIconProvider
    {
        private readonly string _name;
        private readonly string _source;
 
        public IconProviderSvgFromSource(string source, string name)
        { _source = source; _name = name;}
        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            return await XamlTools.FromSvgStringAsync(_source).ConfigureAwait(false);
        }
    }
    public class IconProviderXamlFromSource : IIconProvider
    {
        private readonly string _name;
        private readonly string _source;
 
        public IconProviderXamlFromSource(string source, string name)
        { _source = source; _name = name; }
        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            return await XamlTools.FromXamlStringAsync(_source).ConfigureAwait(false);
        }
    }

    public class IconProviderXamlFromResource : IIconProvider
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _name;
 
        public IconProviderXamlFromResource(ResourceManager resourceManager, string name)
        { _resourceManager = resourceManager; _name = name; }
        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            //var resourceManager = new ResourceManager(_assembly.GetName().Name + ".g", _assembly);
            await using var xamlStream = _resourceManager.GetStream(_name);
            if (xamlStream == null) return null;

            return await XamlTools.FromXamlStreamAsync(xamlStream).ConfigureAwait(false);
        }
    }

    public class IconProviderXamlFromUri : IIconProvider
    {
        private readonly Uri _uri;
        public IconProviderXamlFromUri(Uri uri)
        {
            _uri = uri;
        }

        
        public async Task<object> GetAsync()
        {
            return Application.LoadComponent(_uri);
        }
    }
}
