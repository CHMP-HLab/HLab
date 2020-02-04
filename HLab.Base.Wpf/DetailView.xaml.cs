using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
namespace HLab.Base
{
    using H = DependencyHelper<DetailView>;
    /// <summary>
    /// Logique d'interaction pour UserDetailView.xaml
    /// </summary>
    [ContentProperty(nameof(Children))]
    public partial class DetailView : UserControl
    {
        public DetailView()
        {
            InitializeComponent();
            Children = PART_Host.Children;
        }

        public static readonly DependencyPropertyKey ChildrenProperty = H.Property<UIElementCollection>().RegisterReadOnly();

        public UIElementCollection Children
        {
            get => (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty);
            private set => SetValue(ChildrenProperty, value);
        }
        private void UpdateChildren()
        {
            //if (Children.Count == _count) return;
            //_count = Children.Count;

            var rowSize = PART_Host.RowDefinitions.Count;
            var colSize = PART_Host.ColumnDefinitions.Count;

            var col = 0;
            var row = 0;
            for (var index = 0; index < Children.Count; index++)
            {
                var child = Children[index];
                if (child == null) continue;

                if (Grid.GetRow(child) != row)
                    Grid.SetRow(child, row);

                if (Grid.GetColumn(child) != col)
                    Grid.SetColumn(child, col);

                if (!(child is Border))
                {
                    col += Grid.GetColumnSpan(child);
                    if (col >= colSize)
                    {
                        col = 0;
                        row++;
                        //if(row >= rowSize)
                        //    RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    }
                }
            }
        }
    }
}
