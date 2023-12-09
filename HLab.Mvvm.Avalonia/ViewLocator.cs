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
using Avalonia.Threading;
using HLab.Base.Avalonia.DependencyHelpers;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Avalonia;

using H = DependencyHelper<ViewLocator>;

/// <inheritdoc />
/// <summary>
/// Logique d'interaction pour EntityViewLocator.xaml
/// </summary>
///
public class ViewLocator : ContentControl 
{
    protected override Type StyleKeyOverride => typeof(ContentControl);

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
            e.SetModel(a.NewValue);
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
    }


    void SetModel(object? model)
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
                        _oldModel = model;
                        se.DataContext = model;
                        return;
                    }
                    break;

                case IViewModel vm:
                    o = vm.Model;
                    if (ReferenceEquals(o, _oldModel))
                    {
                        _oldModel = model;
                        vm.Model = model;
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

    readonly ConcurrentStack<CancellationTokenSource> _cancel = new();

    protected void Update()
    {
        var context = MvvmContext;
        var viewMode = ViewMode;
        var viewClass = ViewClass;
        var model = Model;

        Debug.Assert(viewMode != null);
        Debug.Assert(viewClass != null);

        if (context == null) return;
        if(model==null) return;

        if (Design.IsDesignMode) return;

        //cancel current running updates
        while (_cancel.TryPop(out var c))
        {
            c.Cancel();
        }

        var cancel = new CancellationTokenSource();
        _cancel.Push(cancel);

        var token = cancel.Token;
        var t = Dispatcher.UIThread.InvokeAsync(async() =>
        {
            if(token.IsCancellationRequested) return;

            var view = await context.GetViewAsync(model, viewMode, viewClass);

            var old = Content;

            Content = view;

            if (old is IDisposable d)
            {
                d.Dispose();
            }

            this.InvalidateVisual();

        }, DispatcherPriority.Default, cancel.Token);

    }
}