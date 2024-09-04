using System.Diagnostics;

namespace HLab.Geo;

[Flags]
internal enum MatrixTypes
{
    TRANSFORM_IS_IDENTITY = 0,
    TRANSFORM_IS_TRANSLATION = 1,
    TRANSFORM_IS_SCALING = 2,
    TRANSFORM_IS_UNKNOWN = 4,
}

    internal static class MatrixUtil
    {
        /// <summary>
        /// TransformRect - Internal helper for perf
        /// </summary>
        /// <param name="rect"> The Rect to transform. </param>
        /// <param name="matrix"> The Matrix with which to transform the Rect. </param>
        internal static void TransformRect(ref Rect rect, ref Matrix matrix)
        {
            if (rect.IsEmpty)
            {
                return;
            }

            MatrixTypes matrixType = matrix._type;

            // If the matrix is identity, don't worry.
            if (matrixType == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return;
            }

            // Scaling
            if (0 != (matrixType & MatrixTypes.TRANSFORM_IS_SCALING))
            {
                rect._x *= matrix._m11;
                rect._y *= matrix._m22;
                rect._width *= matrix._m11;
                rect._height *= matrix._m22;

                // Ensure the width is always positive.  For example, if there was a reflection about the
                // y axis followed by a translation into the visual area, the width could be negative.
                if (rect._width < 0.0)
                {
                    rect._x += rect._width;
                    rect._width = -rect._width;
                }

                // Ensure the height is always positive.  For example, if there was a reflection about the
                // x axis followed by a translation into the visual area, the height could be negative.
                if (rect._height < 0.0)
                {
                    rect._y += rect._height;
                    rect._height = -rect._height;
                }
            }

            // Translation
            if (0 != (matrixType & MatrixTypes.TRANSFORM_IS_TRANSLATION))
            {
                // X
                rect._x += matrix._offsetX;

                // Y
                rect._y += matrix._offsetY;
            }

            if (matrixType == MatrixTypes.TRANSFORM_IS_UNKNOWN)
            {
                // Al Bunny implementation.
                Point point0 = matrix.Transform(rect.TopLeft);
                Point point1 = matrix.Transform(rect.TopRight);
                Point point2 = matrix.Transform(rect.BottomRight);
                Point point3 = matrix.Transform(rect.BottomLeft);

                // Width and height is always positive here.
                rect._x = Math.Min(Math.Min(point0.X, point1.X), Math.Min(point2.X, point3.X));
                rect._y = Math.Min(Math.Min(point0.Y, point1.Y), Math.Min(point2.Y, point3.Y));

                rect._width = Math.Max(Math.Max(point0.X, point1.X), Math.Max(point2.X, point3.X)) - rect._x;
                rect._height = Math.Max(Math.Max(point0.Y, point1.Y), Math.Max(point2.Y, point3.Y)) - rect._y;
            }
        }

        /// <summary>
        /// Multiplies two transformations, where the behavior is matrix1 *= matrix2.
        /// This code exists so that we can efficient combine matrices without copying
        /// the data around, since each matrix is 52 bytes.
        /// To reduce duplication and to ensure consistent behavior, this is the
        /// method which is used to implement Matrix * Matrix as well.
        /// </summary>
        internal static void MultiplyMatrix(ref Matrix matrix1, ref Matrix matrix2)
        {
            MatrixTypes type1 = matrix1._type;
            MatrixTypes type2 = matrix2._type;

            // Check for idents

            // If the second is ident, we can just return
            if (type2 == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return;
            }

            // If the first is ident, we can just copy the memory across.
            if (type1 == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                matrix1 = matrix2;
                return;
            }

            // Optimize for translate case, where the second is a translate
            if (type2 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
            {
                // 2 additions
                matrix1._offsetX += matrix2._offsetX;
                matrix1._offsetY += matrix2._offsetY;

                // If matrix 1 wasn't unknown we added a translation
                if (type1 != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix1._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }

                return;
            }

            // Check for the first value being a translate
            if (type1 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
            {
                // Save off the old offsets
                double offsetX = matrix1._offsetX;
                double offsetY = matrix1._offsetY;

                // Copy the matrix
                matrix1 = matrix2;

                matrix1._offsetX = offsetX * matrix2._m11 + offsetY * matrix2._m21 + matrix2._offsetX;
                matrix1._offsetY = offsetX * matrix2._m12 + offsetY * matrix2._m22 + matrix2._offsetY;

                if (type2 == MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix1._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                }
                else
                {
                    matrix1._type = MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
                return;
            }

            // The following code combines the type of the transformations so that the high nibble
            // is "this"'s type, and the low nibble is mat's type.  This allows for a switch rather
            // than nested switches.

            // trans1._type |  trans2._type
            //  7  6  5  4   |  3  2  1  0
            int combinedType = ((int)type1 << 4) | (int)type2;

            switch (combinedType)
            {
                case 34:  // S * S
                    // 2 multiplications
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    return;

                case 35:  // S * S|T
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX = matrix2._offsetX;
                    matrix1._offsetY = matrix2._offsetY;

                    // Transform set to Translate and Scale
                    matrix1._type = MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING;
                    return;

                case 50: // S|T * S
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX *= matrix2._m11;
                    matrix1._offsetY *= matrix2._m22;
                    return;

                case 51: // S|T * S|T
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX = matrix2._m11 * matrix1._offsetX + matrix2._offsetX;
                    matrix1._offsetY = matrix2._m22 * matrix1._offsetY + matrix2._offsetY;
                    return;
                case 36: // S * U
                case 52: // S|T * U
                case 66: // U * S
                case 67: // U * S|T
                case 68: // U * U
                    matrix1 = new Matrix(
                        matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21,
                        matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22,

                        matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21,
                        matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22,

                        matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 + matrix2._offsetX,
                        matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 + matrix2._offsetY);
                    return;
#if DEBUG
            default:
                Debug.Fail("Matrix multiply hit an invalid case: " + combinedType);
                break;
#endif
            }
        }        

        /// <summary>
        /// Applies an offset to the specified matrix in place.
        /// </summary>
        internal static void PrependOffset(
            ref Matrix matrix,
            double offsetX,
            double offsetY)
        {
            if (matrix._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                matrix = new Matrix(1, 0, 0, 1, offsetX, offsetY);
                matrix._type = MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }
            else
            {
                // 
                //  / 1   0   0 \       / m11   m12   0 \
                //  | 0   1   0 |   *   | m21   m22   0 |
                //  \ tx  ty  1 /       \  ox    oy   1 /
                //
                //       /   m11                  m12                     0 \
                //  =    |   m21                  m22                     0 |
                //       \   m11*tx+m21*ty+ox     m12*tx + m22*ty + oy    1 /
                //

                matrix._offsetX += matrix._m11 * offsetX + matrix._m21 * offsetY;
                matrix._offsetY += matrix._m12 * offsetX + matrix._m22 * offsetY;

                // It just gained a translate if was a scale transform. Identity transform is handled above.
                Debug.Assert(matrix._type != MatrixTypes.TRANSFORM_IS_IDENTITY);
                if (matrix._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }
    }

    internal static class DoubleUtil
    {
        // Const values come from sdk\inc\crt\float.h
        internal const double DBL_EPSILON  =   2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */
        internal const float  FLT_MIN      =   1.175494351e-38F; /* Number close to zero, where float.MinValue is -float.MaxValue */

        /// <summary>
        /// AreClose - Returns whether or not two doubles are "close".  That is, whether or 
        /// not they are within epsilon of each other.  Note that this epsilon is proportional
        /// to the numbers themselves to that AreClose survives scalar multiplication.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool AreClose(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            if(value1 == value2) return true;
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return(-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// LessThan - Returns whether or not the first double is less than the second double.
        /// That is, whether or not the first is strictly less than *and* not within epsilon of
        /// the other number.  Note that this epsilon is proportional to the numbers themselves
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the LessThan comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool LessThan(double value1, double value2)
        {
            return (value1 < value2) && !AreClose(value1, value2);
        }


        /// <summary>
        /// GreaterThan - Returns whether or not the first double is greater than the second double.
        /// That is, whether or not the first is strictly greater than *and* not within epsilon of
        /// the other number.  Note that this epsilon is proportional to the numbers themselves
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the GreaterThan comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool GreaterThan(double value1, double value2)
        {
            return (value1 > value2) && !AreClose(value1, value2);
        }

        /// <summary>
        /// LessThanOrClose - Returns whether or not the first double is less than or close to
        /// the second double.  That is, whether or not the first is strictly less than or within
        /// epsilon of the other number.  Note that this epsilon is proportional to the numbers 
        /// themselves to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the LessThanOrClose comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool LessThanOrClose(double value1, double value2)
        {
            return (value1 < value2) || AreClose(value1, value2);
        }

        /// <summary>
        /// GreaterThanOrClose - Returns whether or not the first double is greater than or close to
        /// the second double.  That is, whether or not the first is strictly greater than or within
        /// epsilon of the other number.  Note that this epsilon is proportional to the numbers 
        /// themselves to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the GreaterThanOrClose comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool GreaterThanOrClose(double value1, double value2)
        {
            return (value1 > value2) || AreClose(value1, value2);
        }

        /// <summary>
        /// IsOne - Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 1. </param>
        public static bool IsOne(double value)
        {
            return Math.Abs(value-1.0) < 10.0 * DBL_EPSILON;
        }

        /// <summary>
        /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 0. </param>
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * DBL_EPSILON;
        }

        // The Point, Size, Rect and Matrix class have moved to WinCorLib.  However, we provide
        // internal AreClose methods for our own use here.

        /// <summary>
        /// Compares two points for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='point1'>The first point to compare</param>
        /// <param name='point2'>The second point to compare</param>
        /// <returns>Whether or not the two points are equal</returns>
        public static bool AreClose(Point point1, Point point2)
        {
            return DoubleUtil.AreClose(point1.X, point2.X) && 
            DoubleUtil.AreClose(point1.Y, point2.Y);
        }

        /// <summary>
        /// Compares two Size instances for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='size1'>The first size to compare</param>
        /// <param name='size2'>The second size to compare</param>
        /// <returns>Whether or not the two Size instances are equal</returns>
        public static bool AreClose(Size size1, Size size2)
        {
            return DoubleUtil.AreClose(size1.Width, size2.Width) && 
                   DoubleUtil.AreClose(size1.Height, size2.Height);
        }
        
        /// <summary>
        /// Compares two Vector instances for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='vector1'>The first Vector to compare</param>
        /// <param name='vector2'>The second Vector to compare</param>
        /// <returns>Whether or not the two Vector instances are equal</returns>
        public static bool AreClose(Vector vector1, Vector vector2)
        { 
            return DoubleUtil.AreClose(vector1.X, vector2.X) && 
                   DoubleUtil.AreClose(vector1.Y, vector2.Y);
        }

        /// <summary>
        /// Compares two rectangles for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='rect1'>The first rectangle to compare</param>
        /// <param name='rect2'>The second rectangle to compare</param>
        /// <returns>Whether or not the two rectangles are equal</returns>
        public static bool AreClose(Rect rect1, Rect rect2)
        {
            // If they're both empty, don't bother with the double logic.
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            // At this point, rect1 isn't empty, so the first thing we can test is
            // rect2.IsEmpty, followed by property-wise compares.

            return (!rect2.IsEmpty) &&
                DoubleUtil.AreClose(rect1.X, rect2.X) &&
                DoubleUtil.AreClose(rect1.Y, rect2.Y) &&
                DoubleUtil.AreClose(rect1.Height, rect2.Height) &&
                DoubleUtil.AreClose(rect1.Width, rect2.Width);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsBetweenZeroAndOne(double val)
        {
            return (GreaterThanOrClose(val, 0) && LessThanOrClose(val, 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int DoubleToInt(double val)
        {
            return (0 < val) ? (int)(val + 0.5) : (int)(val - 0.5);
        }


        /// <summary>
        /// rectHasNaN - this returns true if this rect has X, Y , Height or Width as NaN.
        /// </summary>
        /// <param name='r'>The rectangle to test</param>
        /// <returns>returns whether the Rect has NaN</returns>        
        public static bool RectHasNaN(Rect r)
        {
            if (    double.IsNaN(r.X)
                 || double.IsNaN(r.Y) 
                 || double.IsNaN(r.Height)
                 || double.IsNaN(r.Width) )
            {
                return true;
            }
            return false;                               
        }
    }


public partial struct Matrix: IFormattable
{
        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods




        /// <summary>
        /// Compares two Matrix instances for exact equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Matrix instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='matrix1'>The first Matrix to compare</param>
        /// <param name='matrix2'>The second Matrix to compare</param>
        public static bool operator == (Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }
            else
            {
                return matrix1.M11 == matrix2.M11 &&
                       matrix1.M12 == matrix2.M12 &&
                       matrix1.M21 == matrix2.M21 &&
                       matrix1.M22 == matrix2.M22 &&
                       matrix1.OffsetX == matrix2.OffsetX &&
                       matrix1.OffsetY == matrix2.OffsetY;
            }
        }

        /// <summary>
        /// Compares two Matrix instances for exact inequality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Matrix instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='matrix1'>The first Matrix to compare</param>
        /// <param name='matrix2'>The second Matrix to compare</param>
        public static bool operator != (Matrix matrix1, Matrix matrix2)
        {
            return !(matrix1 == matrix2);
        }
        /// <summary>
        /// Compares two Matrix instances for object equality.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Matrix instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='matrix1'>The first Matrix to compare</param>
        /// <param name='matrix2'>The second Matrix to compare</param>
        public static bool Equals (Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }
            else
            {
                return matrix1.M11.Equals(matrix2.M11) &&
                       matrix1.M12.Equals(matrix2.M12) &&
                       matrix1.M21.Equals(matrix2.M21) &&
                       matrix1.M22.Equals(matrix2.M22) &&
                       matrix1.OffsetX.Equals(matrix2.OffsetX) &&
                       matrix1.OffsetY.Equals(matrix2.OffsetY);
            }
        }

        /// <summary>
        /// Equals - compares this Matrix with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Matrix and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Matrix))
            {
                return false;
            }

            Matrix value = (Matrix)o;
            return Matrix.Equals(this,value);
        }

        /// <summary>
        /// Equals - compares this Matrix with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Matrix to compare to "this"</param>
        public bool Equals(Matrix value)
        {
            return Matrix.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this Matrix
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Matrix
        /// </returns>
        public override int GetHashCode()
        {
            if (IsDistinguishedIdentity)
            {
                return c_identityHashCode;
            }
            else
            {
                // Perform field-by-field XOR of HashCodes
                return M11.GetHashCode() ^
                       M12.GetHashCode() ^
                       M21.GetHashCode() ^
                       M22.GetHashCode() ^
                       OffsetX.GetHashCode() ^
                       OffsetY.GetHashCode();
            }
        }

        /// <summary>
        /// Parse - returns an instance converted from the provided string using
        /// the culture "en-US"
        /// <param name="source"> string with Matrix data </param>
        /// </summary>
        //public static Matrix Parse(string source)
        //{
        //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

        //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

        //    Matrix value;

        //    String firstToken = th.NextTokenRequired();

        //    // The token will already have had whitespace trimmed so we can do a
        //    // simple string compare.
        //    if (firstToken == "Identity")
        //    {
        //        value = Identity;
        //    }
        //    else
        //    {
        //        value = new Matrix(
        //            Convert.ToDouble(firstToken, formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider));
        //    }

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
            if (IsIdentity)
            {
                return "Identity";
            }

            // Helper to get the numeric list separator for a given culture.
            char separator = ',';//MS.Internal.TokenizerHelper.GetNumericListSeparator(provider);
            return String.Format(provider,
                                 "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}",
                                 separator,
                                 _m11,
                                 _m12,
                                 _m21,
                                 _m22,
                                 _offsetX,
                                 _offsetY);
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







        #endregion Internal Fields



        #region Constructors

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------




        #endregion Constructors




    // the transform is identity by default
    // Actually fill in the fields - some (internal) code uses the fields directly for perf.
    private static Matrix s_identity = CreateIdentity();

    #region Constructor

    /// <summary>
    /// Creates a matrix of the form
    ///             / m11, m12, 0 \
    ///             | m21, m22, 0 |
    ///             \ offsetX, offsetY, 1 /
    /// </summary>
    public Matrix(double m11, double m12,
        double m21, double m22,
        double offsetX, double offsetY)
    {
        this._m11 = m11;
        this._m12 = m12;
        this._m21 = m21;
        this._m22 = m22;
        this._offsetX = offsetX;
        this._offsetY = offsetY;
        _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
        _padding = 0;

        // We will detect EXACT identity, scale, translation or
        // scale+translation and use special case algorithms.
        DeriveMatrixType();
    }

    #endregion Constructor

    #region Identity

    /// <summary>
    /// Identity
    /// </summary>
    public static Matrix Identity
    {
        get
        {
            return s_identity;
        }
    }

    /// <summary>
    /// Sets the matrix to identity.
    /// </summary>
    public void SetIdentity()
    {
        _type = MatrixTypes.TRANSFORM_IS_IDENTITY;
    }

    /// <summary>
    /// Tests whether or not a given transform is an identity transform
    /// </summary>
    public bool IsIdentity
    {
        get
        {
            return (_type == MatrixTypes.TRANSFORM_IS_IDENTITY ||
                    (_m11 == 1 && _m12 == 0 && _m21 == 0 && _m22 == 1 && _offsetX == 0 && _offsetY == 0));
        }
    }

    #endregion Identity

    #region Operators
    /// <summary>
    /// Multiplies two transformations.
    /// </summary>
    public static Matrix operator *(Matrix trans1, Matrix trans2)
    {
        MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
        trans1.Debug_CheckType();
        return trans1;
    }

    /// <summary>
    /// Multiply
    /// </summary>
    public static Matrix Multiply(Matrix trans1, Matrix trans2)
    {
        MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
        trans1.Debug_CheckType();
        return trans1;
    }

    #endregion Operators

    #region Combine Methods

    /// <summary>
    /// Append - "this" becomes this * matrix, the same as this *= matrix.
    /// </summary>
    /// <param name="matrix"> The Matrix to append to this Matrix </param>
    public void Append(Matrix matrix)
    {
        this *= matrix;
    }

    /// <summary>
    /// Prepend - "this" becomes matrix * this, the same as this = matrix * this.
    /// </summary>
    /// <param name="matrix"> The Matrix to prepend to this Matrix </param>
    public void Prepend(Matrix matrix)
    {
        this = matrix * this;
    }

    /// <summary>
    /// Rotates this matrix about the origin
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in degrees</param>
    public void Rotate(double angle)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        this *= CreateRotationRadians(angle * (Math.PI/180.0));
    }

    /// <summary>
    /// Prepends a rotation about the origin to "this"
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in degrees</param>
    public void RotatePrepend(double angle)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        this = CreateRotationRadians(angle * (Math.PI/180.0)) * this;
    }

    /// <summary>
    /// Rotates this matrix about the given point
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in degrees</param>
    /// <param name='centerX'>The centerX of rotation</param>
    /// <param name='centerY'>The centerY of rotation</param>
    public void RotateAt(double angle, double centerX, double centerY)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        this *= CreateRotationRadians(angle * (Math.PI/180.0), centerX, centerY);
    }

    /// <summary>
    /// Prepends a rotation about the given point to "this"
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in degrees</param>
    /// <param name='centerX'>The centerX of rotation</param>
    /// <param name='centerY'>The centerY of rotation</param>
    public void RotateAtPrepend(double angle, double centerX, double centerY)
    {
        angle %= 360.0; // Doing the modulo before converting to radians reduces total error
        this = CreateRotationRadians(angle * (Math.PI/180.0), centerX, centerY) * this;
    }

    /// <summary>
    /// Scales this matrix around the origin
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    public void Scale(double scaleX, double scaleY)
    {
        this *= CreateScaling(scaleX, scaleY);
    }

    /// <summary>
    /// Prepends a scale around the origin to "this"
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    public void ScalePrepend(double scaleX, double scaleY)
    {
        this = CreateScaling(scaleX, scaleY) * this;
    }

    /// <summary>
    /// Scales this matrix around the center provided
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    /// <param name="centerX">The centerX about which to scale</param>
    /// <param name="centerY">The centerY about which to scale</param>
    public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
    {
        this *= CreateScaling(scaleX, scaleY, centerX, centerY);
    }

    /// <summary>
    /// Prepends a scale around the center provided to "this"
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    /// <param name="centerX">The centerX about which to scale</param>
    /// <param name="centerY">The centerY about which to scale</param>
    public void ScaleAtPrepend(double scaleX, double scaleY, double centerX, double centerY)
    {
        this = CreateScaling(scaleX, scaleY, centerX, centerY) * this;
    }

    /// <summary>
    /// Skews this matrix
    /// </summary>
    /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
    /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
    public void Skew(double skewX, double skewY)
    {
        skewX %= 360;
        skewY %= 360;
        this *= CreateSkewRadians(skewX * (Math.PI/180.0),
            skewY * (Math.PI/180.0));
    }

    /// <summary>
    /// Prepends a skew to this matrix
    /// </summary>
    /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
    /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
    public void SkewPrepend(double skewX, double skewY)
    {
        skewX %= 360;
        skewY %= 360;
        this = CreateSkewRadians(skewX * (Math.PI/180.0),
            skewY * (Math.PI/180.0)) * this;
    }

    /// <summary>
    /// Translates this matrix
    /// </summary>
    /// <param name='offsetX'>The offset in the x dimension</param>
    /// <param name='offsetY'>The offset in the y dimension</param>
    public void Translate(double offsetX, double offsetY)
    {
        //
        // / a b 0 \   / 1 0 0 \    / a      b       0 \
        // | c d 0 | * | 0 1 0 | = |  c      d       0 |
        // \ e f 1 /   \ x y 1 /    \ e+x    f+y     1 /
        //
        // (where e = _offsetX and f == _offsetY)
        //

        if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
        {
            // Values would be incorrect if matrix was created using default constructor.
            // or if SetIdentity was called on a matrix which had values.
            //
            SetMatrix(1, 0,
                0, 1,
                offsetX, offsetY,
                MatrixTypes.TRANSFORM_IS_TRANSLATION);
        }
        else if (_type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
        {
            _offsetX += offsetX;
            _offsetY += offsetY;
        }
        else
        {
            _offsetX += offsetX;
            _offsetY += offsetY;

            // If matrix wasn't unknown we added a translation
            _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
        }

        Debug_CheckType();
    }

    /// <summary>
    /// Prepends a translation to this matrix
    /// </summary>
    /// <param name='offsetX'>The offset in the x dimension</param>
    /// <param name='offsetY'>The offset in the y dimension</param>
    public void TranslatePrepend(double offsetX, double offsetY)
    {
        this = CreateTranslation(offsetX, offsetY) * this;
    }

    #endregion Set Methods

    #region Transformation Services

    /// <summary>
    /// Transform - returns the result of transforming the point by this matrix
    /// </summary>
    /// <returns>
    /// The transformed point
    /// </returns>
    /// <param name="point"> The Point to transform </param>
    public Point Transform(Point point)
    {
        Point newPoint = point;
        MultiplyPoint(ref newPoint._x, ref newPoint._y);
        return newPoint;
    }

    /// <summary>
    /// Transform - Transforms each point in the array by this matrix
    /// </summary>
    /// <param name="points"> The Point array to transform </param>
    public void Transform(Point[] points)
    {
        if (points != null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                MultiplyPoint(ref points[i]._x, ref points[i]._y);
            }
        }
    }

    /// <summary>
    /// Transform - returns the result of transforming the Vector by this matrix.
    /// </summary>
    /// <returns>
    /// The transformed vector
    /// </returns>
    /// <param name="vector"> The Vector to transform </param>
    public Vector Transform(Vector vector)
    {
        Vector newVector = vector;
        MultiplyVector(ref newVector._x, ref newVector._y);
        return newVector;
    }

    /// <summary>
    /// Transform - Transforms each Vector in the array by this matrix.
    /// </summary>
    /// <param name="vectors"> The Vector array to transform </param>
    public void Transform(Vector[] vectors)
    {
        if (vectors != null)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                MultiplyVector(ref vectors[i]._x, ref vectors[i]._y);
            }
        }
    }

    #endregion Transformation Services

    #region Inversion

    /// <summary>
    /// The determinant of this matrix
    /// </summary>
    public double Determinant
    {
        get
        {
            switch (_type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    return 1.0;
                case MatrixTypes.TRANSFORM_IS_SCALING:
                case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    return(_m11  * _m22);
                default:
                    return(_m11  * _m22) - (_m12 * _m21);
            }
        }
    }

    /// <summary>
    /// HasInverse Property - returns true if this matrix is invertable, false otherwise.
    /// </summary>
    public bool HasInverse
    {
        get
        {
            return !DoubleUtil.IsZero(Determinant);
        }
    }

    /// <summary>
    /// Replaces matrix with the inverse of the transformation.  This will throw an InvalidOperationException
    /// if !HasInverse
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// This will throw an InvalidOperationException if the matrix is non-invertable
    /// </exception>
    public void Invert()
    {
        double determinant = Determinant;

        if (DoubleUtil.IsZero(determinant))
        {
            throw new System.InvalidOperationException("SR.Transform_NotInvertible");
        }

        // Inversion does not change the type of a matrix.
        switch (_type)
        {
            case MatrixTypes.TRANSFORM_IS_IDENTITY:
                break;
            case MatrixTypes.TRANSFORM_IS_SCALING:
            {
                _m11 = 1.0 / _m11;
                _m22 = 1.0 / _m22;
            }
                break;
            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                _offsetX = -_offsetX;
                _offsetY = -_offsetY;
                break;
            case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
            {
                _m11 = 1.0 / _m11;
                _m22 = 1.0 / _m22;
                _offsetX = -_offsetX * _m11;
                _offsetY = -_offsetY * _m22;
            }
                break;
            default:
            {
                double invdet = 1.0/determinant;
                SetMatrix(_m22 * invdet,
                    -_m12 * invdet,
                    -_m21 * invdet,
                    _m11 * invdet,
                    (_m21 * _offsetY - _offsetX * _m22) * invdet,
                    (_offsetX * _m12 - _m11 * _offsetY) * invdet,
                    MatrixTypes.TRANSFORM_IS_UNKNOWN);
            }
                break;
        }
    }

    #endregion Inversion

    #region Public Properties

    /// <summary>
    /// M11
    /// </summary>
    public double M11
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return 1.0;
            }
            else
            {
                return _m11;
            }
        }
        set
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(value, 0,
                    0, 1,
                    0, 0,
                    MatrixTypes.TRANSFORM_IS_SCALING);
            }
            else
            {
                _m11 = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_SCALING;
                }
            }
        }
    }

    /// <summary>
    /// M12
    /// </summary>
    public double M12
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return 0;
            }
            else
            {
                return _m12;
            }
        }
        set
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1, value,
                    0, 1,
                    0, 0,
                    MatrixTypes.TRANSFORM_IS_UNKNOWN);
            }
            else
            {
                _m12 = value;
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            }
        }
    }

    /// <summary>
    /// M22
    /// </summary>
    public double M21
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return 0;
            }
            else
            {
                return _m21;
            }
        }
        set
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1, 0,
                    value, 1,
                    0, 0,
                    MatrixTypes.TRANSFORM_IS_UNKNOWN);
            }
            else
            {
                _m21 = value;
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            }
        }
    }

    /// <summary>
    /// M22
    /// </summary>
    public double M22
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return 1.0;
            }
            else
            {
                return _m22;
            }
        }
        set
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1, 0,
                    0, value,
                    0, 0,
                    MatrixTypes.TRANSFORM_IS_SCALING);
            }
            else
            {
                _m22 = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_SCALING;
                }
            }
        }
    }

    /// <summary>
    /// OffsetX
    /// </summary>
    public double OffsetX
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return 0;
            }
            else
            {
                return _offsetX;
            }
        }
        set
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1, 0,
                    0, 1,
                    value, 0,
                    MatrixTypes.TRANSFORM_IS_TRANSLATION);
            }
            else
            {
                _offsetX = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }
    }

    /// <summary>
    /// OffsetY
    /// </summary>
    public double OffsetY
    {
        get
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return 0;
            }
            else
            {
                return _offsetY;
            }
        }
        set
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1, 0,
                    0, 1,
                    0, value,
                    MatrixTypes.TRANSFORM_IS_TRANSLATION);
            }
            else
            {
                _offsetY = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }
    }

    #endregion Public Properties

    #region Internal Methods
    /// <summary>
    /// MultiplyVector
    /// </summary>
    internal void MultiplyVector(ref double x, ref double y)
    {
        switch (_type)
        {
            case MatrixTypes.TRANSFORM_IS_IDENTITY:
            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                return;
            case MatrixTypes.TRANSFORM_IS_SCALING:
            case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                x *= _m11;
                y *= _m22;
                break;
            default:
                double xadd = y * _m21;
                double yadd = x * _m12;
                x *= _m11;
                x += xadd;
                y *= _m22;
                y += yadd;
                break;
        }
    }

    /// <summary>
    /// MultiplyPoint
    /// </summary>
    internal void MultiplyPoint(ref double x, ref double y)
    {
        switch (_type)
        {
            case MatrixTypes.TRANSFORM_IS_IDENTITY:
                return;
            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                x += _offsetX;
                y += _offsetY;
                return;
            case MatrixTypes.TRANSFORM_IS_SCALING:
                x *= _m11;
                y *= _m22;
                return;
            case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                x *= _m11;
                x += _offsetX;
                y *= _m22;
                y += _offsetY;
                break;
            default:
                double xadd = y * _m21 + _offsetX;
                double yadd = x * _m12 + _offsetY;
                x *= _m11;
                x += xadd;
                y *= _m22;
                y += yadd;
                break;
        }
    }

    /// <summary>
    /// Creates a rotation transformation about the given point
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in radians</param>
    internal static Matrix CreateRotationRadians(double angle)
    {
        return CreateRotationRadians(angle, /* centerX = */ 0, /* centerY = */ 0);
    }

    /// <summary>
    /// Creates a rotation transformation about the given point
    /// </summary>
    /// <param name='angle'>The angle to rotate specified in radians</param>
    /// <param name='centerX'>The centerX of rotation</param>
    /// <param name='centerY'>The centerY of rotation</param>
    internal static Matrix CreateRotationRadians(double angle, double centerX, double centerY)
    {
        Matrix matrix = new Matrix();

        double sin = Math.Sin(angle);
        double cos = Math.Cos(angle);
        double dx    = (centerX * (1.0 - cos)) + (centerY * sin);
        double dy    = (centerY * (1.0 - cos)) - (centerX * sin);

        matrix.SetMatrix( cos, sin,
            -sin, cos,
            dx,    dy,
            MatrixTypes.TRANSFORM_IS_UNKNOWN);

        return matrix;
    }

    /// <summary>
    /// Creates a scaling transform around the given point
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    /// <param name='centerX'>The centerX of scaling</param>
    /// <param name='centerY'>The centerY of scaling</param>
    internal static Matrix CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
    {
        Matrix matrix = new Matrix();

        matrix.SetMatrix(scaleX,  0,
            0, scaleY,
            centerX - scaleX*centerX, centerY - scaleY*centerY,
            MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION);

        return matrix;
    }

    /// <summary>
    /// Creates a scaling transform around the origin
    /// </summary>
    /// <param name='scaleX'>The scale factor in the x dimension</param>
    /// <param name='scaleY'>The scale factor in the y dimension</param>
    internal static Matrix CreateScaling(double scaleX, double scaleY)
    {
        Matrix matrix = new Matrix();
        matrix.SetMatrix(scaleX,  0,
            0, scaleY,
            0, 0,
            MatrixTypes.TRANSFORM_IS_SCALING);
        return matrix;
    }

    /// <summary>
    /// Creates a skew transform
    /// </summary>
    /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
    /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
    internal static Matrix CreateSkewRadians(double skewX, double skewY)
    {
        Matrix matrix = new Matrix();

        matrix.SetMatrix(1.0,  Math.Tan(skewY),
            Math.Tan(skewX), 1.0,
            0.0, 0.0,
            MatrixTypes.TRANSFORM_IS_UNKNOWN);

        return matrix;
    }

    /// <summary>
    /// Sets the transformation to the given translation specified by the offset vector.
    /// </summary>
    /// <param name='offsetX'>The offset in X</param>
    /// <param name='offsetY'>The offset in Y</param>
    internal static Matrix CreateTranslation(double offsetX, double offsetY)
    {
        Matrix matrix = new Matrix();

        matrix.SetMatrix(1, 0,
            0, 1,
            offsetX, offsetY,
            MatrixTypes.TRANSFORM_IS_TRANSLATION);

        return matrix;
    }

    #endregion Internal Methods

    #region Private Methods
    /// <summary>
    /// Sets the transformation to the identity.
    /// </summary>
    private static Matrix CreateIdentity()
    {
        Matrix matrix = new Matrix();
        matrix.SetMatrix(1, 0,
            0, 1,
            0, 0,
            MatrixTypes.TRANSFORM_IS_IDENTITY);
        return matrix;
    }

    ///<summary>
    /// Sets the transform to
    ///             / m11, m12, 0 \
    ///             | m21, m22, 0 |
    ///             \ offsetX, offsetY, 1 /
    /// where offsetX, offsetY is the translation.
    ///</summary>
    private void SetMatrix(double m11, double m12,
        double m21, double m22,
        double offsetX, double offsetY,
        MatrixTypes type)
    {
        this._m11 = m11;
        this._m12 = m12;
        this._m21 = m21;
        this._m22 = m22;
        this._offsetX = offsetX;
        this._offsetY = offsetY;
        this._type = type;
    }

    /// <summary>
    /// Set the type of the matrix based on its current contents
    /// </summary>
    private void DeriveMatrixType()
    {
        _type = 0;

        // Now classify our matrix.
        if (!(_m21 == 0 && _m12 == 0))
        {
            _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            return;
        }

        if (!(_m11 == 1 && _m22 == 1))
        {
            _type = MatrixTypes.TRANSFORM_IS_SCALING;
        }

        if (!(_offsetX == 0 && _offsetY == 0))
        {
            _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
        }

        if (0 == (_type & (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING)))
        {
            // We have an identity matrix.
            _type = MatrixTypes.TRANSFORM_IS_IDENTITY;
        }
        return;
    }

    /// <summary>
    /// Asserts that the matrix tag is one of the valid options and
    /// that coefficients are correct.   
    /// </summary>
    [Conditional("DEBUG")]
    private void Debug_CheckType()
    {
        switch(_type)
        {
            case MatrixTypes.TRANSFORM_IS_IDENTITY:
                return;
            case MatrixTypes.TRANSFORM_IS_UNKNOWN:
                return;
            case MatrixTypes.TRANSFORM_IS_SCALING:
                Debug.Assert(_m21 == 0);
                Debug.Assert(_m12 == 0);
                Debug.Assert(_offsetX == 0);
                Debug.Assert(_offsetY == 0);
                return;
            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                Debug.Assert(_m21 == 0);
                Debug.Assert(_m12 == 0);
                Debug.Assert(_m11 == 1);
                Debug.Assert(_m22 == 1);
                return;
            case MatrixTypes.TRANSFORM_IS_SCALING|MatrixTypes.TRANSFORM_IS_TRANSLATION:
                Debug.Assert(_m21 == 0);
                Debug.Assert(_m12 == 0);
                return;
            default:
                Debug.Assert(false);
                return;
        }
    }

    #endregion Private Methods
    
    #region Private Properties and Fields

    /// <summary>
    /// Efficient but conservative test for identity.  Returns
    /// true if the the matrix is identity.  If it returns false
    /// the matrix may still be identity.
    /// </summary>
    private bool IsDistinguishedIdentity
    {
        get
        {
            return _type == MatrixTypes.TRANSFORM_IS_IDENTITY;
        }
    }

    // The hash code for a matrix is the xor of its element's hashes.
    // Since the identity matrix has 2 1's and 4 0's its hash is 0.
    private const int c_identityHashCode = 0;
    
    #endregion Private Properties and Fields

    internal double _m11;
    internal double _m12;
    internal double _m21;
    internal double _m22;
    internal double _offsetX;
    internal double _offsetY;
    internal MatrixTypes _type;

// This field is only used by unmanaged code which isn't detected by the compiler.
#pragma warning disable 0414
    // Matrix in blt'd to unmanaged code, so this is padding 
    // to align structure.
    //
    // Testing note: Validate that this blt will work on 64-bit
    //
    internal Int32 _padding;
#pragma warning restore 0414


}