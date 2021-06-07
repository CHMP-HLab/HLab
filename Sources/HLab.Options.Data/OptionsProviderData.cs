using System;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;

namespace HLab.Options.Data
{
    [Export(typeof(IOptionsProvider))]
    public class OptionsProviderData : IOptionsProvider
    {
        [Import]
        private IOptionsService _options;

        [Import]
        private IDataService _data;

        public async Task<T> GetValueAsync<T>(string name, int? userid=null, Func<T> defaultValue = null)
        {
            var o = await _data.FetchOneAsync<Option>(e => e.UserId==userid && e.Name==name) 
                    ?? await _data.FetchOneAsync<Option>(e => e.UserId == null && e.Name == name);


            return OptionsServices.GetValueFromString<T>(o?.Value);
        }

        public async Task SetValueAsync<T>(string name, T value, int? userId)
        {
            var o = await _data.FetchOneAsync<Option>(e => e.Name == name && e.UserId == userId).ConfigureAwait(false);

            if (o == null)
            {
                 o = await _data.AddAsync<Option>(e =>
                 {
                     e.Name = name;
                     e.UserId = userId;
                     e.Value = value.ToString();
                 }).ConfigureAwait(false);

                return;
            }

            o.Value = value.ToString();
            await _data.SaveAsync(o).ConfigureAwait(false);
        }
    }
}
