using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// A marker for appending a car parking space at the end of
    /// a driveway tile.
    /// </summary>
    [Serializable]
    public class DrivewayCarParkingSpaceAppendMarker : ISGMarker
    {
        #region Constructors

        public DrivewayCarParkingSpaceAppendMarker()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected DrivewayCarParkingSpaceAppendMarker(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            DrivewayTile =
                reader.GetValue<DrivewayTile>(nameof(DrivewayTile));

            Side = reader.GetValue<WayTileSide>(nameof(Side));
        }

        #endregion

        #region Methods

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new IWorkspaceItem[] { DrivewayTile };

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(DrivewayTile), DrivewayTile);
            writer.AddValue(nameof(Side), Side);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public DrivewayTile DrivewayTile { get; set; }

        public WayTileSide Side { get; set; }

        #endregion
    }
}
