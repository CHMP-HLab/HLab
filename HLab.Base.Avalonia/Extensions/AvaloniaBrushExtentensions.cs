using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;

namespace HLab.Base.Avalonia.Extensions
{
    public static class AvaloniaBrushExtensions
    {

        public struct WeightedColor
        {

            public double W { get; init; }
            public double A { get; init; }
            public double R { get; init; }
            public double G { get; init; }
            public double B { get; init; }

            public static WeightedColor FromColor(Color c, double w) => new WeightedColor()
            {
                A = c.A / 255.0,
                R = c.R / 255.0,
                G = c.G / 255.0,
                B = c.B / 255.0,
                W = w
            };

            public Color Color => new Color((byte)(A * 255.0), (byte)(R * 255.0), (byte)(G * 255.0), (byte)(B * 255.0));

            public static WeightedColor Average(params WeightedColor[] color)
            {
                var w = 0.0;

                var a = 0.0;

                var r = 0.0;
                var g = 0.0;
                var b = 0.0;

                foreach (var wc in color)
                {
                    w += wc.W;
                    a += wc.A * wc.W;
                    r += wc.R * wc.W;
                    g += wc.G * wc.W;
                    b += wc.B * wc.W;
                }

                return new WeightedColor
                {
                    A = a / w,
                    R = r / w,
                    G = g / w,
                    B = b / w,
                    W = w
                };

            }
        }



        public static Color AverageColor(this IEnumerable<IGradientStop> @this)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            var stops = @this.ToArray();

            double w;

            var color = stops[0].Color;
            var offset = 0.0;

            //            Span<WeightedColor> wc = stackalloc WeightedColor[@this.Count+1];
            var wc = new WeightedColor[stops.Length + 1];

            var i = 0;
            while (i < stops.Length)
            {
                w = (stops[i].Offset - offset) / 2;

                wc[i] = WeightedColor.Average(
                    WeightedColor.FromColor(color, w),
                    WeightedColor.FromColor(stops[i].Color, w)
                    );

                color = stops[i].Color;
                offset = stops[i].Offset;
                i++;
            }

            w = (1.0 - offset) / 2;

            wc[i] = WeightedColor.Average(
                WeightedColor.FromColor(color, w),
                WeightedColor.FromColor(stops[i].Color, w)
                );

            return WeightedColor.Average(wc).Color;
        }
    }
}
