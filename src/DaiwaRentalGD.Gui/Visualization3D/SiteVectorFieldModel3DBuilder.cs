using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ParkingLotDesign;
using MathNet.Numerics.LinearAlgebra;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.SiteVectorField"/>.
    /// </summary>
    public class SiteVectorFieldModel3DBuilder
    {
        #region Constructors

        public SiteVectorFieldModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(ParkingLot parkingLot)
        {
            var svfModel = new Model3DGroup
            {
                Transform = new TranslateTransform3D
                {
                    OffsetX = 0.0,
                    OffsetY = 0.0,
                    OffsetZ = VectorFieldOffsetZ
                }
            };

            var svf = parkingLot.ParkingLotComponent.SiteVectorField;

            var site = parkingLot.ParkingLotComponent.Site;
            var siteBoundary = site.SiteComponent.Boundary;
            var siteBbox = siteBoundary.GetBBox();

            for (double sampleX = siteBbox.MinX;
                sampleX <= siteBbox.MaxX;
                sampleX += VectorSampleStep)
            {
                for (double sampleY = siteBbox.MinY;
                    sampleY <= siteBbox.MaxY;
                    sampleY += VectorSampleStep)
                {
                    var samplePoint = new Point(sampleX, sampleY, 0.0);

                    if (!siteBoundary.IsPointInside2D(samplePoint))
                    {
                        continue;
                    }

                    var vectorPair = svf.GetVectorPair(site, samplePoint);

                    var vectorModel =
                        CreateVectorModel(samplePoint, vectorPair);

                    svfModel.Children.Add(vectorModel);
                }
            }

            return svfModel;
        }

        public Model3D CreateVectorModel(
            Point point, Tuple<Vector<double>, Vector<double>> vectorPair
        )
        {
            double vectorLineLength = VectorSampleStep * VectorLineLengthScale;

            var lineSegment0 = new Geometries.LineSegment(
                new Point(-vectorPair.Item1 * vectorLineLength * 0.5),
                new Point(vectorPair.Item1 * vectorLineLength * 0.5)
            );
            var lineSegment1 = new Geometries.LineSegment(
                new Point(-vectorPair.Item2 * vectorLineLength * 0.5),
                new Point(vectorPair.Item2 * vectorLineLength * 0.5)
            );

            var vectorGeometry = Viewport3DUtils.CreateLinesGeometry3D(
                new[] { lineSegment0, lineSegment1 },
                VectorLineThickness
            );

            var vectorModel = new GeometryModel3D
            {
                Geometry = vectorGeometry,
                Material = VectorLineMaterial,
                Transform = new TranslateTransform3D(point.X, point.Y, 0.0)
            };

            return vectorModel;
        }

        #endregion

        #region Properties

        public double VectorFieldOffsetZ { get; set; } =
            DefaultVectorFieldOffsetZ;

        public double VectorSampleStep { get; set; } =
            DefaultVectorSampleStep;

        public double VectorLineThickness { get; set; } =
            DefaultVectorLineThickness;

        public double VectorLineLengthScale { get; set; } =
            DefaultVectorLineLengthScale;

        public Brush VectorLineBrush { get; set; } =
            DefaultVectorLineBrush;

        public Material VectorLineMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(VectorLineBrush);

        #endregion

        #region Constants

        public const double DefaultVectorFieldOffsetZ = 0.1;
        public const double DefaultVectorSampleStep = 2.5;
        public const double DefaultVectorLineLengthScale = 0.3;
        public const double DefaultVectorLineThickness = 0.03;

        public static readonly Brush DefaultVectorLineBrush =
            Brushes.DeepPink;

        #endregion
    }
}
