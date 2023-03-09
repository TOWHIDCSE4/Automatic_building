using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// JSON-related extension methods for
    /// types in <see cref="ParkingLotDesign"/>.
    /// </summary>
    public static class ParkingRequirementsJsonExtensions
    {
        #region Methods

        public static void LoadJson(
            this ParkingLotRequirementsComponent plrc, string json
        )
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var jObject = JObject.Parse(json);

            plrc.LoadJson(jObject);
        }

        internal static void LoadJson(
            this ParkingLotRequirementsComponent plrc, JObject jObject
        )
        {
            if (jObject == null)
            {
                throw new ArgumentNullException(nameof(jObject));
            }

            plrc.ClearUnitRequirementsTable();
    
            var uprJArray = (JArray)jObject[UnitRequirementsTable];

            foreach (var uprJObject in uprJArray.Cast<JObject>())
            {
                int numOfBedrooms = (int)uprJObject[NumberOfBedrooms];
                UnitParkingRequirements upr = new UnitParkingRequirements(numOfBedrooms);
                upr.LoadJson(uprJObject);

                plrc.AddUnitRequirements(upr);
            }
        }

        private static void LoadJson(
            this UnitParkingRequirements upr, JObject jObject
        )
        {
            upr.CarParkingSpaceMin = (double)jObject[CarParkingSpaceMin];
            upr.CarParkingSpaceMax = (double)jObject[CarParkingSpaceMax];
            upr.BicycleParkingSpaceMin = (double)jObject[BicycleParkingSpaceMin];
            upr.BicycleParkingSpaceMax = (double)jObject[BicycleParkingSpaceMax];

        }

        #endregion

        #region Constants
        public const string UnitRequirementsTable = "Unit_Requirements_Table";
        public const string NumberOfBedrooms = "Number_Of_Bedrooms";
        public const string CarParkingSpaceMin = "Car_Parking_Space_Min";
        public const string CarParkingSpaceMax = "Car_Parking_Space_Max";
        public const string BicycleParkingSpaceMin = "Bicycle_Parking_Space_Min";
        public const string BicycleParkingSpaceMax = "Bicycle_Parking_Space_Max";
        #endregion
    }
}
