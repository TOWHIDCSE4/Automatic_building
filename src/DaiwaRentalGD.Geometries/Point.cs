using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Represents a point in 3D space.
    /// </summary>
    [Serializable]
    public class Point : IBBox, IEquatable<Point>, ISerializable
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="Point"/> with specified
        /// coordinates.
        /// </summary>
        /// 
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates an instance of <see cref="Point"/> at the origin.
        /// </summary>
        public Point() : this(0.0, 0.0, 0.0)
        { }

        /// <summary>
        /// Creates a copy of a given <see cref="Point"/>.
        /// </summary>
        /// 
        /// <param name="point">
        /// The <see cref="Point"/> to create a copy from.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point"/> is <see langword="null"/>.
        /// </exception>
        public Point(Point point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        /// <summary>
        /// Creates an instance of <see cref="Point"/> with specified
        /// coordinates in a 3D vector.
        /// </summary>
        /// 
        /// <param name="vector">
        /// A 3D vector containing the X, Y and Z coordinates of the point.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="vector"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="vector"/> is not a 3D vector.
        /// </exception>
        public Point(Vector<double> vector)
        {
            if (vector == null)
            {
                throw new ArgumentNullException(nameof(vector));
            }

            if (vector.Count != 3)
            {
                throw new ArgumentException(
                    $"{nameof(vector)} must be a 3D vector",
                    nameof(vector)
                );
            }

            Vector = vector;
        }

        /// <summary>
        /// Creates an instance of <see cref="Point"/> with specified
        /// coordinates in a list.
        /// </summary>
        /// 
        /// <param name="coordinates">
        /// A list containing the X, Y and Z coordinates of the point.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="coordinates"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="coordinates"/> does not contain
        /// exactly 3 items.
        /// </exception>
        public Point(IReadOnlyList<double> coordinates)
        {
            if (coordinates == null)
            {
                throw new ArgumentNullException(nameof(coordinates));
            }

            if (coordinates.Count != 3)
            {
                throw new ArgumentException(
                    $"{nameof(coordinates)} must contain exactly 3 items",
                    nameof(coordinates)
                );
            }

            X = coordinates[0];
            Y = coordinates[1];
            Z = coordinates[2];
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info">
        /// Contains data of a serialized <see cref="Point"/> instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected Point(SerializationInfo info, StreamingContext context)
        {
            X = info.GetDouble(nameof(X));
            Y = info.GetDouble(nameof(Y));
            Z = info.GetDouble(nameof(Z));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Transforms this <see cref="Point"/> with a given transform.
        /// This <see cref="Point"/> will be transformed in place, i.e.
        /// modified.
        /// </summary>
        /// 
        /// <param name="transform">
        /// The transform to be applied to this <see cref="Point"/>.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="transform"/> is <see langword="null"/>.
        /// </exception>
        public void Transform(ITransform3D transform)
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            Vector = transform.Transform(Vector);
        }

        /// <summary>
        /// Gets the bounding box of this <see cref="Point"/>,
        /// which has zero sizes in all dimensions and bounds at
        /// this <see cref="Point"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The bounding box of this <see cref="Point"/>.
        /// </returns>
        public BBox GetBBox()
        {
            BBox bbox = BBox.FromMinAndMax(X, Y, Z, X, Y, Z);
            return bbox;
        }

        /// <summary>
        /// Gets the distance between this <see cref="Point"/> and
        /// a given <see cref="Point"/>.
        /// </summary>
        /// 
        /// <param name="point">
        /// The <see cref="Point"/> to from which to compute the distance.
        /// </param>
        /// 
        /// <returns>
        /// The distance between this <see cref="Point"/> and
        /// <paramref name="point"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point"/> is <see langword="null"/>.
        /// </exception>
        public double GetDistance(Point point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            double distance = (point.Vector - Vector).L2Norm();
            return distance;
        }

        /// <summary>
        /// Get the coordinates of this <see cref="Point"/> as a list.
        /// </summary>
        /// 
        /// <returns>
        /// A read only list containing X, Y and Z coordinates.
        /// </returns>
        public IReadOnlyList<double> AsList() => new[] { X, Y, Z };

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
            info.AddValue(nameof(Z), Z);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -307843816;

            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// Determines whether this <see cref="Point"/> and a given object
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The object to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is
        /// a <see cref="Point"/> and have the same coordiantes as
        /// this <see cref="Point"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as Point);

        /// <summary>
        /// Determines whether this <see cref="Point"/> and another
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="other">
        /// The other <see cref="Point"/> to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="other"/>
        /// has the same coordiantes as this <see cref="Point"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool Equals(Point other)
        {
            return other != null &&
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }

        /// <summary>
        /// Determines whether two <see cref="Point"/>s
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="Point"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="Point"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have the same coordiantes.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Point left, Point right) =>
            EqualityComparer<Point>.Default.Equals(left, right);

        /// <summary>
        /// Determines whether two <see cref="Point"/>s
        /// have different values.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="Point"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="Point"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have different coordiantes.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Point left, Point right) =>
            !(left == right);

        #endregion

        #region Properties

        /// <summary>
        /// X coordinate of this <see cref="Point"/>.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y coordinate of this <see cref="Point"/>.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z coordinate of this <see cref="Point"/>.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Coordinates of this <see cref="Point"/> in a 3D vector.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="value"/> is not a 3D vector.
        /// </exception>
        public Vector<double> Vector
        {
            get
            {
                return new DenseVector(new double[] { X, Y, Z });
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value.Count != 3)
                {
                    throw new ArgumentException(
                        $"{nameof(value)} must be a 3D vector",
                        nameof(value)
                    );
                }

                X = value[0];
                Y = value[1];
                Z = value[2];
            }
        }

        #endregion
    }
}
