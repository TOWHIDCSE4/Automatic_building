using System;
using System.Collections.Generic;
using System.Linq;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign
    /// .BuildingPlacementComponent"/>.
    /// </summary>
    public class BuildingPlacementViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public BuildingPlacementViewModel(GDModelScene gdms) : base(gdms)
        { }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(BuildingMinX));
            NotifyPropertyChanged(nameof(BuildingMaxX));
            NotifyPropertyChanged(nameof(BuildingX));

            NotifyPropertyChanged(nameof(BuildingMinY));
            NotifyPropertyChanged(nameof(BuildingMaxY));
            NotifyPropertyChanged(nameof(BuildingY));

            NotifyPropertyChanged(nameof(BuildingOrientationMode));
            NotifyPropertyChanged(nameof(BuildingMinTNAngleDegrees));
            NotifyPropertyChanged(nameof(BuildingMaxTNAngleDegrees));
            NotifyPropertyChanged(nameof(BuildingTNAngleDegrees));
            NotifyPropertyChanged(nameof(BuildingActualTNAngleDegrees));
        }

        #endregion

        #region Properties

        #region Location

        public double? BuildingMinX =>
            GDModelScene?.BuildingDesigner
            .BuildingPlacementComponent.BuildingMinX;

        public double? BuildingMaxX =>
            GDModelScene?.BuildingDesigner
            .BuildingPlacementComponent.BuildingMaxX;

        public double? BuildingX
        {
            get => GDModelScene?.BuildingDesigner
                .BuildingPlacementComponent.BuildingX;
            set
            {
                if (GDModelScene == null)
                {
                    return;
                }

                GDModelScene.BuildingDesigner.BuildingPlacementComponent
                    .BuildingX = (double)value;

                UpdateGDModelScene();
            }
        }

        public double? BuildingMinY =>
            GDModelScene?.BuildingDesigner
            .BuildingPlacementComponent.BuildingMinY;

        public double? BuildingMaxY =>
            GDModelScene?.BuildingDesigner
            .BuildingPlacementComponent.BuildingMaxY;

        public double? BuildingY
        {
            get => GDModelScene?.BuildingDesigner
                .BuildingPlacementComponent.BuildingY;
            set
            {
                if (GDModelScene == null)
                {
                    return;
                }

                GDModelScene.BuildingDesigner.BuildingPlacementComponent
                    .BuildingY = (double)value;

                UpdateGDModelScene();
            }
        }

        #endregion

        #region Orientation

        public IReadOnlyList<BuildingOrientationMode>
            AllBuildingOrientationModes =>
            Enum.GetValues(typeof(BuildingOrientationMode))
            .OfType<BuildingOrientationMode>().ToList();

        public BuildingOrientationMode? BuildingOrientationMode
        {
            get => GDModelScene?.BuildingDesigner
                .BuildingPlacementComponent.BuildingOrientationMode;
            set
            {
                if (GDModelScene == null)
                {
                    return;
                }

                GDModelScene.BuildingDesigner.BuildingPlacementComponent
                    .BuildingOrientationMode = (BuildingOrientationMode)value;

                UpdateGDModelScene();
            }
        }

        public double? BuildingMinTNAngleDegrees
        {
            get => GDModelScene?.BuildingDesigner
                .BuildingPlacementComponent.BuildingMinTNAngleDegrees;
            set
            {
                if (GDModelScene == null)
                {
                    return;
                }

                GDModelScene.BuildingDesigner.BuildingPlacementComponent
                    .BuildingMinTNAngleDegrees = (double)value;

                UpdateGDModelScene();
            }
        }

        public double? BuildingMaxTNAngleDegrees
        {
            get => GDModelScene?.BuildingDesigner
                .BuildingPlacementComponent.BuildingMaxTNAngleDegrees;
            set
            {
                if (GDModelScene == null)
                {
                    return;
                }

                GDModelScene.BuildingDesigner.BuildingPlacementComponent
                    .BuildingMaxTNAngleDegrees = (double)value;

                UpdateGDModelScene();
            }
        }

        public double? BuildingTNAngleDegrees
        {
            get => GDModelScene?.BuildingDesigner
                .BuildingPlacementComponent.BuildingTNAngleDegrees;
            set
            {
                if (GDModelScene == null)
                {
                    return;
                }

                GDModelScene.BuildingDesigner.BuildingPlacementComponent
                    .BuildingTNAngleDegrees = (double)value;

                UpdateGDModelScene();
            }
        }

        public double? BuildingActualTNAngleDegrees =>
            GDModelScene?.BuildingDesigner
            .BuildingPlacementComponent.BuildingActualTNAngleDegrees;

        #endregion

        #endregion
    }
}
