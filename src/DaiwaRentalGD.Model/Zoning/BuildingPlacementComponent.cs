using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Zoning
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BuildingPlacementComponent : Component
    {
        #region Constructors

        public BuildingPlacementComponent() : base()
        { }

        protected BuildingPlacementComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _site = reader.GetValue<Site>(nameof(Site));

            _building = reader.GetValue<Building>(nameof(Building));

            _contact2DResolver = reader.GetValue<Contact2DResolver>(nameof(Contact2DResolver));
        }

        #endregion

        #region Methods  
        public Polygon GetSiteBoundary()
        {
            var siteBoundary = Site.SiteComponent.Boundary;
            return siteBoundary;
        }

        private void DetectCollision()
        {

            if (Building == null)
                return;

            if (Site == null)
                return;

            if (CollisionBody2DComponent == null)
                return;

            if (RigidBody2DComponent == null)
                return;

            if (TransformComponent == null)
                return;

            TransformComponent.Transform = new TrsTransform3D(Site.TransformComponent.Transform);

            // Clear Rigid body and Collision body
            Contact2DResolver.ClearRigidBodies();

            // Update Site and Building
            CollisionBody2DComponent.ClearCollisionPolygons();
            CollisionBody2DComponent.AddCollisionPolygon(GetSiteBoundary());
            RigidBody2DComponent.IsSpace = true;
            RigidBody2DComponent.IsStatic = true;


            //Building.BuildingComponent.UpdateCollisionBody2D();
            Contact2DResolver.AddRigidBody(SceneObject);
            Contact2DResolver.AddRigidBody(Building);
        }

        protected override void Update()
        {
            DetectCollision();
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() => base.GetReferencedItems()
            .Append(Site)
            .Append(Building)
            .Append(Contact2DResolver);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Site), _site);

            writer.AddValue(nameof(Building), _building);

            writer.AddValue(nameof(Contact2DResolver), _contact2DResolver);
        }

        #endregion

        #region Properties
        public bool IsValidBuildingPlacement => !Contact2DResolver.HasContact;
        public Site Site
        {
            get => _site;
            set
            {
                _site = value;
                NotifyPropertyChanged();
            }
        }

        public Building Building
        {
            get => _building;
            set
            {
                _building = value;
                NotifyPropertyChanged();
            }
        }
        public Contact2DResolver Contact2DResolver
        {
            get => _contact2DResolver;
        }
        #endregion

        #region Member variables

        private Site _site;
        private Building _building;
        private Contact2DResolver _contact2DResolver { get; } = new Contact2DResolver();
        public CollisionBody2DComponent CollisionBody2DComponent => SceneObject?.GetComponent<CollisionBody2DComponent>();
        public RigidBody2DComponent RigidBody2DComponent => SceneObject?.GetComponent<RigidBody2DComponent>();
        public TransformComponent TransformComponent => SceneObject?.GetComponent<TransformComponent>();
        #endregion
    }
}
