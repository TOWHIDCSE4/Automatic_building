using DaiwaRentalGD.Gui.Visualization3D;

namespace DaiwaRentalGD.Gui.ViewModels.Model
{
    /// <summary>
    /// View model for options for
    /// <see cref="DaiwaRentalGD.Gui.ViewModels.Model
    /// .GDModelSceneViewportViewModel"/>.
    /// </summary>
    public class GDModelSceneViewportOptionsViewModel : ViewModelBase
    {
        #region Constructors

        public GDModelSceneViewportOptionsViewModel(
            GDModelSceneViewportViewModel viewportViewModel
        ) : base()
        {
            GDModelSceneViewportViewModel = viewportViewModel;
        }

        #endregion

        #region Properties

        public GDModelSceneViewportViewModel GDModelSceneViewportViewModel
        { get; }

        private GDModelSceneModel3DBuilder GDModelSceneModel3DBuilder =>
            GDModelSceneViewportViewModel.GDModelSceneModel3DBuilder;

        public bool DoesShowAdjacentSiteSlantPlanes
        {
            get => GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                .DoesShowAdjacentSiteSlantPlanes;

            set
            {
                GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                    .DoesShowAdjacentSiteSlantPlanes = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();
                
                NotifyPropertyChanged();
            }
        }

        public bool DoesShowAdjacentSiteSlantPlanesViolations
        {
            get => GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                .DoesShowAdjacentSiteSlantPlanesViolations;

            set
            {
                GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                    .DoesShowAdjacentSiteSlantPlanesViolations = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        public bool DoesShowRoadSlantPlanes
        {
            get => GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                .DoesShowRoadSlantPlanes;

            set
            {
                GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                    .DoesShowRoadSlantPlanes = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        public bool DoesShowRoadSlantPlanesViolations
        {
            get => GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                .DoesShowRoadSlantPlanesViolations;

            set
            {
                GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                    .DoesShowRoadSlantPlanesViolations = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        public bool DoesShowNorthSlantPlanes
        {
            get => GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                .DoesShowNorthSlantPlanes;

            set
            {
                GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                    .DoesShowNorthSlantPlanes = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        public bool DoesShowNorthSlantPlanesViolations
        {
            get => GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                .DoesShowNorthSlantPlanesViolations;

            set
            {
                GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                    .DoesShowNorthSlantPlanesViolations = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        public bool DoesShowAbsoluteHeightPlane

        {
            get => GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder.DoesShowAbsoluteHeightPlanes;

            set
            {
                GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder.DoesShowAbsoluteHeightPlanes = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        public bool DoesShowAbsoluteHeightPlaneViolations
        {
            get => GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder.DoesShowAbsoluteHeightPlanesViolations;

            set
            {
                GDModelSceneModel3DBuilder.SlantPlanesModel3DBuilder
                    .DoesShowAbsoluteHeightPlanesViolations = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        public bool DoesShowSiteVectorField
        {
            get => GDModelSceneModel3DBuilder.ParkingLotModel3DBuilder
                .DoesShowSiteVectorField;

            set
            {
                GDModelSceneModel3DBuilder.ParkingLotModel3DBuilder
                    .DoesShowSiteVectorField = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        public bool DoesShowWayTiles
        {
            get => GDModelSceneModel3DBuilder.ParkingLotModel3DBuilder
                .DoesShowWayTiles;

            set
            {
                GDModelSceneModel3DBuilder.ParkingLotModel3DBuilder
                    .DoesShowWayTiles = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        public bool DoesShowWalkwayGraph
        {
            get => GDModelSceneModel3DBuilder.ParkingLotModel3DBuilder
                .DoesShowWalkwayGraph;

            set
            {
                GDModelSceneModel3DBuilder.ParkingLotModel3DBuilder
                    .DoesShowWalkwayGraph = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        public bool DoesShowWalkwayPaths
        {
            get => GDModelSceneModel3DBuilder.ParkingLotModel3DBuilder
                .DoesShowWalkwayPaths;

            set
            {
                GDModelSceneModel3DBuilder.ParkingLotModel3DBuilder
                    .DoesShowWalkwayPaths = value;

                GDModelSceneViewportViewModel.UpdateGDModelScene();

                NotifyPropertyChanged();
            }
        }

        #endregion
    }
}
