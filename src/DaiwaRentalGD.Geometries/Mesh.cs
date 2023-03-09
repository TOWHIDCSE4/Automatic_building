using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// A polygon mesh represented in the face-vertex data structure.
    /// </summary>
    [Serializable]
    public class Mesh : IBBox, IEquatable<Mesh>, ISerializable
    {
        #region Constructors

        /// <summary>
        /// Creates an empty <see cref="Mesh"/>.
        /// </summary>
        public Mesh()
        {
            Points = _points.AsReadOnly();
            Faces = _faces.AsReadOnly();
        }

        /// <summary>
        /// Creates a deep copy of a given <see cref="Mesh"/>.
        /// </summary>
        /// 
        /// <param name="mesh">
        /// The <see cref="Mesh"/> to be copied.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="mesh"/> is <see langword="null"/>.
        /// </exception>
        public Mesh(Mesh mesh) : this()
        {
            if (mesh == null)
            {
                throw new ArgumentNullException(nameof(mesh));
            }

            foreach (Point point in mesh.Points)
            {
                _points.Add(new Point(point));
            }

            foreach (var face in mesh.Faces)
            {
                _faces.Add(new List<int>(face));
            }
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// 
        /// <param name="info">
        /// Contains data of a serialized <see cref="Mesh"/>
        /// instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected Mesh(SerializationInfo info, StreamingContext context) :
            this()
        {
            var pointPositions =
                info.GetValue<List<List<double>>>(nameof(Points));

            _points.AddRange(pointPositions.Select(
                pointPosition => new Point(pointPosition)
            ));

            _faces.AddRange(info.GetValue<List<List<int>>>(nameof(Faces)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a <see cref="Point"/> to this <see cref="Mesh"/> as
        /// a vertex at the end of <see cref="Points"/>.
        /// </summary>
        /// 
        /// <param name="point">
        /// The <see cref="Point"/> to be added as a vertex.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="point"/> is <see langword="null"/>.
        /// </exception>
        public void AddPoint(Point point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            _points.Add(point);
        }

        /// <summary>
        /// Adds a face to this <see cref="Mesh"/> by referencing
        /// existing vertex indices in <see cref="Points"/>.
        /// </summary>
        /// 
        /// <param name="faceVertices">
        /// Ordered indices of vertices from <see cref="Points"/>
        /// that define vertices of the face to be added.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="faceVertices"/> is
        /// <see langword="null"/>.
        /// </exception>
        public void AddFace(IReadOnlyList<int> faceVertices)
        {
            foreach (int faceVertex in faceVertices)
            {
                if (faceVertex < 0 || faceVertex >= Points.Count)
                {
                    throw new ArgumentException(
                        $"Invalid face vertex {faceVertices} " +
                        $"in {nameof(faceVertices)}",
                        nameof(faceVertices)
                    );
                }
            }

            _faces.Add(new List<int>(faceVertices));
        }

        /// <summary>
        /// Adds a <see cref="Polygon"/> to this <see cref="Mesh"/> as a face.
        /// The vertices of the <see cref="Polygon"/> will be added
        /// by reference, and a new face will be made by referencing
        /// the indices to these vertices after they are added to
        /// this <see cref="Mesh"/>.
        /// </summary>
        /// 
        /// <param name="polygon">
        /// The <see cref="Polygon"/> to be added to this <see cref="Mesh"/>.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="polygon"/> is <see langword="null"/>.
        /// </exception>
        /// 
        /// <remarks>
        /// This method does not check for or handle overlapping
        /// <see cref="Point"/>s between vertices of
        /// <paramref name="polygon"/> and existing vertices of
        /// this <see cref="Mesh"/>.
        /// </remarks>
        public void AddPolygon(Polygon polygon)
        {
            if (polygon == null)
            {
                throw new ArgumentNullException(nameof(polygon));
            }

            List<int> faceVertices = new List<int>();

            foreach (Point point in polygon.Points)
            {
                _points.Add(point);

                faceVertices.Add(Points.Count - 1);
            }

            AddFace(faceVertices);
        }

        /// <summary>
        /// Gets the <see cref="Polygon"/> that represents a face of
        /// this <see cref="Mesh"/>.
        /// </summary>
        /// 
        /// <param name="faceIndex">
        /// The index of the face to get.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="Polygon"/> representing the face specified by
        /// <paramref name="faceIndex"/>. The <see cref="Points"/> of
        /// the returned <see cref="Polygon"/> are added by reference from
        /// <see cref="Points"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="faceIndex"/> is out of range.
        /// </exception>
        public Polygon GetPolygon(int faceIndex)
        {
            if (faceIndex < 0 || faceIndex >= Faces.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(faceIndex));
            }

            IReadOnlyList<int> faceVertices = Faces[faceIndex];

            var points = faceVertices.Select(
                faceVertex => Points[faceVertex]
            );

            Polygon polygon = new Polygon(points);
            return polygon;
        }

        /// <summary>
        /// Gets the list of edges on this <see cref="Mesh"/>.
        /// </summary>
        /// 
        /// <returns>
        /// A list of <see cref="Tuple{T1, T2}"/>ss each specifying an edge
        /// on this <see cref="Mesh"/> using a pair of vertex indicies into
        /// <see cref="Points"/> specifying the endpoints of an edge.
        /// All edges are undirectional, and a pair of vertex indices are
        /// always ordered with the smaller one being
        /// <see cref="Tuple{T1, T2}.Item1"/>.
        /// </returns>
        public IReadOnlyList<Tuple<int, int>> GetEdges()
        {
            var edgeSet = new SortedSet<Tuple<int, int>>();

            foreach (var face in Faces)
            {
                for (int faceVertexIndex0 = 0; faceVertexIndex0 < face.Count;
                    ++faceVertexIndex0)
                {
                    int faceVertexIndex1 =
                        (faceVertexIndex0 + 1) % face.Count;

                    int faceVertex0 = face[faceVertexIndex0];
                    int faceVertex1 = face[faceVertexIndex1];

                    int minFaceVertex = Math.Min(faceVertex0, faceVertex1);
                    int maxFaceVertex = Math.Max(faceVertex0, faceVertex1);

                    var edge =
                        new Tuple<int, int>(minFaceVertex, maxFaceVertex);

                    edgeSet.Add(edge);
                }
            }

            var edges = edgeSet.ToList();

            return edges;
        }

        /// <summary>
        /// Transforms this <see cref="Mesh"/> in place with a given
        /// transform. This <see cref="Mesh"/> will be modified.
        /// </summary>
        /// 
        /// <param name="transform">
        /// The transform used to transform this <see cref="Mesh"/>.
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

        /// <summary>
        /// Gets the bounding box of this <see cref="Mesh"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The bounding box of this <see cref="Mesh"/>.
        /// </returns>
        public BBox GetBBox()
        {
            return BBox.GetBBox(Points);
        }

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var pointPositions =
                _points.Select(point => point.AsList()).ToList();

            info.AddValue(nameof(Points), pointPositions);

            info.AddValue(nameof(Faces), _faces);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1100501661;

            foreach (var point in _points)
            {
                hashCode = hashCode * -1521134295 +
                    EqualityComparer<Point>.Default.GetHashCode(point);
            }

            foreach (var face in _faces)
            {
                hashCode = hashCode * -1521134295 + face.Count;

                foreach (var pointIndex in face)
                {
                    hashCode = hashCode * -1521134295 + pointIndex;
                }
            }

            return hashCode;
        }

        /// <summary>
        /// Determines whether this <see cref="Mesh"/> and
        /// a given object have the same value.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The object to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is
        /// a <see cref="Mesh"/> and has the same points and faces
        /// as this <see cref="Mesh"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as Mesh);

        /// <summary>
        /// Determines whether this <see cref="Mesh"/> and another
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="other">
        /// The other <see cref="Mesh"/> to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="other"/>
        /// has the same points and faces as this <see cref="Mesh"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool Equals(Mesh other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (!_points.SequenceEqual(other._points))
            {
                return false;
            }

            if (_faces.Count != other._faces.Count)
            {
                return false;
            }

            bool isAnyFaceDiff =
                _faces.Zip(
                    other._faces,
                    (face, otherFace) => face.SequenceEqual(otherFace)
                ).Any(isFaceSame => !isFaceSame);

            if (isAnyFaceDiff)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether two <see cref="Mesh"/> instances
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="Mesh"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="Mesh"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/>
        /// and <paramref name="right"/> have the same points and faces.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Mesh left, Mesh right) =>
            EqualityComparer<Mesh>.Default.Equals(left, right);

        /// <summary>
        /// Determines whether two <see cref="Mesh"/> instances
        /// have different values.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="Mesh"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="Mesh"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/>
        /// and <paramref name="right"/> have different points or faces.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Mesh left, Mesh right) =>
            !(left == right);

        #endregion

        #region Properties

        /// <summary>
        /// A list of <see cref="Point"/>s representing the
        /// vertices on this <see cref="Mesh"/>.
        /// </summary>
        public IReadOnlyList<Point> Points { get; }

        /// <summary>
        /// A list of lists of vertex indices. Each inner list of
        /// vertex indices specifies the vertices of a face on
        /// this <see cref="Mesh"/>.
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> Faces { get; }

        #endregion

        #region Fields

        private readonly List<Point> _points = new List<Point>();
        private readonly List<List<int>> _faces = new List<List<int>>();

        #endregion
    }
}
