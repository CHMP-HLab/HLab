namespace HLab.ColorTools.Wpf
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

        private double _y;
        private double _u;
        private double _v;

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
            get => _y;
            set
            {
                _y = value;
                _y = (_y > 1) ? 1 : ((_y < 0) ? 0 : _y);
            }
        }

        public double U
        {
            get => _u;
            set
            {
                _u = value;
                _u = (_u > 0.436) ? 0.436 : ((_u < -0.436) ? -0.436 : _u);
            }
        }

        public double V
        {
            get => _v;
            set
            {
                _v = value;
                _v = (_v > 0.615) ? 0.615 : ((_v < -0.615) ? -0.615 : _v);
            }
        }

        /// <summary>
        /// Creates an instance of a YUV structure.
        /// </summary>
        public YUV(double y, double u, double v)
        {
            this._y = (y > 1) ? 1 : ((y < 0) ? 0 : y);
            this._u = (u > 0.436) ? 0.436 : ((u < -0.436) ? -0.436 : u);
            this._v = (v > 0.615) ? 0.615 : ((v < -0.615) ? -0.615 : v);
        }

        public override bool Equals(object obj)
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
