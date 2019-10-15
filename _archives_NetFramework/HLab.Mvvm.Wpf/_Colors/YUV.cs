using System;

namespace HLab.Mvvm.Wpf._Colors
{
    /// <summary>
    /// Structure to define YUV.
    /// </summary>
    public struct YUV
    {
        /// <summary>
        /// Gets an empty YUV structure.
        /// </summary>
        public static readonly YUV Empty = new YUV();

        private double y;
        private double u;
        private double v;

        public static bool operator ==(YUV item1, YUV item2)
        {
            return (
                item1.Y == item2.Y
                && item1.U == item2.U
                && item1.V == item2.V
                );
        }

        public static bool operator !=(YUV item1, YUV item2)
        {
            return (
                item1.Y != item2.Y
                || item1.U != item2.U
                || item1.V != item2.V
                );
        }

        public double Y
        {
            get => y;
            set
            {
                y = value;
                y = (y > 1) ? 1 : ((y < 0) ? 0 : y);
            }
        }

        public double U
        {
            get => u;
            set
            {
                u = value;
                u = (u > 0.436) ? 0.436 : ((u < -0.436) ? -0.436 : u);
            }
        }

        public double V
        {
            get => v;
            set
            {
                v = value;
                v = (v > 0.615) ? 0.615 : ((v < -0.615) ? -0.615 : v);
            }
        }

        /// <summary>
        /// Creates an instance of a YUV structure.
        /// </summary>
        public YUV(double y, double u, double v)
        {
            this.y = (y > 1) ? 1 : ((y < 0) ? 0 : y);
            this.u = (u > 0.436) ? 0.436 : ((u < -0.436) ? -0.436 : u);
            this.v = (v > 0.615) ? 0.615 : ((v < -0.615) ? -0.615 : v);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return (this == (YUV)obj);
        }

        public override int GetHashCode()
        {
            return Y.GetHashCode() ^ U.GetHashCode() ^ V.GetHashCode();
        }

    }
}
