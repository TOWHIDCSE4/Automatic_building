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
    /// Represents the road slant planes of a site.
    /// </summary>
    [Serializable]
    public class RoadSlantPlanesComponent : Component
    {
        #region Constructors

        public RoadSlantPlanesComponent() : base()
        { }

        protected RoadSlantPlanesComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _slopeProjectedLength =
                reader.GetValue<double>(nameof(SlopeProjectedLength));

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
                reader.GetValues<RoadSlantPlanesViolation>(nameof(Violations))
            );
        }

        #endregion

        #region Methods

        #region Planes

        public Polygon GetRoadsideSlopePlane(int edgeIndex)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            SiteComponent sc = Site.SiteComponent;

            LineSegment edge = sc.OppositeRoadEdges[edgeIndex];

            Vector<double> edgeNormal =
                sc.GetOppositeRoadEdgeNormal(edgeIndex);

            Vector<double> slopeCombinedVector =
                edgeNormal + UpDirection * SlopeAngleTangent;

            Point point0 = edge.Point0;
            Point point1 = edge.Point1;

            Vector<double> vector2 =
                point1.Vector +
                slopeCombinedVector * SlopeProjectedLength;
            Point point2 = new Point(vector2);

            Vector<double> vector3 =
                point0.Vector +
                slopeCombinedVector * SlopeProjectedLength;
            Point point3 = new Point(vector3);

            Polygon slopePlane = new Polygon(
                new[] { point0, point1, point2, point3 }
            );

            if (slopePlane.Normal.DotProduct(UpDirection) < 0.0)
            {
                slopePlane.Flip();
            }

            return slopePlane;
        }

        public Polygon GetRoadsideVerticalPlane(int edgeIndex)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            SiteComponent sc = Site.SiteComponent;

            LineSegment edge = sc.OppositeRoadEdges[edgeIndex];

            Vector<double> edgeNormal =
                sc.GetOppositeRoadEdgeNormal(edgeIndex);

            Vector<double> slopeCombinedVector =
                edgeNormal + UpDirection * SlopeAngleTangent;

            Vector<double> vector0 =
                edge.Point0.Vector +
                slopeCombinedVector * SlopeProjectedLength;
            Point point0 = new Point(vector0);

            Vector<double> vector1 =
                edge.Point1.Vector +
                slopeCombinedVector * SlopeProjectedLength;
            Point point1 = new Point(vector1);

            Vector<double> vector2 =
                vector1 + UpDirection * VerticalPlaneHeight;
            Point point2 = new Point(vector2);

            Vector<double> vector3 =
                vector0 + UpDirection * VerticalPlaneHeight;
            Point point3 = new Point(vector3);

            Polygon verticalPlane = new Polygon(
                new[] { point0, point1, point2, point3 }
            );

            if (verticalPlane.Normal.DotProduct(edgeNormal) > 0.0)
            {
                verticalPlane.Flip();
            }

            return verticalPlane;
        }

        public IReadOnlyList<Polygon> GetRoadsidePlanes(int edgeIndex)
        {
            if (Site == null) { return new List<Polygon>(); }

            List<Polygon> edgePlanes = new List<Polygon>
            {
                GetRoadsideSlopePlane(edgeIndex),
                GetRoadsideVerticalPlane(edgeIndex)
            };

            return edgePlanes;
        }

        private void UpdatePlanes()
        {
            _planesByEdge.Clear();

            if (Site == null) { return; }

            for (
                int edgeIndex = 0;
                edgeIndex < Site.SiteComponent.OppositeRoadEdges.Count;
                ++edgeIndex
            )
            {
                IReadOnlyList<Polygon> roadsidePlanes =
                    GetRoadsidePlanes(edgeIndex);

                _planesByEdge.Add(edgeIndex, roadsidePlanes);
            }
        }

        #endregion

        #region Violations

        public bool IsPointInRoadsidePlanesRange(int edgeIndex, Point point)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            SiteComponent sc = Site.SiteComponent;

            LineSegment edge = sc.OppositeRoadEdges[edgeIndex];

            Vector<double> edgeNormal =
                sc.GetOppositeRoadEdgeNormal(edgeIndex);

            Vector<double> vector0 = point.Vector - edge.Point0.Vector;
            Vector<double> vector1 = point.Vector - edge.Point1.Vector;

            bool isInRange =
                MathUtils.CrossProduct(vector0, edgeNormal)
                .DotProduct(UpDirection) >= 0.0 &&
                MathUtils.CrossProduct(edgeNormal, vector1)
                .DotProduct(UpDirection) >= 0.0;

            return isInRange;
        }

        private bool ViolatesRoadsidePlanes(int edgeIndex, Point point)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            bool isInRange = IsPointInRoadsidePlanesRange(edgeIndex, point);

            if (!isInRange) { return false; }

            IReadOnlyList<Polygon> edgePlanes = _planesByEdge[edgeIndex];

            return edgePlanes.All(plane => plane.IsPointAbove(point));
        }

        public RoadSlantPlanesViolation GetViolation(Point point)
        {
            if (Site == null)
            {
                return new RoadSlantPlanesViolation { Point = point };
            }

            var violation = new RoadSlantPlanesViolation
            {
                Site = Site,
                Point = point
            };

            for (
                int edgeIndex = 0;
                edgeIndex < Site.SiteComponent.OppositeRoadEdges.Count;
                ++edgeIndex
            )
            {
                if (ViolatesRoadsidePlanes(edgeIndex, point))
                {
                    violation.OppositeRoadEdgeIndices.Add(edgeIndex);
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

            if (bc.NumOfUnits == 0)
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

            List<RoadSlantPlanesViolation> violations =
                testPoints
                .Select(point => GetViolation(point))
                .Where(violation => violation.OppositeRoadEdgeIndices.Any())
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

            writer.AddValue(
                nameof(SlopeProjectedLength), _slopeProjectedLength
            );

            writer.AddValue(nameof(SlopeAngleTangent), _slopeAngleTangent);

            writer.AddValue(nameof(Site), _site);

            writer.AddValue(nameof(Building), _building);

            writer.AddValues(nameof(PlanesByEdge), _planesByEdge.ToList());

            writer.AddValues(nameof(Violations), _violations);
        }

        #endregion

        #region Properties

        public double SlopeProjectedLength
        {
            get => _slopeProjectedLength;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SlopeProjectedLength)} " +
                        "cannot be negative"
                    );
                }

                _slopeProjectedLength = value;
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
                        $"{nameof(SlopeAngleTangent)} " +
                        "cannot be negative"
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

        public IReadOnlyList<RoadSlantPlanesViolation> Violations
        { get => _violations; }

        public bool IsValid => !Violations.Any();

        public static Vector<double> UpDirection =>
            new DenseVector(new[] { 0.0, 0.0, 1.0 });

        #endregion

        #region Member variables

        private double _slopeProjectedLength = DefaultSlopeProjectedLength;
        private double _slopeAngleTangent = DefaultSlopeAngleTangent;

        private Site _site;
        private Building _building;

        private readonly Dictionary<int, IReadOnlyList<Polygon>>
            _planesByEdge = new Dictionary<int, IReadOnlyList<Polygon>>();

        private readonly List<RoadSlantPlanesViolation>
            _violations = new List<RoadSlantPlanesViolation>();

        #endregion

        #region Constants

        public const double DefaultSlopeProjectedLength = 20.0;
        public const double DefaultSlopeAngleTangent = 1.25;

        public const double VerticalPlaneHeight = 10.0;

        #endregion
    }
}
