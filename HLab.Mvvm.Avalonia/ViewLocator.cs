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

using System.Collections.Concurrent;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using HLab.Base.Avalonia;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Avalonia
{
    using H = DependencyHelper<ViewLocator>;

    /// <inheritdoc />
    /// <summary>
    /// Logique d'interaction pour EntityViewLocator.xaml
    /// </summary>
    ///
    public class ViewLocator : ContentControl, IStyleable
    {
        bool _loaded = false;
        Type IStyleable.StyleKey => typeof(ContentControl);

        public static readonly StyledProperty<Type> ViewModeProperty =
            H.Property<Type>()
                .OnChanged((e,a) =>
                {
                    if(!a.NewValue.HasValue) return;
                    if(a.OldValue.HasValue && a.NewValue.Value == a.OldValue.Value) return;

                    e.Update();
                })
                .Default(typeof(DefaultViewMode))
                .Inherits
                .Attached
                .Register();

        public static readonly StyledProperty<Type> ViewClassProperty =
            H.Property<Type>()
                .OnChanged((e,a) =>
                { 
                    if(!a.NewValue.HasValue) return;
                    if(a.OldValue.HasValue && ReferenceEquals(a.NewValue.Value,a.OldValue.Value)) return;

                    e.Update();
                })
                .Default(typeof(IDefaultViewClass))
                .Inherits
                .Attached
                .Register();

        public static readonly StyledProperty<IMvvmContext?> MvvmContextProperty =
            H.Property<IMvvmContext?>()
                .OnChanged((e,a) =>
                {
                    if(!a.NewValue.HasValue) return;
                    if(a.OldValue.HasValue && ReferenceEquals(a.NewValue.Value,a.OldValue.Value)) return;

                    e.Update();
                })
                .Default(null)
                .Inherits
                .Attached
                .Register();

        object? _oldModel = null;

       public static readonly StyledProperty<object?> ModelProperty = H.Property<object?>()
            .OnChanged((e, a) =>
            {
                e.SetModel();
            })
            .Register();

        public static object? GetModel(AvaloniaObject obj)
            => obj.GetValue(ModelProperty);

        public static void SetModel(AvaloniaObject obj, object value)
            => obj.SetValue(ModelProperty, value);

        public static Type GetViewMode(AvaloniaObject obj)
            => obj.GetValue(ViewModeProperty);

        public static void SetViewMode(AvaloniaObject obj, Type value)
            => obj.SetValue(ViewModeProperty, value);

        public static Type GetViewClass(AvaloniaObject obj)
            => obj.GetValue(ViewClassProperty);

        public static void SetViewClass(AvaloniaObject obj, Type value)
            => obj.SetValue(ViewClassProperty, value);

        public static IMvvmContext? GetMvvmContext(AvaloniaObject obj)
            => obj.GetValue(MvvmContextProperty);

        public static void SetMvvmContext(AvaloniaObject obj, IMvvmContext value)
            => obj.SetValue(MvvmContextProperty, value);


        public object? Model
        {
            get => GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public Type ViewMode
        {
            get => GetValue(ViewModeProperty);
            set => SetValue(ViewModeProperty, value);
        }

        public Type ViewClass
        {
            get => GetValue(ViewClassProperty);
            set => SetValue(ViewClassProperty, value);
        }

        public IMvvmContext? MvvmContext
        {
            get => GetValue(MvvmContextProperty);
            set => SetValue(MvvmContextProperty, value);
        }

        public ViewLocator()
        {

            //DataContextChanged += ViewLocator_DataContextChanged;
            
            // AttachedToVisualTree += ViewLocator_Loaded;


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


        void SetModel()
        {
            var o = Content;

            while (o != null)
            {
                switch (o)
                {
                    case StyledElement se:
                        o = se.DataContext;
                        if (ReferenceEquals(o, _oldModel))
                        {
                            _oldModel = o;
                            se.DataContext = Model;
                            return;
                        }
                        break;
                    case IViewModel vm:
                        o = vm.Model;
                        if (ReferenceEquals(o, _oldModel))
                        {
                            _oldModel = o;
                            vm.Model = Model;
                            return;
                        }
                        break;
                    default:
                        o = null;
                        break;
                }

            }

            Update();
        }


        async void ViewLocator_Loaded(object? sender, VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
        {
            _loaded = true;
            Update();
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
            //if(!_loaded) return;

            var context = MvvmContext;
            var viewMode = ViewMode;
            var viewClass = ViewClass;
            var model = Model;

            Debug.Assert(viewMode != null);
            Debug.Assert(viewClass != null);

            if (context == null) return;
            if(model==null) return;

            if (Design.IsDesignMode) return;

            while (_cancel.TryPop(out var c))
            {
                c.Cancel();
            }


            var cancel = new Canceler();
            _cancel.Push(cancel);

            var t = Dispatcher.UIThread.InvokeAsync(() =>
            {
                if(cancel.State) return;

                var view = context.GetView(model, viewMode, viewClass);

                if(cancel.State) return;

                if (view is AvaloniaObject obj)
                {
                    SetViewClass(obj, typeof(IDefaultViewClass));
                    SetViewMode(obj, typeof(DefaultViewMode));
                }

                var old = Content;

                Content = view;

                if (old is IDisposable d)
                {
                    d.Dispose();
                }

                //InvalidateVisual();

            }, DispatcherPriority.Default);

        }

        
    }
}
