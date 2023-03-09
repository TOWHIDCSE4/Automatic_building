using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.BikewayTile"/>.
    /// </summary>
    public class BikewayTileModel3DBuilder
    {
        #region Constructors

        public BikewayTileModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(BikewayTile bikewayTile)
        {
            var model = new Model3DGroup
            {
                Transform = Viewport3DUtils.ConvertToTransform3D(
                    bikewayTile.TransformComponent.Transform
                )
            };

            var tileModel = CreateTileModel(bikewayTile);
            model.Children.Add(tileModel);

            var leftBpas = bikewayTile.BikewayTileComponent
                .LeftBicycleParkingAreaAnchorComponent
                .BicycleParkingAreas.Values;

            foreach (BicycleParkingArea bpa in leftBpas)
            {
                var bpaModel =
                    BicycleParkingAreaModel3DBuilder.CreateModel(bpa);

                model.Children.Add(bpaModel);
            }

            var rightBpas = bikewayTile.BikewayTileComponent
                .RightBicycleParkingAreaAnchorComponent
                .BicycleParkingAreas.Values;

            foreach (BicycleParkingArea bpa in rightBpas)
            {
                var bpaModel =
                    BicycleParkingAreaModel3DBuilder.CreateModel(bpa);

                model.Children.Add(bpaModel);
            }

            return model;
        }

        private Model3D CreateTileModel(BikewayTile bikewayTile)
        {
            var tileModel = new Model3DGroup();

            var tileMesh = GeometryUtils.Extrude(
               bikewayTile.BikewayTileComponent.GetPlan(),
               Height
           );

            var tileSolidModel = CreateTileSolidModel(tileMesh);
            tileModel.Children.Add(tileSolidModel);

            if (DoesShowWireframe)
            {
                var tileWireframeModel = CreateTileWireframeModel(tileMesh);
                tileModel.Children.Add(tileWireframeModel);
            }

            return tileModel;
        }

        private Model3D CreateTileSolidModel(Mesh tileMesh)
        {
            var tileSolidGeometry =
                Viewport3DUtils.ConvertToGeometry3D(tileMesh);

            var tileSolidModel = new GeometryModel3D
            {
                Geometry = tileSolidGeometry,
                Material = SolidMaterial
            };

            return tileSolidModel;
        }

        private Model3D CreateTileWireframeModel(Mesh tileMesh)
        {
            var tileWireframeGeometry =
                Viewport3DUtils.CreateWireframeGeometry3D(
                    tileMesh, WireframeThickness
                );

            var tileWireframeModel = new GeometryModel3D
            {
                Geometry = tileWireframeGeometry,
                Material = WireframeMaterial
            };

            return tileWireframeModel;
        }

        #endregion

        #region Properties

        public BicycleParkingAreaModel3DBuilder
            BicycleParkingAreaModel3DBuilder
        { get; } = new BicycleParkingAreaModel3DBuilder();

        public bool DoesShowWireframe { get; set; } =
            DefaultDoesShowWireframe;

        public double Height { get; set; } = DefaultHeight;

        public double WireframeThickness { get; set; } =
            DefaultWireframeThickness;

        public Brush SolidBrush { get; set; } = DefaultSolidBrush;

        public Brush WireframeBrush { get; set; } = DefaultWireframeBrush;

        public Material SolidMaterial => new DiffuseMaterial
        {
            Brush = SolidBrush
        };

        public Material WireframeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(WireframeBrush);

        #endregion

        #region Constants

        public const bool DefaultDoesShowWireframe = false;

        public const double DefaultHeight = 0.01;
        public const double DefaultWireframeThickness = 0.03;

        public static readonly Brush DefaultSolidBrush = Brushes.Gainsboro;
        public static readonly Brush DefaultWireframeBrush = Brushes.DimGray;

        #endregion
    }
}
