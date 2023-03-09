using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Finance
{
    /// <summary>
    /// Calculates financial metrics of units.
    /// </summary>
    [Serializable]
    public class UnitFinanceComponent : Component
    {
        #region Constructors

        public UnitFinanceComponent() : base()
        {
            CostAndrevenueEntries = _costAndrevenueEntries.AsReadOnly();
        }

        protected UnitFinanceComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            CostAndrevenueEntries = _costAndrevenueEntries.AsReadOnly();

            var reader = new WorkspaceItemReader(this, info, context);

            _costAndrevenueEntries.AddRange(
                reader.GetValues<UnitCostsAndRevenuesEntry>(nameof(CostAndrevenueEntries))
            );
        }

        #endregion

        #region Methods

        #region Revenue related

        public void AddCostAndRevenueEntries(UnitCostsAndRevenuesEntry unitCostsAndRevenuesEntry)
        {
            if (unitCostsAndRevenuesEntry == null)
            {
                throw new ArgumentNullException(nameof(unitCostsAndRevenuesEntry));
            }

            _costAndrevenueEntries.Add(unitCostsAndRevenuesEntry);
        }

        public bool RemoveCostAndRevenueEntries(UnitCostsAndRevenuesEntry unitCostsAndRevenuesEntry)
        {
            return _costAndrevenueEntries.Remove(unitCostsAndRevenuesEntry);
        }

        public void ClearCostAndRevenueEntries()
        {
            _costAndrevenueEntries.Clear();
        }
        public UnitCostsAndRevenuesEntry GetCostsAndRevenueEntry(
            int numOfBedrooms, double unitArea
        )
        {
            var costAndrevenueEntries = CostAndrevenueEntries
                .Where(x => x.NumOfBedrooms == numOfBedrooms)
                .OrderBy(re => re.MaxArea).FirstOrDefault(x=>x.MaxArea >= unitArea);

            return costAndrevenueEntries;
        }

        public UnitCostsAndRevenuesEntry GetCostsAndRevenueEntry(Unit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            int numOfBedrooms = unit.UnitComponent.NumOfBedrooms;

            double unitArea = unit.UnitComponent.TotalRoomPlanArea;

            if (unit.UnitComponent is CatalogUnitComponent catalogUnit)
            {
                unitArea = (catalogUnit.EntryName.SizeXInP + (catalogUnit.EntryName.VariantType & 1) * 0.5) * 0.91 * catalogUnit.EntryName.SizeYInP * 0.91;
            }

            var revenueEntry = GetCostsAndRevenueEntry(numOfBedrooms, unitArea);

            if (revenueEntry == null)
            {
                throw new ArgumentException(
                    $"No revenue entry found for {nameof(unit)}",
                    nameof(unit)
                );
            }

            return revenueEntry;
        }

        public double GetUnitRevenueYenPerMonth(Unit unit)
        {
            double unitArea = unit.UnitComponent.TotalRoomPlanArea;

            var CostAndrevenueEntry = GetCostsAndRevenueEntry(unit);

            double revenueYenPerMonth =
                CostAndrevenueEntry.RevenueYenPerSqmPerMonth * unitArea;

            return revenueYenPerMonth;
        }

        public double GetUnitRevenueYenPerYear(Unit unit)
        {
            return GetUnitRevenueYenPerMonth(unit) * NumOfMonthsPerYear;
        }

        public double GetBuildingRevenueYenPerMonth(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            double buildingRevenueYenPerMonth = 0.0;

            foreach (Unit unit in building.BuildingComponent.Units)
            {
                if (unit == null)
                {
                    continue;
                }

                double unitRevenueYenPerMonth =
                    GetUnitRevenueYenPerMonth(unit);

                buildingRevenueYenPerMonth += unitRevenueYenPerMonth;
            }

            return buildingRevenueYenPerMonth;
        }

        public double GetBuildingRevenueYenPerYear(Building building)
        {
            return
                GetBuildingRevenueYenPerMonth(building) * NumOfMonthsPerYear;
        }

        public double GetBuildingCostYen(Building building)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building));
            }

            double buildingCostYen = 0.0;

            foreach (Unit unit in building.BuildingComponent.Units)
            {
                if (unit == null)
                {
                    continue;
                }

                var costEntry = GetCostsAndRevenueEntry(unit);

                double unitCostYen = costEntry.CostYen;

                buildingCostYen += unitCostYen;
            }

            return buildingCostYen;
        }


        #endregion

        public void ClearEntries()
        {
            ClearCostAndRevenueEntries();
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);
            writer.AddValues(nameof(CostAndrevenueEntries), _costAndrevenueEntries);
        }

        #endregion

        #region Properties

        public IReadOnlyList<UnitCostsAndRevenuesEntry> CostAndrevenueEntries { get; }

        #endregion

        #region Member variables

        private readonly List<UnitCostsAndRevenuesEntry> _costAndrevenueEntries =
           new List<UnitCostsAndRevenuesEntry>();

        #endregion

        #region Constants

        public const int NumOfMonthsPerYear = 12;

        #endregion
    }
}
