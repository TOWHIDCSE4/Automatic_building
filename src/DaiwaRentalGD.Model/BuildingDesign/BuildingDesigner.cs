using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A scene object that contains the components for designing
    /// various aspects of a building.
    /// </summary>
    [Serializable]
    public class BuildingDesigner : SceneObject
    {
        #region Constructors

        public BuildingDesigner() : base()
        {
            InitializeComponents();
        }

        protected BuildingDesigner(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _unitArrangerComponent = reader.GetValue<UnitArrangerComponent>(
                nameof(UnitArrangerComponent)
            );

            _roofCreatorComponent = reader.GetValue<RoofCreatorComponent>(
                nameof(RoofCreatorComponent)
            );

            _buildingEntranceCreatorComponent =
                reader.GetValue<BuildingEntranceCreatorComponent>(
                    nameof(BuildingEntranceCreatorComponent)
                );

            _buildingPlacementComponent =
                reader.GetValue<BuildingPlacementComponent>(
                    nameof(BuildingPlacementComponent)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(UnitArrangerComponent);
            AddComponent(RoofCreatorComponent);
            AddComponent(BuildingEntranceCreatorComponent);
            AddComponent(BuildingPlacementComponent);
        }

        protected override void OnAdded(SceneObjectAddedEventArgs e)
        {
            base.OnAdded(e);

            // TODO This is a temporary way of reconnection

            if (e.ReplacedSceneObject is BuildingDesigner buildingDesigner)
            {
                var bpc = buildingDesigner.BuildingPlacementComponent;

                BuildingPlacementComponent.BuildingX = bpc.BuildingX;
                BuildingPlacementComponent.BuildingY = bpc.BuildingY;

                BuildingPlacementComponent.BuildingTNAngle =
                    bpc.BuildingTNAngle;
                BuildingPlacementComponent.BuildingOrientationMode =
                    bpc.BuildingOrientationMode;
            }
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(UnitArrangerComponent),
                _unitArrangerComponent
            );
            writer.AddValue(
                nameof(RoofCreatorComponent),
                _roofCreatorComponent
            );
            writer.AddValue(
                nameof(BuildingEntranceCreatorComponent),
                _buildingEntranceCreatorComponent
            );
            writer.AddValue(
                nameof(BuildingPlacementComponent),
                _buildingPlacementComponent
            );
        }

        #endregion

        #region Properties

        public virtual UnitArrangerComponent UnitArrangerComponent
        {
            get => _unitArrangerComponent;
            set
            {
                var oldUnitArrangerComponent = _unitArrangerComponent;

                ReplaceComponent(_unitArrangerComponent, value);
                _unitArrangerComponent = value;

                _unitArrangerComponent.Building =
                    oldUnitArrangerComponent.Building;

                _unitArrangerComponent.UnitCatalog =
                    oldUnitArrangerComponent.UnitCatalog;

                NotifyPropertyChanged();
            }
        }

        public virtual RoofCreatorComponent RoofCreatorComponent
        {
            get => _roofCreatorComponent;
            set
            {
                var oldRoofCreatorComponent = _roofCreatorComponent;

                ReplaceComponent(_roofCreatorComponent, value);
                _roofCreatorComponent = value;

                _roofCreatorComponent.Building =
                    oldRoofCreatorComponent.Building;

                NotifyPropertyChanged();
            }
        }

        public virtual BuildingEntranceCreatorComponent
            BuildingEntranceCreatorComponent
        {
            get => _buildingEntranceCreatorComponent;
            set
            {
                var oldBuildingEntranceCreatorComponent =
                    _buildingEntranceCreatorComponent;

                ReplaceComponent(_buildingEntranceCreatorComponent, value);
                _buildingEntranceCreatorComponent = value;

                _buildingEntranceCreatorComponent.Building =
                    oldBuildingEntranceCreatorComponent.Building;

                NotifyPropertyChanged();
            }
        }

        public virtual BuildingPlacementComponent BuildingPlacementComponent
        {
            get => _buildingPlacementComponent;
            set
            {
                var oldBuildingPlacementComponent =
                    _buildingPlacementComponent;

                ReplaceComponent(_buildingPlacementComponent, value);
                _buildingPlacementComponent = value;

                _buildingPlacementComponent.Building =
                    oldBuildingPlacementComponent.Building;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private UnitArrangerComponent _unitArrangerComponent =
            new UnitArrangerComponent();

        private RoofCreatorComponent _roofCreatorComponent =
            new RoofCreatorComponent();

        private BuildingEntranceCreatorComponent
            _buildingEntranceCreatorComponent =
            new BuildingEntranceCreatorComponent();

        private BuildingPlacementComponent _buildingPlacementComponent =
            new BuildingPlacementComponent();

        #endregion
    }
}
