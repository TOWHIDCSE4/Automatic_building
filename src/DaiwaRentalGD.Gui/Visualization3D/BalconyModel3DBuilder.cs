using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.Balcony"/>.
    /// </summary>
    public class BalconyModel3DBuilder
    {
        #region Constructors

        public BalconyModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(Balcony balcony)
        {
            var balconyMesh = balcony.BalconyComponent.GetMesh();
            var balconyModel = CreateModel(balconyMesh);

            var balconyTransform = Viewport3DUtils.ConvertToTransform3D(
                balcony.TransformComponent.Transform
            );
            balconyModel.Transform = balconyTransform;

            return balconyModel;
        }

        private Model3D CreateModel(Mesh balconyMesh)
        {
            var balconySolidGeometry =
                Viewport3DUtils.ConvertToGeometry3D(balconyMesh);

            var balconySolidModel = new GeometryModel3D
            {
                Geometry = balconySolidGeometry,
                Material = SolidMaterial
            };

            var balconyWireframeGeometry =
                Viewport3DUtils.CreateWireframeGeometry3D(
                    balconyMesh, WireframeThickness
                );

            var balconyWireframeModel = new GeometryModel3D
            {
                Geometry = balconyWireframeGeometry,
                Material = WireframeMaterial
            };

            var balconyModel = new Model3DGroup
            {
                Children =
                {
                    balconySolidModel,
                    balconyWireframeModel
                }
            };

            return balconyModel;
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
            Brushes.Salmon;

        public static readonly Brush DefaultWireframeBrush =
            Brushes.DarkRed;

        #endregion
    }
}
