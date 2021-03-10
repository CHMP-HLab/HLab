using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
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
                Filter<TextFilter>(f => f. Title("{Path}"))
                    .Link(List,e => e.Path);
            }
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}