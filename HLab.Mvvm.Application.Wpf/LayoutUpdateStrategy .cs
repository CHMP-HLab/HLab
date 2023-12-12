using System.Linq;
using System.Reflection;
using AvalonDock.Layout;
using HLab.Erp.Core;

namespace HLab.Mvvm.Application.Wpf
{
    internal class LayoutUpdateStrategy : ILayoutUpdateStrategy
    {
        bool BeforeInsertContent(LayoutRoot layout, LayoutContent anchorableToShow)
        {
            var viewModel = (IViewClassAnchorable)anchorableToShow.Content;
            var layoutContent = layout.Descendents().OfType<LayoutContent>().FirstOrDefault(x => x.ContentId == viewModel.ContentId);
            if (layoutContent == null)
                return false;
            //layoutContent.Content = anchorableToShow.Content;

            // Add layoutContent to it's previous container
            var layoutContainer = layoutContent.GetType().GetProperty("PreviousContainer", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(layoutContent, null) as ILayoutContainer;

            if (layoutContainer is LayoutAnchorablePane lap)
            {
                lap.Children.Add(anchorableToShow as LayoutAnchorable);
                lap.Children.Remove(layoutContent as LayoutAnchorable);

                layoutContent.Parent.RemoveChild(layoutContent);
            }

            else if (layoutContainer is LayoutDocumentPane ldc)
            {
                ldc.Children.Add(anchorableToShow);
                ldc.Children.Remove(layoutContent);
            }
            else
                return false;
                //throw new NotSupportedException();

            return true;
        }
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            return BeforeInsertContent(layout, anchorableToShow);
        }
        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown) { }
        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
            return BeforeInsertContent(layout, anchorableToShow);
        }
        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown) { }
    }
}
