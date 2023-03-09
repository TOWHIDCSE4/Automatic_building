using System;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Model;
using Svg;
using static DaiwaRentalGD.Gui.VisualizationSVG.SiteModelSVGBuilder;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.GDModelScene"/>.
    /// </summary>
    public class GDModelSceneModelSVGBuilder
    {
        #region Constructors

        public GDModelSceneModelSVGBuilder()
        { }

        #endregion

        #region Methods

        public SvgDocument CreateSVG(GDModelScene gdms)
        {
            var siteInfo = new SiteInfo(gdms.Site);
            int padding = 15;
            float maxLength = Math.Max((float)siteInfo.GetWidth(), (float)siteInfo.GetHeight());
            var svgDoc = new SvgDocument
            {
                ViewBox = new SvgViewBox((float)siteInfo.MinX - padding, (float)siteInfo.MinY - padding, maxLength + padding * 2, maxLength + padding * 2),
            };

            // svgDoc.CustomAttributes.Add("preserveAspectRatio", "none");
            svgDoc.CustomAttributes.Add("font-family", "-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Tohoma, Helvetica, Arial, sans-serif, 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol'");
            svgDoc.Transforms = new Svg.Transforms.SvgTransformCollection();
            svgDoc.Transforms.Add(new Svg.Transforms.SvgScale(1, -1));


            SiteModelSVGBuilder = new SiteModelSVGBuilder(gdms.Site);
            ParkingLotModelSVGBuilder = new ParkingLotModelSVGBuilder(gdms.ParkingLot);
            BuildingModelSVGBuilder = new BuildingModelSVGBuilder(gdms.Building);
            CompassModelSVGBuilder = new CompassModelSVGBuilder(gdms.Site);
            ScaleBarModelSVGBuilder = new ScaleBarModelSVGBuilder(gdms.Site);

            svgDoc.Children.Add(SiteModelSVGBuilder.SvgGroup);
            svgDoc.Children.Add(ParkingLotModelSVGBuilder.SvgGroup);
            svgDoc.Children.Add(BuildingModelSVGBuilder.SvgGroup);
            svgDoc.Children.Add(CompassModelSVGBuilder.SvgGroup);
            svgDoc.Children.Add(ScaleBarModelSVGBuilder.SvgGroup);
            svgDoc.Children.Add(SiteModelSVGBuilder.SiteEdgeGroup);
            

            return svgDoc;
        }

        #endregion

        #region Properties

        public SiteModelSVGBuilder SiteModelSVGBuilder { get; private set; }

        public ParkingLotModelSVGBuilder ParkingLotModelSVGBuilder { get; private set; }

        public BuildingModelSVGBuilder BuildingModelSVGBuilder { get; private set; }

        public CompassModelSVGBuilder CompassModelSVGBuilder { get; private set; }

        public ScaleBarModelSVGBuilder ScaleBarModelSVGBuilder { get; private set; }
        #endregion
    }
}
