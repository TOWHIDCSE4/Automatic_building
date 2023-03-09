using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The marker for growing an existing bikeway in forward direction.
    /// </summary>
    [Serializable]
    public class BikewayForwardMarker : ISGMarker
    {
        #region Constructors

        public BikewayForwardMarker()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected BikewayForwardMarker(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            BikewayTile = reader.GetValue<BikewayTile>(nameof(BikewayTile));
            MaxLength = reader.GetValue<double>(nameof(MaxLength));
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
            writer.AddValue(nameof(MaxLength), MaxLength);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public BikewayTile BikewayTile { get; set; }

        public double MaxLength { get; set; } = double.PositiveInfinity;

        #endregion
    }
}
