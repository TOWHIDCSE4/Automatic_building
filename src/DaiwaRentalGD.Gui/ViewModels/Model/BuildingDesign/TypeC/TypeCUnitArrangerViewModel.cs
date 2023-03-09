using System;
using System.Collections.Generic;
using System.Linq;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeC
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.TypeC
    /// .TypeCUnitArrangerComponent"/>.
    /// </summary>
    public class TypeCUnitArrangerViewModel : UnitArrangerViewModel
    {
        #region Constructors

        public TypeCUnitArrangerViewModel(GDModelScene gdms) : base(gdms)
        {
            this._gdms = gdms;
        }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            base.NotifyAllGDModelScenePropertiesChanged();
            NotifyPropertyChanged(nameof(EntranceType));
        }

        #endregion

        #region Properties

        private TypeCUnitArrangerComponent TypeCUnitArrangerComponent =>
            UnitArrangerComponent as TypeCUnitArrangerComponent;

        public IReadOnlyList<TypeCUnitEntranceType> AllEntranceTypes =>
            Enum.GetValues(typeof(TypeCUnitEntranceType))
            .OfType<TypeCUnitEntranceType>().ToList();

        public TypeCUnitEntranceType? EntranceType
        {
            get => TypeCUnitArrangerComponent?.EntranceType;
            set
            {
                if (TypeCUnitArrangerComponent == null)
                {
                    return;
                }

                TypeCUnitArrangerComponent.EntranceType =
                    (TypeCUnitEntranceType)value;

                UpdateGDModelScene();
            }
        }
        public IReadOnlyList<TypeCUnitRoofType> AllRoofTypes =>
Enum.GetValues(typeof(TypeCUnitRoofType))
.OfType<TypeCUnitRoofType>().ToList();
        
        public TypeCUnitRoofType? RoofType
        {
            get => TypeCUnitArrangerComponent?.RoofType;
            set
            {
                if (TypeCUnitArrangerComponent == null)
                {
                    return;
                }
                TypeCUnitRoofType? dropdownText = value;
                if (dropdownText != null)
                {
                    switch (dropdownText)
                    {
                        case TypeCUnitRoofType.Gable:
                            _gdms.BuildingDesigner.RoofCreatorComponent = new GableRoofCreatorComponent();
                            break;
                        case TypeCUnitRoofType.Flat:
                            _gdms.BuildingDesigner.RoofCreatorComponent = new FlatRoofCreatorComponent();
                            break;
                    }
                    TypeCUnitArrangerComponent.RoofType = (TypeCUnitRoofType)dropdownText;
                    UpdateGDModelScene();
                }
            }
        }
        private GDModelScene _gdms;
        #endregion
    }
}
