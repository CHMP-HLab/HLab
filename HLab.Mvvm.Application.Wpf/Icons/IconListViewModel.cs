using System;
using System.Threading.Tasks;

using HLab.Erp.Base.Data;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Application.Wpf.Icons
{
    public class IconListViewModel: Erp.Core.EntityLists.EntityListViewModel<Icon>, IMvvmContextProvider
    {
        protected override bool CanExecuteAdd(Action<string> errorAction) => true;
        protected override bool CanExecuteDelete(Icon arg, Action<string> errorAction) => true;

        protected override bool CanExecuteExport(Action<string> errorAction) => true;
        protected override bool CanExecuteImport(Action<string> errorAction) => true;

        public IconListViewModel(Injector i) : base(i, c => c
            .Column("Path")
            .Header("{Path}")
            .Width(210).Link(e => e.Path)
                    .Filter()
                .Column("Icon")
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