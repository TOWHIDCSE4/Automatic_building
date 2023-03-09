using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeB
{
    /// <summary>
    /// A component that creates entrances for a building with Type B units.
    /// </summary>
    [Serializable]
    public class TypeBBuildingEntranceCreatorComponent :
        BuildingEntranceCreatorComponent
    {
        #region Constructors

        public TypeBBuildingEntranceCreatorComponent() : base()
        { }

        protected TypeBBuildingEntranceCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public override void CreateEntrances(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            BuildingComponent bc = building.BuildingComponent;

            bc.ClearEntrances();

            if (bc.NumOfUnits == 0)
            {
                return;
            }

            var buildingEntrances = new List<BuildingEntrance>();

            for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
            {
                var corridor = bc.GetCorridor(0, stack);

                if (corridor == null)
                {
                    continue;
                }

                var cc = corridor.CorridorComponent;
                var corridorTf = corridor.TransformComponent.Transform;

                var be = new BuildingEntrance(cc.Entrance);
                be.Transform(corridorTf);

                buildingEntrances.Add(be);
            }

            foreach (var be in buildingEntrances)
            {
                bc.AddEntrance(be);
            }
        }

        #endregion
    }
}
