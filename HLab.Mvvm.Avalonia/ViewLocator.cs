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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Metadata;
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
    public class ViewLocator : ContentControl
    {
        bool _loaded = false;

        public static readonly StyledProperty<Type> ViewModeProperty =
            H.Property<Type>()
                .OnChangeBeforeNotification((e) =>
                {
                    // TODO : Avalonia
                    //if(a.NewValue ==  null) return;
                    //if(a.OldValue != null && a.NewValue == a.OldValue) return;

                    e.Update();
                })
                .Default(typeof(ViewModeDefault))
                .Inherits
                .RegisterAttached();

        public static readonly StyledProperty<Type> ViewClassProperty =
            H.Property<Type>()
                .OnChangeBeforeNotification((e) =>
                { 
                    // TODO : Avalonia
                    //if(a.NewValue ==  null) return;
                    //if(a.OldValue != null && ReferenceEquals(a.NewValue,a.OldValue)) return;

                    e.Update();
                })
                .Default(typeof(IViewClassDefault))
                .Inherits
                .RegisterAttached();

        public static readonly StyledProperty<IMvvmContext> MvvmContextProperty =
            H.Property<IMvvmContext>()
                .OnChangeBeforeNotification((e) =>
                {
                    // TODO : Avalonia
                    //if(a.NewValue ==  null) return;
                    //if(ReferenceEquals(a.NewValue,a.OldValue)) return;

                    e.Update();
                })
                .Default(null)
                .Inherits
                .RegisterAttached();

        bool _hasModel = false;
        object _oldModel = null;

        public static readonly StyledProperty<object> ModelProperty = H.Property<object>()
            .OnChangeBeforeNotification((e) =>
            {
                var oldModel = e._oldModel;
                var model = e.Model;

                if (e._hasModel)
                {
                    var o = e.Content;

                    while(o is StyledElement se)
                    {
                        if (ReferenceEquals(se.DataContext, oldModel))
                        {
                            se.DataContext = model;
                            return;
                        }
                        o = se.DataContext;
                    }

                    while (o is IViewModel vm)
                    {
                        if (ReferenceEquals(vm.Model, oldModel))
                        {
                            vm.Model = model;
                            return;
                        }

                        o = vm.Model;
                    }
                }
                e._oldModel = model;
                e._hasModel = true;

                e.Update();
            })
            .Register();

        public static object GetModel(AvaloniaObject obj)
            => obj.GetValue(ModelProperty);

        public static void SetModel(AvaloniaObject obj, object value)
            => obj.SetValue(ModelProperty, value);

        public static Type GetViewMode(AvaloniaObject obj)
            => (Type)obj.GetValue(ViewModeProperty);

        public static void SetViewMode(AvaloniaObject obj, Type value)
            => obj.SetValue(ViewModeProperty, value);

        public static Type GetViewClass(AvaloniaObject obj)
            => (Type)obj.GetValue(ViewClassProperty);

        public static void SetViewClass(AvaloniaObject obj, Type value)
            => obj.SetValue(ViewClassProperty, value);

        public static IMvvmContext GetMvvmContext(AvaloniaObject obj)
            => (IMvvmContext)obj.GetValue(MvvmContextProperty);

        public static void SetMvvmContext(AvaloniaObject obj, IMvvmContext value)
            => obj.SetValue(MvvmContextProperty, value);


        [Content]
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

            
            AttachedToVisualTree += ViewLocator_Loaded;
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

        async void ViewLocator_Loaded(object? sender, VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
        {
            _loaded = true;
            Update();
        }

        void ViewLocator_DataContextChanged(object? sender, EventArgs eventArgs)
        {
            if(!_hasModel) Dispatcher.UIThread.InvokeAsync(Update);
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

                var view = (StyledElement)context.GetView(model, viewMode, viewClass);
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
