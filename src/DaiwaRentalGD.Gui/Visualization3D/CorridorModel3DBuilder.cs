using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.Corridor"/>.
    /// </summary>
    public class CorridorModel3DBuilder
    {
        #region Constructors

        public CorridorModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(Corridor corridor)
        {
            var corridorMesh = corridor.CorridorComponent.GetMesh();
            var corridorModel = CreateModel(corridorMesh);

            var corridorTransform = Viewport3DUtils.ConvertToTransform3D(
                corridor.TransformComponent.Transform
            );
            corridorModel.Transform = corridorTransform;

            return corridorModel;
        }

        private Model3D CreateModel(Mesh corridorMesh)
        {
            var corridorSolidGeometry =
                Viewport3DUtils.ConvertToGeometry3D(corridorMesh);

            var corridorSolidModel = new GeometryModel3D
            {
                Geometry = corridorSolidGeometry,
                Material = SolidMaterial
            };

            var corridorWireframeGeometry =
                Viewport3DUtils.CreateWireframeGeometry3D(
                    corridorMesh, WireframeThickness
                );

            var corridorWireframeModel = new GeometryModel3D
            {
                Geometry = corridorWireframeGeometry,
                Material = WireframeMaterial
            };

            var corridorModel = new Model3DGroup
            {
                Children =
                {
                    corridorSolidModel,
                    corridorWireframeModel
                }
            };

            return corridorModel;
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
            Brushes.LightSteelBlue;

        public static readonly Brush DefaultWireframeBrush =
            Brushes.DarkSlateGray;

        #endregion
    }
}
