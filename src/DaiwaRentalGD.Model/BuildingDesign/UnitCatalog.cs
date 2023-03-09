using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.Finance;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A scene object that contains a unit catalog.
    /// </summary>
    [Serializable]
    public class UnitCatalog : SceneObject
    {
        #region Constructors

        public UnitCatalog() : base()
        {
            InitializeComponents();
        }

        protected UnitCatalog(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _unitCatalogComponent = reader.GetValue<UnitCatalogComponent>(
                nameof(UnitCatalogComponent)
            );

            _unitCatalogJsonComponent =
                reader.GetValue<UnitCatalogJsonComponent>(
                    nameof(UnitCatalogJsonComponent)
                );

            if (!_unitCatalogComponent.DoesSerializeEntries)
            {
                _unitCatalogJsonComponent.Load();
            }
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(UnitCatalogComponent);
            AddComponent(UnitCatalogJsonComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(UnitCatalogComponent), _unitCatalogComponent
            );

            writer.AddValue(
                nameof(UnitCatalogJsonComponent), _unitCatalogJsonComponent
            );
        }

        public void FilterUnitCatalogEntriesByFinance(IEnumerable<UnitCostsAndRevenuesEntry> financeEntries)
        {
            // Filter unit catalog entries by cost and revenue of finance.
            // Entries that are not match should be removed from _unitCatalog.
            var entries = _unitCatalogComponent.Entries;
            var expectedEntries = entries.Where(entry =>
                            financeEntries.Any(
                                x =>
                                {
                                    var area = (entry.EntryName.SizeXInP + (entry.EntryName.VariantType & 1) * 0.5) * 0.91 * entry.EntryName.SizeYInP * 0.91;
                                    if (x.NumOfBedrooms == entry.NumOfBedrooms && area > x.MaxArea - 10 && area <= x.MaxArea)
                                        return true;

                                    return false;
                                }
                                )
                            );
            var removedEntries = entries.Except(expectedEntries);
            foreach (var entry in removedEntries)
            {
                _unitCatalogComponent.RemoveEntry(entry);
            }

            //_unitCatalogComponent.ReplaceBy(expectedEntries);
        }

        #endregion

        #region Properties

        public UnitCatalogComponent UnitCatalogComponent
        {
            get => _unitCatalogComponent;
            set
            {
                ReplaceComponent(_unitCatalogComponent, value);
                _unitCatalogComponent = value;

                NotifyPropertyChanged();
            }
        }

        public UnitCatalogJsonComponent UnitCatalogJsonComponent
        {
            get => _unitCatalogJsonComponent;
            set
            {
                ReplaceComponent(_unitCatalogJsonComponent, value);
                _unitCatalogJsonComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private UnitCatalogComponent _unitCatalogComponent =
            new UnitCatalogComponent();

        private UnitCatalogJsonComponent _unitCatalogJsonComponent =
            new UnitCatalogJsonComponent();

        #endregion
    }
}
