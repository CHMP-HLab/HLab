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

namespace HLab.Mvvm.Annotations;

public abstract class ViewMode { }
public class DefaultViewMode : ViewMode { }
public class DetailViewMode : DefaultViewMode { }
public class EditViewMode : DefaultViewMode { }
public class SummaryViewMode : DefaultViewMode { }
public class StringViewMode : DefaultViewMode { }
public class ListViewMode : DefaultViewMode { }
public class PreviewViewMode : DefaultViewMode { }
public class CollapsedViewMode : DefaultViewMode { }

public class DocumentViewMode : ViewMode { }
public class DraggableViewMode : ViewMode { }

public interface IViewClass { }
public interface IDefaultViewClass  : IViewClass{ }
public interface IListItemViewClass  : IViewClass{ }
public interface IContentViewClass  : IViewClass{ }

public interface IDesignViewModel { }


[MvvmCacheability(MvvmCacheability.NotCacheable)]
public interface IView
{
}

public interface IView<TViewMode, out TViewModel> : IView
    where TViewMode : ViewMode
{
}



public interface IView<out TViewModel> : IView<DefaultViewMode,TViewModel>
{
}

public interface IViewUnload
{
    void OnUnload();
}

public interface IModel
{ }

public enum MvvmCacheability
{
    Cacheable,
    NotCacheable
}

[AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface,Inherited = true)]
public class MvvmCacheabilityAttribute : Attribute
{
    public MvvmCacheabilityAttribute(MvvmCacheability cacheability)
    {
        Cacheability = cacheability;
    }

    public MvvmCacheability Cacheability { get; }
}
