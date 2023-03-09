using System;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeA
{
    /// <summary>
    /// A scene object that designs a building with Type A units.
    /// </summary>
    [Serializable]
    public class TypeABuildingDesigner : BuildingDesigner
    {
        #region Constructors
        public TypeABuildingDesigner() : base()
        {
            UnitArrangerComponent = new TypeAUnitArrangerComponent();

            RoofCreatorComponent = GetDefaultRoofCreatorComponent((TypeAUnitArrangerComponent)UnitArrangerComponent);

            BuildingEntranceCreatorComponent =
                new TypeABuildingEntranceCreatorComponent();

            BuildingPlacementComponent = new BuildingPlacementComponent();
        }

        protected TypeABuildingDesigner(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public static RoofCreatorComponent GetDefaultRoofCreatorComponent(TypeAUnitArrangerComponent typeAUnitArrangerComponent)
        {
            RoofCreatorComponent Roof = new FlatRoofCreatorComponent();
            if (typeAUnitArrangerComponent != null)
            {

                switch (typeAUnitArrangerComponent.RoofType)
                {
                    case TypeAUnitRoofType.Gable:
                        Roof = new GableRoofCreatorComponent();
                        break;
                    case TypeAUnitRoofType.Flat:
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
                value as TypeAUnitArrangerComponent ??
                throw new ArgumentException(
                    $"{nameof(value)} must be of type " +
                    $"{nameof(TypeAUnitArrangerComponent)}"
                );
        }

        public override BuildingEntranceCreatorComponent
            BuildingEntranceCreatorComponent
        {
            get => base.BuildingEntranceCreatorComponent;
            set =>
                base.BuildingEntranceCreatorComponent =
                value as TypeABuildingEntranceCreatorComponent ??
                throw new ArgumentException(
                    $"{nameof(value)} must be of type " +
                    $"{nameof(TypeABuildingEntranceCreatorComponent)}"
                );
        }

        #endregion
    }
}
