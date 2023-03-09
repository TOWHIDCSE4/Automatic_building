using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Contains information about the intersection or non-intersection
    /// between a pair of <see cref="Polygon"/>s.
    /// </summary>
    [Serializable]
    public class PolygonPolygonIntersection :
        IEquatable<PolygonPolygonIntersection>, ISerializable
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="PolygonPolygonIntersection"/>
        /// for two involved <see cref="Polygon"/>s.
        /// </summary>
        /// 
        /// <param name="polygon0">
        /// The first <see cref="Polygon"/> involved in (non-)intersection.
        /// </param>
        /// <param name="polygon1">
        /// The second <see cref="Polygon"/> involved in (non-)intersection.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="polygon0"/> or
        /// <paramref name="polygon1"/> is <see langword="null"/>.
        /// </exception>
        internal PolygonPolygonIntersection(
            Polygon polygon0, Polygon polygon1
        )
        {
            Polygon0 = polygon0 ??
                throw new ArgumentNullException(nameof(polygon0));

            Polygon1 = polygon1 ??
                throw new ArgumentNullException(nameof(polygon1));

            IntersectingEdgeIndexPairs =
                _intersectingEdgeIndexPairs.AsReadOnly();

            EdgeIntersections = _edgeIntersections.AsReadOnly();
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// 
        /// <param name="info">
        /// Contains data of
        /// a serialized <see cref="PolygonPolygonIntersection"/> instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected PolygonPolygonIntersection(
            SerializationInfo info, StreamingContext context
        )
        {
            Polygon0 = info.GetValue<Polygon>(nameof(Polygon0));
            Polygon1 = info.GetValue<Polygon>(nameof(Polygon1));

            _intersectingEdgeIndexPairs.AddRange(
                info.GetValue<List<int[]>>(
                    nameof(IntersectingEdgeIndexPairs)
                ).Select(pair => (pair[0], pair[1]))
            );

            _edgeIntersections.AddRange(
                info.GetValue<List<LineLineIntersection>>(
                    nameof(EdgeIntersections)
                )
            );

            IntersectingEdgeIndexPairs =
                _intersectingEdgeIndexPairs.AsReadOnly();

            EdgeIntersections = _edgeIntersections.AsReadOnly();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a description of the intersection between two edges
        /// from <see cref="Polygon0"/> and <see cref="Polygon1"/>.
        /// </summary>
        /// 
        /// <param name="edgeIndex0">
        /// Index of the intersecting edge on <see cref="Polygon0"/>.
        /// </param>
        /// <param name="edgeIndex1">
        /// Index of the intersecting edge on <see cref="Polygon1"/>.
        /// </param>
        /// 
        /// <param name="edgeIntersection">
        /// A <see cref="LineLineIntersection"/> describing the intersection
        /// between the involved edges.
        /// </param>
        internal void AddEdgeIntersection(
            int edgeIndex0, int edgeIndex1,
            LineLineIntersection edgeIntersection
        )
        {
            var edgeIndexPair = (edgeIndex0, edgeIndex1);
            _intersectingEdgeIndexPairs.Add(edgeIndexPair);

            _edgeIntersections.Add(edgeIntersection);
        }

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(Polygon0), Polygon0);
            info.AddValue(nameof(Polygon1), Polygon1);

            info.AddValue(
                nameof(IntersectingEdgeIndexPairs),
                _intersectingEdgeIndexPairs
                .Select(pair => new[] { pair.Item1, pair.Item2 })
                .ToList()
            );

            info.AddValue(nameof(EdgeIntersections), _edgeIntersections);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            Equals(obj as PolygonPolygonIntersection);

        /// <inheritdoc/>
        public bool Equals(PolygonPolygonIntersection other)
        {
            return ReferenceEquals(this, other) || (
                other != null &&
                EqualityComparer<Polygon>.Default
                    .Equals(Polygon0, other.Polygon0) &&
                EqualityComparer<Polygon>.Default
                    .Equals(Polygon1, other.Polygon1) &&
                _intersectingEdgeIndexPairs.SequenceEqual(
                    other._intersectingEdgeIndexPairs
                ) &&
                _edgeIntersections.SequenceEqual(other._edgeIntersections)
            );
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -530949681;

            hashCode = hashCode * -1521134295 +
                EqualityComparer<Polygon>.Default.GetHashCode(Polygon0);

            hashCode = hashCode * -1521134295 +
                EqualityComparer<Polygon>.Default.GetHashCode(Polygon1);

            foreach (var pair in _intersectingEdgeIndexPairs)
            {
                hashCode = hashCode * -1521134295 +
                    EqualityComparer<(int, int)>.Default.GetHashCode(pair);
            }

            foreach (var intersection in _edgeIntersections)
            {
                hashCode = hashCode * -1521134295 +
                    EqualityComparer<LineLineIntersection>.Default
                    .GetHashCode(intersection);
            }

            return hashCode;
        }

        public static bool operator ==(
            PolygonPolygonIntersection left, PolygonPolygonIntersection right
        ) => EqualityComparer<PolygonPolygonIntersection>.Default
            .Equals(left, right);

        public static bool operator !=(
            PolygonPolygonIntersection left, PolygonPolygonIntersection right
        ) => !(left == right);

        #endregion

        #region Properties

        /// <summary>
        /// The first <see cref="Polygon"/> involved
        /// in the (non-)intersection.
        /// </summary>
        public Polygon Polygon0 { get; }

        /// <summary>
        /// The second <see cref="Polygon"/> involved
        /// in the (non-)intersection.
        /// </summary>
        public Polygon Polygon1 { get; }

        /// <summary>
        /// A list of edge index pairs specifying edges on
        /// <see cref="Polygon0"/> and <see cref="Polygon1"/> that intersect.
        /// For each <see cref="Tuple{T1, T2}"/>, the first item is
        /// the index of the intersecting edge on <see cref="Polygon0"/>,
        /// and the second item is the index of the intersecting edge
        /// on <see cref="Polygon1"/>.
        /// </summary>
        public IReadOnlyList<(int, int)> IntersectingEdgeIndexPairs
        { get; }

        /// <summary>
        /// A list of <see cref="LineLineIntersection"/>s describing
        /// each pair of intersecting edges specified by
        /// <see cref="IntersectingEdgeIndexPairs"/>.
        /// </summary>
        public IReadOnlyList<LineLineIntersection> EdgeIntersections
        { get; }

        /// <summary>
        /// Specifies whether <see cref="Polygon0"/> and
        /// <see cref="Polygon1"/> intersect. This is determined by
        /// whether any pair of edges on <see cref="Polygon0"/> and
        /// <see cref="Polygon1"/> intersect.
        /// </summary>
        public bool DoesIntersect => EdgeIntersections.Count > 0;

        #endregion

        #region Fields

        private readonly List<(int, int)> _intersectingEdgeIndexPairs =
            new List<(int, int)>();

        private readonly List<LineLineIntersection> _edgeIntersections =
            new List<LineLineIntersection>();

        #endregion
    }
}
