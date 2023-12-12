using System.Windows;
using System.Windows.Controls;
using HLab.Base.Wpf;
using HLab.Erp.Core;
using HLab.Icons.Annotations.Icons;
using HLab.Mvvm.Wpf;

namespace HLab.Mvvm.Application.Wpf.Icons
{
    using H = DependencyHelper<IconSelectorView>;

    /// <summary>
    /// Logique d'interaction pour IconSelectorView.xaml
    /// </summary>
    public partial class IconSelectorView : UserControl
    {
        public IconSelectorView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PathProperty = H.Property<string>().BindsTwoWayByDefault/*.OnChange((e,a) => e.OnPathChanged(a))*/.Register();

        public string Path
        {
            get => (string) GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //var icon = ViewLocator.GetMvvmContext(this).Locate<IIconService>().GetIconAsync(Path);
            //ViewLocator.GetMvvmContext(this).Locate<IDocumentService>().OpenDocumentAsync(icon);
        }

        //[Import] private Func<IconListViewModel> _getIconListViewModel;

        void ButtonDropDown_OnClick(object sender, RoutedEventArgs e)
        {
            if (Popup.IsOpen) return;

            Popup.IsOpen = true;
            var ctx = ViewLocator.GetMvvmContext(this);

            var vm = ctx.Locate<IconListViewModel>(this);

            if (vm is IEntityListViewModel lvm)
            {
                lvm.SetSelectAction(t =>
                {
                    Popup.IsOpen = false;
                    if(vm.Selected!=null)
                        Path = vm.Selected.Path;
                });
            }

            //var view = ctx.GetView(vm,typeof(ViewModeDefault),typeof(IViewClassDefault));
            PopupContent.Content = vm;
        }

    }
}
