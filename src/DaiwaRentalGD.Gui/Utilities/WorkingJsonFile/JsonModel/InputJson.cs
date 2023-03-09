using Newtonsoft.Json;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile.JsonModel
{
    /// <summary>
    /// Model for loading input json to the system.
    /// </summary>
    public class InputJson
    {
        /// <summary>
        /// This is used for Optimization - Problem - Building Coverage Ratio Min
        /// </summary>
        [JsonProperty("BCR_Min")]
        public double BCRMin { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Building Coverage Ratio Max
        /// </summary>
        [JsonProperty("BCR_Max")]
        public double BCRMax { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Floor Area Ratio Min
        /// </summary>
        [JsonProperty("FAR_Min")]
        public double FARMin { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Floor Area Ratio Max
        /// </summary>
        [JsonProperty("FAR_Max")]
        public double FARMax { get; set; }

        /// <summary>
        /// This is used for Model - Site
        /// </summary>
        [JsonProperty("Site_Slant_Planes_Start_Height")]
        public float SiteSlantPlanesStartHeight { get; set; }

        /// <summary>
        /// This is used for Model - Site
        /// </summary>
        [JsonProperty("Site_Slant_Planes_Start_Slope")]
        public float SiteSlantPlanesStartSlope { get; set; }

        /// <summary>
        /// This is used for Model - Site
        /// </summary>
        [JsonProperty("North_Slant_Planes_Start_Height")]
        public float NorthSlantPlanesStartHeight { get; set; }

        /// <summary>
        /// This is used for Model - Site
        /// </summary>
        [JsonProperty("North_Slant_Planes_Start_Slope")]
        public float NorthSlantPlanesStartSlope { get; set; }

        /// <summary>
        /// This is used for Model - Site
        /// </summary>
        [JsonProperty("Road_Slant_Planes_Start_Height")]
        public float RoadSlantPlanesStartHeight { get; set; }

        /// <summary>
        /// This is used for Model - Site
        /// </summary>
        [JsonProperty("Road_Slant_Planes_Start_Slope")]
        public float RoadSlantPlanesStartSlope { get; set; }

        /// <summary>
        /// This is used for Model - Site
        /// </summary>
        [JsonProperty("Absolute_Height_Plane_Height")]
        public float AbsoluteHeightPlaneHeight { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Building (Type A)
        /// </summary>
        [JsonProperty("Use_Staircase_Type_A")]
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool UseStaircaseTypeA { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Building (Type B)
        /// </summary>
        [JsonProperty("Use_Side_Corridor_Type_B")]
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool UseSideCorridorTypeB { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Building (Type C)
        /// </summary>
        [JsonProperty("Use_Nagaya_Type_C")]
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool UseNagayaTypeC { get; set; }

        /// <summary>
        /// This is used for Model - Building
        /// </summary>
        [JsonProperty("Roof_Type")]
        public byte RoofType { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Building True North Angle Min
        /// </summary>
        [JsonProperty("Building_Rotation_From_North_Min_Degree")]
        public float BuildingRotationFromNorthMinDegree { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Building True North Angle Max
        /// </summary>
        [JsonProperty("Building_Rotation_From_North_Max_Degree")]
        public float BuildingRotationFromNorthMaxDegree { get; set; }

        /// <summary>
        /// This used for Model - Building - Siting - Orientation - Mode
        /// </summary>
        [JsonProperty("Enable_Building_Rotation_Snapping")]
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool EnableBuildingRotationSnapping { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Number Of Floors Min
        /// </summary>
        [JsonProperty("Number_Of_Floors_Min")]
        public int NumberOfFloorsMin { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Number Of Floors Max
        /// </summary>
        [JsonProperty("Number_Of_Floors_Max")]
        public int NumberOfFloorsMax { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Num of Unit per Floor Min
        /// </summary>
        [JsonProperty("Units_Per_Floor_Min")]
        public int NumberOfUnitsPerFloorMin { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Num of Unit per Floor Max
        /// </summary>
        [JsonProperty("Units_Per_Floor_Max")]
        public int NumberOfUnitsPerFloorMax { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Parking at Roadside or Driveway Min(Max)
        /// </summary>
        [JsonProperty("Enable_Roadside_Car_Parking")]
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool EnableRoadsideCarParking { get; set; }

        /// <summary>
        /// This is used for Optimization - Problem - Parking at Roadside or Driveway Min(Max)
        /// </summary>
        [JsonProperty("Enable_Driveway")]
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool EnableDriveway { get; set; }
    }
}
