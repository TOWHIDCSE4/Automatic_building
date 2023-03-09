using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile.JsonModel
{
    public class OutputLayout
    {
        /// <summary>
        /// This is used  for Model - Data - Land Use - Building Coverage Ratio(%)
        /// </summary>
        [JsonProperty("BCR")]
        public double? BCR { get; set; }
        /// <summary>
        /// This is used for Model - Data - Land Use - Floor Area Ratio(%)
        /// </summary>
        [JsonProperty("FAR")]
        public double FAR { get; set; }
        /// <summary>
        /// This is used for Model - Data - Finance - Cost (Million Yen)
        /// </summary>
        [JsonProperty("Cost")]
        public double Cost { get; set; }

        /// <summary>
        /// This is used for Model - Data - Finance - Revenue (Million Yen per Year)
        /// </summary>
        [JsonProperty("Profit_Per_Year")]
        public double ProfitPerYear { get; set; }

        /// <summary>
        /// This is used for Model - Data - Finance - Gross Rate of Return (% per Year)
        /// </summary>
        [JsonProperty("ROI_Per_Year")]
        public double ROIPerYear { get; set; }

        /// <summary>
        /// This is used for Model - Data - Slant Planes - North Slant Planes Valid?
        /// </summary>
        [JsonProperty("Clear_North_Slant_Planes")]
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool ClearNorthSlantPlanes { get; set; }

        /// <summary>
        /// This is used for Model - Data - Slant Planes - Adjacent Slant Planes Valid?
        /// </summary>
        [JsonProperty("Clear_Adjacent_Site_Slant_Planes")]
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool ClearAdjacentSiteSlantPlanes { get; set; }

        /// <summary>
        /// This is used for Model - Data - Slant Planes - Absolute Height Planes Valid?
        /// </summary>
        [JsonProperty("Clear_Absolute_Height_Planes")]
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool ClearAbsoluteHeightPlanes { get; set; }

        /// <summary>
        /// This is used for Model - Data - Slant Planes - Road Slant Planes Valid?
        /// </summary>
        [JsonProperty("Clear_Road_Slant_Planes")]
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool ClearRoadSlantPlanes { get; set; }

        /// <summary>
        /// This is used for Model - Data - Parking Lot - # of Car Parking Spaces
        /// </summary>
        [JsonProperty("Number_Of_Car_Parking_Spots")]
        public double NumberOfCarParkingSpots { get; set; }

        /// <summary>
        /// This is used for (calc) "# of Car Parking Spaces" * (1 - "Car Parking Spaces Fullfillment")
        /// </summary>
        [JsonProperty("Number_Of_Missing_Car_Parking_Spots")]
        public double NumberOfMissingCarParkingSpots { get; set; }

        /// <summary>
        /// This is used for Model - Data - Parking Lot - # of Bicycle Parking Spaces
        /// </summary>
        [JsonProperty("Number_Of_Bicycle_Slots")]
        public double NumberOfBicycleSlots { get; set; }

        /// <summary>
        /// This is used for (calc) "# of Bicycle Parking Spaces" * (1 - "Bicycle Parking Spaces Fullfillment")
        /// </summary>
        [JsonProperty("Number_Of_Missing_Bicycle_Slots")]
        public double NumberOfMissingBicycleSlots { get; set; }
        /// <summary>
        /// This is used for Model - Building - Units - Number of Floors
        /// </summary>
        [JsonProperty("Number_Of_Floors")]
        public double NumberOfFloors { get; set; }

        /// <summary>
        /// This is used for Model - Building - Units - Number of Unit per Floor
        /// </summary>
        [JsonProperty("Number_Of_Units_Per_Floor")]
        public double NumberOfUnitsPerFloor { get; set; }

        /// <summary>
        /// This is used for (calc) Number of Floors * Number of Unit per Floor
        /// </summary>
        [JsonProperty("Number_Of_Units")]
        public double NumberOfUnits { get; set; }

        /// <summary>
        /// This is used for (Model - Building - Units - Stack Unit Entry Indices)
        /// </summary>
        [JsonProperty("Unit_Types")]
        public List<List<string>> UnitTypes { get; set; }

        /// <summary>
        /// This is used for (Model - Building - Units - Stack Unit Entry Indices)
        /// </summary>
        [JsonProperty("Unit_Stats")]
        public List<UnitStat> UnitStats { get; set; }


      

    }

    public class UnitStat
    {
        [JsonProperty("Unit_Type")]
        public string Unit_Type { get; set; }

        [JsonProperty("Number_Of_Units_Of_Type")]
        public int NumberOfUnitsOfType { get; set; }
    }
}
