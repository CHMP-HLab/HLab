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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using HLab.Base.Wpf;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf
{
    using H = DependencyHelper<ViewLocator>;

    /// <inheritdoc />
    /// <summary>
    /// Logique d'interaction pour EntityViewLocator.xaml
    /// </summary>
    ///
    [ContentProperty(nameof(Model))]
    public class ViewLocator : ContentControl
    {
        bool _loaded = false;

        public static readonly DependencyProperty ViewModeProperty =
            H.Property<Type>()
                .OnChange((e, a) =>
                {
                    if(a.NewValue ==  null) return;
                    if(a.OldValue != null && a.NewValue == a.OldValue) return;

                    e.Update();
                })
                .Default(typeof(ViewModeDefault))
                .Inherits
                .RegisterAttached();

        public static readonly DependencyProperty ViewClassProperty =
            H.Property<Type>()
                .OnChange((e, a) =>
                {
                    if(a.NewValue ==  null) return;
                    if(a.OldValue != null && ReferenceEquals(a.NewValue,a.OldValue)) return;

                    e.Update();
                })
                .Default(typeof(IViewClassDefault))
                .Inherits
                .RegisterAttached();

        public static readonly DependencyProperty MvvmContextProperty =
            H.Property<IMvvmContext>()
                .OnChange((e, a) =>
                {
                    if(a.NewValue ==  null) return;
                    if(ReferenceEquals(a.NewValue,a.OldValue)) return;

                    e.Update();
                })
                .Default(null)
                .Inherits
                .RegisterAttached();

        bool _hasModel = false;
        public static readonly DependencyProperty ModelProperty = H.Property<object>()
            .OnChange((e, a) =>
            {
                if (e._hasModel)
                {
                    var o = e.Content;
                        while(o is FrameworkElement fe)
                        {
                            if (ReferenceEquals(fe.DataContext, a.OldValue))
                            {
                                fe.DataContext = a.NewValue;
                                return;
                            }
                            o = fe.DataContext;
                        }

                        while (o is IViewModel vm)
                        {
                            if (ReferenceEquals(vm.Model, a.OldValue))
                            {
                                vm.Model = a.NewValue;
                                return;
                            }

                            o = vm.Model;
                        }
                }
                e._hasModel = true;

                e.Update();
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
            DataContextChanged += ViewLocator_DataContextChanged;
            Loaded += ViewLocator_Loaded;
            //var b = new Binding
            //{
            //    Source = this,
            //    Path = new PropertyPath("DataContext"),
            //    Mode = BindingMode.OneWay,
            //    //IsAsync = true
            //};
            //BindingOperations.SetBinding(this, ModelProperty, b);
            // Update();
        }

        async void ViewLocator_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
            Update();
        }

        void ViewLocator_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(!_hasModel) Dispatcher.InvokeAsync(Update);
        }

        class Canceler
        {
            public bool State { get; private set; }

            public void Cancel()
            {
                State = true;
            }
        }

        readonly ConcurrentStack<Canceler> _cancel = new();

        protected void Update()
        {
            if(!_loaded) return;
            var context = MvvmContext;
            var viewMode = ViewMode;
            var viewClass = ViewClass;
            var model = Model;

            if (viewMode == null) return;
            if (viewClass == null) return;
            if (context == null) return;
            if(model==null) return;

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            while (_cancel.TryPop(out var c))
            {
                c.Cancel();
            }


            var cancel = new Canceler();
            _cancel.Push(cancel);

            var t = Dispatcher.BeginInvoke(() =>
            {
                if(cancel.State) return;

                var view = (FrameworkElement)context.GetView(model, viewMode, viewClass);
                if(cancel.State) return;

                if (view != null)
                {
                    SetViewClass(view, typeof(IViewClassDefault));
                    SetViewMode(view, typeof(ViewModeDefault));
                }
                Content = view;
            }, DispatcherPriority.Input);

        }
    }
}
