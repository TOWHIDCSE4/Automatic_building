using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;

namespace DaiwaRentalGD.Revit
{
    /// <summary>
    /// Converts <see cref="Document"/>s from Revit unit catalog representing
    /// Type C units into <see cref="TypeCUnitComponent"/> instances.
    /// </summary>
    public class TypeCUnitCatalogEntryConverter : UnitCatalogEntryConverter
    {
        #region Constructors

        public TypeCUnitCatalogEntryConverter() : base()
        { }

        #endregion

        #region Methods

        protected override CatalogUnitComponent Convert(
            Document document,
            List<(string levelName, List<Room> rooms)> roomGroups,
            int index
        )
        {
            var unitComp = base.Convert(document, roomGroups, index)
                as TypeCUnitComponent;

            var rooms = roomGroups[index].rooms;

            unitComp.PositionType = GetPositionType(index);
            unitComp.EntranceType = GetEntranceType(document, rooms);

            UpdateBalconySettings(unitComp);

            return unitComp;
        }

        protected override CatalogUnitComponent Create() =>
            new TypeCUnitComponent();

        private TypeCUnitPositionType GetPositionType(int index)
        {
            switch (index)
            {
                case 0:
                    return TypeCUnitPositionType.FirstFloor;

                case 1:
                    return TypeCUnitPositionType.SecondFloor;

                default:

                    throw new ArgumentException(
                        $"Invalid {nameof(index)}",
                        nameof(index)
                    );
            }
        }

        private TypeCUnitEntranceType GetEntranceType(
            Document document, IReadOnlyList<Room> rooms
        )
        {
            var doorInstances =
                RoomConverter.GetUnitDoors(document, UnitDoorFamilyName);

            var doorLocations = doorInstances.Select(
                doorInstance => doorInstance.Location
            ).OfType<LocationPoint>();

            var doorSourceYs =
                doorLocations.Select(location => location.Point.Y);

            var doorYs = doorSourceYs.Select(
                sourceY => UnitUtils.Convert(
                    sourceY,
                    RoomConverter.SourceUnitType,
                    RoomConverter.TargetUnitType
                )
            );

            var roomPlans = RoomConverter.GetUnionedRoomPlans(rooms);

            var (minPoint, maxPoint) =
                RoomConverter.GetMinMaxPoints(roomPlans);

            double maxDoorNorthDistance =
                doorYs.Max(doorY => Math.Abs(maxPoint.Y - doorY));

            double maxDoorSouthDistance =
                doorYs.Max(doorY => Math.Abs(minPoint.Y - doorY));

            return maxDoorNorthDistance <= maxDoorSouthDistance ?
                TypeCUnitEntranceType.North : TypeCUnitEntranceType.South;
        }

        private void UpdateBalconySettings(TypeCUnitComponent unitComp)
        {
            // `anchorPoint` is the corner point of the unit with
            // minimum X among all the corner points with minimum Y

            var anchorPoint = new Geometries.Point(
                double.PositiveInfinity,
                double.PositiveInfinity,
                0.0
            );

            foreach (var point in
                unitComp.RoomPlans.SelectMany(roomPlan => roomPlan.Points))
            {
                if (point.Y < anchorPoint.Y)
                {
                    anchorPoint = point;
                }
                else if (point.Y == anchorPoint.Y && point.X < anchorPoint.X)
                {
                    anchorPoint = point;
                }
            }

            var balconyEdge = unitComp.RoomPlans[0].Edges.First(
                edge => edge.Point0 == unitComp.BalconyAnchorPoint
            );

            double balconyMargin = LengthP.PToM(BalconyMarginP);

            var balconyAnchorPoint = new Geometries.Point(
                anchorPoint.Vector + balconyEdge.Direction * balconyMargin
            );

            double balconyLength = balconyEdge.Length - balconyMargin * 2.0;

            unitComp.BalconyAnchorPoint = balconyAnchorPoint;
            unitComp.BalconyLength = balconyLength;
        }

        #endregion

        #region Properties

        public new static TypeCUnitCatalogEntryConverter Default { get; } =
            new TypeCUnitCatalogEntryConverter();

        public string UnitDoorFamilyName
        {
            get => _unitDoorFamilyName;
            set => _unitDoorFamilyName = value ??
                throw new ArgumentNullException(nameof(value));
        }

        public double BalconyMarginP
        {
            get => _balconyMarginP;
            set => _balconyMarginP = value >= 0.0 ? value :
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"{nameof(BalconyMarginP)} must be positive"
                );
        }

        #endregion

        #region Fields

        private string _unitDoorFamilyName = DefaultUnitDoorFamilyName;

        private double _balconyMarginP = DefaultBalconyMarginP;

        #endregion

        #region Constants

        public const string DefaultUnitDoorFamilyName = "玄関ドア15D7GD42";

        public const double DefaultBalconyMarginP = 0.5;

        #endregion
    }
}
