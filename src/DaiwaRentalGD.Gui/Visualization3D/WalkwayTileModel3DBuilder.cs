using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.WalkwayTile"/>.
    /// </summary>
    public class WalkwayTileModel3DBuilder
    {
        #region Constructors

        public WalkwayTileModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(WalkwayTile walkwayTile)
        {
            var walkwayTileModel = new Model3DGroup
            {
                Transform = Viewport3DUtils.ConvertToTransform3D(
                    walkwayTile.TransformComponent.Transform
                )
            };

            var walkwayTileMesh = GeometryUtils.Extrude(
                walkwayTile.WalkwayTileComponent.GetPlan(),
                Height
            );

            var walkwayTileSolidModel = CreateSolidModel(walkwayTileMesh);

            walkwayTileModel.Children.Add(walkwayTileSolidModel);

            if (DoesShowWireframe)
            {
                var walkwayTileWireframeModel =
                    CreateWireframeModel(walkwayTileMesh);

                walkwayTileModel.Children.Add(walkwayTileWireframeModel);
            }

            return walkwayTileModel;
        }

        private Model3D CreateSolidModel(Mesh walkwayTileMesh)
        {
            var walkwayTileSolidGeometry =
                Viewport3DUtils.ConvertToGeometry3D(walkwayTileMesh);

            var walkwayTileSolidModel = new GeometryModel3D
            {
                Geometry = walkwayTileSolidGeometry,
                Material = SolidMaterial
            };

            return walkwayTileSolidModel;
        }

        private Model3D CreateWireframeModel(Mesh walkwayTileMesh)
        {
            var walkwayTileWireframeGeometry =
                Viewport3DUtils.CreateWireframeGeometry3D(
                    walkwayTileMesh, WireframeThickness
                );

            var walkwayTileWireframeModel = new GeometryModel3D
            {
                Geometry = walkwayTileWireframeGeometry,
                Material = WireframeMaterial
            };

            return walkwayTileWireframeModel;
        }

        #endregion

        #region Properties

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
