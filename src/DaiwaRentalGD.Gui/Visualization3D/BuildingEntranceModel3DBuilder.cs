using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.BuildingEntrance"/>.
    /// </summary>
    public class BuildingEntranceModel3DBuilder
    {
        #region Constructors

        public BuildingEntranceModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(BuildingEntrance entrance)
        {
            var markModel = CreateMarkModel(entrance);
            
            var entranceModel = new Model3DGroup
            {
                Children =
                {
                    markModel
                }
            };

            return entranceModel;
        }

        private Model3D CreateMarkModel(BuildingEntrance entrance)
        {
            var markGeometry = GetMarkGeometry(entrance);

            GeometryModel3D markModel = new GeometryModel3D
            {
                Geometry = markGeometry,
                Material = MarkSolidMaterial
            };

            return markModel;
        }

        private Geometry3D GetMarkGeometry(BuildingEntrance entrance)
        {
            Mesh markMesh = GeometryUtils.Extrude(
                new Polygon(
                    new[]
                    {
                        new Point(-1.0, -0.2, 0.0),
                        new Point(0.0, 0.0, 0.0),
                        new Point(-1.0, 0.2, 0.0)
                    }
                ),
                MarkHeight
            );

            double entranceAngle = MathUtils.GetAngle2D(
                new DenseVector(new[] { 1.0, 0.0, 0.0 }),
                entrance.EntranceDirection
            );

            TrsTransform3D entranceTransform = new TrsTransform3D
            {
                Tx = entrance.EntrancePoint.X,
                Ty = entrance.EntrancePoint.Y,
                Tz = entrance.EntrancePoint.Z,
                Sx = MarkScale,
                Sy = MarkScale,
                Sz = MarkScale,
                Rz = entranceAngle
            };
            markMesh.Transform(entranceTransform);

            Geometry3D markGeometry =
                Viewport3DUtils.ConvertToGeometry3D(markMesh);

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

        public const double DefaultMarkHeight = 0.02;

        public static readonly Brush DefaultMarkSolidBrush = Brushes.Orange;

        #endregion
    }
}
