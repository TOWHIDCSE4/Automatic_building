using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The marker for pruning an existing bikeway tile.
    /// </summary>
    [Serializable]
    public class BikewayPruneMarker : ISGMarker
    {
        #region Constructors

        public BikewayPruneMarker()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected BikewayPruneMarker(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            BikewayTile = reader.GetValue<BikewayTile>(nameof(BikewayTile));
        }

        #endregion

        #region Methods

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new IWorkspaceItem[] { BikewayTile };

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(BikewayTile), BikewayTile);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public BikewayTile BikewayTile { get; set; }

        #endregion
    }
}
