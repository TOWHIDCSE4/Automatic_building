using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.Site"/>.
    /// </summary>
    public class SiteModel3DBuilder
    {
        #region Constructors

        public SiteModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(Site site)
        {
            var siteModel = new Model3DGroup
            {
                Transform = Viewport3DUtils.ConvertToTransform3D(
                    site.TransformComponent.Transform
                )
            };

            var landModel = CreateLandModel(site);
            siteModel.Children.Add(landModel);

            var sc = site.SiteComponent;

            for (int edgeIndex = 0;
                edgeIndex < sc.Boundary.NumOfEdges;
                ++edgeIndex)
            {
                var siteEdge = sc.Boundary.GetEdge(edgeIndex);
                var siteEdgeType = sc.BoundaryEdgeTypes[edgeIndex];

                var siteEdgeModel =
                    CreateSiteEdgeModel(siteEdge, siteEdgeType);

                siteModel.Children.Add(siteEdgeModel);
            }

            foreach (var oppositeRoadEdge in sc.OppositeRoadEdges)
            {
                var oppositeRoadEdgeModel =
                    CreateOppositeRoadEdgeModel(oppositeRoadEdge);

                siteModel.Children.Add(oppositeRoadEdgeModel);
            }

            return siteModel;
        }

        public Model3D CreateLandModel(Site site)
        {
            var landMesh = Viewport3DUtils.ConvertToGeometry3D(
                GeometryUtils.CovnertToMesh(site.SiteComponent.Boundary)
            );

            GeometryModel3D landModel = new GeometryModel3D
            {
                Geometry = landMesh,
                Material = LandMaterial
            };

            return landModel;
        }

        public Model3D CreateSiteEdgeModel(
            Geometries.LineSegment siteEdge, SiteEdgeType siteEdgeType
        )
        {
            var siteEdgeMaterial = GetSiteEdgeMaterial(siteEdgeType);

            var siteEdgeModel = CreateEdgeModel(siteEdge, siteEdgeMaterial);

            return siteEdgeModel;
        }

        public Model3D CreateOppositeRoadEdgeModel(
            Geometries.LineSegment oppositeRoadEdge
        )
        {
            var oppositeRoadEdgeModel =
                CreateEdgeModel(oppositeRoadEdge, RoadEdgeMaterial);

            return oppositeRoadEdgeModel;
        }

        public Model3D CreateEdgeModel(
            Geometries.LineSegment edge, Material edgeMaterial
        )
        {
            Geometry3D edgeGeometry =
                Viewport3DUtils.CreateLinesGeometry3D(
                    new[] { edge }, SiteEdgeThickness
                );

            var edgeModel = new GeometryModel3D
            {
                Geometry = edgeGeometry,
                Material = edgeMaterial
            };

            return edgeModel;
        }

        private Material GetSiteEdgeMaterial(SiteEdgeType siteEdgeType)
        {
            switch (siteEdgeType)
            {
                case SiteEdgeType.Property:

                    return PropertyEdgeMaterial;

                case SiteEdgeType.Road:

                    return RoadEdgeMaterial;

                default:

                    return UnknownEdgeMaterial;
            }
        }

        #endregion

        #region Properties

        public double SiteEdgeThickness { get; set; } =
            DefaultSiteEdgeThickness;

        public Brush LandBrush { get; set; } =
            DefaultLandBrush;

        public Brush PropertyEdgeBrush { get; set; } =
            DefaultPropertyEdgeBrush;

        public Brush RoadEdgeBrush { get; set; } =
            DefaultRoadEdgeBrush;

        public Brush UnknownEdgeBrush { get; set; } =
            DefaultUnknownEdgeBrush;

        public Material LandMaterial => new DiffuseMaterial
        {
            Brush = LandBrush
        };

        public Material PropertyEdgeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(PropertyEdgeBrush);

        public Material RoadEdgeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(RoadEdgeBrush);

        public Material UnknownEdgeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(UnknownEdgeBrush);

        #endregion

        #region Constants

        public const double DefaultSiteEdgeThickness = 0.05;

        public static readonly Brush DefaultLandBrush =
            new SolidColorBrush(Color.FromRgb(230, 255, 214));

        public static readonly Brush DefaultPropertyEdgeBrush =
            Brushes.DarkOliveGreen;

        public static readonly Brush DefaultRoadEdgeBrush =
            Brushes.LimeGreen;

        public static readonly Brush DefaultUnknownEdgeBrush =
            Brushes.DimGray;

        #endregion
    }
}
