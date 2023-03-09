using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.Roof"/>.
    /// </summary>
    public class RoofModel3DBuilder
    {
        #region Constructors

        public RoofModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(Roof roof)
        {
            var roofMesh = roof.RoofComponent.GetMesh();
            var roofModel = CreateModel(roofMesh);

            var roofTransform = Viewport3DUtils.ConvertToTransform3D(
                roof.TransformComponent.Transform
            );
            roofModel.Transform = roofTransform;

            return roofModel;
        }

        private Model3D CreateModel(Mesh roofMesh)
        {
            var roofSolidGeometry =
                Viewport3DUtils.ConvertToGeometry3D(roofMesh);

            var roofSolidModel = new GeometryModel3D
            {
                Geometry = roofSolidGeometry,
                Material = SolidMaterial
            };

            var roofWireframeGeometry =
                Viewport3DUtils.CreateWireframeGeometry3D(
                    roofMesh, WireframeThickness
                );

            var roofWireframeModel = new GeometryModel3D
            {
                Geometry = roofWireframeGeometry,
                Material = WireframeMaterial
            };

            var roofModel = new Model3DGroup
            {
                Children =
                {
                    roofSolidModel,
                    roofWireframeModel
                }
            };

            return roofModel;
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
            Brushes.White;

        public static readonly Brush DefaultWireframeBrush =
            Brushes.DimGray;

        #endregion
    }
}
