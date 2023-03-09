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
    /// Represents the a Absolute Height Slant Planes Component.
    /// </summary>
    [Serializable]
    public class AbsoluteHeightSlantPlanesComponent : Component
    {
        #region Constructors

        public AbsoluteHeightSlantPlanesComponent() : base()
        { }

        protected AbsoluteHeightSlantPlanesComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);
            _absoluteHeightPlane = reader.GetValue<double>(nameof(AbsoluteHeightPlane));

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
                reader.GetValues<AbsoluteHeightSlantPlanesViolation>(
                    nameof(Violations)
                )
            );
        }

        #endregion
        #region Methods

        #region Planes

        public Polygon GetPropertyEdgeVerticalPlane(int edgeIndex)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            SiteComponent sc = Site.SiteComponent;

            LineSegment edge = sc.Boundary.GetEdge(edgeIndex);

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

        public double GetPropertyEdgeSlopeLength(int edgeIndex)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            BBox siteBbox = Site.SiteComponent.Boundary.GetBBox();

            double slopeLength = siteBbox.MaxX;
            if (edgeIndex == 2)
            {
                slopeLength = siteBbox.MaxY;
            }


            return slopeLength;
        }

        public Polygon GetPropertyEdgeSlopePlane(int edgeIndex)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            SiteComponent sc = Site.SiteComponent;

            Polygon boundary = sc.Boundary;

            LineSegment edge = boundary.GetEdge(edgeIndex);

            Vector<double> vector0 =
                edge.Point0.Vector +
                UpDirection * AbsoluteHeightPlane;
            Vector<double> vector1 =
                edge.Point1.Vector +
                UpDirection * AbsoluteHeightPlane;

            Vector<double> edgeNormal = boundary.GetEdgeNormal(edgeIndex);
            Vector<double> slopeDirection = (-edgeNormal).Normalize(2.0);

            double slopeLength = GetPropertyEdgeSlopeLength(edgeIndex);


            Vector<double> vector2 = vector1 + slopeDirection * slopeLength;
            Vector<double> vector3 = vector0 + slopeDirection * slopeLength;
            //Vector<double> slopeDirection =
            //  (-edgeNormal + UpDirection * SlopeAngleTangent)
            //  .Normalize(2.0);

            //double slopeLength = GetPropertyEdgeSlopeLength();

            //Vector<double> vector2 = vector1 + slopeDirection * slopeLength;
            //Vector<double> vector3 = vector0 + slopeDirection * slopeLength;



            Point point0 = new Point(vector0);
            Point point1 = new Point(vector1);
            Point point2 = new Point(vector2);
            Point point3 = new Point(vector3);

            Polygon slopePlane = new Polygon(
                new[] { point0, point1, point2, point3 }
            );
            return slopePlane;
        }

        public IReadOnlyList<Polygon> GetPropertyEdgePlanes(int edgeIndex)
        {
            if (Site == null) { return new List<Polygon>(); }

            List<Polygon> edgePlanes = new List<Polygon>
            {
               // GetPropertyEdgeVerticalPlane(edgeIndex),
                GetPropertyEdgeSlopePlane(edgeIndex)
            };

            return edgePlanes;
        }

        private void UpdatePlanes()
        {
            _planesByEdge.Clear();

            if (Site == null) { return; }
            var data = Site.SiteComponent.PropertyEdgeIndices;
            if (data.Count == 0)
            {
                IReadOnlyList<Polygon> propertyEdgePlanes =
                       GetPropertyEdgePlanes(5);

                _planesByEdge.Add(5, propertyEdgePlanes);
            }
            else
            {
                foreach (int edgeIndex in Site.SiteComponent.PropertyEdgeIndices)
                {
                    IReadOnlyList<Polygon> propertyEdgePlanes =
                        GetPropertyEdgePlanes(edgeIndex);

                    _planesByEdge.Add(edgeIndex, propertyEdgePlanes);
                    break;
                }
            }


        }

        #endregion

        #region Violations

        public bool IsPointInPropertyEdgePlanesRange(
            int edgeIndex, Point point
        )
        {
            if (Site == null) { return false; }

            Polygon boundary = Site.SiteComponent.Boundary;

            LineSegment edge = boundary.GetEdge(edgeIndex);

            Vector<double> edgeNormal = boundary.GetEdgeNormal(edgeIndex);

            Vector<double> vector0 = point.Vector - edge.Point0.Vector;
            Vector<double> vector1 = point.Vector - edge.Point1.Vector;

            bool isInRange =
                MathUtils.CrossProduct(edgeNormal, vector0)
                .DotProduct(UpDirection) >= 0.0 &&
                MathUtils.CrossProduct(vector1, edgeNormal)
                .DotProduct(UpDirection) >= 0.0;

            return isInRange;
        }

        private bool ViolatesPropertyEdgePlanes(int edgeIndex, Point point)
        {
            if (Site == null) { return false; }

            bool isInRange =
                IsPointInPropertyEdgePlanesRange(edgeIndex, point);

            if (!isInRange) { return false; }

            IReadOnlyList<Polygon> edgePlanes = _planesByEdge[edgeIndex];

            return edgePlanes.Any(plane => plane.IsPointAbove(point));
        }

        public AbsoluteHeightSlantPlanesViolation GetViolation(Point point)
        {
            if (Site == null)
            {
                return new AbsoluteHeightSlantPlanesViolation { Point = point };
            }

            var violation = new AbsoluteHeightSlantPlanesViolation
            {
                Site = Site,
                Point = point
            };

            foreach (int edgeIndex in Site.SiteComponent.PropertyEdgeIndices)
            {
                violation.PropertyEdgeIndices.Add(edgeIndex);
                break;

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

            IReadOnlyList<Unit> topFloorUnits = bc.GetFloorUnits(bc.NumOfFloors - 1);
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

            List<AbsoluteHeightSlantPlanesViolation> violations =
                testPoints
                .Select(point => GetViolation(point))
                .Where(violation => violation.Point.Z > _absoluteHeightPlane)
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
            writer.AddValue(nameof(AbsoluteHeightPlane), _absoluteHeightPlane);

            writer.AddValue(nameof(SlopeStartHeight), _slopeStartHeight);

            writer.AddValue(nameof(SlopeAngleTangent), _slopeAngleTangent);

            writer.AddValue(nameof(Site), _site);

            writer.AddValue(nameof(Building), _building);

            writer.AddValues(nameof(PlanesByEdge), _planesByEdge.ToList());

            writer.AddValues(nameof(Violations), _violations);
        }

        #endregion

        #region Properties
        public double AbsoluteHeightPlane
        {
            get => _absoluteHeightPlane;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(AbsoluteHeightPlane)} " +
                        "cannot be negative"
                    );
                }

                _absoluteHeightPlane = value;
                NotifyPropertyChanged();
            }
        }

        public double SlopeStartHeight
        {
            get => _slopeStartHeight;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SlopeStartHeight)} " +
                        "cannot be negative"
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

        public IReadOnlyList<AbsoluteHeightSlantPlanesViolation> Violations =>
            _violations;

        public bool IsValid => !Violations.Any();

        public static Vector<double> UpDirection =>
            new DenseVector(new[] { 0.0, 0.0, 1.0 });

        #endregion

        #region Member variables

        private double _slopeStartHeight = DefaultSlopeStartHeight;
        private double _slopeAngleTangent = DefaultSlopeAngleTangent;
        private double _absoluteHeightPlane = DefaultAbsoluteHeightPlane;


        private Site _site;
        private Building _building;

        private readonly Dictionary<int, IReadOnlyList<Polygon>>
            _planesByEdge = new Dictionary<int, IReadOnlyList<Polygon>>();

        private readonly List<AbsoluteHeightSlantPlanesViolation>
            _violations = new List<AbsoluteHeightSlantPlanesViolation>();

        #endregion

        #region Constants

        public const double DefaultSlopeStartHeight = 20.0;
        public const double DefaultSlopeAngleTangent = 1.25;
        public const double DefaultAbsoluteHeightPlane = 10.0;

        #endregion
    }
}
