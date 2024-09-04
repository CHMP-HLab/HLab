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


using System.Runtime.CompilerServices;
using HLab.Base.Avalonia;
using HLab.Mvvm.Annotations;
using ReactiveUI;

namespace HLab.Mvvm.ReactiveUI;

public interface IModel {}

public abstract class ViewModel : ReactiveModel
{
    static int _lastId = 0;

    readonly Lazy<int> _id = new(() => Interlocked.Increment(ref _lastId));

    public int Id => _id.Value;
}

public abstract class ViewModel<T> : ViewModel, IViewModel<T> where T : class
{
    object? IViewModel.Model
    {
        get => Model;
        set => Model = Unsafe.As<T?>(value);
    }

    public T? Model
    {
        get => _model;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_model, value)) return;

            this.RaisePropertyChanging();

            _model = OnModelChanging(_model,value);

            this.RaisePropertyChanged();
        }
    }
    T? _model;

    protected virtual T? OnModelChanging(T? oldModel, T? newModel)
    {
        return newModel;
    }

    public Type ModelType => typeof(T);
}