using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A scene object that represents a staircase.
    /// </summary>
    [Serializable]
    public class Staircase : SceneObject
    {
        #region Constructors

        public Staircase() : base()
        {
            InitializeComponents();
        }

        protected Staircase(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _transformComponent = reader.GetValue<TransformComponent>(
                nameof(TransformComponent)
            );

            _staircaseComponent = reader.GetValue<StaircaseComponent>(
                nameof(StaircaseComponent)
            );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(StaircaseComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(TransformComponent), _transformComponent);
            writer.AddValue(nameof(StaircaseComponent), _staircaseComponent);
        }

        #endregion

        #region Properties

        public TransformComponent TransformComponent
        {
            get => _transformComponent;
            set
            {
                ReplaceComponent(_transformComponent, value);
                _transformComponent = value;

                NotifyPropertyChanged();
            }
        }

        public StaircaseComponent StaircaseComponent
        {
            get => _staircaseComponent;
            set
            {
                ReplaceComponent(_staircaseComponent, value);
                _staircaseComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private StaircaseComponent _staircaseComponent =
            new StaircaseComponent();

        #endregion
    }
}
