using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// A factory class that creates a sample
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.UnitCatalog"/>.
    /// </summary>
    [Serializable]
    public class SampleUnitCatalogFactory : IWorkspaceItem
    {
        #region Constructors

        public SampleUnitCatalogFactory()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected SampleUnitCatalogFactory(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();
        }

        #endregion

        #region Methods

        public UnitCatalog Create()
        {
            var unitCatalog = new UnitCatalog();

            AddTypeANorthEntries(unitCatalog);
            AddTypeASouthEntries(unitCatalog);

            AddTypeBEntries(unitCatalog);

            AddTypeCNorthEntries(unitCatalog);
            AddTypeCSouthEntries(unitCatalog);

            return unitCatalog;
        }

        private void AddTypeANorthEntries(UnitCatalog unitCatalog)
        {
            var unitInfos = new List<Tuple<double, double, int>>();

            // 1R/1K/1DK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.5, maxSizeXInP: 7.0, sizeYInP: 6.0,
                numOfBedrooms: 0
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.0, maxSizeXInP: 6.0, sizeYInP: 7.0,
                numOfBedrooms: 0
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.5, maxSizeXInP: 5.5, sizeYInP: 8.0,
                numOfBedrooms: 0
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.0, maxSizeXInP: 5.0, sizeYInP: 9.0,
                numOfBedrooms: 0
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.0, maxSizeXInP: 4.5, sizeYInP: 10.0,
                numOfBedrooms: 0
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.0, maxSizeXInP: 4.0, sizeYInP: 11.0,
                numOfBedrooms: 0
            ));

            // 1LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.5, maxSizeXInP: 9.5, sizeYInP: 6.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.5, maxSizeXInP: 8.0, sizeYInP: 7.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.0, maxSizeXInP: 7.0, sizeYInP: 8.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.5, maxSizeXInP: 6.5, sizeYInP: 9.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.0, maxSizeXInP: 5.5, sizeYInP: 10.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.5, maxSizeXInP: 5.0, sizeYInP: 11.0,
                numOfBedrooms: 1
            ));

            // 2LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 10.0, maxSizeXInP: 11.0, sizeYInP: 6.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.5, maxSizeXInP: 10.5, sizeYInP: 7.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.5, maxSizeXInP: 9.0, sizeYInP: 8.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.0, maxSizeXInP: 8.0, sizeYInP: 9.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.0, maxSizeXInP: 7.0, sizeYInP: 10.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.5, maxSizeXInP: 6.5, sizeYInP: 11.0,
                numOfBedrooms: 2
            ));

            // 3LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 11.0, maxSizeXInP: 11.0, sizeYInP: 7.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 9.5, maxSizeXInP: 11.0, sizeYInP: 8.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.5, maxSizeXInP: 11.0, sizeYInP: 9.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.5, maxSizeXInP: 11.0, sizeYInP: 10.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.0, maxSizeXInP: 11.0, sizeYInP: 11.0,
                numOfBedrooms: 3
            ));

            var entries = new List<TypeAUnitComponent>();

            foreach (var unitInfo in unitInfos)
            {
                double sizeXInP = unitInfo.Item1;
                double sizeYInP = unitInfo.Item2;
                int numOfBedrooms = unitInfo.Item3;

                entries.Add(CreateTypeA(
                    sizeXInP, sizeYInP, numOfBedrooms,
                    TypeAUnitEntranceType.North,
                    TypeAUnitPositionType.Basic
                ));

                entries.Add(CreateTypeA(
                    sizeXInP, sizeYInP, numOfBedrooms,
                    TypeAUnitEntranceType.North,
                    TypeAUnitPositionType.End
                ));
            }

            foreach (var entry in entries)
            {
                unitCatalog.UnitCatalogComponent.AddEntry(entry);
            }
        }

        private void AddTypeASouthEntries(UnitCatalog unitCatalog)
        {
            var unitInfos = new List<Tuple<double, double, int>>();

            // 1LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.5, maxSizeXInP: 9.0, sizeYInP: 6.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.5, maxSizeXInP: 8.5, sizeYInP: 7.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.5, maxSizeXInP: 7.5, sizeYInP: 8.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.0, maxSizeXInP: 6.5, sizeYInP: 9.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.5, maxSizeXInP: 6.0, sizeYInP: 10.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.0, maxSizeXInP: 5.5, sizeYInP: 11.0,
                numOfBedrooms: 1
            ));

            // 2LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 9.5, maxSizeXInP: 11.0, sizeYInP: 6.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 9.0, maxSizeXInP: 11.0, sizeYInP: 7.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.0, maxSizeXInP: 10.5, sizeYInP: 8.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.0, maxSizeXInP: 9.0, sizeYInP: 9.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.5, maxSizeXInP: 8.0, sizeYInP: 10.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.0, maxSizeXInP: 7.5, sizeYInP: 11.0,
                numOfBedrooms: 2
            ));

            // 3LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 11.0, maxSizeXInP: 11.0, sizeYInP: 8.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 9.5, maxSizeXInP: 11.0, sizeYInP: 9.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.5, maxSizeXInP: 11.0, sizeYInP: 10.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.0, maxSizeXInP: 11.0, sizeYInP: 11.0,
                numOfBedrooms: 3
            ));

            var entries = new List<TypeAUnitComponent>();

            foreach (var unitInfo in unitInfos)
            {
                double sizeXInP = unitInfo.Item1;
                double sizeYInP = unitInfo.Item2;
                int numOfBedrooms = unitInfo.Item3;

                entries.Add(CreateTypeA(
                    sizeXInP, sizeYInP, numOfBedrooms,
                    TypeAUnitEntranceType.South,
                    TypeAUnitPositionType.Basic
                ));

                entries.Add(CreateTypeA(
                    sizeXInP, sizeYInP, numOfBedrooms,
                    TypeAUnitEntranceType.South,
                    TypeAUnitPositionType.End
                ));
            }

            foreach (var entry in entries)
            {
                unitCatalog.UnitCatalogComponent.AddEntry(entry);
            }
        }

        private void AddTypeBEntries(UnitCatalog unitCatalog)
        {
            var unitInfos = new List<Tuple<double, double, int>>();

            // 1R/1K/1DK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.5, maxSizeXInP: 8.0, sizeYInP: 6.0,
                numOfBedrooms: 0
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.0, maxSizeXInP: 6.5, sizeYInP: 7.0,
                numOfBedrooms: 0
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 3.5, maxSizeXInP: 6.0, sizeYInP: 8.0,
                numOfBedrooms: 0
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 3.0, maxSizeXInP: 5.0, sizeYInP: 9.0,
                numOfBedrooms: 0
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 3.0, maxSizeXInP: 4.5, sizeYInP: 10.0,
                numOfBedrooms: 0
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 3.0, maxSizeXInP: 4.0, sizeYInP: 11.0,
                numOfBedrooms: 0
            ));

            // 1LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.5, maxSizeXInP: 9.5, sizeYInP: 6.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.0, maxSizeXInP: 8.0, sizeYInP: 7.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.5, maxSizeXInP: 7.0, sizeYInP: 8.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.5, maxSizeXInP: 6.5, sizeYInP: 9.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.0, maxSizeXInP: 5.5, sizeYInP: 10.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.5, maxSizeXInP: 5.0, sizeYInP: 11.0,
                numOfBedrooms: 1
            ));

            // 2LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 10.0, maxSizeXInP: 10.0, sizeYInP: 6.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.5, maxSizeXInP: 10.0, sizeYInP: 7.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.5, maxSizeXInP: 9.5, sizeYInP: 8.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.0, maxSizeXInP: 8.0, sizeYInP: 9.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.0, maxSizeXInP: 7.5, sizeYInP: 10.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.5, maxSizeXInP: 6.5, sizeYInP: 11.0,
                numOfBedrooms: 2
            ));

            // 3LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 10.0, maxSizeXInP: 10.0, sizeYInP: 8.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.5, maxSizeXInP: 10.0, sizeYInP: 9.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.0, maxSizeXInP: 10.0, sizeYInP: 10.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.0, maxSizeXInP: 10.0, sizeYInP: 11.0,
                numOfBedrooms: 3
            ));

            var entries = new List<TypeBUnitComponent>();

            foreach (var unitInfo in unitInfos)
            {
                double sizeXInP = unitInfo.Item1;
                double sizeYInP = unitInfo.Item2;
                int numOfBedrooms = unitInfo.Item3;

                entries.Add(CreateTypeB(
                    sizeXInP, sizeYInP, numOfBedrooms,
                    TypeBUnitLayoutType.Basic
                ));

                var stairEntry = CreateTypeB(
                    sizeXInP, sizeYInP, numOfBedrooms,
                    TypeBUnitLayoutType.Stair
                );

                if (stairEntry != null)
                {
                    entries.Add(stairEntry);
                }
            }

            foreach (var entry in entries)
            {
                unitCatalog.UnitCatalogComponent.AddEntry(entry);
            }
        }

        private void AddTypeCNorthEntries(UnitCatalog unitCatalog)
        {
            var unitInfos = new List<Tuple<double, double, int>>();

            // 1LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.0, maxSizeXInP: 10.0, sizeYInP: 6.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.0, maxSizeXInP: 8.0, sizeYInP: 7.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.5, maxSizeXInP: 7.0, sizeYInP: 8.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.5, maxSizeXInP: 6.0, sizeYInP: 9.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.0, maxSizeXInP: 5.5, sizeYInP: 10.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 4.0, maxSizeXInP: 5.0, sizeYInP: 11.0,
                numOfBedrooms: 1
            ));

            // 2LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 10.5, maxSizeXInP: 11.0, sizeYInP: 6.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.5, maxSizeXInP: 10.5, sizeYInP: 7.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.5, maxSizeXInP: 9.5, sizeYInP: 8.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.5, maxSizeXInP: 8.0, sizeYInP: 9.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.0, maxSizeXInP: 7.0, sizeYInP: 10.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.5, maxSizeXInP: 6.5, sizeYInP: 11.0,
                numOfBedrooms: 2
            ));

            // 3LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 11.0, maxSizeXInP: 11.0, sizeYInP: 7.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 10.0, maxSizeXInP: 11.0, sizeYInP: 8.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.5, maxSizeXInP: 11.0, sizeYInP: 9.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.5, maxSizeXInP: 11.0, sizeYInP: 10.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.0, maxSizeXInP: 11.0, sizeYInP: 11.0,
                numOfBedrooms: 3
            ));

            var entries = new List<TypeCUnitComponent>();

            foreach (var unitInfo in unitInfos)
            {
                double sizeXInP = unitInfo.Item1;
                double sizeYInP = unitInfo.Item2;
                int numOfBedrooms = unitInfo.Item3;

                entries.Add(CreateTypeC(
                    sizeXInP, sizeYInP, numOfBedrooms,
                    TypeCUnitEntranceType.North,
                    TypeCUnitPositionType.FirstFloor
                ));

                entries.Add(CreateTypeC(
                    sizeXInP, sizeYInP, numOfBedrooms,
                    TypeCUnitEntranceType.North,
                    TypeCUnitPositionType.SecondFloor
                ));
            }

            foreach (var entry in entries)
            {
                unitCatalog.UnitCatalogComponent.AddEntry(entry);
            }
        }

        private void AddTypeCSouthEntries(UnitCatalog unitCatalog)
        {
            var unitInfos = new List<Tuple<double, double, int>>();

            // 1LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.5, maxSizeXInP: 9.5, sizeYInP: 6.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.5, maxSizeXInP: 8.0, sizeYInP: 7.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.0, maxSizeXInP: 7.0, sizeYInP: 8.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.0, maxSizeXInP: 6.5, sizeYInP: 9.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.0, maxSizeXInP: 5.5, sizeYInP: 10.0,
                numOfBedrooms: 1
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.0, maxSizeXInP: 5.0, sizeYInP: 11.0,
                numOfBedrooms: 1
            ));

            // 2LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 10.0, maxSizeXInP: 11.0, sizeYInP: 6.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 8.5, maxSizeXInP: 10.5, sizeYInP: 7.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.5, maxSizeXInP: 9.5, sizeYInP: 8.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.0, maxSizeXInP: 8.5, sizeYInP: 9.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 6.0, maxSizeXInP: 7.0, sizeYInP: 10.0,
                numOfBedrooms: 2
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 5.5, maxSizeXInP: 6.5, sizeYInP: 11.0,
                numOfBedrooms: 2
            ));

            // 3LDK

            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 11.0, maxSizeXInP: 11.0, sizeYInP: 7.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 10.0, maxSizeXInP: 11.0, sizeYInP: 8.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 9.0, maxSizeXInP: 11.0, sizeYInP: 9.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.5, maxSizeXInP: 11.0, sizeYInP: 10.0,
                numOfBedrooms: 3
            ));
            unitInfos.AddRange(GetRowUnitInfos(
                minSizeXInP: 7.0, maxSizeXInP: 11.0, sizeYInP: 11.0,
                numOfBedrooms: 3
            ));

            var entries = new List<TypeCUnitComponent>();

            foreach (var unitInfo in unitInfos)
            {
                double sizeXInP = unitInfo.Item1;
                double sizeYInP = unitInfo.Item2;
                int numOfBedrooms = unitInfo.Item3;

                entries.Add(CreateTypeC(
                    sizeXInP, sizeYInP, numOfBedrooms,
                    TypeCUnitEntranceType.South,
                    TypeCUnitPositionType.FirstFloor
                ));

                entries.Add(CreateTypeC(
                    sizeXInP, sizeYInP, numOfBedrooms,
                    TypeCUnitEntranceType.South,
                    TypeCUnitPositionType.SecondFloor
                ));
            }

            foreach (var entry in entries)
            {
                unitCatalog.UnitCatalogComponent.AddEntry(entry);
            }
        }

        public IReadOnlyList<Tuple<double, double, int>> GetRowUnitInfos(
            double minSizeXInP, double maxSizeXInP, double sizeYInP,
            int numOfBedrooms
        )
        {
            var rowUnitInfos = new List<Tuple<double, double, int>>();

            for (double sizeXInP = minSizeXInP; sizeXInP <= maxSizeXInP;
                sizeXInP += SizeStepXInP)
            {
                var unitInfo = new Tuple<double, double, int>(
                    sizeXInP, sizeYInP, numOfBedrooms
                );
                rowUnitInfos.Add(unitInfo);
            }

            return rowUnitInfos;
        }

        public TypeAUnitComponent CreateTypeA(
            double sizeXInP, double sizeYInP, int numOfBedrooms,
            TypeAUnitEntranceType entranceType,
            TypeAUnitPositionType positionType
        )
        {
            var entryCreator = new SampleTypeAUnitEntryCreator
            {
                RoomSizeXInP = sizeXInP,
                RoomSizeYInP = sizeYInP,
                NumOfBedrooms = numOfBedrooms,
                EntranceType = entranceType,
                PositionType = positionType
            };

            var uc = entryCreator.Create();

            return uc;
        }

        public TypeBUnitComponent CreateTypeB(
            double sizeXInP, double sizeYInP, int numOfBedrooms,
            TypeBUnitLayoutType layoutType
        )
        {
            var entryCreator = new SampleTypeBUnitEntryCreator
            {
                RoomSizeXInP = sizeXInP,
                RoomSizeYInP = sizeYInP,
                NumOfBedrooms = numOfBedrooms,
                LayoutType = layoutType
            };

            if (entryCreator.LayoutType == TypeBUnitLayoutType.Stair &&
                entryCreator.RoomSizeXInP <= entryCreator.StairSizeXInP)
            {
                return null;
            }

            var uc = entryCreator.Create();

            return uc;
        }

        public TypeCUnitComponent CreateTypeC(
            double sizeXInP, double sizeYInP, int numOfBedrooms,
            TypeCUnitEntranceType entranceType,
            TypeCUnitPositionType positionType
        )
        {
            var entryCreator = new SampleTypeCUnitEntryCreator
            {
                RoomSizeXInP = sizeXInP,
                RoomSizeYInP = sizeYInP,
                NumOfBedrooms = numOfBedrooms,
                EntranceType = entranceType,
                PositionType = positionType
            };

            if (positionType == TypeCUnitPositionType.SecondFloor)
            {
                entryCreator.EntryOffsetXInP = 0.0;
                entryCreator.EntryOffsetYInP = 0.0;
            }

            var uc = entryCreator.Create();

            return uc;
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>();

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        #endregion

        #region Constants

        public const double SizeStepXInP = 0.5;

        #endregion
    }
}
