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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace HLab.Base.Avalonia.Converters;

public class MarginRatioConverter : IValueConverter
{
    public Rect PhysicalRect { get; set; }
    public Control? Element { get; set; }

    double Ratio => Math.Min(
        Element?.Bounds.Width??0 / PhysicalRect.Width,
        Element?.Bounds.Height??0 / PhysicalRect.Height
    );
    public double PhysicalToUiX(double x)
        => (x - PhysicalRect.Left) * Ratio
           + (Element?.Bounds.Width??0
               - PhysicalRect.Width * Ratio) / 2;
    public double PhysicalToUiY(double y)
        => (y - PhysicalRect.Top) * Ratio
           + (Element?.Bounds.Height??0
               - PhysicalRect.Height * Ratio) / 2;

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var rect = (Rect) value;

        return new Thickness(PhysicalToUiX(rect.X), PhysicalToUiY(rect.Y),0,0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return ((GridLength)value).Value / Ratio;
    }
}