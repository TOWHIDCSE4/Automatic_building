using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeA
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.TypeA
    /// .TypeAUnitArrangerComponent"/>.
    /// </summary>
    public class TypeAUnitArrangerViewModel : UnitArrangerViewModel
    {
        #region Constructors
       
        public TypeAUnitArrangerViewModel(GDModelScene gdms) : base(gdms)
        {
            this._gdms = gdms;
        }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            base.NotifyAllGDModelScenePropertiesChanged();
            NotifyPropertyChanged(nameof(EntranceType));
            NotifyPropertyChanged(nameof(RoofType));
        }

        #endregion

        #region Properties

        private TypeAUnitArrangerComponent TypeAUnitArrangerComponent =>
            UnitArrangerComponent as TypeAUnitArrangerComponent;

        public IReadOnlyList<TypeAUnitEntranceType> AllEntranceTypes =>
            Enum.GetValues(typeof(TypeAUnitEntranceType))
            .OfType<TypeAUnitEntranceType>().ToList();
        public IReadOnlyList<TypeAUnitRoofType> AllRoofTypes =>
    Enum.GetValues(typeof(TypeAUnitRoofType))
    .OfType<TypeAUnitRoofType>().ToList();
        public TypeAUnitEntranceType? EntranceType
        {
            get => TypeAUnitArrangerComponent?.EntranceType;
            set
            {
                if (TypeAUnitArrangerComponent == null)
                {
                    return;
                }

                TypeAUnitArrangerComponent.EntranceType =
                    (TypeAUnitEntranceType)value;

                UpdateGDModelScene();
            }
        }
        public TypeAUnitRoofType? RoofType
        {
            get => TypeAUnitArrangerComponent?.RoofType;
            set
            {
                if (TypeAUnitArrangerComponent == null)
                {
                    return;
                }
                TypeAUnitRoofType? dropdownText = value;
                if(dropdownText!=null)
                {
                    switch(dropdownText)
                    {
                        case TypeAUnitRoofType.Gable:
                            _gdms.BuildingDesigner.RoofCreatorComponent = new GableRoofCreatorComponent();
                            break;
                        case TypeAUnitRoofType.Flat:
                            _gdms.BuildingDesigner.RoofCreatorComponent = new FlatRoofCreatorComponent();
                            break;
                    }
                    TypeAUnitArrangerComponent.RoofType = (TypeAUnitRoofType)dropdownText;
                    UpdateGDModelScene();
                }    
            }
        }

        private GDModelScene _gdms;
        #endregion
    }
}
