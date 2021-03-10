using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using H = HLab.Base.Wpf.DependencyHelper<HLab.Mvvm.AsyncView>;

namespace HLab.Mvvm
{
    /// <summary>
    /// Logique d'interaction pour AsyncView.xaml
    /// </summary>
    public partial class AsyncView : UserControl
    {
        public static readonly DependencyProperty GetterProperty = 
            H.Property<Func<Task<object>>>()
                .OnChange((s, e) => s.Update(e.NewValue))
                .AffectsRender
                .Register();

        public Func<Task<object>> Getter
        {
            get => (Func<Task<object>>)GetValue(GetterProperty); 
            set => SetValue(GetterProperty, value);
        }

        public async void Update(Func<Task<object>> getter)
        {
            if(getter!=null)
                Content = await getter().ConfigureAwait(true);
        }
    }
}