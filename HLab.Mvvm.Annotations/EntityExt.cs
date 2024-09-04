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

using System.ComponentModel;


//using System.Data.Model;

namespace HLab.Mvvm.Annotations;

public static class EntityExt
{
    //public static TEntityViewModel ViewModel<TEntityViewModel, T>(this T entity)
    //    where TEntityViewModel : IViewModel<T>, new()
    //{
    //    var viewModel = new TEntityViewModel {Model = entity};
    //    return viewModel;
    //}

    //public static IView GetView<TViewMode>(this IViewModel viewModel,object model)
    //    where TViewMode : ViewMode
    //{
    //    return viewModel.MvvmContext.GetView<TViewMode>(model);
    //}

    //public static T GetLinked<T>(this INotifyPropertyChanged viewModel) => viewModel.Get<T>("#Linked");
    public static void SetLinked<T>(this IViewModel viewModel, T view)
    {
        //TODO
            //var token = viewModel.Get<SuspenderToken>("#EntityNullToken");
            //if (token != null && viewModel != null) token.Dispose();
            //if (viewModel.GetNotifier().Set(view, "#Linked"))
            //{
            //    //if (model == null) viewModel.Set<SuspenderToken>(viewModel.Suspend(), "#EntityNullToken");
            //}
    }

    //public static void SetModel<T,TViewModel>(this TViewModel viewModel, T model)
    //    where TViewModel : INotifyPropertyChanged, IViewModel<T>
    //{
    //    ((INotifyPropertyChanged)viewModel).SetModel(model);
    //}

    public static bool IsDbLinked(this INotifyPropertyChanged viewModel) => true;
}
