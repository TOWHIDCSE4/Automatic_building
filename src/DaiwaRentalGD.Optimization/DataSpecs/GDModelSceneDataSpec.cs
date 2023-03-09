using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model;
using O3.Foundation;
using Workspaces.Core;
using Workspaces.Json;

namespace DaiwaRentalGD.Optimization
{
    /// <summary>
    /// Data spec for <see cref="GDModelScene"/>.
    /// </summary>
    [Serializable]
    public class GDModelSceneDataSpec : IDataSpec<GDModelScene>
    {
        #region Constructors

        public GDModelSceneDataSpec()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected GDModelSceneDataSpec(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            _name = reader.GetValue<string>(nameof(Name));
        }

        #endregion

        #region Methods

        public GDModelScene Convert(object value) => (GDModelScene)value;

        object IDataSpec.Convert(object value) =>
            ((IDataSpec<GDModelScene>)this).Convert(value);

        public object ConvertToSerializable(object value)
        {
            if (value == null)
            {
                return null;
            }

            var scene = ((IDataSpec<GDModelScene>)this).Convert(value);

            var workspace = new JsonWorkspace();
            workspace.Save(scene);

            var workspaceJson = workspace.ToJson();
            return workspaceJson;
        }

        public object ConvertFromSerializable(object serializable)
        {
            if (serializable == null)
            {
                return null;
            }

            var workspaceJson = (string)serializable;
            var workspace = JsonWorkspace.FromJson(workspaceJson);

            var scene = workspace.Load<GDModelScene>(workspace.ItemUids[0]);
            return scene;
        }

        public GDModelScene CopyValue(GDModelScene value)
        {
            if (value == null)
            {
                return null;
            }

            var workspace = new JsonWorkspace();
            workspace.Save(value);

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var sceneCopy =
                workspaceCopy.Load<GDModelScene>(value.ItemInfo.Uid);
            return sceneCopy;
        }

        public object CopyValue(object value) =>
            CopyValue(((IDataSpec<GDModelScene>)this).Convert(value));

        protected virtual void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>();

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Name), _name);
        }

        #endregion

        #region Events

        public event EventHandler<EventArgs> Changed;

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public string Name
        {
            get => _name;
            set => _name = value ??
                throw new ArgumentNullException(nameof(value));
        }

        public string TypeName => DataSpecTypeName;

        #endregion

        #region Member variables

        private string _name = DataSpecTypeName;

        #endregion

        #region Constants

        public const string DataSpecTypeName = "GD Model Scene Data Spec";

        #endregion
    }
}
