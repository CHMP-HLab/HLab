namespace HLab.ColorTools
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

        public double Y { get; }

        public double U { get; }

        public double V { get; }

        /// <summary>
        /// Creates an instance of a YUV structure.
        /// </summary>
        public YUV(double y, double u, double v)
        {
            Y = (y > 1) ? 1 : ((y < 0) ? 0 : y);
            U = (u > 0.436) ? 0.436 : ((u < -0.436) ? -0.436 : u);
            V = (v > 0.615) ? 0.615 : ((v < -0.615) ? -0.615 : v);
        }

        public override bool Equals(object? obj)
        {
            if (obj is YUV yuv) return this == yuv;
                
                
            return false;
        }

        public override int GetHashCode()
        {
            return Y.GetHashCode() ^ U.GetHashCode() ^ V.GetHashCode();
        }

    }
}
