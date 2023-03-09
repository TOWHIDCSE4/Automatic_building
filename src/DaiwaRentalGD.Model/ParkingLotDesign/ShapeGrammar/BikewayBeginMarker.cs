using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The marker for adding a bikeway tile.
    /// </summary>
    [Serializable]
    public class BikewayBeginMarker : ISGMarker
    {
        #region Constructors

        public BikewayBeginMarker()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected BikewayBeginMarker(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            WalkwayTile = reader.GetValue<WalkwayTile>(nameof(WalkwayTile));
        }

        #endregion

        #region Methods

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new IWorkspaceItem[] { WalkwayTile };

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(WalkwayTile), WalkwayTile);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public WalkwayTile WalkwayTile { get; set; }

        #endregion
    }
}
