using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Zoning
{
    /// <summary>
    /// The setback of a site.
    /// </summary>
    [Serializable]
    public class Setback : SceneObject
    {
        #region Constructors

        public Setback() : base()
        {
            InitializeComponents();
        }

        protected Setback(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            TransformComponent =
                reader.GetValue<TransformComponent>(
                    nameof(TransformComponent)
                );

            CollisionBody2DComponent =
                reader.GetValue<CollisionBody2DComponent>(
                    nameof(CollisionBody2DComponent)
                );

            RigidBody2DComponent =
                reader.GetValue<RigidBody2DComponent>(
                    nameof(RigidBody2DComponent)
                );

            SetbackComponent =
                reader.GetValue<SetbackComponent>(
                    nameof(SetbackComponent)
                );

            SetbackResolverComponent =
                reader.GetValue<SetbackResolverComponent>(
                    nameof(SetbackResolverComponent)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(CollisionBody2DComponent);
            AddComponent(RigidBody2DComponent);
            AddComponent(SetbackComponent);
            AddComponent(SetbackResolverComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(TransformComponent), TransformComponent);

            writer.AddValue(
                nameof(CollisionBody2DComponent), CollisionBody2DComponent
            );

            writer.AddValue(
                nameof(RigidBody2DComponent), RigidBody2DComponent
            );

            writer.AddValue(
                nameof(SetbackComponent), SetbackComponent
            );

            writer.AddValue(
                nameof(SetbackResolverComponent), SetbackResolverComponent
            );
        }

        #endregion

        #region Properties

        public TransformComponent TransformComponent { get; } =
            new TransformComponent();

        public CollisionBody2DComponent CollisionBody2DComponent { get; } =
            new CollisionBody2DComponent();

        public RigidBody2DComponent RigidBody2DComponent { get; } =
            new RigidBody2DComponent();

        public SetbackComponent SetbackComponent { get; } =
            new SetbackComponent();

        public SetbackResolverComponent SetbackResolverComponent { get; } =
            new SetbackResolverComponent();

        #endregion
    }
}
