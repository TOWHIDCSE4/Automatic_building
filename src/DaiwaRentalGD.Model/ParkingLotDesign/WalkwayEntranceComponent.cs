using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component representing the entrance of a walkway
    /// on the site boundary.
    /// </summary>
    [Serializable]
    public class WalkwayEntranceComponent : SiteEntranceComponent
    {
        #region Constructors

        public WalkwayEntranceComponent() : base()
        {
            Width = WalkwayTileComponent.DefaultWidth;
        }

        protected WalkwayEntranceComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            WalkwayTile = reader.GetValue<WalkwayTile>(nameof(WalkwayTile));
        }

        #endregion

        #region Methods

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(WalkwayTile);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(WalkwayTile), WalkwayTile);
        }

        #endregion

        #region Properties

        public WalkwayTile WalkwayTile { get; set; }

        #endregion
    }
}
