using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeC
{
    /// <summary>
    /// A component that creates entrances for a building with Type C units.
    /// </summary>
    [Serializable]
    public class TypeCBuildingEntranceCreatorComponent :
        BuildingEntranceCreatorComponent
    {
        #region Constructors

        public TypeCBuildingEntranceCreatorComponent() : base()
        { }

        protected TypeCBuildingEntranceCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public BuildingEntrance GetUnitEntrance(
            IReadOnlyList<Unit> groundFloorUnits, int stack
        )
        {
            Unit unit = groundFloorUnits[stack];

            TypeCUnitComponent uc = unit.UnitComponent as TypeCUnitComponent;

            Polygon roomPlan = uc.RoomPlans[0];
            BBox roomPlanBbox = roomPlan.GetBBox();

            Point entrancePoint;
            Vector<double> entranceDirection;

            int numOfUnitsPerFloor = groundFloorUnits.Count;

            bool isLastOddUnit =
                numOfUnitsPerFloor % 2 == 1 &&
                stack == numOfUnitsPerFloor - 1;
            bool isLeftUnit = stack % 2 == 0;

            if (isLastOddUnit)
            {
                if (uc.EntranceType == TypeCUnitEntranceType.North)
                {
                    entrancePoint = new Point
                    {
                        X = (roomPlan.Points[2].X + roomPlan.Points[3].X)
                            / 2.0,
                        Y = roomPlanBbox.MaxY,
                        Z = roomPlanBbox.MinZ
                    };
                    entranceDirection =
                        new DenseVector(new[] { 0.0, -1.0, 0.0 });
                }
                else
                {
                    entrancePoint = new Point
                    {
                        X = (roomPlan.Points[2].X + roomPlan.Points[3].X)
                            / 2.0,
                        Y = roomPlanBbox.MinY,
                        Z = roomPlanBbox.MinZ
                    };
                    entranceDirection =
                        new DenseVector(new[] { 0.0, 1.0, 0.0 });
                }
            }
            else if (isLeftUnit)
            {
                if (uc.EntranceType == TypeCUnitEntranceType.North)
                {
                    entrancePoint = new Point
                    {
                        X = roomPlanBbox.MaxX,
                        Y = roomPlanBbox.MaxY,
                        Z = roomPlanBbox.MinZ
                    };
                    entranceDirection =
                        new DenseVector(new[] { 0.0, -1.0, 0.0 });
                }
                else
                {
                    entrancePoint = new Point
                    {
                        X = roomPlanBbox.MaxX,
                        Y = roomPlanBbox.MinY,
                        Z = roomPlanBbox.MinZ
                    };
                    entranceDirection =
                        new DenseVector(new[] { 0.0, 1.0, 0.0 });
                }
            }
            else
            {
                return null;
            }

            ITransform3D unitTf = unit.TransformComponent.Transform;
            entrancePoint.Transform(unitTf);

            BuildingEntrance entrance = new BuildingEntrance
            {
                EntrancePoint = entrancePoint,
                EntranceDirection = entranceDirection
            };
            return entrance;
        }

        public override void CreateEntrances(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            BuildingComponent bc = building.BuildingComponent;

            bc.ClearEntrances();

            if (bc.NumOfUnits == 0) { return; }

            var groundFloorUnits = bc.GetFloorUnits(0);

            List<BuildingEntrance> entrances = new List<BuildingEntrance>();

            for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
            {
                if (stack % 2 == 1) { continue; }

                Unit unit = groundFloorUnits[stack];

                if (unit == null) { continue; }

                var entrance = GetUnitEntrance(groundFloorUnits, stack);

                entrances.Add(entrance);
            }

            foreach (var entrance in entrances)
            {
                bc.AddEntrance(entrance);
            }
        }

        #endregion
    }
}
