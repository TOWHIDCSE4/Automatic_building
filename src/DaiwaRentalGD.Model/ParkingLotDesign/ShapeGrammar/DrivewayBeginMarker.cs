using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// A marker for adding a driveway tile connected to a driveway entrance.
    /// </summary>
    [Serializable]
    public class DrivewayBeginMarker : ISGMarker
    {
        #region Constructors

        public DrivewayBeginMarker()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected DrivewayBeginMarker(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            DrivewayEntrance =
                reader.GetValue<DrivewayEntrance>(nameof(DrivewayEntrance));
        }

        #endregion

        #region Methods

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new IWorkspaceItem[] { DrivewayEntrance };

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(DrivewayEntrance), DrivewayEntrance);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public DrivewayEntrance DrivewayEntrance { get; set; }

        #endregion
    }
}
