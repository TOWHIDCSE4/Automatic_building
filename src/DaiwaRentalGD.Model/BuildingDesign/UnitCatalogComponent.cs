using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that represents a unit catalog.
    /// </summary>
    [Serializable]
    public class UnitCatalogComponent : Component
    {
        #region Constructors

        public UnitCatalogComponent() : base()
        { }

        protected UnitCatalogComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            DoesSerializeEntries =
                reader.GetValue<bool>(nameof(DoesSerializeEntries));

            if (DoesSerializeEntries)
            {
                var entries =
                    reader.GetValues<CatalogUnitComponent>(nameof(Entries));

                foreach (var entry in entries)
                {
                    _entries.Add(entry);
                }
            }
        }

        #endregion

        #region Methods

        public void AddEntry(CatalogUnitComponent entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            _entries.Add(entry);
        }

        public bool RemoveEntry(CatalogUnitComponent entry)
        {
            return _entries.Remove(entry);
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Concat(
                DoesSerializeEntries ?
                Entries : Enumerable.Empty<IWorkspaceItem>()
            );

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(DoesSerializeEntries), DoesSerializeEntries
            );

            if (DoesSerializeEntries)
            {
                writer.AddValues(nameof(Entries), _entries);
            }
        }

        #endregion

        #region Properties

        public IReadOnlyList<CatalogUnitComponent> Entries =>
            _entries.ToList();

        public bool DoesSerializeEntries { get; set; } =
            DefaultDoesSerializeEntries;

        #endregion

        #region Member variables

        private readonly SortedSet<CatalogUnitComponent> _entries =
            new SortedSet<CatalogUnitComponent>(
                new CatalogUnitComponentComparer()
            );

        #endregion

        #region Constants

        public const bool DefaultDoesSerializeEntries = false;

        #endregion
    }
}
