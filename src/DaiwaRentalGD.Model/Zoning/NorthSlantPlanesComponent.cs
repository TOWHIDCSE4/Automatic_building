using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Scene;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Zoning
{
    /// <summary>
    /// A component that describes north slant planes.
    /// </summary>
    [Serializable]
    public class NorthSlantPlanesComponent : Component
    {
        #region Constructors

        public NorthSlantPlanesComponent() : base()
        { }

        protected NorthSlantPlanesComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _slopeStartHeight =
                reader.GetValue<double>(nameof(SlopeStartHeight));

            _slopeAngleTangent =
                reader.GetValue<double>(nameof(SlopeAngleTangent));

            _site = reader.GetValue<Site>(nameof(Site));

            _building = reader.GetValue<Building>(nameof(Building));

            var planesByEdgePairs =
                reader.GetValues<KeyValuePair<int, IReadOnlyList<Polygon>>>(
                    nameof(PlanesByEdge)
                );

            foreach (var pair in planesByEdgePairs)
            {
                _planesByEdge.Add(pair.Key, pair.Value);
            }

            _violations.AddRange(
                reader.GetValues<NorthSlantPlanesViolation>(
                    nameof(Violations)
                )
            );
        }

        #endregion

        #region Methods

        #region Planes

        public IReadOnlyList<int> GetNorthPropertyEdgeIndices()
        {
            if (Site == null)
            {
                return new List<int>();
            }

            var sc = Site.SiteComponent;

            List<int> northPropertyEdgeIndices =
                sc.GetNorthEdgeIndices(true)
                .Where(
                    edgeIndex =>
                    sc.BoundaryEdgeTypes[edgeIndex] ==
                    SiteEdgeType.Property
                ).ToList();

            return northPropertyEdgeIndices;
        }

        public Polygon GetEdgeVerticalPlane(int edgeIndex)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            Polygon boundary = Site.SiteComponent.Boundary;

            LineSegment edge = boundary.GetEdge(edgeIndex);

            Point point0 = edge.Point0;
            Point point1 = edge.Point1;

            Vector<double> vector2 =
                point1.Vector + UpDirection * SlopeStartHeight;
            Point point2 = new Point(vector2);

            Vector<double> vector3 =
                point0.Vector + UpDirection * SlopeStartHeight;
            Point point3 = new Point(vector3);

            Polygon verticalPlane = new Polygon(
                new[] { point0, point1, point2, point3 }
            );

            return verticalPlane;
        }

        public double GetSlopeProjectedLength(int pointIndex)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            var sc = Site.SiteComponent;

            var rotatedSiteBoundary = new Polygon(sc.Boundary);
            var rotationTf = new TrsTransform3D { Rz = -sc.TrueNorthAngle };
            rotatedSiteBoundary.Transform(rotationTf);

            Point point = rotatedSiteBoundary.Points[pointIndex];

            BBox rotatedSiteBoundaryBbox =
                rotatedSiteBoundary.GetBBox();

            double slopeProjectedLength =
                point.Y - rotatedSiteBoundaryBbox.MinY;

            return slopeProjectedLength;
        }

        public Polygon GetEdgeSlopePlane(int edgeIndex)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            Polygon boundary = Site.SiteComponent.Boundary;

            int pointIndex0 = edgeIndex;
            int pointIndex1 = (edgeIndex + 1) % boundary.Points.Count;

            double slopeProjectedLength0 =
                GetSlopeProjectedLength(pointIndex0);
            double slopeProjectedLength1 =
                GetSlopeProjectedLength(pointIndex1);

            LineSegment edge = boundary.GetEdge(edgeIndex);

            Vector<double> vector0 =
                edge.Point0.Vector + UpDirection * SlopeStartHeight;
            Vector<double> vector1 =
                edge.Point1.Vector + UpDirection * SlopeStartHeight;

            Vector<double> slopeCombinedVector =
                -TrueNorthDirection + UpDirection * SlopeAngleTangent;

            Vector<double> vector2 =
                vector1 + slopeCombinedVector * slopeProjectedLength1;

            Vector<double> vector3 =
                vector0 + slopeCombinedVector * slopeProjectedLength0;

            Point point0 = new Point(vector0);
            Point point1 = new Point(vector1);
            Point point2 = new Point(vector2);
            Point point3 = new Point(vector3);

            Polygon slopePlane = new Polygon(
                new[] { point0, point1, point2, point3 }
            );
            return slopePlane;
        }

        public IReadOnlyList<Polygon> GetEdgePlanes(int edgeIndex)
        {
            if (Site == null) { return new List<Polygon>(); }

            List<Polygon> edgePlanes = new List<Polygon>
            {
                GetEdgeVerticalPlane(edgeIndex),
                GetEdgeSlopePlane(edgeIndex)
            };

            return edgePlanes;
        }

        public void UpdatePlanes()
        {
            _planesByEdge.Clear();

            if (Site == null) { return; }

            foreach (int edgeIndex in GetNorthPropertyEdgeIndices())
            {
                IReadOnlyList<Polygon> edgePlanes = GetEdgePlanes(edgeIndex);

                _planesByEdge.Add(edgeIndex, edgePlanes);
            }
        }

        #endregion

        #region Violations

        public bool IsPointInEdgePlanesRange(int edgeIndex, Point point)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            Polygon boundary = Site.SiteComponent.Boundary;
            LineSegment edge = boundary.GetEdge(edgeIndex);

            Point point0 = edge.Point0;
            Point point1 = edge.Point1;

            bool isOnOrLeftOfEdge = MathUtils.CrossProduct(
                edge.Direction,
                point.Vector - point0.Vector
            )[2] >= 0.0;

            bool isOnOrRightOfNegTrueNorth0 = MathUtils.CrossProduct(
                point.Vector - point0.Vector,
                -TrueNorthDirection
            )[2] >= 0.0;

            bool isOnOrLeftOfNegTrueNorth1 = MathUtils.CrossProduct(
                -TrueNorthDirection,
                point.Vector - point1.Vector
            )[2] >= 0.0;

            bool isInRange =
                isOnOrLeftOfEdge &&
                isOnOrRightOfNegTrueNorth0 &&
                isOnOrLeftOfNegTrueNorth1;

            return isInRange;
        }

        private bool ViolatesEdgePlanes(int edgeIndex, Point point)
        {
            if (Site == null) { return false; }

            bool isInRange = IsPointInEdgePlanesRange(edgeIndex, point);

            if (!isInRange) { return false; }

            IReadOnlyList<Polygon> edgePlanes = _planesByEdge[edgeIndex];

            return edgePlanes.Any(plane => plane.IsPointAbove(point));
        }

        public NorthSlantPlanesViolation GetViolation(Point point)
        {
            if (Site == null)
            {
                return new NorthSlantPlanesViolation { Point = point };
            }

            var violation = new NorthSlantPlanesViolation
            {
                Site = Site,
                Point = point
            };

            foreach (int edgeIndex in PlanesByEdge.Keys)
            {
                if (ViolatesEdgePlanes(edgeIndex, point))
                {
                    violation.NorthPropertyEdgeIndices.Add(edgeIndex);
                }
            }

            return violation;
        }

        public static IReadOnlyList<Point> GetTestPoints(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            var bc = building.BuildingComponent;

            if (bc.NumOfFloors == 0)
            {
                return new List<Point>();
            }

            List<Point> testPoints = new List<Point>();

            IReadOnlyList<Unit> topFloorUnits =
                bc.GetFloorUnits(bc.NumOfFloors - 1);

            foreach (Unit unit in topFloorUnits)
            {
                if (unit == null) { continue; }

                UnitComponent uc = unit.UnitComponent;

                List<Point> unitTestPoints = new List<Point>();

                ITransform3D worldTf =
                    unit.TransformComponent.GetWorldTransform();

                for (
                    int roomIndex = 0; roomIndex < uc.RoomPlans.Count;
                    ++roomIndex
                )
                {
                    Mesh roomMesh = uc.GetRoomMesh(roomIndex);
                    unitTestPoints.AddRange(roomMesh.Points.Select(
                        point => new Point(point)
                    ));
                }

                foreach (Point point in unitTestPoints)
                {
                    point.Transform(worldTf);
                }
                testPoints.AddRange(unitTestPoints);
            }

            for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
            {
                var staircase = bc.GetStaircase(bc.NumOfFloors - 1, stack);

                if (staircase == null)
                {
                    continue;
                }

                var sc = staircase.StaircaseComponent;
                var staircaseWorldTf =
                    staircase.TransformComponent.GetWorldTransform();

                Mesh staircaseMesh = sc.GetMesh();
                staircaseMesh.Transform(staircaseWorldTf);

                var staircaseTestPoints = staircaseMesh.Points.Select(
                    point => new Point(point)
                );

                testPoints.AddRange(staircaseTestPoints);
            }

            for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
            {
                var corridor = bc.GetCorridor(bc.NumOfFloors - 1, stack);

                if (corridor == null)
                {
                    continue;
                }

                var cc = corridor.CorridorComponent;
                var corridorWorldTf =
                    corridor.TransformComponent.GetWorldTransform();

                Mesh corridorMesh = cc.GetMesh();
                corridorMesh.Transform(corridorWorldTf);

                var corridorTestPoints = corridorMesh.Points.Select(
                    point => new Point(point)
                );

                testPoints.AddRange(corridorTestPoints);
            }

            for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
            {
                var balcony = bc.GetBalcony(bc.NumOfFloors - 1, stack);

                if (balcony == null)
                {
                    continue;
                }

                Mesh balconyMesh =
                    balcony.BalconyComponent.GetMesh();

                var balconyWorldTf =
                    balcony.TransformComponent.GetWorldTransform();

                balconyMesh.Transform(balconyWorldTf);

                var balconyTestPoints = balconyMesh.Points.Select(
                    point => new Point(point)
                );

                testPoints.AddRange(balconyTestPoints);
            }

            for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
            {
                var roof = bc.GetRoof(bc.NumOfFloors - 1, stack);

                if (roof == null)
                {
                    continue;
                }

                var rc = roof.RoofComponent;
                var roofWorldTf = roof.TransformComponent.GetWorldTransform();

                Mesh roofMesh = rc.GetMesh();
                roofMesh.Transform(roofWorldTf);

                var roofTestPoints = roofMesh.Points.Select(
                    point => new Point(point)
                );

                testPoints.AddRange(roofTestPoints);
            }

            return testPoints;
        }

        public void UpdateViolations()
        {
            _violations.Clear();

            if (Site == null || Building == null) { return; }

            IReadOnlyList<Point> testPoints = GetTestPoints(Building);

            List<NorthSlantPlanesViolation> violations =
                testPoints
                .Select(point => GetViolation(point))
                .Where(violation => violation.NorthPropertyEdgeIndices.Any())
                .ToList();

            _violations.AddRange(violations);

            NotifyPropertyChanged(nameof(Violations));
            NotifyPropertyChanged(nameof(IsValid));
        }

        #endregion

        protected override void Update()
        {
            UpdatePlanes();
            UpdateViolations();
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(Site).Append(Building)
            .Concat(Violations);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(SlopeStartHeight), _slopeStartHeight);

            writer.AddValue(nameof(SlopeAngleTangent), _slopeAngleTangent);

            writer.AddValue(nameof(Site), _site);

            writer.AddValue(nameof(Building), _building);

            writer.AddValues(nameof(PlanesByEdge), _planesByEdge.ToList());

            writer.AddValues(nameof(Violations), _violations);

        }

        #endregion

        #region Properties

        public double SlopeStartHeight
        {
            get => _slopeStartHeight;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SlopeStartHeight)} cannot be negative"
                    );
                }

                _slopeStartHeight = value;
                NotifyPropertyChanged();
            }
        }

        public double SlopeAngleTangent
        {
            get => _slopeAngleTangent;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SlopeAngleTangent)} cannot be negative"
                    );
                }

                _slopeAngleTangent = value;
                NotifyPropertyChanged();
            }
        }

        public Site Site
        {
            get => _site;
            set
            {
                _site = value;
                NotifyPropertyChanged();
            }
        }

        public Building Building
        {
            get => _building;
            set
            {
                _building = value;
                NotifyPropertyChanged();
            }
        }

        public IReadOnlyDictionary<int, IReadOnlyList<Polygon>>
            PlanesByEdge => _planesByEdge;

        public IReadOnlyList<NorthSlantPlanesViolation> Violations
        { get => _violations; }

        public bool IsValid => !Violations.Any();

        public Vector<double> TrueNorthDirection =>
            Site.SiteComponent.TrueNorthDirection;

        public Vector<double> UpDirection
            => new DenseVector(new[] { 0.0, 0.0, 1.0 });

        #endregion

        #region Member variables

        private double _slopeStartHeight = DefaultSlopeStartHeight;
        private double _slopeAngleTangent = DefaultSlopeAngleTangent;

        private Site _site;
        private Building _building;

        private readonly Dictionary<int, IReadOnlyList<Polygon>>
            _planesByEdge = new Dictionary<int, IReadOnlyList<Polygon>>();

        private readonly List<NorthSlantPlanesViolation>
            _violations = new List<NorthSlantPlanesViolation>();

        #endregion

        #region Constants

        public const double DefaultSlopeStartHeight = 5.0;
        public const double DefaultSlopeAngleTangent = 0.8;

        #endregion
    }
}
