using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeA
{
    /// <summary>
    /// A component that creates entrances for a building with Type A units.
    /// </summary>
    [Serializable]
    public class TypeABuildingEntranceCreatorComponent :
        BuildingEntranceCreatorComponent
    {
        #region Constructors

        public TypeABuildingEntranceCreatorComponent() : base()
        { }

        protected TypeABuildingEntranceCreatorComponent(
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
                var staircase = bc.GetStaircase(0, stack);

                if (staircase == null)
                {
                    continue;
                }

                var sc = staircase.StaircaseComponent;
                var staircaseTf = staircase.TransformComponent.Transform;

                var be = new BuildingEntrance(sc.Entrance);
                be.Transform(staircaseTf);

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
