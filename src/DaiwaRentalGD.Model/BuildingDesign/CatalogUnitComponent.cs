using System;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that describes a unit from a unit catalog.
    /// </summary>
    [Serializable]
    public class CatalogUnitComponent : UnitComponent
    {
        #region Constructors

        public CatalogUnitComponent() : base()
        { }

        public CatalogUnitComponent(CatalogUnitComponent uc) : base(uc)
        {
            var entryNameCopy = new UnitCatalogEntryName(uc.EntryName);
            EntryName = entryNameCopy;
        }

        protected CatalogUnitComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _entryName =
                reader.GetValue<UnitCatalogEntryName>(nameof(EntryName));
        }

        #endregion

        #region Methods

        public virtual CatalogUnitComponent Copy()
        {
            return new CatalogUnitComponent(this);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(EntryName), _entryName);
        }

        #endregion

        #region Properties

        public virtual UnitCatalogEntryName EntryName
        {
            get => _entryName;
            set
            {
                _entryName =
                    value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        #endregion

        #region Member variables

        private UnitCatalogEntryName _entryName =
            new UnitCatalogEntryName();

        #endregion
    }
}
