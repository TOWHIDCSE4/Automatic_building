using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that creates entrances for a building.
    /// </summary>
    [Serializable]
    public class BuildingEntranceCreatorComponent : Component
    {
        #region Constructors

        public BuildingEntranceCreatorComponent() : base()
        { }

        protected BuildingEntranceCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _building = reader.GetValue<Building>(nameof(Building));
        }

        #endregion

        #region Methods

        public virtual void CreateEntrances(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            building.BuildingComponent.ClearEntrances();
        }

        protected override void Update()
        {
            if (Building == null) { return; }

            CreateEntrances(Building);
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
