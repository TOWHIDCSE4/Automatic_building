using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MathNet.Numerics.LinearAlgebra;
using Workspaces.Core;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Represents a line segment in 3D space.
    /// </summary>
    [Serializable]
    public class LineSegment : IBBox, IEquatable<LineSegment>, ISerializable
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="LineSegment"/> with
        /// both endpoints at origin.
        /// </summary>
        public LineSegment() : this(new Point(), new Point())
        { }

        /// <summary>
        /// Creates an instance of <see cref="LineSegment"/> with
        /// a given pair of endpoints.
        /// </summary>
        /// 
        /// <param name="point0">
        /// First endpoint of the line segment.
        /// </param>
        /// <param name="point1">
        /// Second endpoint of the line segment.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point0"/> or <paramref name="point1"/>
        /// is <see langword="null"/>.
        /// </exception>
        /// 
        /// <remarks>
        /// <paramref name="point0"/> and <paramref name="point1"/> are set to
        /// <see cref="Point0"/> and <see cref="Point1"/> respectively
        /// by reference, not by value.
        /// </remarks>
        public LineSegment(Point point0, Point point1)
        {
            Point0 = point0;
            Point1 = point1;
        }

        /// <summary>
        /// Creates a copy of a given <see cref="LineSegment"/>.
        /// </summary>
        /// 
        /// <param name="lineSegment">
        /// The line segment to be copied.
        /// </param>
        /// 
        /// <remarks>
        /// The copies of the endpoints of <paramref name="lineSegment"/>
        /// are used to create the new <see cref="LineSegment"/>.
        /// </remarks>
        public LineSegment(LineSegment lineSegment) : this(
            new Point(lineSegment.Point0), new Point(lineSegment.Point1)
        )
        { }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// 
        /// <param name="info">
        /// Contains data of a serialized <see cref="LineSegment"/>
        /// instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected LineSegment(
            SerializationInfo info, StreamingContext context
        )
        {
            var position0 = info.GetValue<List<double>>(nameof(Point0));
            var position1 = info.GetValue<List<double>>(nameof(Point1));

            _point0 = new Point(position0);
            _point1 = new Point(position1);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a <see cref="Point"/> at the position specified by
        /// a given parameter.
        /// </summary>
        /// 
        /// <param name="param">
        /// The parameter specifying the position of the <see cref="Point"/>.
        /// 0.0 indicates <see cref="Point0"/> and
        /// 1.0 indicates <see cref="Point1"/>.
        /// The parameter can be between, equal to or outside 0.0 and 1.0.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="Point"/> at the position specified by
        /// <paramref name="param"/>.
        /// </returns>
        public Point GetPointByParam(double param)
        {
            var pointVector =
                Point0.Vector + (Point1.Vector - Point0.Vector) * param;

            var point = new Point(pointVector);
            return point;
        }

        /// <summary>
        /// Gets a <see cref="Point"/> at the position specified by
        /// a signed length from <see cref="Point0"/>.
        /// </summary>
        /// 
        /// <param name="length">
        /// The signed length from <see cref="Point0"/> specifying
        /// the position of the <see cref="Point"/>. Negative values indicate
        /// a position on the extension line from <see cref="Point1"/> to
        /// <see cref="Point0"/>, while positive values indicate a position
        /// between <see cref="Point0"/> and <see cref="Point1"/> or
        /// on the extension line from <see cref="Point0"/> to
        /// <see cref="Point1"/>.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="Point"/> at the position specified by
        /// <paramref name="length"/>.
        /// </returns>
        public Point GetPointByLength(double length)
        {
            var pointVector = Point0.Vector + Direction * length;

            var point = new Point(pointVector);
            return point;
        }

        /// <summary>
        /// Gets the parameter of the point on this <see cref="LineSegment"/>
        /// or its extension lines on both ends that is closest to
        /// a given <see cref="Point"/>.
        /// </summary>
        /// 
        /// <param name="point">
        /// The <see cref="Point"/> to find
        /// the closest <see cref="Point"/> to.
        /// </param>
        /// 
        /// <returns>
        /// The parameter of the point on this <see cref="LineSegment"/>
        /// or its extension lines on both ends that is closest to
        /// <paramref name="point"/>. It may or may not be
        /// between 0.0 and 1.0.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point"/> is <see langword="null"/>.
        /// </exception>
        public double GetClosestParam(Point point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            double closestLength =
                (point.Vector - Point0.Vector).DotProduct(Direction);

            double closestParam =
                Length == 0.0 ? 0.0 : closestLength / Length;

            return closestParam;
        }

        /// <summary>
        /// Gets the parameter of the point on this <see cref="LineSegment"/>
        /// between and including <see cref="Point0"/> and
        /// <see cref="Point1"/>
        /// that is closest to a given <see cref="Point"/>.
        /// </summary>
        /// 
        /// <param name="point">
        /// The <see cref="Point"/> to find
        /// the closest <see cref="Point"/> to.
        /// </param>
        /// 
        /// <returns>
        /// The parameter of the point on this <see cref="LineSegment"/>
        /// between and including <see cref="Point0"/> and
        /// <see cref="Point1"/> that is closest to
        /// <paramref name="point"/>. It must be between 0.0 and 1.0
        /// inclusively.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point"/> is <see langword="null"/>.
        /// </exception>
        public double GetClosestParamBetween(Point point)
        {
            double closestParam = GetClosestParam(point);

            closestParam = Math.Min(1.0, Math.Max(0.0, closestParam));

            return closestParam;
        }

        /// <summary>
        /// Gets a <see cref="Point"/> on this <see cref="LineSegment"/>
        /// or its extension lines on both ends that is closest to
        /// a given <see cref="Point"/>.
        /// </summary>
        /// 
        /// <param name="point">
        /// The <see cref="Point"/> to find
        /// the closest <see cref="Point"/> to.
        /// </param>
        /// 
        /// <returns>
        /// The <see cref="Point"/> on this <see cref="LineSegment"/>
        /// or its extension lines on both ends that is closest to
        /// <paramref name="point"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point"/> is <see langword="null"/>.
        /// </exception>
        public Point GetClosestPoint(Point point)
        {
            double closestParam = GetClosestParam(point);

            return GetPointByParam(closestParam);
        }

        /// <summary>
        /// Gets a <see cref="Point"/> on this <see cref="LineSegment"/>
        /// between and including <see cref="Point0"/> and
        /// <see cref="Point1"/>
        /// that is closest to a given <see cref="Point"/>.
        /// </summary>
        /// 
        /// <param name="point">
        /// The <see cref="Point"/> to find
        /// the closest <see cref="Point"/> to.
        /// </param>
        /// 
        /// <returns>
        /// The <see cref="Point"/> on this <see cref="LineSegment"/>
        /// between and including <see cref="Point0"/> and
        /// <see cref="Point1"/> that is closest to
        /// <paramref name="point"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point"/> is <see langword="null"/>.
        /// </exception>
        public Point GetClosestPointBetween(Point point)
        {
            double closestParam = GetClosestParamBetween(point);

            return GetPointByParam(closestParam);
        }

        /// <summary>
        /// Gets the parameters for the pair of <see cref="Point"/>s on two
        /// <see cref="LineSegment"/>s or their extension lines on both ends
        /// that are closest to each other.
        /// </summary>
        ///
        /// <param name="ls0">
        /// The first <see cref="LineSegment"/>.
        /// </param>
        /// <param name="ls1">
        /// The second <see cref="LineSegment"/>.
        /// </param>
        /// <param name="epsilon">
        /// A tolerance used to deal with floating point errors
        /// from the calculation.
        /// </param>
        /// 
        /// <returns>
        /// A pair of parameters specifying the closest pair of
        /// <see cref="Point"/>s on <paramref name="ls0"/> and
        /// <paramref name="ls1"/> or their extension lines on both ends,
        /// which may or may not be between 0.0 and 1.0.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="ls0"/> or <paramref name="ls1"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="epsilon"/> is negative.
        /// </exception>
        /// 
        /// <remarks>
        /// References:
        /// <list type="bullet">
        /// <item>
        ///     <description>
        ///     Goldman, Ronald. "Intersection of two lines in three-space."
        ///     In Graphics Gems, p. 304. Academic Press Professional, Inc.,
        ///     1990.
        ///     </description>
        /// </item>
        /// </list>
        /// </remarks>
        public static Tuple<double, double> GetClosestParams(
            LineSegment ls0, LineSegment ls1, double epsilon = DefaultEpsilon
        )
        {
            if (ls0 == null)
            {
                throw new ArgumentNullException(nameof(ls0));
            }

            if (ls1 == null)
            {
                throw new ArgumentNullException(nameof(ls1));
            }

            if (epsilon < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(epsilon),
                    $"{nameof(epsilon)} cannot be negative"
                );
            }

            Vector<double> p0 = ls0.Point0.Vector;
            Vector<double> p1 = ls1.Point0.Vector;

            Vector<double> v0 = ls0.Point1.Vector - p0;
            Vector<double> v1 = ls1.Point1.Vector - p1;

            Vector<double> v0v1Cross = MathUtils.CrossProduct(v0, v1);
            double v0v1Norm = v0v1Cross.L2Norm();

            if (v0v1Norm <= epsilon)
            {
                Point pointA = ls0.Point0;
                double closestParam0A = 0.0;
                double closestDistanceA =
                    ls1.GetClosestPointBetween(pointA).GetDistance(pointA);

                Point pointB = ls0.Point1;
                double closestParam0B = 1.0;
                double closestDistanceB =
                    ls1.GetClosestPointBetween(pointB).GetDistance(pointB);

                Point pointC = ls1.Point0;
                double closestParam1C = 0.0;
                double closestDistanceC =
                    ls0.GetClosestPointBetween(pointC).GetDistance(pointC);

                Point pointD = ls1.Point1;
                double closestParam1D = 1.0;
                double closestDistanceD =
                    ls0.GetClosestPointBetween(pointD).GetDistance(pointD);

                double minClosestDistance = Math.Min(
                    Math.Min(closestDistanceA, closestDistanceB),
                    Math.Min(closestDistanceC, closestDistanceD)
                );

                if (minClosestDistance == closestDistanceA)
                {
                    double closestParam1A = ls1.GetClosestParam(pointA);
                    return new Tuple<double, double>(
                        closestParam0A, closestParam1A
                    );
                }
                else if (minClosestDistance == closestDistanceB)
                {
                    double closestParam1B = ls1.GetClosestParam(pointB);
                    return new Tuple<double, double>(
                        closestParam0B, closestParam1B
                    );
                }
                else if (minClosestDistance == closestDistanceC)
                {
                    double closestParam0C = ls0.GetClosestParam(pointC);
                    return new Tuple<double, double>(
                        closestParam0C, closestParam1C
                    );
                }
                else
                {
                    double closestParam0D = ls0.GetClosestParam(pointD);
                    return new Tuple<double, double>(
                        closestParam0D, closestParam1D
                    );
                }
            }
            else
            {
                Vector<double> p1p0Diff = p1 - p0;

                double closestParam0 =
                    MathUtils.CrossProduct(p1p0Diff, v1)
                    .DotProduct(v0v1Cross) / (v0v1Norm * v0v1Norm);

                double closestParam1 =
                    MathUtils.CrossProduct(-p1p0Diff, v0)
                    .DotProduct(-v0v1Cross) / (v0v1Norm * v0v1Norm);

                var closestParams =
                    new Tuple<double, double>(closestParam0, closestParam1);
                return closestParams;
            }
        }

        /// <summary>
        /// Gets the description of the (non-)intersection between
        /// two <see cref="LineSegment"/>s.
        /// </summary>
        /// 
        /// <param name="ls0">
        /// The first <see cref="LineSegment"/> involved in the check.
        /// </param>
        /// <param name="ls1">
        /// The second <see cref="LineSegment"/> involved in the check.
        /// </param>
        /// <param name="epsilon">
        /// A tolerance used to deal with floating point errors
        /// from the calculation.
        /// </param>
        /// 
        /// <returns>
        /// A description of the (non-)intersection between
        /// <paramref name="ls0"/> and <paramref name="ls1"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="ls0"/> or <paramref name="ls1"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="epsilon"/> is negative.
        /// </exception>
        public static LineLineIntersection Intersect(
            LineSegment ls0, LineSegment ls1, double epsilon = DefaultEpsilon
        )
        {
            var closestParams = GetClosestParams(ls0, ls1, epsilon);

            var intersection = new LineLineIntersection(
                ls0, ls1,
                closestParams.Item1, closestParams.Item2,
                epsilon
            );

            return intersection;
        }

        /// <summary>
        /// Gets the bounding box of this <see cref="LineSegment"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The bounding box of this <see cref="LineSegment"/>.
        /// </returns>
        public BBox GetBBox()
        {
            BBox bbox = BBox.GetBBox(new[] { Point0, Point1 });
            return bbox;
        }

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(Point0), Point0.AsList());
            info.AddValue(nameof(Point1), Point1.AsList());
        }

        /// <summary>
        /// Determines whether this <see cref="LineSegment"/> and
        /// a given object have the same value.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The object to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is
        /// a <see cref="LineSegment"/> and has the same endpoints in order
        /// as this <see cref="LineSegment"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as LineSegment);

        /// <summary>
        /// Determines whether this <see cref="LineSegment"/> and another
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="other">
        /// The other <see cref="LineSegment"/> to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="other"/>
        /// has the same endpoints in order as this <see cref="LineSegment"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool Equals(LineSegment other)
        {
            return ReferenceEquals(this, other) || (
                other != null &&
                EqualityComparer<Point>.Default
                    .Equals(Point0, other.Point0) &&
                EqualityComparer<Point>.Default
                    .Equals(Point1, other.Point1)
            );
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -2130701941;

            hashCode = hashCode * -1521134295 +
                EqualityComparer<Point>.Default.GetHashCode(Point0);

            hashCode = hashCode * -1521134295 +
                EqualityComparer<Point>.Default.GetHashCode(Point1);

            return hashCode;
        }

        /// <summary>
        /// Determines whether two <see cref="LineSegment"/>s
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="LineSegment"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="LineSegment"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have the same endpoints in order.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(LineSegment left, LineSegment right) =>
            EqualityComparer<LineSegment>.Default.Equals(left, right);

        /// <summary>
        /// Determines whether two <see cref="LineSegment"/>s
        /// have different values.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="LineSegment"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="LineSegment"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have different endpoints.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(LineSegment left, LineSegment right) =>
            !(left == right);

        #endregion

        #region Properties

        /// <summary>
        /// First end point of this <see cref="LineSegment"/>.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public Point Point0
        {
            get => _point0;
            set
            {
                _point0 =
                    value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Second end point of this <see cref="LineSegment"/>.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public Point Point1
        {
            get => _point1;
            set
            {
                _point1 =
                    value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Direction vector of this <see cref="LineSegment"/>.
        /// It is a unit 3D vector.
        /// </summary>
        public Vector<double> Direction =>
            (Point1.Vector - Point0.Vector).Normalize(2.0);

        /// <summary>
        /// Length of this <see cref="LineSegment"/>.
        /// </summary>
        public double Length =>
            (Point1.Vector - Point0.Vector).L2Norm();

        #endregion

        #region Fields

        private Point _point0;
        private Point _point1;

        #endregion

        #region Constants

        /// <summary>
        /// Default tolerance used in certain methods that need to deal with
        /// floating point errors, such as
        /// <see cref="GetClosestParam(Point)"/> and
        /// <see cref="Intersect(LineSegment, LineSegment, double)"/>.
        /// </summary>
        public const double DefaultEpsilon = 1e-6;

        #endregion
    }
}
