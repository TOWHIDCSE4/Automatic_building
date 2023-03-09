using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A scene object that represents a balcony.
    /// </summary>
    [Serializable]
    public class Balcony : SceneObject
    {
        #region Constructors

        public Balcony() : base()
        {
            InitializeComponents();
        }

        protected Balcony(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _transformComponent = reader.GetValue<TransformComponent>(
                nameof(TransformComponent)
            );

            _balconyComponent =
                reader.GetValue<BalconyComponent>(nameof(BalconyComponent));
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(BalconyComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(TransformComponent), _transformComponent);
            writer.AddValue(nameof(BalconyComponent), _balconyComponent);
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

        public BalconyComponent BalconyComponent
        {
            get => _balconyComponent;
            set
            {
                ReplaceComponent(_balconyComponent, value);
                _balconyComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private BalconyComponent _balconyComponent =
            new BalconyComponent();

        #endregion
    }
}
