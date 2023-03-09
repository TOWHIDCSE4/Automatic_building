using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Revit
{
    /// <summary>
    /// Converts <see cref="Room"/> into types from
    /// <see cref="Geometries"/> and extracts other information.
    /// </summary>
    public class RoomConverter
    {
        #region Constructors

        public RoomConverter()
        { }

        #endregion

        #region Methods

        public IReadOnlyList<Room> GetRooms(Document document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var filter = new ElementCategoryFilter(BuiltInCategory.OST_Rooms);

            var collector = new FilteredElementCollector(document);

            var elements = collector.WherePasses(filter).ToElements();

            var rooms = elements.OfType<Room>().ToList();

            return rooms;
        }

        public List<(string levelName, List<Room> rooms)> GroupRoomsByLevel(
            IReadOnlyList<Room> rooms
        )
        {
            if (rooms is null)
            {
                throw new ArgumentNullException(nameof(rooms));
            }

            var roomGroups = rooms
                .GroupBy(room => room.Level.Name)
                .Select(roomGroup => (
                    levelName: roomGroup.Key,
                    rooms: roomGroup.ToList()
                ))
                .OrderBy(roomGroup => roomGroup.levelName)
                .ToList();

            return roomGroups;
        }

        public double GetRoomHeight(Room room)
        {
            if (room is null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            var closedShell = room.ClosedShell;

            var bbox = closedShell.GetBoundingBox();

            double sourceHeight = bbox.Max.Z - bbox.Min.Z;

            double height = UnitUtils.Convert(
                sourceHeight, SourceUnitType, TargetUnitType
            );

            return height;
        }

        public IReadOnlyList<FamilyInstance> GetUnitDoors(
            Document document, string familyName
        )
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (familyName is null)
            {
                throw new ArgumentNullException(nameof(familyName));
            }

            var filter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);

            var collector = new FilteredElementCollector(document);

            var instances = collector
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .ToElements()
                .OfType<FamilyInstance>()
                .ToList();

            var unitDoorInstances = instances
                .Where(instance => instance.Symbol.FamilyName == familyName)
                .ToList();

            return unitDoorInstances;
        }

        public IReadOnlyList<Polygon> GetRegionPolygons(Room room)
        {
            if (room is null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            var options = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation =
                    SpatialElementBoundaryLocation.Center
            };

            var regions = room.GetBoundarySegments(options);

            var regionPolygons = regions.Select(GetRegionPolygon).ToList();

            return regionPolygons;
        }

        public Polygon GetRegionPolygon(IList<BoundarySegment> region)
        {
            if (region is null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            var points = new List<Geometries.Point>();

            foreach (var boundarySegment in region)
            {
                var endpoint = boundarySegment.GetCurve().GetEndPoint(0);

                double x = UnitUtils.Convert(
                    endpoint.X, SourceUnitType, TargetUnitType
                );

                double y = UnitUtils.Convert(
                    endpoint.Y, SourceUnitType, TargetUnitType
                );

                var point = new Geometries.Point(x, y, 0.0);

                points.Add(point);
            }

            var regionPolygon = new Polygon(points);

            return regionPolygon;
        }

        public IReadOnlyList<Polygon> GetUnionedRoomPlans(
            IReadOnlyList<Room> rooms
        )
        {
            if (rooms is null)
            {
                throw new ArgumentNullException(nameof(rooms));
            }

            var regionPolygons = rooms.SelectMany(GetRegionPolygons).ToList();

            var roomPlans = PolygonBooleanOp.Union(regionPolygons);

            return roomPlans;
        }

        public IReadOnlyList<Polygon> ConvertToShiftedQuantizedPlanPs(
            IReadOnlyList<Polygon> roomPlans
        )
        {
            if (roomPlans is null)
            {
                throw new ArgumentNullException(nameof(roomPlans));
            }

            var shiftedRoomPlans = ShiftPlansToOrigin(roomPlans);

            var shiftedRoomPlanPs = shiftedRoomPlans
                .Select(UnitComponent.ConvertPlanToPlanP).ToList();

            var quantizedRoomPlanPs = GetQuantizedPlanPs(shiftedRoomPlanPs);

            return quantizedRoomPlanPs;
        }

        public (Geometries.Point min, Geometries.Point max) GetMinMaxPoints(
            IReadOnlyList<Polygon> roomPlans
        )
        {
            if (roomPlans is null)
            {
                throw new ArgumentNullException(nameof(roomPlans));
            }

            var minPoint = new Geometries.Point(
                double.PositiveInfinity,
                double.PositiveInfinity,
                0.0
            );

            var maxPoint = new Geometries.Point(
                double.NegativeInfinity,
                double.NegativeInfinity,
                0.0
            );

            foreach (var point in
                roomPlans.SelectMany(roomPlan => roomPlan.Points))
            {
                if (point.X < minPoint.X)
                {
                    minPoint.X = point.X;
                }
                if (point.Y < minPoint.Y)
                {
                    minPoint.Y = point.Y;
                }

                if (point.X > maxPoint.X)
                {
                    maxPoint.X = point.X;
                }
                if (point.Y > maxPoint.Y)
                {
                    maxPoint.Y = point.Y;
                }
            }

            return (min: minPoint, max: maxPoint);
        }

        private IReadOnlyList<Polygon> ShiftPlansToOrigin(
            IReadOnlyList<Polygon> roomPlans
        )
        {
            var minPoint = GetMinMaxPoints(roomPlans).min;

            var shiftedRoomPlans = new List<Polygon>();

            foreach (var roomPlan in roomPlans)
            {
                var shiftedPoints = roomPlan.Points.Select(
                    point => new Geometries.Point(
                        point.X - minPoint.X,
                        point.Y - minPoint.Y,
                        0.0
                    )
                );

                var shiftedRoomPlan = new Polygon(shiftedPoints);

                shiftedRoomPlans.Add(shiftedRoomPlan);
            }

            return shiftedRoomPlans;
        }

        private IReadOnlyList<Polygon> GetQuantizedPlanPs(
            IReadOnlyList<Polygon> roomPlanPs
        )
        {
            var quantizedRoomPlanPs = new List<Polygon>();

            foreach (var roomPlanP in roomPlanPs)
            {
                var quantizedPointPs = roomPlanP.Points.Select(QuantizeP);

                var quantizedRoomPlanP = new Polygon(quantizedPointPs);

                quantizedRoomPlanPs.Add(quantizedRoomPlanP);
            }

            return quantizedRoomPlanPs;
        }

        private Geometries.Point QuantizeP(Geometries.Point pointP)
        {
            if (PrecisionP == 0.0)
            {
                return new Geometries.Point(pointP);
            }

            double quantizedXP =
                Math.Round(pointP.X / PrecisionP) * PrecisionP;

            double quantizedYP =
                Math.Round(pointP.Y / PrecisionP) * PrecisionP;

            double quantizedZP =
                Math.Round(pointP.Z / PrecisionP) * PrecisionP;

            var quantizedPointP = new Geometries.Point(
                quantizedXP, quantizedYP, quantizedZP
            );
            return quantizedPointP;
        }

        #endregion

        #region Properties

        public static RoomConverter Default { get; } = new RoomConverter();

        public PolygonBooleanOp PolygonBooleanOp
        {
            get => _polygonBooleanOp;
            set => _polygonBooleanOp = value ??
                throw new ArgumentNullException(nameof(value));
        }

        public DisplayUnitType SourceUnitType { get; set; } =
            DefaultSourceUnitType;

        public DisplayUnitType TargetUnitType { get; set; } =
            DefaultTargetUnitType;

        public double PrecisionP
        {
            get => _precisionP;
            set => _precisionP = value >= 0.0 ? value :
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"{nameof(PrecisionP)} must be non-negative"
                );
        }

        #endregion

        #region Fields

        public PolygonBooleanOp _polygonBooleanOp = PolygonBooleanOp.Default;

        private double _precisionP = DefaultPrecisionP;

        #endregion

        #region Constants

        public const double DefaultPrecisionP = 0.5;

        public const DisplayUnitType DefaultSourceUnitType =
            DisplayUnitType.DUT_DECIMAL_FEET;

        public const DisplayUnitType DefaultTargetUnitType =
            DisplayUnitType.DUT_METERS;

        #endregion
    }
}
