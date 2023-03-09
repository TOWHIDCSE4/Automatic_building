using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Zoning
{
    /// <summary>
    /// A component that resolves the contacts between buildings and
    /// site setbacks.
    /// </summary>
    [Serializable]
    public class SetbackResolverComponent : Component
    {
        #region Constructors

        public SetbackResolverComponent() : base()
        { }

        protected SetbackResolverComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            Building = reader.GetValue<Building>(nameof(Building));

            Contact2DResolver =
                reader.GetValue<Contact2DResolver>(nameof(Contact2DResolver));
        }

        #endregion

        #region Methods

        protected override void Update()
        {
            Contact2DResolver.ClearRigidBodies();

            if (SceneObject?.GetComponent<SetbackComponent>() == null)
            {
                return;
            }

            if (Building == null)
            {
                return;
            }

            Contact2DResolver.AddRigidBody(SceneObject);
            Contact2DResolver.AddRigidBody(Building);

            Building.BuildingComponent.UpdateCollisionBody2D();

            Contact2DResolver.ResolveContacts();

            NotifyPropertyChanged(nameof(IsValid));
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(Building).Append(Contact2DResolver);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Building), Building);
            writer.AddValue(nameof(Contact2DResolver), Contact2DResolver);
        }

        #endregion

        #region Properties

        public Building Building { get; set; }

        public Contact2DResolver Contact2DResolver { get; } =
            new Contact2DResolver();

        public bool IsValid => Contact2DResolver.Contacts.Count == 0;

        #endregion
    }
}
