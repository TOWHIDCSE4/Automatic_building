using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Utilities for mathematical calculations.
    /// </summary>
    [Serializable]
    public class MathUtils
    {
        #region Methods

        /// <summary>
        /// Calculates the cross products of two 3D vectors.
        /// </summary>
        /// <param name="v0">The first 3D vector.</param>
        /// <param name="v1">The second 3D vector.</param>
        /// 
        /// <returns>
        /// A 3D vector that is the cross product of the two 3D vectors
        /// provided.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="v0"/> or <paramref name="v1"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="v0"/> or <paramref name="v1"/> is
        /// not a 3D vector.
        /// </exception>
        public static Vector<double> CrossProduct(
            Vector<double> v0, Vector<double> v1
        )
        {
            if (v0 == null)
            {
                throw new ArgumentNullException(nameof(v0));
            }

            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }

            if (v0.Count != 3)
            {
                throw new ArgumentException(
                    $"{nameof(v0)} must be a 3D vector",
                    nameof(v0)
                );
            }

            if (v1.Count != 3)
            {
                throw new ArgumentException(
                    $"{nameof(v1)} must be a 3D vector",
                    nameof(v1)
                );
            }

            Vector<double> crossProduct = new DenseVector(
                new[]
                {
                    v0[1] * v1[2] - v0[2] * v1[1],
                    v0[2] * v1[0] - v0[0] * v1[2],
                    v0[0] * v1[1] - v0[1] * v1[0]
                }
            );

            return crossProduct;
        }

        /// <summary>
        /// Calculates the angle from
        /// 3D vector <paramref name="v0"/> to
        /// 3D vector <paramref name="v1"/> on the X-Y Plane,
        /// i.e. the angle between their projections on X-Y Plane.
        /// </summary>
        /// 
        /// <param name="v0">The first 3D vector.</param>
        /// <param name="v1">The second 3D vector.</param>
        /// 
        /// <returns>
        /// The angle from <paramref name="v0"/> to <paramref name="v1"/>
        /// on X-Y Plane.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="v0"/> or <paramref name="v1"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="v0"/> or <paramref name="v1"/> is
        /// not a 3D vector.
        /// </exception>
        /// 
        /// <remarks>
        /// The calcualtion is done in a right-hand coordinate system.
        /// And the angle is directional, thus the angle from
        /// <paramref name="v0"/> to <paramref name="v1"/> is the opposite of
        /// the angle from <paramref name="v1"/> to <paramref name="v0"/>.
        /// </remarks>
        public static double GetAngle2D(
            Vector<double> v0, Vector<double> v1
        )
        {
            if (v0 == null)
            {
                throw new ArgumentNullException(nameof(v0));
            }

            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }

            if (v0.Count != 3)
            {
                throw new ArgumentException(
                    $"{nameof(v0)} must be a 3D vector",
                    nameof(v0)
                );
            }

            if (v1.Count != 3)
            {
                throw new ArgumentException(
                    $"{nameof(v1)} must be a 3D vector",
                    nameof(v1)
                );
            }

            var normalizedV0 =
                (new DenseVector(new[] { v0[0], v0[1], 0.0 }))
                .Normalize(2);

            var normalizedV1 =
                (new DenseVector(new[] { v1[0], v1[1], 0.0 }))
                .Normalize(2);

            double cos = normalizedV0.DotProduct(normalizedV1);

            // Clamp to prevent invalid cosine values
            // due to possible floating point errors
            cos = Math.Min(1.0, Math.Max(-1.0, cos));

            double angle = Math.Acos(cos);

            var crossProduct = CrossProduct(normalizedV0, normalizedV1);

            if (crossProduct[2] < 0.0)
            {
                angle = -angle;
            }

            return angle;
        }

        /// <summary>
        /// Maps an integer within an integer range (source) to
        /// a real number within a real number range (target).
        /// </summary>
        /// 
        /// <param name="value">
        /// The integer value to be mapped.
        /// </param>
        /// <param name="minInclusive">
        /// Lower bound of the source range, inclusive.
        /// </param>
        /// <param name="maxInclusive">
        /// Upper bound of the source range, inclusive.
        /// </param>
        /// <param name="targetMinInclusive">
        /// Lower bound of the target range, inclusive.
        /// </param>
        /// <param name="targetMaxInclusive">
        /// Upper bound of the target range, inclusive.
        /// </param>
        /// 
        /// <returns>
        /// The target real number value that is mapped from
        /// the source integer value.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown if:
        /// <list type="bullet">
        ///     <item>
        ///     <description>
        ///     <paramref name="maxInclusive"/> is smaller than
        ///     <paramref name="minInclusive"/>
        ///     </description>
        ///     </item>
        ///     <item>
        ///     <description>
        ///     <paramref name="targetMaxInclusive"/> is smaller than
        ///     <paramref name="targetMinInclusive"/>
        ///     </description>
        ///     </item>
        ///     <item>
        ///     <description>
        ///     <paramref name="value"/> is smaller than
        ///     <paramref name="minInclusive"/> or greater than
        ///     <paramref name="maxInclusive"/>
        ///     </description>
        ///     </item>
        /// </list>
        /// </exception>
        public static double MapIntToDouble(
            int value, int minInclusive, int maxInclusive,
            double targetMinInclusive, double targetMaxInclusive
        )
        {
            if (maxInclusive < minInclusive)
            {
                throw new ArgumentException(
                    $"{nameof(maxInclusive)} cannot be smaller than " +
                    $"{nameof(minInclusive)}",
                    nameof(maxInclusive)
                );
            }

            if (value < minInclusive)
            {
                throw new ArgumentException(
                    $"{nameof(value)} cannot be smaller than " +
                    $"{nameof(minInclusive)}",
                    nameof(value)
                );
            }

            if (value > maxInclusive)
            {
                throw new ArgumentException(
                    $"{nameof(value)} cannot be greater than " +
                    $"{nameof(maxInclusive)}",
                    nameof(value)
                );
            }

            if (targetMaxInclusive < targetMinInclusive)
            {
                throw new ArgumentException(
                    $"{nameof(targetMaxInclusive)} cannot be smaller than " +
                    $"{nameof(targetMinInclusive)}",
                    nameof(targetMaxInclusive)
                );
            }

            // Each integer is mapped to the center of its corresponding
            // real number interval
            double normalizedValue =
                (value + 0.5 - minInclusive) /
                (maxInclusive - minInclusive + 1);

            double targetValue =
                targetMinInclusive +
                (targetMaxInclusive - targetMinInclusive) * normalizedValue;

            return targetValue;
        }

        /// <summary>
        /// Maps a real number within a real number range (source) to
        /// an integer within an integer range (target).
        /// </summary>
        /// 
        /// <param name="value">
        /// The real number value to be mapped.
        /// </param>
        /// <param name="minInclusive">
        /// Lower bound of the source range, inclusive.
        /// </param>
        /// <param name="maxInclusive">
        /// Upper bound of the source range, inclusive.
        /// </param>
        /// <param name="targetMinInclusive">
        /// Lower bound of the target range, inclusive.
        /// </param>
        /// <param name="targetMaxInclusive">
        /// Upper bound of the target range, inclusive.
        /// </param>
        /// 
        /// <returns>
        /// The target integer value that is mapped from
        /// the source real number value.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown if:
        /// <list type="bullet">
        ///     <item>
        ///     <description>
        ///     <paramref name="maxInclusive"/> is smaller than
        ///     <paramref name="minInclusive"/>
        ///     </description>
        ///     </item>
        ///     <item>
        ///     <description>
        ///     <paramref name="targetMaxInclusive"/> is smaller than
        ///     <paramref name="targetMinInclusive"/>
        ///     </description>
        ///     </item>
        ///     <item>
        ///     <description>
        ///     <paramref name="value"/> is smaller than
        ///     <paramref name="minInclusive"/> or greater than
        ///     <paramref name="maxInclusive"/>
        ///     </description>
        ///     </item>
        /// </list>
        /// </exception>
        public static int MapDoubleToInt(
            double value, double minInclusive, double maxInclusive,
            int targetMinInclusive, int targetMaxInclusive
        )
        {
            if (maxInclusive < minInclusive)
            {
                throw new ArgumentException(
                    $"{nameof(maxInclusive)} cannot be smaller than " +
                    $"{nameof(minInclusive)}",
                    nameof(maxInclusive)
                );
            }

            if (value < minInclusive)
            {
                throw new ArgumentException(
                    $"{nameof(value)} cannot be smaller than " +
                    $"{nameof(minInclusive)}",
                    nameof(value)
                );
            }

            if (value > maxInclusive)
            {
                throw new ArgumentException(
                    $"{nameof(value)} cannot be greater than " +
                    $"{nameof(maxInclusive)}",
                    nameof(value)
                );
            }

            if (targetMaxInclusive < targetMinInclusive)
            {
                throw new ArgumentException(
                    $"{nameof(targetMaxInclusive)} cannot be smaller than " +
                    $"{nameof(targetMinInclusive)}",
                    nameof(targetMaxInclusive)
                );
            }

            if (maxInclusive == minInclusive)
            {
                return targetMinInclusive;
            }

            double normalizedValue =
                (value - minInclusive) / (maxInclusive - minInclusive);

            int targetValue = targetMinInclusive + (int)(
                (targetMaxInclusive - targetMinInclusive + 1)
                * normalizedValue
            );

            // This will only be true if normalizedValue is 1.0
            if (targetValue > targetMaxInclusive)
            {
                targetValue = targetMaxInclusive;
            }

            return targetValue;
        }

        #endregion
    }
}
