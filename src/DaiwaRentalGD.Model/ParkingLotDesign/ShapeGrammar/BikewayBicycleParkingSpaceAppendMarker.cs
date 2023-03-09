using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The marker for appending a bicycle parking space at the end of
    /// a bikeway tile.
    /// </summary>
    [Serializable]
    public class BikewayBicycleParkingSpaceAppendMarker : ISGMarker
    {
        #region Constructors

        public BikewayBicycleParkingSpaceAppendMarker()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected BikewayBicycleParkingSpaceAppendMarker(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            BikewayTile = reader.GetValue<BikewayTile>(nameof(BikewayTile));
            Side = reader.GetValue<WayTileSide>(nameof(Side));
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
            writer.AddValue(nameof(Side), Side);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public BikewayTile BikewayTile { get; set; }

        public WayTileSide Side { get; set; }

        #endregion
    }
}
