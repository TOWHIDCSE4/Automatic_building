using System;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using Svg;
using Svg.Transforms;
using System.Drawing;
using Svg.Pathing;
using DaiwaRentalGD.Gui.Utilities.Configs;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.Site"/>.
    /// </summary>
    public class SiteModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public SiteModelSVGBuilder()
        { }
        public SiteModelSVGBuilder(Site site)
        {
            _site = site;
            CreateModel();
        }
        #endregion

        #region Methods

        private void CreateModel()
        {
            var sc = _site.SiteComponent;
            var siteTransform = _site.TransformComponent.Transform;
            AddSvgTransforms(siteTransform);

            // Create site land with background-color.
            var landGroup = CreateSvgPath(sc.Boundary, _siteBackgroundColor, Color.Transparent);
            AddChild(landGroup);

            // Create Opposite Road Edge
            foreach (var oppositeRoadEdge in sc.OppositeRoadEdges)
            {
                var oppositeRoadEdgeLine = CreateSvgLine(oppositeRoadEdge, _roadEdgeColor);
                AddChild(oppositeRoadEdgeLine);
            }

            // Create site edge with color. On top.
            SiteEdgeGroup = CreateSiteEdgeWithColor();
        }

        public SvgGroup CreateSiteEdgeWithColor()
        {
            var sc = _site.SiteComponent;
            var siteTransform = _site.TransformComponent.Transform;

            var siteEdgeGroup = new SvgGroup();
            siteEdgeGroup.Transforms = new Svg.Transforms.SvgTransformCollection();
            AddSvgTransforms(siteEdgeGroup, ConvertToSvgTransforms(siteTransform));

            // Create and Set color for each edge of the site.
            var plan = sc.Boundary;

            var numOfLines = plan.NumOfEdges;
            for (int index = 0; index < numOfLines; index++)
            {
                var line = plan.GetEdge(index);
                var siteEdgeType = _site.SiteComponent.BoundaryEdgeTypes[index];
                var color = GetColorForSiteEdge(siteEdgeType);
                var lineWithColor = CreateSvgLine(line, color);
                siteEdgeGroup.Children.Add(lineWithColor);
            }
            return siteEdgeGroup;
        }

        private Color GetColorForSiteEdge(SiteEdgeType siteEdgeType)
        {
            switch (siteEdgeType)
            {
                case SiteEdgeType.Property:
                    return _propertyEdgeColor;
                case SiteEdgeType.Road:
                    return _roadEdgeColor;
                default:
                    return _unknownEdgeColor;
            }
        }
        #endregion

        #region Properties

        public SvgGroup SiteEdgeGroup { get; private set; }

        private Site _site;
        #endregion

        #region Constants

        private Color _siteBackgroundColor = Color.FromArgb(230, 255, 214);

        private Color _roadEdgeColor = Color.LimeGreen;

        private Color _propertyEdgeColor = Color.DarkOliveGreen;

        private Color _unknownEdgeColor = Color.DimGray;

        #endregion

        public class SiteInfo
        {
            public double MinX { get; set; }
            public double MinY { get; set; }
            public double MaxX { get; set; }
            public double MaxY { get; set; }

            private Site _site;
            public SiteInfo()
            {

            }
            public SiteInfo(Site site)
            {
                _site = site;
                GetSize();
            }

            private void GetSize()
            {
                #region getSize
                //// Way 2  - Get min max from objects of site;
                /// Gets size of land.
                //var firstPoint = sc.Boundary.GetEdge(0);
                //var siteInfo = new SiteInfo()
                //{
                //    MinX = firstPoint.Point0.X,
                //    MinY = firstPoint.Point0.Y,
                //    MaxX = firstPoint.Point0.X,
                //    MaxY = firstPoint.Point0.Y,
                //};

                //// Gets size of land.
                //for (int edgeIndex = 0; edgeIndex < sc.Boundary.NumOfEdges; ++edgeIndex)
                //{
                //    var siteEdge = sc.Boundary.GetEdge(edgeIndex);
                //    GetMinMax(siteInfo, siteEdge);
                //}

                //// Gets size of oppositeRoad.
                //foreach (var oppositeRoadEdge in sc.OppositeRoadEdges)
                //{
                //    GetMinMax(siteInfo, oppositeRoadEdge);
                //}
                #endregion

                var bBox = _site.SiteComponent.GetBBox();

                // Includes compass arrow.
                double offsetX = bBox.MinX - Configs.Compass.LabeledArrowDiameter;
                double offsetY = bBox.MinY;

                this.MinX = offsetX * svgRatio;
                this.MinY = offsetY * svgRatio;
                this.MaxX = bBox.MaxX * svgRatio;
                this.MaxY = bBox.MaxY * svgRatio;
            }
            private void GetMinMax(SiteInfo siteInfo, LineSegment line)
            {
                siteInfo.MinX = Math.Min(Math.Min(siteInfo.MinX, line.Point0.X), line.Point1.X);
                siteInfo.MinY = Math.Min(Math.Min(siteInfo.MinY, line.Point0.Y), line.Point1.Y);
                siteInfo.MaxX = Math.Max(Math.Max(siteInfo.MaxX, line.Point0.X), line.Point1.X);
                siteInfo.MaxY = Math.Max(Math.Max(siteInfo.MaxY, line.Point0.Y), line.Point1.Y);
            }

            public double GetHeight()
            {
                return getLength(this.MinX, this.MaxX);
            }

            public double GetWidth()
            {
                return getLength(this.MinY, this.MaxY);
            }

            private double getLength(double min, double max)
            {
                return (min * max <= 0 ? (Math.Abs(min) + Math.Abs(max)) : Math.Abs(max > 0 ? max : min));
            }
        }
    }
}
