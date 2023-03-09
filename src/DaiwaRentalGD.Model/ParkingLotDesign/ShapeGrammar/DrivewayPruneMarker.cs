using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// A marker for pruning an existing driveway tile.
    /// </summary>
    [Serializable]
    public class DrivewayPruneMarker : ISGMarker
    {
        #region Constructors

        public DrivewayPruneMarker()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected DrivewayPruneMarker(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            DrivewayTile =
                reader.GetValue<DrivewayTile>(nameof(DrivewayTile));
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
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public DrivewayTile DrivewayTile { get; set; }

        #endregion
    }
}
