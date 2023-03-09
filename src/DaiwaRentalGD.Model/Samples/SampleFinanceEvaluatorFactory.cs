using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.Finance;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// A factory class that creates a sample
    /// <see cref="DaiwaRentalGD.Model.Finance.FinanceEvaluator"/>.
    /// </summary>
    [Serializable]
    public class SampleFinanceEvaluatorFactory : IWorkspaceItem
    {
        #region Constructors

        public SampleFinanceEvaluatorFactory()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected SampleFinanceEvaluatorFactory(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();
        }

        #endregion

        #region Methods

        public FinanceEvaluator Create()
        {
            FinanceEvaluator financeEvaluator = new FinanceEvaluator();

            UnitFinanceComponent ufe = financeEvaluator.UnitFinanceComponent;

            //AddCostEntries(ufe);
            //AddRevenueEntries(ufe);

            return financeEvaluator;
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

        //private void AddCostEntries(UnitFinanceComponent ufe)
        //{
        //    ufe.ClearCostEntries();

        //    ufe.AddCostEntry(
        //        new UnitCostEntry
        //        {
        //            NumOfBedrooms = 0,
        //            CostYen = 10_000_000
        //        }
        //    );
        //    ufe.AddCostEntry(
        //        new UnitCostEntry
        //        {
        //            NumOfBedrooms = 1,
        //            CostYen = 11_000_000
        //        }
        //    );
        //    ufe.AddCostEntry(
        //        new UnitCostEntry
        //        {
        //            NumOfBedrooms = 2,
        //            CostYen = 12_000_000
        //        }
        //    );
        //    ufe.AddCostEntry(
        //        new UnitCostEntry
        //        {
        //            NumOfBedrooms = 3,
        //            CostYen = 13_000_000
        //        }
        //    );
        //}

        //private void AddRevenueEntries(UnitFinanceComponent ufe)
        //{
        //    ufe.ClearRevenueEntries();

        //    // 1R/1K/1DK

        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 0,
        //            RevenueYenPerSqmPerMonth = 2520
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 30,
        //            RevenueYenPerSqmPerMonth = 2520
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 35,
        //            RevenueYenPerSqmPerMonth = 2331
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 40,
        //            RevenueYenPerSqmPerMonth = 2185
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 45,
        //            RevenueYenPerSqmPerMonth = 2064
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 50,
        //            RevenueYenPerSqmPerMonth = 2014
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 60,
        //            RevenueYenPerSqmPerMonth = 2009
        //        }
        //    );

        //    // 1LDK

        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 0,
        //            RevenueYenPerSqmPerMonth = 2606
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 35,
        //            RevenueYenPerSqmPerMonth = 2606
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 40,
        //            RevenueYenPerSqmPerMonth = 2425
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 45,
        //            RevenueYenPerSqmPerMonth = 2278
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 50,
        //            RevenueYenPerSqmPerMonth = 2206
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 60,
        //            RevenueYenPerSqmPerMonth = 2003
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 70,
        //            RevenueYenPerSqmPerMonth = 1849
        //        }
        //    );

        //    // 2LDK

        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 0,
        //            RevenueYenPerSqmPerMonth = 2360
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 40,
        //            RevenueYenPerSqmPerMonth = 2360
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 45,
        //            RevenueYenPerSqmPerMonth = 2220
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 50,
        //            RevenueYenPerSqmPerMonth = 2154
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 60,
        //            RevenueYenPerSqmPerMonth = 1960
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 70,
        //            RevenueYenPerSqmPerMonth = 1811
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 80,
        //            RevenueYenPerSqmPerMonth = 1696
        //        }
        //    );

        //    // 3LDK

        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 0,
        //            RevenueYenPerSqmPerMonth = 2188
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 50,
        //            RevenueYenPerSqmPerMonth = 2188
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 60,
        //            RevenueYenPerSqmPerMonth = 1987
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 70,
        //            RevenueYenPerSqmPerMonth = 1836
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 80,
        //            RevenueYenPerSqmPerMonth = 1716
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 90,
        //            RevenueYenPerSqmPerMonth = 1620
        //        }
        //    );
        //}

        //private void AddCostAndRevenueEntries(UnitFinanceComponent ufe)
        //{
        //    ufe.ClearCostAndRevenueEntries();

        //    // 1R/1K/1DK

        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 0,
        //            RevenueYenPerSqmPerMonth = 2520
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 30,
        //            RevenueYenPerSqmPerMonth = 2520
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 35,
        //            RevenueYenPerSqmPerMonth = 2331
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 40,
        //            RevenueYenPerSqmPerMonth = 2185
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 45,
        //            RevenueYenPerSqmPerMonth = 2064
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 50,
        //            RevenueYenPerSqmPerMonth = 2014
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 0,
        //            MinArea = 60,
        //            RevenueYenPerSqmPerMonth = 2009
        //        }
        //    );

        //    // 1LDK

        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 0,
        //            RevenueYenPerSqmPerMonth = 2606
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 35,
        //            RevenueYenPerSqmPerMonth = 2606
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 40,
        //            RevenueYenPerSqmPerMonth = 2425
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 45,
        //            RevenueYenPerSqmPerMonth = 2278
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 50,
        //            RevenueYenPerSqmPerMonth = 2206
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 60,
        //            RevenueYenPerSqmPerMonth = 2003
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 1,
        //            MinArea = 70,
        //            RevenueYenPerSqmPerMonth = 1849
        //        }
        //    );

        //    // 2LDK

        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 0,
        //            RevenueYenPerSqmPerMonth = 2360
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 40,
        //            RevenueYenPerSqmPerMonth = 2360
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 45,
        //            RevenueYenPerSqmPerMonth = 2220
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 50,
        //            RevenueYenPerSqmPerMonth = 2154
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 60,
        //            RevenueYenPerSqmPerMonth = 1960
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 70,
        //            RevenueYenPerSqmPerMonth = 1811
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 2,
        //            MinArea = 80,
        //            RevenueYenPerSqmPerMonth = 1696
        //        }
        //    );

        //    // 3LDK

        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 0,
        //            RevenueYenPerSqmPerMonth = 2188
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 50,
        //            RevenueYenPerSqmPerMonth = 2188
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 60,
        //            RevenueYenPerSqmPerMonth = 1987
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 70,
        //            RevenueYenPerSqmPerMonth = 1836
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 80,
        //            RevenueYenPerSqmPerMonth = 1716
        //        }
        //    );
        //    ufe.AddRevenueEntry(
        //        new UnitRevenueEntry
        //        {
        //            NumOfBedrooms = 3,
        //            MinArea = 90,
        //            RevenueYenPerSqmPerMonth = 1620
        //        }
        //    );
        //}
        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        #endregion
    }
}
