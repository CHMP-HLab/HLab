using System.Windows;
using System.Windows.Media;

namespace HLab.Mvvm.Flowchart
{
    public static class HitTestHelper
    {
        public static HitTestResult HitTest(Visual visual, Point point)
        {
            // This 'HitTest' method also takes the 'IsHitTestVisible' and 'IsVisible' properties
            // into account, so use it instead of the normal VisualTreeHelper.HitTest instead!
            HitTestResult result = null;

            // Use the advanced HitTest method and specify a custom filter that filters out the
            // invisible elements or the elements that don't allow hittesting.
            VisualTreeHelper.HitTest(visual,
                (target) => {
                    if (target is UIElement uiElement && (!uiElement.IsHitTestVisible || !uiElement.IsVisible))
                        return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                    else
                        return HitTestFilterBehavior.Continue;
                },
                (target) => {
                    result = target;
                    return HitTestResultBehavior.Stop;
                },
                new PointHitTestParameters(point));

            // Return the result
            return result;
        }
    }
}
