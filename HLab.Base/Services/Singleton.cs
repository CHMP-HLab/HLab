/*
  HLab.Base
  Copyright (c) 2017 Mathieu GRENET.  All right reserved.

  This file is part of HLab.Base.

    HLab.Base is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HLab.Base is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/

namespace HLab.Base.Services
{
    //public abstract class HLabSingleton<T>
    //    where T: HLabSingleton<T>//, new()
    //{
    //    // ReSharper disable once StaticMemberInGenericType
    //    private static bool _lazyCalled = false;
    //    private static readonly Lazy<T> Lazy =
    //        new Lazy<T>(() =>
    //        {
    //            _lazyCalled = true;
    //            T t = (T)Activator.CreateInstance(typeof(T),true);
    //            return t;
    //        });

    //    public static T D => Lazy.Value;

    //    protected HLabSingleton()
    //    {
    //        if(!_lazyCalled || Lazy.IsValueCreated)
    //            throw new InvalidOperationException("Constructing a " + typeof(T).Name +
    //            " manually is not allowed, use the " + nameof(D) + " property.");
    //    }
    //}


}
