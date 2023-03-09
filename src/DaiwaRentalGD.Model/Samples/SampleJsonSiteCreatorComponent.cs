using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.Common;
using DaiwaRentalGD.Model.SiteDesign;
using Newtonsoft.Json;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// Sample
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.SiteCreatorComponent"/> #json.
    /// </summary>
    [Serializable]
    public class SampleJsonSiteCreatorComponent : SampleSiteCreatorComponent
    {
        #region Constructors

        public SampleJsonSiteCreatorComponent() : base()
        {
            Name = SampleJsonSiteCreatorName;
        }

        protected SampleJsonSiteCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
        }
        protected override void OnAdded()
        {
            base.OnAdded();
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

        }
        #endregion

        #region Methods
        public override void UpdateSite(Site site)
        {
            SiteJsonModel siteObj = GetSiteJsonData();
            if (siteObj == null)
                return;
            OptimizationCoordinate(siteObj);

            Polygon polygon = CreateShikichi(siteObj.Shikichi);
            List<LineSegment> neighborBoundaryLines = CreateBoundaryEdge(siteObj.NeighborBoundary);
            List<LineSegment> roadBoundaryLines = CreateBoundaryEdge(siteObj.RoadBoundary);
            List<LineSegment> oppositeRoadBoundaryLines = CreateBoundaryEdge(siteObj.OppositeRoadBoundary);

            site.SiteComponent.ClearSite();
            site.SiteComponent.Boundary = polygon;

            for (int i = 0; i < polygon.NumOfEdges; i++)
            {
                var type = GetBoundaryEdgeType(neighborBoundaryLines, roadBoundaryLines, polygon.GetEdge(i));
                site.SiteComponent.SetBoundaryEdgeType(i, type);
            }

            foreach (var opp in oppositeRoadBoundaryLines)
                site.SiteComponent.AddOppositeRoadEdge(opp);

            site.SiteComponent.TrueNorthAngle = BuildingPlacementComponent.ConvertDegreesToRadians(siteObj.NorthAngle);
        }
        #region private
        private SiteJsonModel GetSiteJsonData()
        {
            if (Path.IsPathRooted(_paramParg))
            {
                string jsonString = File.ReadAllText(_paramParg);
                var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, SiteJsonModel>>(jsonString);
                SiteJsonModel siteObj;
                if (jsonObj.TryGetValue(KEY_SITE_JSON, out siteObj))
                    return siteObj;
            }
            return null;
        }
        private SiteEdgeType GetBoundaryEdgeType(List<LineSegment> neighborBoundaryLines, List<LineSegment> roadBoundaryLines, LineSegment edge)
        {
            if (neighborBoundaryLines.FindIndex(t => t == edge) > -1)
                return SiteEdgeType.Property;
            else
            {
                if (roadBoundaryLines.FindIndex(t => t == edge) > -1)
                    return SiteEdgeType.Road;
                else
                    return SiteEdgeType.Unknown;
            }
        }
        private void OptimizationCoordinate(SiteJsonModel siteObj)
        {
            if (siteObj.Shikichi.Count < 1)
                return;
            double minX;
            double yOfMinX;
            GetMinXandYofminX(out minX, out yOfMinX, siteObj.Shikichi);

            OptimizationCoordinate(siteObj.Shikichi, minX, yOfMinX);
            OptimizationCoordinate(siteObj.NeighborBoundary, minX, yOfMinX);
            OptimizationCoordinate(siteObj.RoadBoundary, minX, yOfMinX);
            OptimizationCoordinate(siteObj.OppositeRoadBoundary, minX, yOfMinX);
        }
        private void OptimizationCoordinate(List<double[]> points, double x, double y)
        {
            foreach (var point in points)
            {
                point[0] = point[0] - x;
                point[1] = point[1] - y;
            }
        }
        private void OptimizationCoordinate(List<List<double[]>> glstPoints, double x, double y)
        {
            foreach (var points in glstPoints)
            {
                foreach (var point in points)
                {
                    point[0] = point[0] - x;
                    point[1] = point[1] - y;
                }
            }
        }
        private void GetMinXandYofminX(out double minX, out double yOfMinX, List<double[]> shikiches)
        {
            minX = shikiches[0][0];
            yOfMinX = shikiches[0][1];
            for (int i = 0; i < shikiches.Count; i++)
            {
                if (shikiches[i][0] < minX)
                {
                    minX = shikiches[i][0];
                    yOfMinX = shikiches[i][1];
                }
            }
        }
        private Polygon CreateShikichi(List<double[]> shikichi)
        {
            Polygon polygon = new Polygon();
            for (int i = 0; i < shikichi.Count - 1; i++)
                polygon.AddPoint(new Point(shikichi[i][0], shikichi[i][1], 0.0));
            return polygon;
        }
        private List<LineSegment> CreateBoundaryEdge(List<List<double[]>> Lines)
        {
            List<LineSegment> glstBoundaryEdge = new List<LineSegment>();
            foreach (var line in Lines)
            {

                for (int i = 0; i < line.Count - 1; i++)
                {
                    Point point1 = new Point(line[i][0], line[i][1], 0.0);
                    Point point2 = new Point(line[i + 1][0], line[i + 1][1], 0.0);
                    glstBoundaryEdge.Add(new LineSegment(point1, point2));
                }
            }
            return glstBoundaryEdge;
        }
        #endregion
        #endregion

        #region Constants

        public const string SampleJsonSiteCreatorName = "Site json";
        public const string KEY_SITE_JSON = "Site";
        private string _paramParg = Path.Combine(ConfigJsonPath.P3.RootP3Path,ConfigJsonPath.P3.Input,ConfigJsonPath.P3.Site);
        #endregion
    }
}

