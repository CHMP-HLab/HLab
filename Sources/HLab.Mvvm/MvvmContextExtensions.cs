using System;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    public static class MvvmContextExtensions
    {
        public static IView GetView(this IMvvmContext ctx, object baseObject, Type viewMode)
            => ctx.GetView(baseObject, viewMode, typeof(IViewClassDefault));
        public static IView GetView<T>(this IMvvmContext ctx, object baseObject, Type viewClass)
            => ctx.GetView(baseObject, typeof(T), viewClass);
        public static IView GetView<T>(this IMvvmContext ctx, object baseObject)
            => ctx.GetView(baseObject, typeof(T), typeof(IViewClassDefault));
        public static IView GetView<TMode,TClass>(this IMvvmContext ctx, object baseObject)
            => ctx.GetView(baseObject, typeof(TMode), typeof(TClass));
        public static object GetLinked<T>(this IMvvmContext ctx, object o, Type viewClass) => ctx.GetLinked(o, typeof(T), viewClass);

        public static object GetLinked<T>(this IMvvmContext ctx, object o) => ctx.GetLinked(o, typeof(T), typeof(IViewClassDefault));

        public static object GetLinked<TMode,TClass>(this IMvvmContext ctx, object o) => ctx.GetLinked(o, typeof(TMode), typeof(TClass));
    }
}