using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HLab.Base
{
    [DesignTimeVisible]
    public class PropertyGrid : Grid
    {
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            UpdateChildren();
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            UpdateChildren();

            var children = Children.OfType<UIElement>();
            var uiElements = children.ToList();
            if (uiElements.Any())
            {
                var maxRowIndex = uiElements.Max(GetRow);

                for (var i = RowDefinitions.Count; i <= maxRowIndex; i++)
                    RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            }

            return base.MeasureOverride(constraint);
        }

        void UpdateChildren()
        {
            //if (Children.Count == _count) return;
            //_count = Children.Count;

            var rowSize = RowDefinitions.Count;
            var colSize = ColumnDefinitions.Count;

            var col = 0;
            var row = 0;
            for (var index = 0; index < Children.Count; index++)
            {
                var child = Children[index];
                if (child == null) continue;

                if(GetRow(child)!=row)
                    SetRow(child, row);

                if(GetColumn(child)!=col)
                    SetColumn(child, col);

                if (!(child is Border))
                {
                    col+=GetColumnSpan(child);
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
