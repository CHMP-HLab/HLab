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
using System.Threading;
using System.Threading.Tasks;
using HLab.Base;
using HLab.Core.Annotations;


////using System.Data.Model;

namespace HLab.Mvvm.Annotations;

//TODO : should not reference HLab.Base

public interface IMvvmService : IService
{
    IMvvmContext MainContext { get; }
    HelperFactory<IViewHelper> ViewHelperFactory { get; }
    Task<Type?> GetLinkedTypeAsync(Type getType, Type viewMode, Type viewClass, CancellationToken token = default);

    Task RegisterAsync();
    Task RegisterAsync(Type baseType, Type type, Type viewMode, Type viewClass);

    Task<IView> GetNotFoundViewAsync(Type getType, Type viewMode, Type viewClass, CancellationToken token = default);
    Task PrepareViewAsync(IView fe, CancellationToken token = default);
}
