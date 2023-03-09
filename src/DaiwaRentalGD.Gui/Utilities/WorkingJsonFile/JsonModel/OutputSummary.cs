using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile.JsonModel
{
    public class OutputSummary
    {
        /// <summary>
        /// This is used for ID
        /// </summary>
        [JsonProperty("ID")]
        public string ID { get; set; }
        /// <summary>
        /// This is used  for Model - Data - Building Coverage Ratio 
        /// </summary>
        [JsonProperty("BCR")]
        public double? BCR { get; set; }
        /// <summary>
        /// This is used for Model - Data -Floor Area Ratio 
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
        [JsonProperty("Profit_Per_Year ")]
        public double ProfitPerYear { get; set; }

        /// <summary>
        /// This is used for Model - Data - Finance - Gross Rate of Return (% per Year)
        /// </summary>
        [JsonProperty("ROI_Per_Year")]
        public double ROIPerYear { get; set; }

        /// <summary>
        /// This is used for Model - Data - Parking Lot - # of Car Parking Spaces
        /// </summary>
        [JsonProperty("Number_Of_Car_Parking_Spots")]
        public float NumofCarParkingSpots { get; set; }

        /// <summary>
        /// This is used for Model - Data - Parking Lot - # of Bicycle Parking Spaces
        /// </summary>
        [JsonProperty("Number_Of_Bicycle_Parking_Slots")]
        public float NumofBicycleParkingSlots { get; set; }


        /// <summary>
        /// This is used for Model - Building - Unit Type
        /// </summary>
        [JsonProperty("Unit_Type")]
        public string UnitType { get; set; }

        /// <summary>
        /// This is used for Model - Building - Units - Number of Floors
        /// </summary>
        [JsonProperty("Number_Of_Floors")]
        public float NumberofFloors { get; set; }


    }
}
