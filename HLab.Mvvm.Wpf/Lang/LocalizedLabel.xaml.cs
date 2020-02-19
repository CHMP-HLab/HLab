using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HLab.Base;

namespace HLab.Mvvm.Lang
{
    using H = DependencyHelper<LocalizedLabel>;
    /// <summary>
    /// Logique d'interaction pour LocalizedLabel.xaml
    /// </summary>
    [ContentProperty(nameof(Text))]
    public partial class LocalizedLabel : UserControl
    {
        public LocalizedLabel()
        {
            InitializeComponent();
        }
        public string Text
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }
        public static readonly DependencyProperty IdProperty =
            H.Property<string>()
                .AffectsRender
                .Register();
    }
}
