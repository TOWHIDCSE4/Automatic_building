using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Zoning
{
    /// <summary>
    /// Evaluates land use related metrics of a design.
    /// </summary>
    [Serializable]
    public class LandUseEvaluator : SceneObject
    {
        #region Constructors

        public LandUseEvaluator() : base()
        {
            InitializeComponents();
        }

        protected LandUseEvaluator(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _buildingCoverageRatioComponent =
                reader.GetValue<BuildingCoverageRatioComponent>(
                    nameof(BuildingCoverageRatioComponent)
                );

            _floorAreaRatioComponent =
                reader.GetValue<FloorAreaRatioComponent>(
                    nameof(FloorAreaRatioComponent)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(BuildingCoverageRatioComponent);
            AddComponent(FloorAreaRatioComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(BuildingCoverageRatioComponent),
                _buildingCoverageRatioComponent
            );

            writer.AddValue(
                nameof(FloorAreaRatioComponent),
                _floorAreaRatioComponent
            );
        }

        #endregion

        #region Properties

        public BuildingCoverageRatioComponent BuildingCoverageRatioComponent
        {
            get => _buildingCoverageRatioComponent;
            set
            {
                ReplaceComponent(_buildingCoverageRatioComponent, value);
                _buildingCoverageRatioComponent = value;

                NotifyPropertyChanged();
            }
        }

        public FloorAreaRatioComponent FloorAreaRatioComponent
        {
            get => _floorAreaRatioComponent;
            set
            {
                ReplaceComponent(_floorAreaRatioComponent, value);
                _floorAreaRatioComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private BuildingCoverageRatioComponent
            _buildingCoverageRatioComponent =
            new BuildingCoverageRatioComponent();

        private FloorAreaRatioComponent _floorAreaRatioComponent =
            new FloorAreaRatioComponent();

        #endregion
    }
}
