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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using HLab.Base;

namespace HLab.Mvvm.Observables
{
    public static class ObservableViewModelCollectionExt
    {
        //public static ObservableViewModelCollection<T> SetViewMode<T>(this ObservableViewModelCollection<T> col,  Type viewMode)
        //    where T : INotifyPropertyChanged
        //{
        //    return col.AddCreator( e =>
        //    {
        //        var vm = (T) col.ViewModeContext.GetLinked(e, viewMode);
        //        if (vm == null)
        //        {
        //            vm = (T) col.ViewModeContext.GetLinked(e, viewMode);
        //        }
        //        return vm;
        //    });
        //}
        //public static ObservableViewModelCollection<T> SetViewModeContext<T>(this ObservableViewModelCollection<T> col, ViewModeContext context)
        //    where T : INotifyPropertyChanged
        //{
        //    col.ViewModeContext = context;
        //    return col;
        //}


        public static T GetOrAdd<T>(this IList<T> col, Func<T, bool> comparator, Func<T> getter)
        {
            var lck = (col as ILockable)?.Lock;
            lck?.EnterReadLock();
            try
            {
                var item = col.FirstOrDefault(comparator);
                if (item != null) return item;
            }
            finally
            {
                lck?.ExitReadLock();
            }


            lck?.EnterWriteLock();
            try
            {
                var item = col.FirstOrDefault(comparator);

                if (item == null)
                {
                    item = getter();
                    col.Add(item);
                }

                return item;
            }
            finally
            {
                lck?.ExitWriteLock();
            }
        }


    }
}
