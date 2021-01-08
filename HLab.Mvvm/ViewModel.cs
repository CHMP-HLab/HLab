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
using System.Threading;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm
{
    using H = H<ViewModel>;

    public interface IModel {}

    public abstract class ViewModel : NotifierBase, IViewModel
    {
        protected ViewModel() => H.Initialize(this);

        private static int _lastId = 0;

        private readonly Lazy<int> _id = new Lazy<int>(() => Interlocked.Increment(ref _lastId));

        public int Id => _id.Value;

        public IMvvmContext MvvmContext { get; set; }

        public virtual Type ModelType => null;
        public object Model
        {
            get => _model.Get();
            set => _model.Set(value);
        }
        private readonly IProperty<object> _model = H.Property<object>();
    }

    public abstract class ViewModel<T> : ViewModel, IViewModel<T>
    {

        public new T Model
        {
            get => (T)base.Model;
            set => base.Model = value;
        }

        public override Type ModelType => typeof(T);
    }
}
