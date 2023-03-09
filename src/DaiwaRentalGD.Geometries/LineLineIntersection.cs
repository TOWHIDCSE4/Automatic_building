using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Contains information about the intersection or non-intersection
    /// between a pair of <see cref="LineSegment"/>s, as returned by
    /// <see cref="LineSegment.Intersect"/>.
    /// </summary>
    [Serializable]
    public class LineLineIntersection :
        IEquatable<LineLineIntersection>, ISerializable
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="LineLineIntersection"/>.
        /// </summary>
        /// 
        /// <param name="ls0">
        /// First line segment in the (non-)intersection.
        /// </param>
        /// <param name="ls1">
        /// Second line segment in the (non-)intersection.
        /// </param>
        /// <param name="param0">
        /// Parameter on <paramref name="ls0"/> specifying
        /// the intersection point.
        /// </param>
        /// <param name="param1">
        /// Paremter on <paramref name="ls1"/> specifying
        /// the intersection point.
        /// </param>
        /// <param name="epsilon">
        /// Tolerance determining whether <see cref="Point0"/> and
        /// <see cref="Point1"/> are close enough for
        /// <paramref name="ls0"/> and <paramref name="ls1"/> to
        /// be considered to intersect.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="epsilon"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="ls0"/> or <paramref name="ls1"/>
        /// is <see langword="null"/>.
        /// </exception>
        public LineLineIntersection(
            LineSegment ls0, LineSegment ls1,
            double param0, double param1,
            double epsilon = LineSegment.DefaultEpsilon
        )
        {
            if (epsilon < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(epsilon),
                    $"{nameof(epsilon)} cannot be negative"
                );
            }

            LineSegment0 =
                ls0 ?? throw new ArgumentNullException(nameof(ls0));

            LineSegment1 =
                ls1 ?? throw new ArgumentNullException(nameof(ls1));

            Param0 = param0;
            Param1 = param1;

            Epsilon = epsilon;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// 
        /// <param name="info">
        /// Contains data of a serialized <see cref="LineLineIntersection"/>
        /// instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected LineLineIntersection(
            SerializationInfo info, StreamingContext context
        )
        {
            LineSegment0 = info.GetValue<LineSegment>(nameof(LineSegment0));
            LineSegment1 = info.GetValue<LineSegment>(nameof(LineSegment1));

            Param0 = info.GetDouble(nameof(Param0));
            Param1 = info.GetDouble(nameof(Param1));

            Epsilon = info.GetDouble(nameof(Epsilon));
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(LineSegment0), LineSegment0);
            info.AddValue(nameof(LineSegment1), LineSegment1);

            info.AddValue(nameof(Param0), Param0);
            info.AddValue(nameof(Param1), Param1);

            info.AddValue(nameof(Epsilon), Epsilon);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            Equals(obj as LineLineIntersection);

        /// <inheritdoc/>
        public bool Equals(LineLineIntersection other)
        {
            return ReferenceEquals(this, other) || (
                other != null &&
                EqualityComparer<LineSegment>.Default
                    .Equals(LineSegment0, other.LineSegment0) &&
                EqualityComparer<LineSegment>.Default
                    .Equals(LineSegment1, other.LineSegment1) &&
                Param0 == other.Param0 &&
                Param1 == other.Param1 &&
                Epsilon == other.Epsilon
            );
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -908009649;

            hashCode = hashCode * -1521134295 +
                EqualityComparer<LineSegment>.Default
                .GetHashCode(LineSegment0);

            hashCode = hashCode * -1521134295 +
                EqualityComparer<LineSegment>.Default
                .GetHashCode(LineSegment1);

            hashCode = hashCode * -1521134295 + Param0.GetHashCode();
            hashCode = hashCode * -1521134295 + Param1.GetHashCode();

            hashCode = hashCode * -1521134295 + Epsilon.GetHashCode();

            return hashCode;
        }

        public static bool operator ==(
            LineLineIntersection left, LineLineIntersection right
        ) => EqualityComparer<LineLineIntersection>.Default
            .Equals(left, right);

        public static bool operator !=(
            LineLineIntersection left, LineLineIntersection right
        ) => !(left == right);

        #endregion

        #region Properties

        /// <summary>
        /// First line segment in the (non-)intersection.
        /// </summary>
        public LineSegment LineSegment0 { get; }

        /// <summary>
        /// Second line segment in the (non-) intersection.
        /// </summary>
        public LineSegment LineSegment1 { get; }

        /// <summary>
        /// Parameter on <see cref="LineSegment0"/> specifying
        /// the intersection point.
        /// </summary>
        public double Param0 { get; }

        /// <summary>
        /// Parameter on <see cref="LineSegment1"/> specifying
        /// the intersection point.
        /// </summary>
        public double Param1 { get; }

        /// <param name="epsilon">
        /// Tolerance determining whether <see cref="Point0"/> and
        /// <see cref="Point1"/> are close enough for
        /// <see cref="LineSegment0"/> and <see cref="LineSegment1"/> to
        /// be considered to intersect.
        /// </param>
        public double Epsilon { get; }

        /// <summary>
        /// (Non-)intersection point on <see cref="LineSegment0"/>.
        /// </summary>
        public Point Point0 => LineSegment0.GetPointByParam(Param0);

        /// <summary>
        /// (Non-)intersection point on <see cref="LineSegment1"/>.
        /// </summary>
        public Point Point1 => LineSegment1.GetPointByParam(Param1);

        /// <summary>
        /// Distance between (non-)intersection points on
        /// <see cref="LineSegment0"/> and <see cref="LineSegment1"/>.
        /// </summary>
        public double Distance => Point0.GetDistance(Point1);

        /// <summary>
        /// Whether the infinite lines which
        /// <see cref="LineSegment0"/> and
        /// <see cref="LineSegment1"/> lie on intersect.
        /// </summary>
        public bool DoesIntersect => Distance <= Epsilon;

        /// <summary>
        /// Whether <see cref="LineSegment0"/> and
        /// <see cref="LineSegment1"/> intersect between
        /// their end points (with their end points included).
        /// </summary>
        public bool DoesIntersectBetween =>
            Param0 >= 0.0 && Param0 <= 1.0 &&
            Param1 >= 0.0 && Param1 <= 1.0 &&
            DoesIntersect;

        #endregion
    }
}
