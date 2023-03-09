using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A scene object that represents a roof.
    /// </summary>
    [Serializable]
    public class Roof : SceneObject
    {
        #region Constructors

        public Roof() : base()
        {
            InitializeComponents();
        }

        protected Roof(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _transformComponent = reader.GetValue<TransformComponent>(
                nameof(TransformComponent)
            );

            _roofComponent =
                reader.GetValue<RoofComponent>(nameof(RoofComponent));
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(RoofComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(TransformComponent), _transformComponent);
            writer.AddValue(nameof(RoofComponent), _roofComponent);
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

        public RoofComponent RoofComponent
        {
            get => _roofComponent;
            set
            {
                ReplaceComponent(_roofComponent, value);
                _roofComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private RoofComponent _roofComponent =
            new RoofComponent();

        #endregion
    }
}
