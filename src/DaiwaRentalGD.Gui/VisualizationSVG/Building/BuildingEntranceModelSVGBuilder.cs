using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Drawing;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.BuildingEntrance"/>.
    /// </summary>
    public class BuildingEntranceModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public BuildingEntranceModelSVGBuilder()
        { }
        public BuildingEntranceModelSVGBuilder(BuildingEntrance entrance)
        {
            _entrance = entrance;
            CreateModel();
        }
        #endregion

        #region Methods

        public void  CreateModel()
        {
            double entranceAngle = MathUtils.GetAngle2D(
                   new DenseVector(new[] { 1.0, 0.0, 0.0 }),
                   _entrance.EntranceDirection
               );

            TrsTransform3D entranceTransform = new TrsTransform3D
            {
                Tx = _entrance.EntrancePoint.X,
                Ty = _entrance.EntrancePoint.Y,
                Tz = _entrance.EntrancePoint.Z,
                Sx = MarkScale,
                Sy = MarkScale,
                Sz = MarkScale,
                Rz = entranceAngle
            };
            AddSvgTransforms(entranceTransform);
            var entrancePlan = new Polygon(
                    new[]
                    {
                        new Geometries.Point(-1.0, -0.2, 0.0),
                        new Geometries.Point(0.0, 0.0, 0.0),
                        new Geometries.Point(-1.0, 0.2, 0.0)
                    }
                );
            var entranceModel = CreateSvgPath(entrancePlan, DefaultMarkSolidBrush, DefaultMarkSolidBrush);
            AddChild(entranceModel);
        }
        #endregion

        #region Properties

        private BuildingEntrance _entrance;
        public double MarkScale { get; set; } = DefaultMarkScale;
        #endregion

        #region Constants
        public const double DefaultMarkScale = 1.0;
        public static readonly Color DefaultMarkSolidBrush = Color.Orange;
        #endregion
    }
}
