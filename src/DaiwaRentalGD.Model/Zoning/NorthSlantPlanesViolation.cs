using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Zoning
{
    /// <summary>
    /// Describes a violation of north slant planes.
    /// </summary>
    [Serializable]
    public class NorthSlantPlanesViolation : IWorkspaceItem
    {
        #region Constructors

        public NorthSlantPlanesViolation()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected NorthSlantPlanesViolation(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            Site = reader.GetValue<Site>(nameof(Site));

            Point = reader.GetValue<Point>(nameof(Point));

            NorthPropertyEdgeIndices.AddRange(
                reader.GetValues<int>(nameof(NorthPropertyEdgeIndices))
            );
        }

        #endregion

        #region Methods

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new[] { Site };

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Site), Site);

            writer.AddValue(nameof(Point), Point);

            writer.AddValues(
                nameof(NorthPropertyEdgeIndices), NorthPropertyEdgeIndices
            );
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public Site Site { get; set; }

        public Point Point { get; set; }

        public List<int> NorthPropertyEdgeIndices { get; } = new List<int>();

        #endregion
    }
}
