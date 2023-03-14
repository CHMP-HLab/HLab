namespace HLab.ColorTools.Avalonia
{
    /// <summary>
    /// Structure to define CIE XYZ.
    /// </summary>
    public struct CIEXYZ
    {
        /// <summary>
        /// Gets an empty CIEXYZ structure.
        /// </summary>
        public static readonly CIEXYZ Empty = new CIEXYZ();
        /// <summary>
        /// Gets the CIE D65 (white) structure.
        /// </summary>
        public static readonly CIEXYZ D65 = new CIEXYZ(0.9505, 1.0, 1.0890);


        double _x;
        double _y;
        double _z;

        public static bool operator ==(CIEXYZ item1, CIEXYZ item2)
        {
            return (
                item1.X == item2.X
                && item1.Y == item2.Y
                && item1.Z == item2.Z
                );
        }

        public static bool operator !=(CIEXYZ item1, CIEXYZ item2)
        {
            return (
                item1.X != item2.X
                || item1.Y != item2.Y
                || item1.Z != item2.Z
                );
        }

        /// <summary>
        /// Gets or sets X component.
        /// </summary>
        public double X
        {
            get => _x;
            set => _x = (value > 0.9505) ? 0.9505 : ((value < 0) ? 0 : value);
        }

        /// <summary>
        /// Gets or sets Y component.
        /// </summary>
        public double Y
        {
            get => _y;
            set => _y = (value > 1.0) ? 1.0 : ((value < 0) ? 0 : value);
        }

        /// <summary>
        /// Gets or sets Z component.
        /// </summary>
        public double Z
        {
            get => _z;
            set => _z = (value > 1.089) ? 1.089 : ((value < 0) ? 0 : value);
        }

        public CIEXYZ(double x, double y, double z)
        {
            _x = (x > 0.9505) ? 0.9505 : ((x < 0) ? 0 : x);
            _y = (y > 1.0) ? 1.0 : ((y < 0) ? 0 : y);
            _z = (z > 1.089) ? 1.089 : ((z < 0) ? 0 : z);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return (this == (CIEXYZ)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

    }
}
