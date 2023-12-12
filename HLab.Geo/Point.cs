﻿namespace HLab.Geo;
/// <summary>
/// Point - Defaults to 0,0
/// </summary>
/// 
[Serializable]
//[TypeConverter(typeof(PointConverter))]
//[ValueSerializer(typeof(PointValueSerializer))] // Used by MarkupWriter
    public partial struct Point : IFormattable
{
    //------------------------------------------------------
    //
    //  Public Methods
    //
    //------------------------------------------------------

    #region Public Methods

        /// <summary>
        /// Offset - update the location by adding offsetX to X and offsetY to Y
        /// </summary>
        /// <param name="offsetX"> The offset in the x dimension </param>
        /// <param name="offsetY"> The offset in the y dimension </param>
        public void Offset(double offsetX, double offsetY)
        {
            _x += offsetX;
            _y += offsetY;
        }

        /// <summary>
        /// Operator Point + Vector
        /// </summary>
        /// <returns>
        /// Point - The result of the addition
        /// </returns>
        /// <param name="point"> The Point to be added to the Vector </param>
        /// <param name="vector"> The Vectr to be added to the Point </param>
        public static Point operator + (Point point, Vector vector)
        {
             return new Point(point._x + vector._x, point._y + vector._y);
        }

        /// <summary>
        /// Add: Point + Vector
        /// </summary>
        /// <returns>
        /// Point - The result of the addition
        /// </returns>
        /// <param name="point"> The Point to be added to the Vector </param>
        /// <param name="vector"> The Vector to be added to the Point </param>
        public static Point Add(Point point, Vector vector)
        {
            return new Point(point._x + vector._x, point._y + vector._y);
        }

        /// <summary>
        /// Operator Point - Vector
        /// </summary>
        /// <returns>
        /// Point - The result of the subtraction
        /// </returns>
        /// <param name="point"> The Point from which the Vector is subtracted </param>
        /// <param name="vector"> The Vector which is subtracted from the Point </param>
        public static Point operator - (Point point, Vector vector)
        {
            return new Point(point._x - vector._x, point._y - vector._y);
        }

        /// <summary>
        /// Subtract: Point - Vector
        /// </summary>
        /// <returns>
        /// Point - The result of the subtraction
        /// </returns>
        /// <param name="point"> The Point from which the Vector is subtracted </param>
        /// <param name="vector"> The Vector which is subtracted from the Point </param>
        public static Point Subtract(Point point, Vector vector)
        {
            return new Point(point._x - vector._x, point._y - vector._y);
        }

        /// <summary>
        /// Operator Point - Point
        /// </summary>
        /// <returns>
        /// Vector - The result of the subtraction
        /// </returns>
        /// <param name="point1"> The Point from which point2 is subtracted </param>
        /// <param name="point2"> The Point subtracted from point1 </param>
        public static Vector operator - (Point point1, Point point2)
        {
            return new Vector(point1._x - point2._x, point1._y - point2._y);
        }

        /// <summary>
        /// Subtract: Point - Point
        /// </summary>
        /// <returns>
        /// Vector - The result of the subtraction
        /// </returns>
        /// <param name="point1"> The Point from which point2 is subtracted </param>
        /// <param name="point2"> The Point subtracted from point1 </param>
        public static Vector Subtract(Point point1, Point point2)
        {
            return new Vector(point1._x - point2._x, point1._y - point2._y);
        }

        /// <summary>
        /// Operator Point * Matrix
        /// </summary>
        public static Point operator * (Point point, Matrix matrix)
        {
            return matrix.Transform(point);
        }

        /// <summary>
        /// Multiply: Point * Matrix
        /// </summary>
        public static Point Multiply(Point point, Matrix matrix)
        {
            return matrix.Transform(point);
        }

        /// <summary>
        /// Explicit conversion to Size.  Note that since Size cannot contain negative values,
        /// the resulting size will contains the absolute values of X and Y
        /// </summary>
        /// <returns>
        /// Size - A Size equal to this Point
        /// </returns>
        /// <param name="point"> Point - the Point to convert to a Size </param>
        public static explicit operator Size(Point point)
        {
            return new Size(Math.Abs(point._x), Math.Abs(point._y));
        }

        /// <summary>
        /// Explicit conversion to Vector
        /// </summary>
        /// <returns>
        /// Vector - A Vector equal to this Point
        /// </returns>
        /// <param name="point"> Point - the Point to convert to a Vector </param>
        public static explicit operator Vector(Point point)
        {
            return new Vector(point._x, point._y);
        }

    /// <summary>
    /// Compares two Point instances for exact equality.
    /// Note that double values can acquire error when operated upon, such that
    /// an exact comparison between two values which are logically equal may fail.
    /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
    /// </summary>
    /// <returns>
    /// bool - true if the two Point instances are exactly equal, false otherwise
    /// </returns>
    /// <param name='point1'>The first Point to compare</param>
    /// <param name='point2'>The second Point to compare</param>
    public static bool operator == (Point point1, Point point2)
    {
        return point1.X == point2.X &&
               point1.Y == point2.Y;
    }

    /// <summary>
    /// Compares two Point instances for exact inequality.
    /// Note that double values can acquire error when operated upon, such that
    /// an exact comparison between two values which are logically equal may fail.
    /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
    /// </summary>
    /// <returns>
    /// bool - true if the two Point instances are exactly unequal, false otherwise
    /// </returns>
    /// <param name='point1'>The first Point to compare</param>
    /// <param name='point2'>The second Point to compare</param>
    public static bool operator != (Point point1, Point point2)
    {
        return !(point1 == point2);
    }
    /// <summary>
    /// Compares two Point instances for object equality.  In this equality
    /// Double.NaN is equal to itself, unlike in numeric equality.
    /// Note that double values can acquire error when operated upon, such that
    /// an exact comparison between two values which
    /// are logically equal may fail.
    /// </summary>
    /// <returns>
    /// bool - true if the two Point instances are exactly equal, false otherwise
    /// </returns>
    /// <param name='point1'>The first Point to compare</param>
    /// <param name='point2'>The second Point to compare</param>
    public static bool Equals (Point point1, Point point2)
    {
        return point1.X.Equals(point2.X) &&
               point1.Y.Equals(point2.Y);
    }

    /// <summary>
    /// Equals - compares this Point with the passed in object.  In this equality
    /// Double.NaN is equal to itself, unlike in numeric equality.
    /// Note that double values can acquire error when operated upon, such that
    /// an exact comparison between two values which
    /// are logically equal may fail.
    /// </summary>
    /// <returns>
    /// bool - true if the object is an instance of Point and if it's equal to "this".
    /// </returns>
    /// <param name='o'>The object to compare to "this"</param>
    public override bool Equals(object o)
    {
        if ((null == o) || !(o is Point))
        {
            return false;
        }

        Point value = (Point)o;
        return Point.Equals(this,value);
    }

    /// <summary>
    /// Equals - compares this Point with the passed in object.  In this equality
    /// Double.NaN is equal to itself, unlike in numeric equality.
    /// Note that double values can acquire error when operated upon, such that
    /// an exact comparison between two values which
    /// are logically equal may fail.
    /// </summary>
    /// <returns>
    /// bool - true if "value" is equal to "this".
    /// </returns>
    /// <param name='value'>The Point to compare to "this"</param>
    public bool Equals(Point value)
    {
        return Point.Equals(this, value);
    }
    /// <summary>
    /// Returns the HashCode for this Point
    /// </summary>
    /// <returns>
    /// int - the HashCode for this Point
    /// </returns>
    public override int GetHashCode()
    {
        // Perform field-by-field XOR of HashCodes
        return X.GetHashCode() ^
               Y.GetHashCode();
    }

    /// <summary>
    /// Parse - returns an instance converted from the provided string using
    /// the culture "en-US"
    /// <param name="source"> string with Point data </param>
    /// </summary>
    //public static Point Parse(string source)
    //{
    //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

    //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

    //    Point value;

    //    String firstToken = th.NextTokenRequired();

    //    value = new Point(
    //        Convert.ToDouble(firstToken, formatProvider),
    //        Convert.ToDouble(th.NextTokenRequired(), formatProvider));

    //    // There should be no more tokens in this string.
    //    th.LastTokenRequired();

    //    return value;
    //}

    #endregion Public Methods

    //------------------------------------------------------
    //
    //  Public Properties
    //
    //------------------------------------------------------




    #region Public Properties

    /// <summary>
    ///     X - double.  Default value is 0.
    /// </summary>
    public double X
    {
        get
        {
            return _x;
        }

        set
        {
            _x = value;
        }
    }

    /// <summary>
    ///     Y - double.  Default value is 0.
    /// </summary>
    public double Y
    {
        get
        {
            return _y;
        }

        set
        {
            _y = value;
        }
    }

    #endregion Public Properties

    //------------------------------------------------------
    //
    //  Protected Methods
    //
    //------------------------------------------------------

    #region Protected Methods





    #endregion ProtectedMethods

    //------------------------------------------------------
    //
    //  Internal Methods
    //
    //------------------------------------------------------

    #region Internal Methods









    #endregion Internal Methods

    //------------------------------------------------------
    //
    //  Internal Properties
    //
    //------------------------------------------------------

    #region Internal Properties


    /// <summary>
    /// Creates a string representation of this object based on the current culture.
    /// </summary>
    /// <returns>
    /// A string representation of this object.
    /// </returns>
    public override string ToString()
    {
        // Delegate to the internal method which implements all ToString calls.
        return ConvertToString(null /* format string */, null /* format provider */);
    }

    /// <summary>
    /// Creates a string representation of this object based on the IFormatProvider
    /// passed in.  If the provider is null, the CurrentCulture is used.
    /// </summary>
    /// <returns>
    /// A string representation of this object.
    /// </returns>
    public string ToString(IFormatProvider provider)
    {
        // Delegate to the internal method which implements all ToString calls.
        return ConvertToString(null /* format string */, provider);
    }

    /// <summary>
    /// Creates a string representation of this object based on the format string
    /// and IFormatProvider passed in.
    /// If the provider is null, the CurrentCulture is used.
    /// See the documentation for IFormattable for more information.
    /// </summary>
    /// <returns>
    /// A string representation of this object.
    /// </returns>
    string IFormattable.ToString(string format, IFormatProvider provider)
    {
        // Delegate to the internal method which implements all ToString calls.
        return ConvertToString(format, provider);
    }

    /// <summary>
    /// Creates a string representation of this object based on the format string
    /// and IFormatProvider passed in.
    /// If the provider is null, the CurrentCulture is used.
    /// See the documentation for IFormattable for more information.
    /// </summary>
    /// <returns>
    /// A string representation of this object.
    /// </returns>
    internal string ConvertToString(string format, IFormatProvider provider)
    {
        // Helper to get the numeric list separator for a given culture.
        char separator = ',';//MS.Internal.TokenizerHelper.GetNumericListSeparator(provider);
        return String.Format(provider,
            "{1:" + format + "}{0}{2:" + format + "}",
            separator,
            _x,
            _y);
    }



    #endregion Internal Properties

    //------------------------------------------------------
    //
    //  Dependency Properties
    //
    //------------------------------------------------------

    #region Dependency Properties



    #endregion Dependency Properties

    //------------------------------------------------------
    //
    //  Internal Fields
    //
    //------------------------------------------------------

    #region Internal Fields


    internal double _x;
    internal double _y;

    #endregion Internal Fields



    #region Constructors

    //------------------------------------------------------
    //
    //  Constructors
    //
    //------------------------------------------------------

    /// <summary>
    /// Constructor which accepts the X and Y values
    /// </summary>
    /// <param name="x">The value for the X coordinate of the new Point</param>
    /// <param name="y">The value for the Y coordinate of the new Point</param>
    public Point(double x, double y)
    {
        _x = x;
        _y = y;
    }

    #endregion Constructors
}