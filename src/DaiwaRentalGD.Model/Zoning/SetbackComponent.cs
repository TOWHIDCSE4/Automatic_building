using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Zoning
{
    /// <summary>
    /// Represents the setback of a site boundary.
    /// </summary>
    [Serializable]
    public class SetbackComponent : Component
    {
        #region Constructors

        public SetbackComponent() : base()
        { }

        protected SetbackComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            Site = reader.GetValue<Site>(nameof(Site));

            OffsetBoundary = reader.GetValue<Polygon>(nameof(OffsetBoundary));

            _setbackDistance =
                reader.GetValue<double>(nameof(SetbackDistance));
        }

        #endregion

        #region Methods

        public Polygon GetSetbackBoundary()
        {
            if (Site == null)
            {
                return null;
            }

            var sc = Site.SiteComponent;

            var siteBoundary = sc.Boundary;

            var setbackBoundary = new Polygon(siteBoundary);

            foreach (int propertyEdgeIndex in sc.PropertyEdgeIndices)
            {
                setbackBoundary.OffsetEdge(
                    propertyEdgeIndex, -SetbackDistance
                );
            }

            return setbackBoundary;
        }

        public void Reset()
        {
            if (Site == null)
            {
                return;
            }

            if (TransformComponent == null)
            {
                return;
            }

            if (CollisionBody2DComponent == null)
            {
                return;
            }

            if (RigidBody2DComponent == null)
            {
                return;
            }

            TransformComponent.Transform =
                new TrsTransform3D(Site.TransformComponent.Transform);

            var setbackBoundary = GetSetbackBoundary();
            CollisionBody2DComponent.ClearCollisionPolygons();
            CollisionBody2DComponent.AddCollisionPolygon(setbackBoundary);

            RigidBody2DComponent.IsSpace = true;
            RigidBody2DComponent.IsStatic = true;
        }

        protected override void Update()
        {
            Reset();
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(Site);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Site), Site);
            writer.AddValue(nameof(OffsetBoundary), OffsetBoundary);
            writer.AddValue(nameof(SetbackDistance), _setbackDistance);
        }

        #endregion

        #region Properties

        public Site Site { get; set; }

        public TransformComponent TransformComponent =>
            SceneObject?.GetComponent<TransformComponent>();

        public CollisionBody2DComponent CollisionBody2DComponent =>
            SceneObject?.GetComponent<CollisionBody2DComponent>();

        public RigidBody2DComponent RigidBody2DComponent =>
            SceneObject?.GetComponent<RigidBody2DComponent>();

        public Polygon OffsetBoundary { get; private set; } = new Polygon();

        public double SetbackDistance
        {
            get => _setbackDistance;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SetbackDistance)} cannot be negative"
                    );
                }

                _setbackDistance = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private double _setbackDistance = DefaultSetbackDistance;

        #endregion

        #region Constants

        public const double DefaultSetbackDistance = 0.5;

        #endregion
    }
}
