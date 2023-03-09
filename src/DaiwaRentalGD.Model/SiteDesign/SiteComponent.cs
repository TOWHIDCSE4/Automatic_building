using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.SiteDesign
{
    /// <summary>
    /// A component that describes a site.
    /// </summary>
    [Serializable]
    public class SiteComponent : Component, IBBox
    {
        #region Constructors

        public SiteComponent() : base()
        {
            Boundary = new Polygon();
        }

        protected SiteComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _boundary = reader.GetValue<Polygon>(nameof(Boundary));
            _boundaryEdgeTypes.AddRange(
                reader.GetValues<SiteEdgeType>(nameof(BoundaryEdgeTypes))
            );
            _oppositeRoadEdges.AddRange(
                reader.GetValues<LineSegment>(nameof(OppositeRoadEdges))
            );
            _trueNorthAngle = reader.GetValue<double>(nameof(TrueNorthAngle));
        }

        #endregion

        #region Methods

        public void SetBoundaryEdgeType(int edgeIndex, SiteEdgeType type)
        {
            _boundaryEdgeTypes[edgeIndex] = type;
        }

        public void InsertOppositeRoadEdge(int edgeIndex, LineSegment edge)
        {
            _oppositeRoadEdges.Insert(
                edgeIndex,
                edge ?? throw new ArgumentNullException(nameof(edge))
            );
        }

        public void AddOppositeRoadEdge(LineSegment edge)
        {
            InsertOppositeRoadEdge(OppositeRoadEdges.Count, edge);
        }

        public void RemoveOppositeRoadEdge(int edgeIndex)
        {
            _oppositeRoadEdges.RemoveAt(edgeIndex);
        }

        public void ClearOppositeRoadEdges()
        {
            _oppositeRoadEdges.Clear();
        }

        public Vector<double> GetOppositeRoadEdgeNormal(int edgeIndex)
        {
            Vector<double> boundaryNormal = Boundary.Normal;

            LineSegment edge = OppositeRoadEdges[edgeIndex];
            Vector<double> edgeDirection = edge.Direction;

            Point boundaryCentroid = Boundary.Centroid;
            Point edgeMidpoint = edge.GetPointByParam(0.5);
            Vector<double> centroidDirection =
                (boundaryCentroid.Vector - edgeMidpoint.Vector)
                .Normalize(2.0);

            Vector<double> edgeNormal =
                MathUtils.CrossProduct(boundaryNormal, edgeDirection)
                .Normalize(2.0);

            if (edgeNormal.DotProduct(centroidDirection) < 0.0)
            {
                edgeNormal *= -1.0;
            }

            return edgeNormal;
        }

        public bool IsNorthEdge(int edgeIndex, bool isStrictNorth)
        {
            var edgeNormal = Boundary.GetEdgeNormal(edgeIndex);

            var trueNorthCos = edgeNormal.DotProduct(TrueNorthDirection);

            if (isStrictNorth)
            {
                return trueNorthCos > 0.0;
            }
            else
            {
                return trueNorthCos >= 0.0;
            }
        }

        public IReadOnlyList<int> GetNorthEdgeIndices(bool isStrictNorth)
        {
            var northEdgeIndices =
                Enumerable.Range(0, Boundary.NumOfEdges)
                .Where(edgeIndex => IsNorthEdge(edgeIndex, isStrictNorth))
                .ToList();

            return northEdgeIndices;
        }

        public void ClearSite()
        {
            Boundary = new Polygon();

            ClearOppositeRoadEdges();

            TrueNorthAngle = 0.0;
        }

        public void UpdateCollisionBody2D()
        {
            var cbc = SceneObject?.GetComponent<CollisionBody2DComponent>();

            if (cbc == null) { return; }

            cbc.ClearCollisionPolygons();

            var collisionPolygon = new Polygon(Boundary);
            collisionPolygon.OffsetEdges(cbc.Epsilon);

            cbc.AddCollisionPolygon(collisionPolygon);
        }

        public BBox GetBBox()
        {
            var bboxObjects = new List<IBBox>();

            bboxObjects.Add(Boundary);
            bboxObjects.AddRange(OppositeRoadEdges);

            BBox scBbox = BBox.GetBBox(bboxObjects);
            return scBbox;
        }

        public BBox GetBBoxOfBounddary()
        {
            var bboxObjects = new List<IBBox>();

            bboxObjects.Add(Boundary);

            BBox scBbox = BBox.GetBBox(bboxObjects);
            return scBbox;
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Boundary), _boundary);
            writer.AddValues(nameof(BoundaryEdgeTypes), _boundaryEdgeTypes);
            writer.AddValues(nameof(OppositeRoadEdges), _oppositeRoadEdges);
            writer.AddValue(nameof(TrueNorthAngle), _trueNorthAngle);
        }

        #endregion

        #region Properties

        public Polygon Boundary
        {
            get => _boundary;
            set
            {
                _boundary =
                    value ?? throw new ArgumentNullException(nameof(value));

                _boundaryEdgeTypes.Clear();

                _boundaryEdgeTypes.AddRange(
                    Enumerable.Repeat(
                        SiteEdgeType.Unknown, _boundary.NumOfEdges
                    )
                );

                UpdateCollisionBody2D();
            }
        }

        public IReadOnlyList<SiteEdgeType> BoundaryEdgeTypes
        {
            get => _boundaryEdgeTypes;
        }

        public IReadOnlyList<LineSegment> PropertyEdges
        {
            get
            {
                List<LineSegment> propertyEdges =

                    Boundary.Edges.Zip(
                        BoundaryEdgeTypes,
                        (e, t) => new Tuple<LineSegment, SiteEdgeType>(e, t)
                    )
                    .Where(et => et.Item2 == SiteEdgeType.Property)
                    .Select(et => et.Item1)
                    .ToList();

                return propertyEdges;
            }
        }

        public IReadOnlyList<int> PropertyEdgeIndices
        {
            get
            {
                List<int> propertyEdgeIndices =

                    Enumerable.Range(0, Boundary.Edges.Count).Zip(
                        BoundaryEdgeTypes,
                        (i, t) => new Tuple<int, SiteEdgeType>(i, t)
                    )
                    .Where(it => it.Item2 == SiteEdgeType.Property)
                    .Select(it => it.Item1)
                    .ToList();

                return propertyEdgeIndices;
            }
        }

        public IReadOnlyList<LineSegment> RoadEdges
        {
            get
            {
                List<LineSegment> roadEdges =

                    Boundary.Edges.Zip(
                        BoundaryEdgeTypes,
                        (e, t) => new Tuple<LineSegment, SiteEdgeType>(e, t)
                    )
                    .Where(et => et.Item2 == SiteEdgeType.Road)
                    .Select(et => et.Item1)
                    .ToList();

                return roadEdges;
            }
        }

        public IReadOnlyList<int> RoadEdgeIndices
        {
            get
            {
                List<int> roadEdgeIndices =

                    Enumerable.Range(0, Boundary.Edges.Count).Zip(
                        BoundaryEdgeTypes,
                        (i, t) => new Tuple<int, SiteEdgeType>(i, t)
                    )
                    .Where(it => it.Item2 == SiteEdgeType.Road)
                    .Select(it => it.Item1)
                    .ToList();

                return roadEdgeIndices;
            }
        }

        public IReadOnlyList<LineSegment> OppositeRoadEdges
        {
            get => _oppositeRoadEdges;
        }

        public double TrueNorthAngle
        {
            get => _trueNorthAngle;
            set
            {
                _trueNorthAngle = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TrueNorthDirection));
            }
        }

        public Vector<double> TrueNorthDirection
        {
            get
            {
                var trueNorthTf = new TrsTransform3D
                {
                    Rz = TrueNorthAngle
                };

                var trueNorthDir = trueNorthTf.Transform(
                    new DenseVector(new[] { 0.0, 1.0, 0.0 })
                );
                return trueNorthDir;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value.L2Norm() == 0.0)
                {
                    throw new ArgumentException(
                        $"{nameof(TrueNorthDirection)} cannot be zero vector",
                        nameof(value)
                    );
                }

                _trueNorthAngle = MathUtils.GetAngle2D(
                    new DenseVector(new[] { 0.0, 1.0, 0.0 }),
                    value.Normalize(2.0)
                );

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(TrueNorthAngle));
            }
        }

        #endregion

        #region Member variables

        private Polygon _boundary;

        private readonly List<SiteEdgeType> _boundaryEdgeTypes =
            new List<SiteEdgeType>();

        private readonly List<LineSegment> _oppositeRoadEdges =
            new List<LineSegment>();

        private double _trueNorthAngle;

        #endregion
    }
}
