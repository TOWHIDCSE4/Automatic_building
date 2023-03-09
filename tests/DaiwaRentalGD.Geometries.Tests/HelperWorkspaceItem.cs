using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Geometries.Tests
{
    [Serializable]
    public class HelperWorkspaceItem<T> : IWorkspaceItem
    {
        #region Constructors

        public HelperWorkspaceItem()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        public HelperWorkspaceItem(T data) : this()
        {
            Data = data;
        }

        protected HelperWorkspaceItem(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            Data = reader.GetValue<T>(nameof(Data));
        }

        #endregion

        #region Methods

        public virtual IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new[] { Data as IWorkspaceItem };

        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Data), Data);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public virtual T Data { get; set; }

        #endregion
    }
}
