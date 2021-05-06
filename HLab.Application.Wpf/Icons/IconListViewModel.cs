using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Application.Wpf.Icons
{
    public class IconListViewModel: EntityListViewModel<Icon>, IMvvmContextProvider
    {
        public IconListViewModel() : base(c => c
// TODO                .AddAllowed()
            // TODO                .DeleteAllowed()
            .Column()
            .Header("{Path}")
            .Width(210).Link(e => e.Path)
                    .Filter()
                .Column()
                    .Icon(s => s.Path)
                .Width(70)
//                .OrderBy(s => s.Path)            
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }


        protected override async Task ImportAsync(IDataService data, Icon newValue)
        {
            var done = false;
            var icons = data.FetchWhereAsync<Icon>(e => e.Path == newValue.Path);
            await foreach (var icon in icons)
            {
                icon.SourceXaml = newValue.SourceXaml;
                icon.SourceSvg = newValue.SourceSvg;
                icon.Foreground = newValue.Foreground;
                await data.UpdateAsync(icon, "SourceXaml", "SourceSvg", "Foreground");
                done = true;
            }

            if (!done) await data.AddAsync<Icon>(icon =>
            {
                icon.SourceXaml = newValue.SourceXaml;
                icon.SourceSvg = newValue.SourceSvg;
                icon.Foreground = newValue.Foreground;
            });
        }
    }
}