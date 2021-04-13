using System.Linq;
using System.Threading.Tasks;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Application.Wpf.Icons
{
    public class IconListViewModel: EntityListViewModel<Icon>, IMvvmContextProvider
    {
        public IconListViewModel() 
        {
            AddAllowed = true;
            DeleteAllowed = true;
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns.Configure(c => c
                .Column.Header("{Path}")
                .Width(210)
                .Content(e => e.Path)
                .Column.Icon((s) => s.Path)
                .Width(70)
                .OrderBy(s => s.Path)
            );
                

            using (List.Suspender.Get())
            {
                Filter<TextFilter>(). Title("{Path}")
                    .Link(List,e => e.Path);
            }
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