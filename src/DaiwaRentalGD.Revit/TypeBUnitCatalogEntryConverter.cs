using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;

namespace DaiwaRentalGD.Revit
{
    /// <summary>
    /// Converts <see cref="Document"/>s from Revit unit catalog representing
    /// Type B units into <see cref="TypeBUnitComponent"/> instances.
    /// </summary>
    public class TypeBUnitCatalogEntryConverter : UnitCatalogEntryConverter
    {
        #region Constructors

        public TypeBUnitCatalogEntryConverter() : base()
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
                as TypeBUnitComponent;

            unitComp.LayoutType = GetLayoutType(unitComp);

            unitComp.StaircaseAnchorPoint = GetStaircaseAnchorPoint(unitComp);

            unitComp.CorridorAnchorPoint = GetCorridorAnchorPoint(unitComp);

            UpdateBalconySettings(unitComp);

            return unitComp;
        }

        protected override CatalogUnitComponent Create() =>
            new TypeBUnitComponent();

        private TypeBUnitLayoutType GetLayoutType(
            TypeBUnitComponent unitComp
        )
        {
            var entryName = unitComp.EntryName;

            switch (entryName.VariantType)
            {
                case TypeBUnitComponent.BasicUnitVariationType:

                    return TypeBUnitLayoutType.Basic;

                case TypeBUnitComponent.StairUnitVariationType:

                    return TypeBUnitLayoutType.Stair;

                default:

                    throw new NotSupportedException(
                        $"Unsupported " +
                        $"{nameof(UnitCatalogEntryName.VariantType)}"
                    );
            }
        }

        private Geometries.Point GetStaircaseAnchorPoint(
            TypeBUnitComponent unitComp
        )
        {
            var roomPlan = unitComp.RoomPlans[0];

            int staircaseAnchorPointIndex =
                Enumerable.Range(0, roomPlan.Points.Count)
                .First(
                    pointIndex =>
                        roomPlan.GetInteriorAngle(pointIndex) > Math.PI
                );

            var staircaseAnchorPoint =
                roomPlan.Points[staircaseAnchorPointIndex];

            return staircaseAnchorPoint;
        }

        private Geometries.Point GetCorridorAnchorPoint(
            TypeBUnitComponent unitComp
        )
        {
            // `anchorPoint` is the corner point of the unit with
            // minimum X among all the corner points with maximum Y

            var anchorPoint = new Geometries.Point(
                double.PositiveInfinity,
                double.NegativeInfinity,
                0.0
            );

            foreach (var point in
                unitComp.RoomPlans.SelectMany(roomPlan => roomPlan.Points))
            {
                if (point.Y > anchorPoint.Y)
                {
                    anchorPoint = point;
                }
                else if (point.Y == anchorPoint.Y && point.X < anchorPoint.X)
                {
                    anchorPoint = point;
                }
            }

            return anchorPoint;
        }

        private void UpdateBalconySettings(TypeBUnitComponent unitComp)
        {
            var minPoint = new Geometries.Point(
                double.PositiveInfinity,
                double.PositiveInfinity,
                0.0
            );

            foreach (var point in
                unitComp.RoomPlans.SelectMany(roomPlan => roomPlan.Points))
            {
                if (point.X < minPoint.X)
                {
                    minPoint = point;
                }
                else if (point.X == minPoint.X && point.Y < minPoint.Y)
                {
                    minPoint = point;
                }
            }

            var balconyEdge = unitComp.RoomPlans[0].Edges.First(
                edge => edge.Point0 == unitComp.BalconyAnchorPoint
            );

            double balconyMargin = LengthP.PToM(BalconyMarginP);

            var balconyAnchorPoint = new Geometries.Point(
                minPoint.Vector + balconyEdge.Direction * balconyMargin
            );

            double balconyLength = balconyEdge.Length - balconyMargin * 2.0;

            unitComp.BalconyAnchorPoint = balconyAnchorPoint;
            unitComp.BalconyLength = balconyLength;
        }

        #endregion

        #region Properties

        public new static TypeBUnitCatalogEntryConverter Default { get; } =
            new TypeBUnitCatalogEntryConverter();

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

        public const double DefaultBalconyMarginP = 0.0;

        #endregion
    }
}
