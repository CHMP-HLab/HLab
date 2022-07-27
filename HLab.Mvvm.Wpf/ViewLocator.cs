/*
  HLab.Mvvm
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of HLab.Mvvm.

    HLab.Mvvm is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HLab.Mvvm is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using HLab.Base.Wpf;
using HLab.Mvvm.Annotations;
namespace HLab.Mvvm
{
    using H = DependencyHelper<ViewLocator>;

    /// <inheritdoc />
    /// <summary>
    /// Logique d'interaction pour EntityViewLocator.xaml
    /// </summary>
    /// 
    public class ViewLocator : UserControl
    {
        bool _loaded = false;

        Type _viewMode;
        public static readonly DependencyProperty ViewModeProperty =
            H.Property<Type>()
                .OnChange(async (e, a) =>
                {
                    if(a.NewValue ==  null) return;
                    if(e._viewMode != null && a.NewValue == e._viewMode) return;

                    e._viewMode = a.NewValue;
                    e._loaded = false;
                    await e.UpdateAsync();
                })
                .Default(typeof(ViewModeDefault))
                .Inherits
                .RegisterAttached();

        Type _viewClass;
        public static readonly DependencyProperty ViewClassProperty =
            H.Property<Type>()
                .OnChange(async (e, a) =>
                {
                    if(a.NewValue ==  null) return;
                    if(e._viewClass != null && a.NewValue == e._viewClass) return;

                    e._viewClass = a.NewValue;
                    e._loaded = false;
                    await e.UpdateAsync();
                })
                .Default(typeof(IViewClassDefault))
                .Inherits
                .RegisterAttached();

        WeakReference<IMvvmContext> _mvvmContextReference;

        public static readonly DependencyProperty MvvmContextProperty =
            H.Property<IMvvmContext>()
                .OnChange(async (e, a) =>
                {
                    if(a.NewValue ==  null) return;
                    if(e._mvvmContextReference!=null && e._mvvmContextReference.TryGetTarget(out var context) && ReferenceEquals(a.NewValue,context)) return;

                    e._mvvmContextReference = new(a.NewValue);
                    e._loaded = false;
                    await e.UpdateAsync();
                })
                .Default(null)
                .Inherits
                .RegisterAttached();

        WeakReference<object> _modelReference;
        public static readonly DependencyProperty ModelProperty = H.Property<object>()
            .OnChange(async (e, a) =>
            {
//                    if(a.NewValue ==  null) return;
                    if(e._modelReference!=null && e._modelReference.TryGetTarget(out var model) && ReferenceEquals(a.NewValue,model)) return;

                    e._modelReference = new(a.NewValue);
                    e._loaded = false;
                await e.UpdateAsync();
            })
            .Register();

        public static object GetModel(DependencyObject obj)
            => obj.GetValue(ModelProperty);

        public static void SetModel(DependencyObject obj, object value)
            => obj.SetValue(ModelProperty, value);

        public static Type GetViewMode(DependencyObject obj)
            => (Type)obj.GetValue(ViewModeProperty);

        public static void SetViewMode(DependencyObject obj, Type value)
            => obj.SetValue(ViewModeProperty, value);

        public static Type GetViewClass(DependencyObject obj)
            => (Type)obj.GetValue(ViewClassProperty);

        public static void SetViewClass(DependencyObject obj, Type value)
            => obj.SetValue(ViewClassProperty, value);

        public static IMvvmContext GetMvvmContext(DependencyObject obj)
            => (IMvvmContext)obj.GetValue(MvvmContextProperty);

        public static void SetMvvmContext(DependencyObject obj, IMvvmContext value)
            => obj.SetValue(MvvmContextProperty, value);

        public object Model
        {
            get => GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public Type ViewMode
        {
            get => (Type)GetValue(ViewModeProperty);
            set => SetValue(ViewModeProperty, value);
        }

        public Type ViewClass
        {
            get => (Type)GetValue(ViewClassProperty);
            set => SetValue(ViewClassProperty, value);
        }

        public IMvvmContext MvvmContext
        {
            get => (IMvvmContext)GetValue(MvvmContextProperty);
            set => SetValue(MvvmContextProperty, value);
        }

        public ViewLocator()
        {
            var b = new Binding
            {
                Source = this,
                Path = new PropertyPath("DataContext"),
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(this, ModelProperty, b);
            // Update();
        }


        int count = 0;
        protected async Task UpdateAsync()
        {

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            var view = GetView();

            if (view != null)
            {
                SetViewClass(view, typeof(IViewClassDefault));
                SetViewMode(view, typeof(ViewModeDefault));
            }

            if (count++ > 0) { }

            await Dispatcher.InvokeAsync(() =>
            {

                if (Content is FrameworkElement f)
                {
                    f.DataContext = null;
                }

                Content = view;
            });
        }

        FrameworkElement GetView()
        {
            if (MvvmContext == null) return null;

            if (Model == null) return null;

            if (ViewMode == typeof(ViewModeCollapsed)) return null;

            if (ViewMode == null || ViewClass == null) return null;

            return (FrameworkElement)MvvmContext.GetView(Model, ViewMode, ViewClass);
        }
    }
}
