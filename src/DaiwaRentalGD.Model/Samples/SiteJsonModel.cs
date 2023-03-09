using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaiwaRentalGD.Model.Samples
{
    public class SiteJsonModel
    {
        /// <summary>
        /// This is used for see sample as Sample0SiteCreatorComponent
        /// </summary>
        [JsonProperty("Shikichi")]
        public List<double[]> Shikichi { get; set; }
        /// <summary>
        /// This is used for see sample as Sample1SiteCreatorComponent
        /// </summary>
        [JsonProperty("Neighbor_Boundary")]
        public List<List<double[]>> NeighborBoundary { get; set; }
        /// <summary>
        /// This is used for see sample as Sample2SiteCreatorComponent
        /// </summary>
        [JsonProperty("Road_Boundary")]
        public List<List<double[]>> RoadBoundary { get; set; }

        /// <summary>
        /// This is used for see sample as Sample3SiteCreatorComponent
        /// </summary>
        [JsonProperty("Opposite_Road_Boundary")]
        public List<List<double[]>> OppositeRoadBoundary { get; set; }
        /// <summary>
        /// This is used for see sample as Sample4SiteCreatorComponent
        /// </summary>
        [JsonProperty("North_Angle")]
        public double NorthAngle { get; set; }
    }
}
