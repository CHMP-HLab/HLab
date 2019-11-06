/*
  HLab.Mvvm
  Copyright (c) 2017 Mathieu GRENET.  All right reserved.

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HLab.Base;
using HLab.Mvvm.Annotations;
using H = HLab.Base.DependencyHelper<HLab.Mvvm.ViewLocator>;
namespace HLab.Mvvm
{
    /// <inheritdoc />
    /// <summary>
    /// Logique d'interaction pour EntityViewLocator.xaml
    /// </summary>
    public class ViewLocator : UserControl
    {
        public static readonly DependencyProperty ViewModeProperty =
            DependencyHelper.Property<ViewLocator, Type>()
                .OnChange((s, a) =>
                {
                        s.Update(s.Model, a.OldValue, s.ViewClass);
                })
                .Default(typeof(ViewModeDefault))
                .Inherits.AffectsRender
                .RegisterAttached();

        public static readonly DependencyProperty ViewClassProperty =
            DependencyHelper.Property<ViewLocator, Type>()
                .OnChange((s, a) =>
                {
                        s.Update(s.Model, s.ViewMode, a.OldValue);
                })
                .Default(typeof(IViewClassDefault))
                .Inherits.AffectsRender
                .RegisterAttached();

        public static readonly DependencyProperty MvvmContextProperty =
            H.Property<IMvvmContext>()
                .OnChange((s, a) =>
                {
                        s.Update(s.Model, s.ViewMode, s.ViewClass);
                })
                .Default(null)
                .Inherits.AffectsRender
                .RegisterAttached();

        public static readonly DependencyProperty ModelProperty = H.Property<object>()
            //.BindsTwoWayByDefault
            .OnChange((vl, a) =>
            {
                vl.Update(a.OldValue, vl.ViewMode, vl.ViewClass);
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
        
        //public static ViewModeContext GetViewModeContext(DependencyObject obj)
        //{
        //    return (ViewModeContext)obj.GetValue(ViewModeContextProperty);
        //}
        //public static void SetViewModeContext(DependencyObject obj, ViewModeContext value)
        //{
        //    obj.SetValue(ViewModeContextProperty, value);
        //}

        //protected void OnViewModeContextChanged()
        //{
        //    ViewModeContextChanged?.Invoke(this,new EventArgs());
        //}

        public object Model
        {
            get => (object)GetValue(ModelProperty);
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
        }

        protected void Update(object oldModel, Type oldViewMode, Type oldViewClass)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            if (Content is FrameworkElement f)
            {
                f.DataContext = null;
            }

            var view = GetView();

            //FrameworkElement r = view;
            //var n = 0;
            //while (r != null)
            //{
            //    r = r.FindParent(view.GetType());
            //    n++;
            //}

            //if(n<5)
            Content = view;
            if (view != null)
            {
                SetViewClass(view,typeof(IViewClassDefault));
                SetViewMode(view,typeof(ViewModeDefault));
            }
        }

        private FrameworkElement GetView()
        {
            if (MvvmContext==null) return null;
            if (Model == null) return null;
            if (ViewMode == typeof(ViewModeCollapsed)) return null;

            if (ViewMode == null || ViewClass == null) return null; //return (FrameworkElement)MvvmContext.Mvvm.GetNotFoundView(Model.GetType(), ViewMode, ViewClass);

            //if (!Model.GetType().IsClass)
            //{
            //    return new ContentControl {Content = Model};
            //}

            return (FrameworkElement)MvvmContext.GetView(Model, ViewMode, ViewClass);
        }
    }
}
