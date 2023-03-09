using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Zoning
{
    /// <summary>
    /// Describes a violation of Absolute Height Slant Planes Violation
    /// </summary>
    [Serializable]
    public class AbsoluteHeightSlantPlanesViolation : IWorkspaceItem
    {
        #region Constructors

        public AbsoluteHeightSlantPlanesViolation()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected AbsoluteHeightSlantPlanesViolation(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            Site = reader.GetValue<Site>(nameof(Site));

            Point = reader.GetValue<Point>(nameof(Point));
            OppositeRoadEdgeIndices.AddRange(
                reader.GetValues<int>(nameof(OppositeRoadEdgeIndices))
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
                nameof(OppositeRoadEdgeIndices), OppositeRoadEdgeIndices
            );
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public Site Site { get; set; }

        public Point Point { get; set; }

        public List<int> OppositeRoadEdgeIndices { get; } = new List<int>();
        public List<int> PropertyEdgeIndices { get; } = new List<int>();

        #endregion
    }
}
