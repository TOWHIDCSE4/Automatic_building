using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// A marker for growing an existing driveway in forward direction.
    /// </summary>
    [Serializable]
    public class DrivewayForwardMarker : ISGMarker
    {
        #region Constructors

        public DrivewayForwardMarker()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected DrivewayForwardMarker(
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
