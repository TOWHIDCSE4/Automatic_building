using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DaiwaRentalGD.Model.Finance
{
    /// <summary>
    /// JSON-related extension methods for types in <see cref="Finance"/>.
    /// </summary>
    public static class FinanceDataJsonExtensions
    {
        #region Methods

        public static void LoadJson(
            this ParkingLotFinanceComponent plfc, string json
        )
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var jObject = JObject.Parse(json);

            plfc.LoadJson(jObject);
        }

        internal static void LoadJson(
            this ParkingLotFinanceComponent plfc, JObject jObject
        )
        {
            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            plfc.CostYenPerSqm = (double)jObject[CostYenPerSqm];

            plfc.RevenueYenPerCarParkingSpacePerMonth =
                (double)jObject[RevenueYenPerCarParkingSpacePerMonth];
        }

        public static void LoadJson(
            this UnitFinanceComponent ufc, string json
        )
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var jObject = JObject.Parse(json);

            ufc.LoadJson(jObject);
        }

        internal static void LoadJson(
            this UnitFinanceComponent ufc, JObject jObject
        )
        {
            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            ufc.ClearEntries();

            var unitCostAndRevenueJArray = (JArray)jObject[UnitCostsAndRevenuesKey];

            foreach (var unitJObject in unitCostAndRevenueJArray.Cast<JObject>())
            {
                var unitEntry = new UnitCostsAndRevenuesEntry();
                unitEntry.LoadJson(unitJObject);

                ufc.AddCostAndRevenueEntries(unitEntry);
            }
        }
        public static void LoadJson(this UnitCostsAndRevenuesEntry uce, string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var jObject = JObject.Parse(json);

            uce.LoadJson(jObject);
        }
        internal static void LoadJson(this UnitCostsAndRevenuesEntry uce, JObject jObject)
        {
            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            uce.NumOfBedrooms =
                (int)jObject[NumOfBedroomsKey];

            uce.CostYen =
                (double)jObject[CostYenKey];
            uce.MaxArea = (double)jObject[MaxAreaKey];

            uce.RevenueYenPerSqmPerMonth = (double)
                jObject[RevenueYenPerSqmPerMonthKey];
        }
        #endregion

        #region Constants
        public const string ParkingLot= "Parking_Lot";
        public const string CostYenPerSqm = "Cost_Yen_Per_Sqm";
        public const string RevenueYenPerCarParkingSpacePerMonth = "Revenue_Yen_Per_Car_Parking_Space_Per_Month";

        public const string UnitCostsAndRevenuesKey = "Costs_And_Revenues";
        public const string NumOfBedroomsKey = "Number_Of_Bedrooms";
        public const string CostYenKey = "Cost_Yen";
        public const string MaxAreaKey = "Max_Area";
        public const string RevenueYenPerSqmPerMonthKey = "Revenue_Yen_Per_Sqm_Per_Month";
        #endregion
    }
}
