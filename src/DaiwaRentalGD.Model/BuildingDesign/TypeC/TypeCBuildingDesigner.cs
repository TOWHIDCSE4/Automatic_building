using System;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeC
{
    /// <summary>
    /// A scene object that designs a building with Type C units.
    /// </summary>
    [Serializable]
    public class TypeCBuildingDesigner : BuildingDesigner
    {
        #region Constructors

        public TypeCBuildingDesigner() : base()
        {
            UnitArrangerComponent = new TypeCUnitArrangerComponent();

            RoofCreatorComponent = GetDefaultRoofCreatorComponent((TypeCUnitArrangerComponent)UnitArrangerComponent);

            BuildingEntranceCreatorComponent =
                new TypeCBuildingEntranceCreatorComponent();

            BuildingPlacementComponent = new BuildingPlacementComponent();
        }

        protected TypeCBuildingDesigner(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public static RoofCreatorComponent GetDefaultRoofCreatorComponent(TypeCUnitArrangerComponent typeCUnitArrangerComponent)
        {
            RoofCreatorComponent Roof = new FlatRoofCreatorComponent();
            if (typeCUnitArrangerComponent != null)
            {

                switch (typeCUnitArrangerComponent.RoofType)
                {
                    case TypeCUnitRoofType.Gable:
                        Roof = new GableRoofCreatorComponent();
                        break;
                    case TypeCUnitRoofType.Flat:
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
                value as TypeCUnitArrangerComponent ??
                throw new ArgumentException(
                    $"{nameof(value)} must be of type " +
                    $"{nameof(TypeCUnitArrangerComponent)}"
                );
        }

        public override BuildingEntranceCreatorComponent
            BuildingEntranceCreatorComponent
        {
            get => base.BuildingEntranceCreatorComponent;
            set =>
                base.BuildingEntranceCreatorComponent =
                value as TypeCBuildingEntranceCreatorComponent ??
                throw new ArgumentException(
                    $"{nameof(value)} must be of type " +
                    $"{nameof(TypeCBuildingEntranceCreatorComponent)}"
                );
        }

        #endregion
    }
}
