using System;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeB
{
    /// <summary>
    /// A scene object that designs a building with Type B units.
    /// </summary>
    [Serializable]
    public class TypeBBuildingDesigner : BuildingDesigner
    {
        #region Constructors

        public TypeBBuildingDesigner() : base()
        {
            UnitArrangerComponent = new TypeBUnitArrangerComponent();

            RoofCreatorComponent = GetDefaultRoofCreatorComponent((TypeBUnitArrangerComponent)UnitArrangerComponent);

            BuildingEntranceCreatorComponent =
                new TypeBBuildingEntranceCreatorComponent();

            BuildingPlacementComponent = new BuildingPlacementComponent();
        }

        protected TypeBBuildingDesigner(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public static RoofCreatorComponent GetDefaultRoofCreatorComponent(TypeBUnitArrangerComponent typeBUnitArrangerComponent)
        {
            RoofCreatorComponent Roof = new FlatRoofCreatorComponent();
            if (typeBUnitArrangerComponent != null)
            {

                switch (typeBUnitArrangerComponent.RoofType)
                {
                    case TypeBUnitRoofType.Gable:
                        Roof = new GableRoofCreatorComponent();
                        break;
                    case TypeBUnitRoofType.Flat:
                        Roof = new FlatRoofCreatorComponent();
                        break;
                }
            }
            return Roof;
        }

        #endregion

        #region Properties

        public override UnitArrangerComponent UnitArrangerComponent
        {
            get => base.UnitArrangerComponent;
            set =>
                base.UnitArrangerComponent =
                value as TypeBUnitArrangerComponent ??
                throw new ArgumentException(
                    $"{nameof(value)} must be of type " +
                    $"{nameof(TypeBUnitArrangerComponent)}"
                );
        }

        public override BuildingEntranceCreatorComponent
            BuildingEntranceCreatorComponent
        {
            get => base.BuildingEntranceCreatorComponent;
            set =>
                base.BuildingEntranceCreatorComponent =
                value as TypeBBuildingEntranceCreatorComponent ??
                throw new ArgumentException(
                    $"{nameof(value)} must be of type " +
                    $"{nameof(TypeBBuildingEntranceCreatorComponent)}"
                );
        }

        #endregion
    }
}
