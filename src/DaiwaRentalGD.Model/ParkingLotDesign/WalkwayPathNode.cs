using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A node on a walkway path, which contains a reference to
    /// the underlying vertex and values for heuristic pathfinding algorithms
    /// such as A* Algorithm.
    /// </summary>
    [Serializable]
    public class WalkwayPathNode : IWorkspaceItem
    {
        #region Constructors

        public WalkwayPathNode()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected WalkwayPathNode(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            Vertex = reader.GetValue<WalkwayGraphVertex>(nameof(Vertex));
            GScore = reader.GetValue<double>(nameof(GScore));
            HScore = reader.GetValue<double>(nameof(HScore));
            Parent = reader.GetValue<WalkwayPathNode>(nameof(Parent));
        }

        #endregion

        #region Methods

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new IWorkspaceItem[] { Vertex, Parent };

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Vertex), Vertex);
            writer.AddValue(nameof(GScore), GScore);
            writer.AddValue(nameof(HScore), HScore);
            writer.AddValue(nameof(Parent), Parent);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public WalkwayGraphVertex Vertex { get; set; }

        public double GScore { get; set; }

        public double HScore { get; set; }

        public double FScore => GScore + HScore;

        public WalkwayPathNode Parent { get; set; }

        #endregion
    }
}
