namespace HLab.Mvvm.Annotations;

//public static class ViewExtensions
//{
//    public static IViewHelper GetHelper(this IView view) => MvvmService.D.ViewHelperFactory.Get(view);
//    public static void SetMvvmContext(this IView view, MvvmContext ctx) => view.GetHelper().Context = ctx;
//    public static MvvmContext GetMvvmContext(this IView view) => view.GetHelper().Context;
//    public static void SetLinked(this IView view, object linked) => view.GetHelper().Linked = linked;
//    public static object GetLinked(this IView view) => view.GetHelper().Linked;
//}

public interface IViewHelper
{
    IMvvmContext Context { get; set; }
    object Linked { get; set; }
}
