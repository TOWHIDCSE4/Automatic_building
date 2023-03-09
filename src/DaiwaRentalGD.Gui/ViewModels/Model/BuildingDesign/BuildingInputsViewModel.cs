using System.Collections.Generic;
using System.Linq;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeA;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeB;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeC;
using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign
{
    /// <summary>
    /// View model for inputs related to building.
    /// </summary>
    public class BuildingInputsViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public BuildingInputsViewModel(GDModelScene gdms) : base(gdms)
        {
            AllBuildingDesignerViewModels =
                new BuildingDesignerViewModelBase[]
                {
                    new TypeABuildingDesignerViewModel(gdms)
                    {
                        IsActivated = false
                    },
                    new TypeBBuildingDesignerViewModel(gdms)
                    {
                        IsActivated = false
                    },
                    new TypeCBuildingDesignerViewModel(gdms)
                    {
                        IsActivated = false
                    }
                };

            BuildingDesignerViewModel =
                AllBuildingDesignerViewModels
                .FirstOrDefault(viewModel => viewModel.IsSupported);
        }

        #endregion

        #region

        public override bool IsActivated
        {
            get => base.IsActivated;
            set
            {
                base.IsActivated = value;

                if (BuildingDesignerViewModel != null)
                {
                    BuildingDesignerViewModel.IsActivated = value;
                }
            }
        }

        public IReadOnlyList<BuildingDesignerViewModelBase>
            AllBuildingDesignerViewModels
        { get; }


        public BuildingDesignerViewModelBase BuildingDesignerViewModel
        {
            get => _buildingDesignerViewModel;
            set
            {
                if (_buildingDesignerViewModel != null)
                {
                    _buildingDesignerViewModel.IsActivated = false;
                }

                _buildingDesignerViewModel = value;

                if (_buildingDesignerViewModel != null)
                {
                    if (!_buildingDesignerViewModel.IsSupported)
                    {
                        _buildingDesignerViewModel.UpdateBuildingDesigner();
                    }

                    _buildingDesignerViewModel.IsActivated = IsActivated;
                }

                NotifyPropertyChanged(nameof(BuildingDesignerViewModel));

                UpdateGDModelScene();
            }
        }
        #endregion

        #region Member variables

        private BuildingDesignerViewModelBase _buildingDesignerViewModel;
        #endregion
    }
}
