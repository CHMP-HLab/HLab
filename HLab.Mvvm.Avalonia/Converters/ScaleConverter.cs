﻿/*
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

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace HLab.Mvvm.Avalonia.Converters
{
    public class ScaleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var v = value switch
            {
                double d => d,
                Control control => Math.Min(control.Bounds.Height, control.Bounds.Width),
                _ => 12
            };

            if (parameter is string s)
            {
                var scale = double.Parse(s, CultureInfo.InvariantCulture);

                scale = Math.Min(Math.Max(v * scale, 0.1), 35791);

                if (double.IsNaN(scale) || double.IsInfinity(scale)) scale = 0.1;

                if (targetType == typeof(double)) return scale;
                if (targetType == typeof(Thickness)) return new Thickness(scale);
                if (targetType == typeof(CornerRadius)) return new CornerRadius(scale);
            }

            return null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ScaleConverter : ConvertBack");
        }

    }
}