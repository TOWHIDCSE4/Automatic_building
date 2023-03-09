using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Represents a polygon in 3D space.
    /// The polygon may be planar or non-plannar.
    /// </summary>
    [Serializable]
    public class Polygon : IBBox, IEquatable<Polygon>, ISerializable
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="Polygon"/> with no vertex.
        /// </summary>
        public Polygon()
        {
            Points = _points.AsReadOnly();
        }

        /// <summary>
        /// Creates an instance of <see cref="Polygon"/> with given
        /// <see cref="Points"/>s as its vertices.
        /// </summary>
        /// 
        /// <param name="points">
        /// <see cref="Points"/>s that are added to the created
        /// <see cref="Polygon"/> as its vertices by reference.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="points"/> is <see langword="null"/>.
        /// </exception>
        public Polygon(IEnumerable<Point> points) : this()
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            foreach (Point point in points)
            {
                AddPoint(point);
            }
        }

        /// <summary>
        /// Creates a deep copy of a given <see cref="Polygon"/>.
        /// </summary>
        /// 
        /// <param name="polygon">
        /// The <see cref="Polygon"/> to be copied.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="polygon"/> is <see langword="null"/>.
        /// </exception>
        public Polygon(Polygon polygon) : this()
        {
            if (polygon == null)
            {
                throw new ArgumentNullException(nameof(polygon));
            }

            foreach (Point point in polygon.Points)
            {
                AddPoint(new Point(point));
            }
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// 
        /// <param name="info">
        /// Contains data of a serialized <see cref="Polygon"/>
        /// instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected Polygon(SerializationInfo info, StreamingContext context) :
            this()
        {
            var pointPositions =
                info.GetValue<List<List<double>>>(nameof(Points));

            _points.AddRange(pointPositions.Select(
                pointPosition => new Point(pointPosition)
            ));
        }

        #endregion

        #region Methods

        #region Points and edges

        /// <summary>
        /// Adds a <see cref="Point"/> to the end of <see cref="Points"/>
        /// as a vertex.
        /// </summary>
        /// 
        /// <param name="point"></param>
        /// The <see cref="Point"/> to be added by reference.
        /// <remarks>
        /// 
        /// Please see <see cref="InsertPoint"/> for possible exceptions.
        /// </remarks>
        public void AddPoint(Point point)
        {
            InsertPoint(_points.Count, point);
        }

        /// <summary>
        /// Inserts a <see cref="Point"/> into <see cref="Points"/> at
        /// a given index as a vertex.
        /// </summary>
        /// 
        /// <param name="pointIndex">
        /// Index in <see cref="Points"/> at which to insert
        /// <paramref name="point"/> by reference.
        /// </param>
        /// <param name="point">
        /// The <see cref="Point"/> to insert.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point"/> is <see langword="null"/>.
        /// </exception>
        public void InsertPoint(int pointIndex, Point point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            _points.Insert(pointIndex, point);
        }

        /// <summary>
        /// Removes a <see cref="Point"/> at a given index
        /// from <see cref="Points"/> as a vertex.
        /// </summary>
        /// 
        /// <param name="pointIndex">
        /// Index of the <see cref="Point"/> in <see cref="Points"/>
        /// to be removed.
        /// </param>
        /// 
        /// <returns>
        /// The <see cref="Point"/> removed.
        /// </returns>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="pointIndex"/> is out of range.
        /// </exception>
        public Point RemovePoint(int pointIndex)
        {
            Point removedPoint = _points[pointIndex];

            _points.RemoveAt(pointIndex);

            return removedPoint;
        }

        /// <summary>
        /// Gets the edge of the <see cref="Polygon"/> at a given index.
        /// </summary>
        /// 
        /// <param name="edgeIndex">
        /// The index of the edge to get.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="LineSegment"/> representing the edge with endpoints
        /// from <see cref="Points"/> by reference.
        /// </returns>
        public LineSegment GetEdge(int edgeIndex)
        {
            int pointIndex0 = edgeIndex;
            int pointIndex1 = (pointIndex0 + 1) % Points.Count;

            Point point0 = Points[pointIndex0];
            Point point1 = Points[pointIndex1];

            LineSegment edge = new LineSegment(point0, point1);

            return edge;
        }

        /// <summary>
        /// Gets the normal vector of the edge of this <see cref="Polygon"/>
        /// at a given index. The normal vector is perpendicular to
        /// the edge, pointing outwards and coplanar with
        /// this <see cref="Polygon"/> (or as coplanar as possible if
        /// this <see cref="Polygon"/> is not planar).
        /// </summary>
        /// 
        /// <param name="edgeIndex">
        /// The index of the edge to calculate the normal vector for.
        /// </param>
        /// 
        /// <returns>
        /// The normal vector of the edge at index
        /// <paramref name="edgeIndex"/>.
        /// </returns>
        public Vector<double> GetEdgeNormal(int edgeIndex)
        {
            int pointIndex0 = edgeIndex;
            int pointIndex1 = (pointIndex0 + 1) % Points.Count;

            Point point0 = Points[pointIndex0];
            Point point1 = Points[pointIndex1];

            Vector<double> vector01 = point1.Vector - point0.Vector;

            Vector<double> edgeNormal =
                MathUtils.CrossProduct(vector01, Normal)
                .Normalize(2.0);

            return edgeNormal;
        }

        /// <summary>
        /// Gets the interior angle at a vertex of this <see cref="Polygon"/>
        /// at a given index.
        /// </summary>
        /// 
        /// <param name="pointIndex">
        /// The index of the vertex to calculate the interior angle for.
        /// </param>
        /// 
        /// <returns>
        /// The interior angle (in radians) at the vertex at index
        /// <paramref name="pointIndex"/>.
        /// </returns>
        public double GetInteriorAngle(int pointIndex)
        {
            int edgeIndex0 = (pointIndex - 1 + NumOfEdges) % NumOfEdges;
            int edgeIndex1 = pointIndex;

            var edgeTangent0 = GetEdge(edgeIndex0).Direction;
            var edgeTangent1 = GetEdge(edgeIndex1).Direction;

            double dotProduct = edgeTangent0.DotProduct(edgeTangent1);
            var crossProduct =
                MathUtils.CrossProduct(edgeTangent0, edgeTangent1);

            double angle;
            if (crossProduct[2] >= 0.0)
            {
                angle = Math.PI - Math.Acos(dotProduct);
            }
            else
            {
                angle = Math.PI + Math.Acos(dotProduct);
            }

            return angle;
        }

        #endregion

        #region Point-plane relation

        /// <summary>
        /// Determines whether a given point is above
        /// this <see cref="Polygon"/>, i.e. in the half-space divided by
        /// the plane this <see cref="Polygon"/> is in and pointed to by
        /// <see cref="Normal"/>.
        /// </summary>
        /// 
        /// <param name="testPoint">
        /// The <see cref="Point"/> to test the spatial relation for.
        /// </param>
        /// <param name="epsilon">
        /// A tolerance dealing with cases where <paramref name="testPoint"/>
        /// is close to this <see cref="Polygon"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if the point is above
        /// this <see cref="Polygon"/> considering <paramref name="epsilon"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="testPoint"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="epsilon"/> is negative.
        /// </exception>
        public bool IsPointAbove(
            Point testPoint, double epsilon = DefaultEpsilon
        )
        {
            if (testPoint == null)
            {
                throw new ArgumentNullException(nameof(testPoint));
            }

            if (epsilon < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(epsilon),
                    $"{nameof(epsilon)} cannot be negative"
                );
            }

            Vector<double> averageDir =
                (testPoint.Vector - Centroid.Vector).Normalize(2.0);

            double cos = averageDir.DotProduct(Normal);

            bool isPointAbove = cos > epsilon;

            return isPointAbove;
        }

        /// <summary>
        /// Determines whether a given point is coplanar with
        /// this <see cref="Polygon"/>.
        /// </summary>
        /// 
        /// <param name="testPoint">
        /// The <see cref="Point"/> to test the spatial relation for.
        /// </param>
        /// <param name="epsilon">
        /// A tolerance dealing with cases where <paramref name="testPoint"/>
        /// is close to this <see cref="Polygon"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if the point is coplanar with
        /// this <see cref="Polygon"/> considering <paramref name="epsilon"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="testPoint"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="epsilon"/> is negative.
        /// </exception>
        public bool IsPointCoplanar(
            Point testPoint, double epsilon = DefaultEpsilon
        )
        {
            if (testPoint == null)
            {
                throw new ArgumentNullException(nameof(testPoint));
            }

            if (epsilon < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(epsilon),
                    $"{nameof(epsilon)} cannot be negative"
                );
            }

            Vector<double> averageDir =
                (testPoint.Vector - Centroid.Vector).Normalize(2.0);

            double cos = averageDir.DotProduct(Normal);

            bool isPointCoplanar = Math.Abs(cos) <= epsilon;

            return isPointCoplanar;
        }

        /// <summary>
        /// Determines whether a given point is below
        /// this <see cref="Polygon"/>, i.e. in the half-space divided by
        /// the plane this <see cref="Polygon"/> is in and pointed to by
        /// negative <see cref="Normal"/>.
        /// </summary>
        /// 
        /// <param name="testPoint">
        /// The <see cref="Point"/> to test the spatial relation for.
        /// </param>
        /// <param name="epsilon">
        /// A tolerance dealing with cases where <paramref name="testPoint"/>
        /// is close to this <see cref="Polygon"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if the point is below
        /// this <see cref="Polygon"/> considering <paramref name="epsilon"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="testPoint"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="epsilon"/> is negative.
        /// </exception>
        public bool IsPointBelow(
            Point testPoint, double epsilon = DefaultEpsilon
        )
        {
            if (testPoint == null)
            {
                throw new ArgumentNullException(nameof(testPoint));
            }

            if (epsilon < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(epsilon),
                    $"{nameof(epsilon)} cannot be negative"
                );
            }

            Vector<double> averageDir =
                (testPoint.Vector - Centroid.Vector).Normalize(2.0);

            double cos = averageDir.DotProduct(Normal);

            bool isPointBelow = cos < -epsilon;

            return isPointBelow;
        }

        #endregion

        #region Point containment

        /// <summary>
        /// Determines whether a <see cref="Point"/> is inside
        /// this <see cref="Polygon"/> on X-Y Plane, i.e.
        /// whether the X-Y Plane projection of the <see cref="Point"/>
        /// is inside the X-Y Plane projection of this <see cref="Polygon"/>.
        /// The Z coordinates of the <see cref="Point"/> and
        /// <see cref="Points"/> are ignored, hence the name suffix 2D.
        /// </summary>
        /// 
        /// <param name="point">
        /// The <see cref="Point"/> to test the spatial relation for.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="point"/> is inside
        /// this <see cref="Polygon"/> on X-Y Plane, i.e.
        /// The X-Y Plane projection of <paramref name="point"/> is inside
        /// the X-Y Plane projection of this <see cref="Polygon"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point"/> is <see langword="null"/>.
        /// </exception>
        /// 
        /// <remarks>
        /// References:
        /// 
        /// <list type="bullet">
        /// <item>
        ///     <description>
        ///     Determining point-polygon spatial relation
        ///     based on winding number:
        ///     http://geomalgorithms.com/a03-_inclusion.html
        ///     </description>
        /// </item>
        /// </list>
        /// </remarks>
        public bool IsPointInside2D(Point point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            int windingNumber = 0;

            foreach (LineSegment edge in Edges)
            {
                if (DoesPointXRayCrossEdge2D(point, edge))
                {
                    if (
                        IsEdgeUpward2D(edge) &&
                        IsPointOnLeft2D(edge, point))
                    {
                        windingNumber += 1;
                    }
                    else if (
                        IsEdgeDownward2D(edge) &&
                        IsPointOnRight2D(edge, point)
                    )
                    {
                        windingNumber -= 1;
                    }
                }
            }

            bool isInside = windingNumber != 0;
            return isInside;
        }

        private static bool DoesPointXRayCrossEdge2D(
            Point point, LineSegment edge
        )
        {
            if (IsEdgeUpward2D(edge))
            {
                bool doesCross =
                    point.Y >= edge.Point0.Y &&
                    point.Y < edge.Point1.Y;

                return doesCross;
            }

            if (IsEdgeDownward2D(edge))
            {
                bool doesCross =
                    point.Y >= edge.Point1.Y &&
                    point.Y < edge.Point0.Y;

                return doesCross;
            }

            return false;
        }

        private static bool IsEdgeUpward2D(LineSegment edge)
        {
            return edge.Point1.Y > edge.Point0.Y;
        }

        private static bool IsEdgeDownward2D(LineSegment edge)
        {
            return edge.Point1.Y < edge.Point0.Y;
        }

        private static bool IsPointOnLeft2D(LineSegment edge, Point point)
        {
            Point point0 = edge.Point0;
            Point point1 = edge.Point1;

            double crossProduct =
                (point0.X - point.X) * (point1.Y - point.Y) -
                (point0.Y - point.Y) * (point1.X - point.X);

            return crossProduct > 0.0;
        }

        private static bool IsPointOnRight2D(LineSegment edge, Point point)
        {
            Point point0 = edge.Point0;
            Point point1 = edge.Point1;

            double crossProduct =
                (point0.X - point.X) * (point1.Y - point.Y) -
                (point0.Y - point.Y) * (point1.X - point.X);

            return crossProduct < 0.0;
        }

        #endregion

        #region Triangulation

        /// <summary>
        /// Triangulates this <see cref="Polygon"/> by returning
        /// topology information of the triangulation result.
        /// </summary>
        /// 
        /// <returns>
        /// A list of <see cref="Tuple{T1, T2, T3}"/>s whose items are
        /// the indices of 3 points from <see cref="Points"/> that make up
        /// a triangle in the triangulation result.
        /// </returns>
        /// 
        /// <remarks>
        /// References:
        /// 
        /// <list type="bullet">
        /// <item>
        ///     <description>
        ///     Eberly, David. "Triangulation by ear clipping."
        ///     Geometric Tools (2008): 2002-2005.
        ///     </description>
        /// </item>
        /// </list>
        /// </remarks>
        public IReadOnlyList<Tuple<int, int, int>> Trianglate()
        {
            if (Points.Count < 3)
            {
                return new List<Tuple<int, int, int>>();
            }

            var triangleIndicesList = new List<Tuple<int, int, int>>();

            var remainingPointIndices =
                Enumerable.Range(0, Points.Count).ToList();

            while (remainingPointIndices.Count > 3)
            {
                foreach (int pointIndex in remainingPointIndices)
                {
                    if (!IsEar(pointIndex, remainingPointIndices))
                    {
                        continue;
                    }

                    var triangleIndices =
                        GetTriangleIndices(pointIndex, remainingPointIndices);

                    triangleIndicesList.Add(triangleIndices);

                    remainingPointIndices.Remove(pointIndex);

                    break;
                }
            }

            triangleIndicesList.Add(
                new Tuple<int, int, int>(
                    remainingPointIndices[0],
                    remainingPointIndices[1],
                    remainingPointIndices[2]
                )
            );

            return triangleIndicesList;
        }

        private double GetInteriorAngle(Tuple<int, int, int> triangleIndices)
        {
            int pointIndex0 = triangleIndices.Item1;
            int pointIndex = triangleIndices.Item2;
            int pointIndex1 = triangleIndices.Item3;

            Point point0 = Points[pointIndex0];
            Point point = Points[pointIndex];
            Point point1 = Points[pointIndex1];

            Vector<double> vector0 = point.Vector - point0.Vector;
            Vector<double> vector = point1.Vector - point.Vector;

            bool isConvexPoint =
                MathUtils.CrossProduct(vector0, vector).DotProduct(Normal)
                >= 0.0;

            double cos =
                vector0.Normalize(2.0).DotProduct(vector.Normalize(2.0));

            double angle = Math.Acos(cos);
            if (!isConvexPoint)
            {
                angle = Math.PI * 2.0 - angle;
            }

            return angle;
        }

        private static bool IsInTriangle(
            Point testPoint, Point point0, Point point1, Point point2
        )
        {
            Vector<double> vector0 = point1.Vector - point0.Vector;
            Vector<double> vector1 = point2.Vector - point1.Vector;
            Vector<double> vector2 = point0.Vector - point2.Vector;

            Vector<double> normal = MathUtils.CrossProduct(vector0, vector1);

            Vector<double> vectorTest0 = testPoint.Vector - point0.Vector;
            Vector<double> vectorTest1 = testPoint.Vector - point1.Vector;
            Vector<double> vectorTest2 = testPoint.Vector - point2.Vector;

            bool isInEdge0 =
                MathUtils.CrossProduct(vector0, vectorTest0)
                .DotProduct(normal)
                > 0.0;

            bool isInEdge1 =
                MathUtils.CrossProduct(vector1, vectorTest1)
                .DotProduct(normal)
                > 0.0;

            bool isInEdge2 =
                MathUtils.CrossProduct(vector2, vectorTest2)
                .DotProduct(normal)
                > 0.0;

            bool isInTriangle = isInEdge0 && isInEdge1 && isInEdge2;
            return isInTriangle;
        }

        private bool IsEar(int pointIndex, IList<int> pointIndices)
        {
            Tuple<int, int, int> triangleIndices =
                GetTriangleIndices(pointIndex, pointIndices);

            if (GetInteriorAngle(triangleIndices) > Math.PI)
            {
                return false;
            }

            foreach (int pointIndex1 in pointIndices)
            {
                if (IsInTriangle(
                    Points[pointIndex1],
                    Points[triangleIndices.Item1],
                    Points[triangleIndices.Item2],
                    Points[triangleIndices.Item3]
                ))
                {
                    return false;
                }
            }

            return true;
        }

        private Tuple<int, int, int> GetTriangleIndices(
            int pointIndex, IList<int> pointIndices
        )
        {
            int pointIndexIndex = pointIndices.IndexOf(pointIndex);

            int pointIndexIndex0 =
                (pointIndexIndex - 1 + pointIndices.Count) %
                pointIndices.Count;

            int pointIndexIndex1 =
                (pointIndexIndex + 1) % pointIndices.Count;

            Tuple<int, int, int> triangleIndices =
                new Tuple<int, int, int>(
                    pointIndices[pointIndexIndex0],
                    pointIndex,
                    pointIndices[pointIndexIndex1]
                );

            return triangleIndices;
        }

        #endregion

        #region Polygon spatial relation

        /// <summary>
        /// Determines whether this <see cref="Polygon"/> contains
        /// a given <see cref="Polygon"/> on X-Y Plane, i.e.
        /// if the X-Y Plane projection of this <see cref="Polygon"/> contains
        /// the X-Y Plane projection of the given <see cref="Polygon"/>.
        /// The Z coordinates of <see cref="Points"/> are ignored,
        /// hence the name suffix 2D.
        /// </summary>
        /// 
        /// <param name="polygon">
        /// The <see cref="Polygon"/> to test if this <see cref="Polygon"/>
        /// contains.
        /// </param>
        /// <param name="epsilon">
        /// Tolerance used to deal with precision errors in the calculation.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if this <see cref="Polygon"/> contains
        /// <paramref name="polygon"/> on X-Y Plane.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="polygon"/> is <see langword="null"/>.
        /// </exception>
        public bool DoesContain2D(
            Polygon polygon, double epsilon = DefaultEpsilon
        )
        {
            if (polygon == null)
            {
                throw new ArgumentNullException(nameof(polygon));
            }

            bool doesContainAllPoints =
                polygon.Points.All(point => IsPointInside2D(point));

            if (!doesContainAllPoints)
            {
                return false;
            }

            var ppi = EdgeIntersect(this, polygon, epsilon);
            var doesIntersect = ppi.DoesIntersect;

            return !doesIntersect;
        }

        /// <summary>
        /// Determines whether this <see cref="Polygon"/> and
        /// a given <see cref="Polygon"/> overlap on X-Y Plane, i.e.
        /// if the X-Y Plane projection of this <see cref="Polygon"/> and
        /// the X-Y Plane projection of the given <see cref="Polygon"/>
        /// overlap. The Z coordinates of <see cref="Points"/> are ignored,
        /// hence the name suffix 2D.
        /// </summary>
        /// 
        /// <param name="polygon">
        /// The <see cref="Polygon"/> to test if this <see cref="Polygon"/>
        /// overlaps with.
        /// </param>
        /// <param name="epsilon">
        /// Tolerance used to deal with precision errors in the calculation.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if this <see cref="Polygon"/> and
        /// <paramref name="polygon"/> overlap on X-Y Plane.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="polygon"/> is <see langword="null"/>.
        /// </exception>
        public bool DoesOverlap2D(
            Polygon polygon, double epsilon = DefaultEpsilon
        )
        {
            if (polygon == null)
            {
                throw new ArgumentNullException(nameof(polygon));
            }

            if (!BBox.DoesOverlap(GetBBox(), polygon.GetBBox()))
            {
                return false;
            }

            foreach (Point point in polygon.Points)
            {
                if (IsPointInside2D(point))
                {
                    return true;
                }
            }

            foreach (Point point in Points)
            {
                if (polygon.IsPointInside2D(point))
                {
                    return true;
                }
            }

            var ppi = EdgeIntersect(this, polygon, epsilon);
            return ppi.DoesIntersect;
        }

        /// <summary>
        /// Calculates the intersections between pairs of edges on
        /// two <see cref="Polygon"/>s.
        /// </summary>
        /// 
        /// <param name="polygon0">
        /// The first <see cref="Polygon"/> involved in
        /// the edge (non-)intersections.
        /// </param>
        /// <param name="polygon1">
        /// The second <see cref="Polygon"/> involved in
        /// the edge (non-)intersections.
        /// </param>
        /// <param name="epsilon">
        /// Tolerance used to deal with precision errors in the calculation.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="PolygonPolygonIntersection"/> with information
        /// on edge-edge intersections.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="polygon0"/> or
        /// <paramref name="polygon1"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="epsilon"/> is negative.
        /// </exception>
        public static PolygonPolygonIntersection EdgeIntersect(
            Polygon polygon0, Polygon polygon1,
            double epsilon = DefaultEpsilon
        )
        {
            if (polygon0 == null)
            {
                throw new ArgumentNullException(nameof(polygon0));
            }

            if (polygon1 == null)
            {
                throw new ArgumentNullException(nameof(polygon1));
            }

            if (epsilon < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(epsilon),
                    $"{nameof(epsilon)} cannot be negative"
                );
            }

            var ppi = new PolygonPolygonIntersection(polygon0, polygon1);

            for (int edgeIndex0 = 0; edgeIndex0 < polygon0.NumOfEdges;
                ++edgeIndex0)
            {
                var edge0 = polygon0.GetEdge(edgeIndex0);

                for (int edgeIndex1 = 0; edgeIndex1 < polygon1.NumOfEdges;
                    ++edgeIndex1)
                {
                    var edge1 = polygon1.GetEdge(edgeIndex1);

                    var lli = LineSegment.Intersect(edge0, edge1, epsilon);

                    if (!lli.DoesIntersectBetween)
                    {
                        continue;
                    }

                    ppi.AddEdgeIntersection(edgeIndex0, edgeIndex1, lli);
                }
            }

            return ppi;
        }

        #endregion

        /// <summary>
        /// Gets a <see cref="Point"/> on the <see cref="Edges"/>
        /// of this <see cref="Polygon"/> that is closest to a given
        /// <see cref="Point"/>.
        /// </summary>
        /// 
        /// <param name="point">
        /// The <see cref="Point"/> to get the closest <see cref="Point"/>
        /// to.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="Tuple{T1, T2, T3}"/> describing
        /// the <see cref="Point"/> on the <see cref="Edges"/> of
        /// this <see cref="Polygon"/>:
        /// 
        /// <list type="bullet">
        /// <item>
        ///     <term><see cref="Tuple{T1, T2, T3}.Item1"/></term>
        ///     <description>
        ///         Index of the edge where the closest <see cref="Point"/>
        ///         is on
        ///     </description>
        /// </item>
        /// <item>
        ///     <term><see cref="Tuple{T1, T2, T3}.Item2"/></term>
        ///     <description>
        ///     Parameter on the edge <see cref="LineSegment"/>
        ///     specifying the position of the closest <see cref="Point"/>
        ///     </description>
        /// </item>
        /// <item>
        ///     <term><see cref="Tuple{T1, T2, T3}.Item3"/></term>
        ///     <description>
        ///     The closest <see cref="Point"/>
        ///     </description>
        /// </item>
        /// </list>
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point"/> is <see langword="null"/>.
        /// </exception>
        public Tuple<int, double, Point> GetClosestPointOnEdge(Point point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            double closestDistance = double.PositiveInfinity;

            int closestEdgeIndex = InvalidEdgeIndex;
            double closestEdgeParam = 0.0;
            Point closestPoint = null;

            for (int edgeIndex = 0; edgeIndex < NumOfEdges; ++edgeIndex)
            {
                LineSegment edge = GetEdge(edgeIndex);

                double currentClosestEdgeParam =
                    edge.GetClosestParamBetween(point);

                Point currentClosestPoint =
                    edge.GetPointByParam(currentClosestEdgeParam);

                double currentClosestDistance =
                    currentClosestPoint.GetDistance(point);

                if (currentClosestDistance < closestDistance)
                {
                    closestDistance = currentClosestDistance;

                    closestEdgeIndex = edgeIndex;
                    closestEdgeParam = currentClosestEdgeParam;
                    closestPoint = currentClosestPoint;
                }
            }

            return new Tuple<int, double, Point>(
                closestEdgeIndex, closestEdgeParam, closestPoint
            );
        }

        /// <summary>
        /// Flips the side of this <see cref="Polygon"/>,
        /// i.e. negating <see cref="Normal"/>.
        /// Since <see cref="Normal"/> is determined by <see cref="Points"/>,
        /// flipping is simply done by reversing <see cref="Points"/>.
        /// </summary>
        public void Flip()
        {
            _points.Reverse();
        }

        /// <summary>
        /// Offsets an edge by a given distance along its normal direction.
        /// This will modify this <see cref="Polygon"/>.
        /// </summary>
        /// 
        /// <param name="edgeIndex">
        /// Index of the edge to be offset.
        /// </param>
        /// <param name="distance">
        /// The distance along the normal direction of the edge
        /// to offset the edge. Positive values offset the edge outwards
        /// and negative values offset the edge inwards.
        /// </param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="edgeIndex"/> is out of range.
        /// </exception>
        /// 
        /// <remarks>
        /// This method does not check for or handle the possible
        /// edge intersections resulting from the offset.
        /// </remarks>
        public void OffsetEdge(int edgeIndex, double distance)
        {
            if (edgeIndex < 0 || edgeIndex >= NumOfEdges)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(edgeIndex),
                    $"{nameof(edgeIndex)} must be between " +
                    $"0 (inclusive) and {nameof(NumOfEdges)} (exclusive)"
                );
            }

            int edgeIndex0 = (edgeIndex - 1 + NumOfEdges) % NumOfEdges;
            double interiorAngle0 = GetInteriorAngle(edgeIndex);
            if (interiorAngle0 == Math.PI)
            {
                throw new InvalidOperationException(
                    "Cannot offset an edge connected to " +
                    "a straight angle or a zero angle"
                );
            }

            int edgeIndex1 = (edgeIndex + 1) % NumOfEdges;
            double interiorAngle1 = GetInteriorAngle(edgeIndex1);
            if (interiorAngle1 == Math.PI)
            {
                throw new InvalidOperationException(
                    "Cannot offset an edge connected to " +
                    "a straight angle or a zero angle"
                );
            }

            var edge0 = GetEdge(edgeIndex0);
            var edgeDir0 = edge0.Direction;
            var point0 = edge0.Point1;

            double distance0 = distance / Math.Sin(interiorAngle0);
            var offset0 = edgeDir0 * distance0;
            point0.Vector += offset0;

            var edge1 = GetEdge(edgeIndex1);
            var edgeDir1 = edge1.Direction;
            var point1 = edge1.Point0;

            double distance1 = distance / Math.Sin(interiorAngle1);
            var offset1 = -edgeDir1 * distance1;
            point1.Vector += offset1;
        }

        /// <summary>
        /// Offsets all edges by a given distance along
        /// their normal directions.
        /// This will modify this <see cref="Polygon"/>.
        /// </summary>
        /// 
        /// <param name="distance">
        /// The distance to offset the edges along their normal directions by.
        /// Positive values offset the edges outwards
        /// and negative values offset the edges inwards.
        /// </param>
        /// 
        /// <remarks>
        /// This method does not check for or handle the possible
        /// edge intersections resulting from the offsets.
        /// </remarks>
        public void OffsetEdges(double distance)
        {
            var pointOffsets = new List<Vector<double>>();

            for (int pointIndex = 0; pointIndex < Points.Count; ++pointIndex)
            {
                int edgeIndex0 =
                    (pointIndex - 1 + Points.Count) % Points.Count;
                int edgeIndex1 = pointIndex;

                var edgeNormal0 = GetEdgeNormal(edgeIndex0);
                var edgeNormal1 = GetEdgeNormal(edgeIndex1);

                var pointDir = (edgeNormal0 + edgeNormal1).Normalize(2.0);

                double interiorAngle = GetInteriorAngle(pointIndex);
                double pointDistance =
                    distance / Math.Sin(interiorAngle / 2.0);

                var pointOffset = pointDir * pointDistance;
                pointOffsets.Add(pointOffset);
            }

            for (int pointIndex = 0; pointIndex < Points.Count; ++pointIndex)
            {
                var point = Points[pointIndex];
                var pointOffset = pointOffsets[pointIndex];

                point.Vector += pointOffset;
            }
        }

        /// <summary>
        /// Gets the bounding box of this <see cref="Polygon"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The bounding box of this <see cref="Polygon"/>.
        /// </returns>
        public BBox GetBBox()
        {
            if (Points.Count == 0)
            {
                return new BBox();
            }

            double minX = Enumerable.Min(Points, point => point.X);
            double minY = Enumerable.Min(Points, point => point.Y);
            double minZ = Enumerable.Min(Points, point => point.Z);

            double maxX = Enumerable.Max(Points, point => point.X);
            double maxY = Enumerable.Max(Points, point => point.Y);
            double maxZ = Enumerable.Max(Points, point => point.Z);

            BBox bbox =
                BBox.FromMinAndMax(minX, minY, minZ, maxX, maxY, maxZ);

            return bbox;
        }

        /// <summary>
        /// Transforms this <see cref="Polygon"/> with a given transform.
        /// This <see cref="Polygon"/> will be transformed in place,
        /// i.e. modified.
        /// </summary>
        /// 
        /// <param name="transform">
        /// The transform to be applied to this <see cref="Polygon"/>.
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

            foreach (Point point in Points)
            {
                point.Transform(transform);
            }
        }

        #region Equality

        /// <summary>
        /// Determines whether this <see cref="Polygon"/> and
        /// a given object have the same value.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The object to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is
        /// a <see cref="Polygon"/> and has the same <see cref="Points"/>
        /// as this <see cref="Polygon"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as Polygon);

        /// <summary>
        /// Determines whether this <see cref="Polygon"/> and another
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The other <see cref="Polygon"/> to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="other"/>
        /// has the same <see cref="Points"/> as this <see cref="Polygon"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool Equals(Polygon other)
        {
            return ReferenceEquals(this, other) || (
                other != null &&
                Points.SequenceEqual(other.Points)
            );
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1755184145;

            foreach (Point point in Points)
            {
                hashCode = hashCode * -1521134295 + point.GetHashCode();
            }

            return hashCode;
        }

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var pointPositions =
                _points.Select(point => point.AsList()).ToList();

            info.AddValue(nameof(Points), pointPositions);
        }

        /// <summary>
        /// Determines whether two <see cref="Polygon"/>s
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="Polygon"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="Polygon"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have the same <see cref="Points"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Polygon left, Polygon right) =>
            EqualityComparer<Polygon>.Default.Equals(left, right);

        /// <summary>
        /// Determines whether two <see cref="Polygon"/>s
        /// have different values.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="Polygon"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="Polygon"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have different <see cref="Points"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Polygon left, Polygon right) =>
            !(left == right);

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// A list of <see cref="Point"/>s representing the
        /// vertices on this <see cref="Polygon"/>.
        /// </summary>
        public IReadOnlyList<Point> Points { get; }

        /// <summary>
        /// A list of <see cref="LineSegment"/>s representing the edges
        /// on this <see cref="Polygon"/>.
        /// </summary>
        public IReadOnlyList<LineSegment> Edges
        {
            get
            {
                List<LineSegment> edges =
                    Enumerable.Range(0, NumOfEdges)
                    .Select(edgeIndex => GetEdge(edgeIndex))
                    .ToList();

                return edges;
            }
        }

        /// <summary>
        /// The number of edges on this <see cref="Polygon"/>.
        /// Equals the number of vertices on this <see cref="Polygon"/>.
        /// </summary>
        public int NumOfEdges => _points.Count;

        /// <summary>
        /// A unit 3D vector representing the normal direction of
        /// this <see cref="Polygon"/>.
        /// </summary>
        public Vector<double> Normal
        {
            get
            {
                Vector<double> normalSum = new DenseVector(3);

                for (
                    int pointIndex0 = 0; pointIndex0 < Points.Count;
                    ++pointIndex0)
                {
                    int pointIndex1 = (pointIndex0 + 1) % Points.Count;
                    int pointIndex2 = (pointIndex0 + 2) % Points.Count;

                    Point point0 = Points[pointIndex0];
                    Point point1 = Points[pointIndex1];
                    Point point2 = Points[pointIndex2];

                    Vector<double> vector0 = point1.Vector - point0.Vector;
                    Vector<double> vector1 = point2.Vector - point1.Vector;

                    Vector<double> localNormal =
                        MathUtils.CrossProduct(vector0, vector1);

                    normalSum += localNormal;
                }

                Vector<double> normal = normalSum.Normalize(2.0);
                return normal;
            }
        }

        /// <summary>
        /// Gets a <see cref="Point"/> representing the centroid of
        /// this <see cref="Polygon"/> at the mean position of
        /// <see cref="Points"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// If this <see cref="Point"/> has 0 vertex, a <see cref="Point"/>
        /// at the origin is returned.
        /// </remarks>
        public Point Centroid
        {
            get
            {
                Vector<double> centroidVector = new DenseVector(3);

                foreach (Point point in Points)
                {
                    centroidVector += point.Vector;
                }

                if (Points.Count > 0)
                {
                    centroidVector /= Points.Count;
                }

                Point centroid = new Point(centroidVector);
                return centroid;
            }
        }

        /// <summary>
        /// Gets the area of this <see cref="Polygon"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// References:
        /// <list type="bullet">
        /// <item>
        ///     <description>
        ///     Shoelace formula for calculating polygon areas:
        ///     https://mathworld.wolfram.com/PolygonArea.html
        ///     </description>
        /// </item>
        /// </list>
        /// </remarks>
        public double Area
        {
            get
            {
                if (Points.Count == 0)
                {
                    return 0.0;
                }

                Vector<double> vectorOrigin = Points[0].Vector;
                Vector<double> normal = Normal;

                double area = 0.0;

                for (int pointIndex0 = 0; pointIndex0 < Points.Count;
                    ++pointIndex0)
                {
                    int pointIndex1 = (pointIndex0 + 1) % Points.Count;

                    Point point0 = Points[pointIndex0];
                    Point point1 = Points[pointIndex1];

                    Vector<double> vector0 = point0.Vector - vectorOrigin;
                    Vector<double> vector1 = point1.Vector - vectorOrigin;

                    Vector<double> crossProduct =
                        MathUtils.CrossProduct(vector0, vector1);

                    double areaItem = crossProduct.L2Norm();

                    if (crossProduct.DotProduct(normal) < 0.0)
                    {
                        areaItem *= -1.0;
                    }

                    area += areaItem;
                }

                area *= 0.5;

                return area;
            }
        }

        #endregion

        #region Fields

        private readonly List<Point> _points = new List<Point>();

        #endregion

        #region Constants

        /// <summary>
        /// Default tolerance used in certain methods that need to deal with
        /// floating point errors, such as
        /// <see cref="DoesContain2D"/>, <see cref="DoesOverlap2D"/> and
        /// <see cref="EdgeIntersect"/>.
        /// </summary>
        public const double DefaultEpsilon = 1e-6;

        /// <summary>
        /// Represents an invalid edge index.
        /// </summary>
        public const int InvalidEdgeIndex = -1;

        #endregion
    }
}
