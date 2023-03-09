using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.Staircase"/>.
    /// </summary>
    public class StaircaseModel3DBuilder
    {
        #region Constructors

        public StaircaseModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(Staircase staircase)
        {
            var staircaseMesh = staircase.StaircaseComponent.GetMesh();
            var staircaseModel = CreateModel(staircaseMesh);

            var staircaseTransform = Viewport3DUtils.ConvertToTransform3D(
                staircase.TransformComponent.Transform
            );
            staircaseModel.Transform = staircaseTransform;

            return staircaseModel;
        }

        private Model3D CreateModel(Mesh staircaseMesh)
        {
            var staircaseSolidGeometry =
                Viewport3DUtils.ConvertToGeometry3D(staircaseMesh);

            var staircaseSolidModel = new GeometryModel3D
            {
                Geometry = staircaseSolidGeometry,
                Material = SolidMaterial
            };

            var staircaseWireframeGeometry =
                Viewport3DUtils.CreateWireframeGeometry3D(
                    staircaseMesh, WireframeThickness
                );

            var staircaseWireframeModel = new GeometryModel3D
            {
                Geometry = staircaseWireframeGeometry,
                Material = WireframeMaterial
            };

            var staircaseModel = new Model3DGroup
            {
                Children =
                {
                    staircaseSolidModel,
                    staircaseWireframeModel
                }
            };

            return staircaseModel;
        }

        #endregion

        #region Properties

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

        public const double DefaultWireframeThickness = 0.03;

        public static readonly Brush DefaultSolidBrush =
            Brushes.SteelBlue;

        public static readonly Brush DefaultWireframeBrush =
            Brushes.DarkSlateGray;

        #endregion
    }
}
