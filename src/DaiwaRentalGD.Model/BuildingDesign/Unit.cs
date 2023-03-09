using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A scene object that represents a unit.
    /// </summary>
    [Serializable]
    public class Unit : SceneObject
    {
        #region Constructors

        public Unit() : base()
        {
            InitializeComponents();
        }

        protected Unit(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            TransformComponent = reader.GetValue<TransformComponent>(
                nameof(TransformComponent)
            );

            _unitComponent =
                reader.GetValue<UnitComponent>(nameof(UnitComponent));
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(UnitComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(TransformComponent), TransformComponent);
            writer.AddValue(nameof(UnitComponent), _unitComponent);
        }

        #endregion

        #region Properties

        public TransformComponent TransformComponent { get; } =
            new TransformComponent();

        public UnitComponent UnitComponent
        {
            get => _unitComponent;
            set
            {
                ReplaceComponent(_unitComponent, value);
                _unitComponent = value;
            }
        }

        #endregion

        #region Member variables

        private UnitComponent _unitComponent = new UnitComponent();

        #endregion
    }
}
