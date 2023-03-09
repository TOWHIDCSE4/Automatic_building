using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeB
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.TypeB
    /// .TypeBUnitArrangerComponent"/>.
    /// </summary>
    public class TypeBUnitArrangerViewModel : UnitArrangerViewModel
    {
        #region Constructors

        public TypeBUnitArrangerViewModel(GDModelScene gdms) : base(gdms)
        {
            this._gdms = gdms;
        }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            base.NotifyAllGDModelScenePropertiesChanged();
            NotifyPropertyChanged(nameof(RoofType));
        }

        #endregion

        #region Properties

        private TypeBUnitArrangerComponent TypeBUnitArrangerComponent =>
            UnitArrangerComponent as TypeBUnitArrangerComponent;

       
        public IReadOnlyList<TypeBUnitRoofType> AllRoofTypes =>
    Enum.GetValues(typeof(TypeBUnitRoofType))
    .OfType<TypeBUnitRoofType>().ToList();

        public TypeBUnitRoofType? RoofType
        {
            get => TypeBUnitArrangerComponent?.RoofType;
            set
            {
                if (TypeBUnitArrangerComponent == null)
                {
                    return;
                }
                TypeBUnitRoofType? dropdownText = value;
                if (dropdownText != null)
                {
                    switch (dropdownText)
                    {
                        case TypeBUnitRoofType.Gable:
                            _gdms.BuildingDesigner.RoofCreatorComponent = new GableRoofCreatorComponent();
                            break;
                        case TypeBUnitRoofType.Flat:
                            _gdms.BuildingDesigner.RoofCreatorComponent = new FlatRoofCreatorComponent();
                            break;
                    }
                    TypeBUnitArrangerComponent.RoofType = (TypeBUnitRoofType)dropdownText;
                    UpdateGDModelScene();
                }
            }
        }

        private GDModelScene _gdms;
        #endregion
    }
}
