using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A scene object that represents a corridor.
    /// </summary>
    [Serializable]
    public class Corridor : SceneObject
    {
        #region Constructors

        public Corridor() : base()
        {
            InitializeComponents();
        }

        protected Corridor(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _transformComponent = reader.GetValue<TransformComponent>(
                nameof(TransformComponent)
            );

            _corridorComponent = reader.GetValue<CorridorComponent>(
                nameof(CorridorComponent)
            );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(CorridorComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(TransformComponent), _transformComponent);
            writer.AddValue(nameof(CorridorComponent), _corridorComponent);
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

        public CorridorComponent CorridorComponent
        {
            get => _corridorComponent;
            set
            {
                ReplaceComponent(_corridorComponent, value);
                _corridorComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private CorridorComponent _corridorComponent =
            new CorridorComponent();

        #endregion
    }
}
