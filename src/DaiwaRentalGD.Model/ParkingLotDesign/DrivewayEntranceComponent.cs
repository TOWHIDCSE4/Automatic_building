using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component representing the entrance of a driveway
    /// on the site boundary.
    /// </summary>
    [Serializable]
    public class DrivewayEntranceComponent : SiteEntranceComponent
    {
        #region Constructors

        public DrivewayEntranceComponent() : base()
        {
            Width = DrivewayTileComponent.DefaultWidth;
        }

        protected DrivewayEntranceComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            DrivewayTile =
                reader.GetValue<DrivewayTile>(nameof(DrivewayTile));
        }

        #endregion

        #region Methods

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(DrivewayTile);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(DrivewayTile), DrivewayTile);
        }

        #endregion

        #region Properties

        public DrivewayTile DrivewayTile { get; set; }

        #endregion
    }
}
