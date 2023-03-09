using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Base class for a component that creates roof for a building.
    /// </summary>
    [Serializable]
    public class RoofCreatorComponent : Component
    {
        #region Constructors

        public RoofCreatorComponent() : base()
        { }

        protected RoofCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _building = reader.GetValue<Building>(nameof(Building));
        }

        #endregion

        #region Methods

        public virtual void CreateRoofs()
        {
            if (Building == null)
            {
                return;
            }

            BuildingComponent bc = Building.BuildingComponent;

            bc.ClearRoofs();
        }

        public virtual TrsTransform3D GetRoofTransform(int floor, int stack)
        {
            if (Building == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Building)} is not set"
                );
            }

            var unit = Building.BuildingComponent.GetUnit(floor, stack);

            if (unit == null)
            {
                return Building.BuildingComponent
                    .GetUnitTransform(floor, stack);
            }

            var roofTf = new TrsTransform3D(
                unit.TransformComponent.Transform
            );

            roofTf.SetTranslateLocal(0.0, 0.0, unit.UnitComponent.RoomHeight);

            return roofTf;
        }

        protected override void Update()
        {
            if (Building == null)
            {
                return;
            }

            CreateRoofs();
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(Building);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Building), _building);
        }

        #endregion

        #region Properties

        public Building Building
        {
            get => _building;
            set
            {
                _building = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private Building _building;

        #endregion
    }
}
