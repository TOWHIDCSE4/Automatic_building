using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// An edge in the graph for walkway generation.
    /// </summary>
    [Serializable]
    public class WalkwayGraphEdge : IWorkspaceItem
    {
        #region Constructors

        internal WalkwayGraphEdge()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected WalkwayGraphEdge(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            Vertex0 = reader.GetValue<WalkwayGraphVertex>(nameof(Vertex0));
            Vertex1 = reader.GetValue<WalkwayGraphVertex>(nameof(Vertex1));

            Weight = reader.GetValue<double>(nameof(Weight));
        }

        #endregion

        #region Methods

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new[] { Vertex0, Vertex1 };

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Vertex0), Vertex0);
            writer.AddValue(nameof(Vertex1), Vertex1);
            writer.AddValue(nameof(Weight), Weight);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public WalkwayGraphVertex Vertex0 { get; internal set; }

        public WalkwayGraphVertex Vertex1 { get; internal set; }

        public double Length => Vertex0.Point.GetDistance(Vertex1.Point);

        public double Weight { get; set; }

        #endregion
    }
}
