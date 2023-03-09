using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.DrivewayTile"/>.
    /// </summary>
    public class DrivewayTileModel3DBuilder
    {
        #region Constructors

        public DrivewayTileModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(DrivewayTile drivewayTile)
        {
            var model = new Model3DGroup
            {
                Transform = Viewport3DUtils.ConvertToTransform3D(
                    drivewayTile.TransformComponent.Transform
                )
            };

            var tileModel = CreateTileModel(drivewayTile);
            model.Children.Add(tileModel);

            var leftCpas = drivewayTile.DrivewayTileComponent
                .LeftCarParkingAreaAnchorComponent
                .CarParkingAreas.Values;

            foreach (CarParkingArea cpa in leftCpas)
            {
                var cpaModel = CarParkingAreaModel3DBuilder.CreateModel(cpa);
                model.Children.Add(cpaModel);
            }

            var rightCpas = drivewayTile.DrivewayTileComponent
                .RightCarParkingAreaAnchorComponent
                .CarParkingAreas.Values;

            foreach (CarParkingArea cpa in rightCpas)
            {
                var cpaModel = CarParkingAreaModel3DBuilder.CreateModel(cpa);
                model.Children.Add(cpaModel);
            }

            return model;
        }

        private Model3D CreateTileModel(DrivewayTile drivewayTile)
        {
            var tileModel = new Model3DGroup();

            var tileMesh = GeometryUtils.Extrude(
               drivewayTile.DrivewayTileComponent.GetPlan(),
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

        public CarParkingAreaModel3DBuilder CarParkingAreaModel3DBuilder
        { get; } = new CarParkingAreaModel3DBuilder();

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
