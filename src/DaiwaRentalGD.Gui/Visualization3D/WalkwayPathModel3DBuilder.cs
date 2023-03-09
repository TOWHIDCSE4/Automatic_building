using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.WalkwayPath"/>.
    /// </summary>
    public class WalkwayPathModel3DBuilder
    {
        #region Constructors

        public WalkwayPathModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(WalkwayPath walkwayPath)
        {
            var pathModel = new Model3DGroup
            {
                Transform = new TranslateTransform3D
                {
                    OffsetZ = PathOffsetZ
                }
            };

            foreach (var edge in walkwayPath.GetEdges())
            {
                var edgeModel = CreateEdgeModel(edge);
                pathModel.Children.Add(edgeModel);
            }

            foreach (var vertex in walkwayPath.GetVertices())
            {
                var vertexModel = CreateVertexModel(vertex);
                pathModel.Children.Add(vertexModel);
            }

            return pathModel;
        }

        private Model3D CreateEdgeModel(WalkwayGraphEdge edge)
        {
            var edgeGeometry = Viewport3DUtils.CreateLineGeometry3D(
                edge.Vertex0.Point, edge.Vertex1.Point,
                EdgeThickness
            );

            var edgeModel = new GeometryModel3D
            {
                Geometry = edgeGeometry,
                Material = EdgeMaterial
            };

            return edgeModel;
        }

        private Model3D CreateVertexModel(WalkwayGraphVertex vertex)
        {
            var point0 = new Point(vertex.Point);

            var point1 = new Point(point0);
            point1.Z += VertexMarkHeight;

            var vertexGeometry = Viewport3DUtils.CreateLineGeometry3D(
                point0, point1, VertexMarkThickness
            );

            Material vertexMaterial = GetVertexMaterial(vertex);

            var vertexModel = new GeometryModel3D
            {
                Geometry = vertexGeometry,
                Material = vertexMaterial
            };

            return vertexModel;
        }

        private Material GetVertexMaterial(WalkwayGraphVertex vertex)
        {
            switch (vertex.Type)
            {
                case WalkwayGraphVertexType.Source:
                    return SourceVertexMaterial;

                case WalkwayGraphVertexType.Destination:
                    return DestinationVertexMaterial;

                default:
                    return EdgeMaterial;
            }
        }

        #endregion

        #region Properties

        public double PathOffsetZ { get; set; } = DefaultPathOffsetZ;

        public double VertexMarkHeight { get; set; } =
            DefaultVertexMarkHeight;

        public double VertexMarkThickness { get; set; } =
            DefaultVertexMarkThickness;

        public double EdgeThickness { get; set; } = DefaultEdgeThickness;

        public Brush EdgeBrush { get; set; } = DefaultEdgeBrush;

        public Brush SourceVertexBrush { get; set; } =
            DefaultSourceVertexBrush;

        public Brush DestinationVertexBrush { get; set; } =
            DefaultDestinationVertexBrush;

        public Material EdgeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(EdgeBrush);

        public Material SourceVertexMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(SourceVertexBrush);

        public Material DestinationVertexMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(DestinationVertexBrush);

        #endregion

        #region Constants

        public const double DefaultPathOffsetZ = 0.05;

        public const double DefaultVertexMarkHeight = 0.5;
        public const double DefaultVertexMarkThickness = 0.15;
        public const double DefaultEdgeThickness = 0.05;

        public static readonly Brush DefaultEdgeBrush =
            Brushes.Red;
        public static readonly Brush DefaultSourceVertexBrush =
            Brushes.LimeGreen;
        public static readonly Brush DefaultDestinationVertexBrush =
            Brushes.Orange;

        #endregion
    }
}
