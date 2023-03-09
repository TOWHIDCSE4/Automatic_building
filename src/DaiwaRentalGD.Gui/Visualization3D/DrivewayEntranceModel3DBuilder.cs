using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.DrivewayEntrance"/>.
    /// </summary>
    public class DrivewayEntranceModel3DBuilder
    {
        #region Constructors

        public DrivewayEntranceModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(DrivewayEntrance drivewayEntrance)
        {
            var drivewayEntranceModel = new Model3DGroup
            {
                Transform = Viewport3DUtils.ConvertToTransform3D(
                    drivewayEntrance.DrivewayEntranceComponent.Transform
                )
            };

            var markModel = CreateMarkModel();

            drivewayEntranceModel.Children.Add(markModel);

            return drivewayEntranceModel;
        }

        private Model3D CreateMarkModel()
        {
            var markGeometry = GetMarkGeometry();

            GeometryModel3D markModel = new GeometryModel3D
            {
                Geometry = markGeometry,
                Material = MarkSolidMaterial
            };

            return markModel;
        }

        private Geometry3D GetMarkGeometry()
        {
            var markPolygon = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(-0.4, -1.0, 0.0),
                    new Point(0.4, -1.0, 0.0)
                }
            );
            markPolygon.Transform(
                new TrsTransform3D
                {
                    Sx = MarkScale,
                    Sy = MarkScale,
                    Sz = MarkScale
                }
            );
            var markMesh = GeometryUtils.Extrude(markPolygon, MarkHeight);

            var markGeometry = Viewport3DUtils.ConvertToGeometry3D(markMesh);
            return markGeometry;
        }

        #endregion

        #region Properties

        public double MarkScale { get; set; } = DefaultMarkScale;

        public double MarkHeight { get; set; } = DefaultMarkHeight;

        public Brush MarkSolidBrush { get; set; } = DefaultMarkSolidBrush;

        public Material MarkSolidMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(MarkSolidBrush);

        #endregion

        #region Constants

        public const double DefaultMarkScale = 1.0;

        public const double DefaultMarkHeight = 0.01;

        public static readonly Brush DefaultMarkSolidBrush =
            Brushes.LimeGreen;

        #endregion
    }
}
