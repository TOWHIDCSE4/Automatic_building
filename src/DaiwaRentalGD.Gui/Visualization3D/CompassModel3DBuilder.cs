using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// a compass to indicate
    /// <see cref="DaiwaRentalGD.Model.SiteDesign
    /// .SiteComponent.TrueNorthDirection"/>.
    /// </summary>
    public class CompassModel3DBuilder
    {
        #region Constructors

        public CompassModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(Site site)
        {
            var compassTransform = GetCompassTransform(site);

            var yAxisLabeledArrowModel =
                CreateYAxisLabeledArrowModel();

            var trueNorthLabeledArrowModel =
                CreateTrueNorthLabeledArrowModel(site);

            var compassModel = new Model3DGroup
            {
                Transform = compassTransform,
                Children =
                {
                    yAxisLabeledArrowModel,
                    trueNorthLabeledArrowModel
                }
            };

            return compassModel;
        }

        private Transform3D GetCompassTransform(Site site)
        {
            var scBbox = site.SiteComponent.GetBBox();

            double compassOffsetX = scBbox.MinX - LabeledArrowDiameter;
            double compassOffsetY = scBbox.MinY;

            var compassTransform = new TranslateTransform3D
            {
                OffsetX = compassOffsetX,
                OffsetY = compassOffsetY
            };

            return compassTransform;
        }

        private Model3D CreateYAxisLabeledArrowModel()
        {
            var labeledArrowTransform =
                GetLabeledArrowTransform(0.0, YAxisLabeledArrowOffsetZ);

            var arrowModel = CreateArrowModel(YAxisArrowFillMaterial);

            var labelModel = CreateLabelModel(
                "+Y",
                LabelFontSize, LabelWidth, LabelHeight,
                YAxisLabelFillBrush
            );

            var labeledArrowModel = new Model3DGroup
            {
                Transform = labeledArrowTransform,
                Children =
                {
                    arrowModel,
                    labelModel
                }
            };

            return labeledArrowModel;
        }

        private Model3D CreateTrueNorthLabeledArrowModel(Site site)
        {
            double trueNorthAngle = site.SiteComponent.TrueNorthAngle;

            var labeledArrowTransform = GetLabeledArrowTransform(
                trueNorthAngle, TrueNorthLabeledArrowOffsetZ
            );

            var arrowModel = CreateArrowModel(TrueNorthArrowFillMaterial);

            var labelModel = CreateLabelModel(
                "N",
                LabelFontSize, LabelWidth, LabelHeight,
                TrueNorthLabelFillBrush
            );

            var labeledArrowModel = new Model3DGroup
            {
                Transform = labeledArrowTransform,
                Children =
                {
                    arrowModel,
                    labelModel
                }
            };

            return labeledArrowModel;
        }

        private Model3D CreateArrowModel(Material arrowFillMaterial)
        {
            var arrowModel = new GeometryModel3D
            {
                Geometry = GetArrowGeometry(),
                Material = arrowFillMaterial
            };

            return arrowModel;
        }

        private Model3D CreateLabelModel(
            string labelText, double fontSize,
            double labelWidth, double labelHeight,
            Brush labelFillBrush
        )
        {
            var labelTextBlock = new TextBlock
            {
                Text = labelText,
                FontSize = fontSize,
                FontWeight = System.Windows.FontWeights.Black,
                Foreground = labelFillBrush
            };

            var labelModel = Viewport3DUtils.CreateTextModel3D(
                labelTextBlock, labelWidth, labelHeight,
                out double actualLabelWidth, out double actualLabelHeight
            );

            labelModel.Transform = GetLabelTransform(
                actualLabelWidth, actualLabelHeight
            );

            return labelModel;
        }

        private Geometry3D GetArrowGeometry()
        {
            Mesh arrowMesh = GeometryUtils.CovnertToMesh(
                new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(ArrowWidth / 2.0, ArrowWidth / 4.0, 0.0),
                        new Point(ArrowWidth, 0.0, 0.0),
                        new Point(ArrowWidth / 2.0, ArrowLength, 0.0)
                    }
                )
            );

            var arrowGeometry =
                Viewport3DUtils.ConvertToGeometry3D(arrowMesh);

            return arrowGeometry;
        }

        private Transform3D GetLabelTransform(
            double actualLabelWidth, double actualLabelHeight
        )
        {
            var labelTransform = new TranslateTransform3D
            {
                OffsetX = (ArrowWidth - actualLabelWidth) / 2.0,
                OffsetY = ArrowLength
            };
            return labelTransform;
        }

        private Transform3D GetLabeledArrowTransform(
            double angle, double offsetZ
        )
        {
            var labeledArrowTransform = new Transform3DGroup
            {
                Children = new Transform3DCollection
                {
                    new RotateTransform3D
                    {
                        Rotation = new AxisAngleRotation3D
                        {
                            Axis = new Vector3D(0.0, 0.0, 1.0),
                            Angle = angle / Math.PI * 180.0
                        },
                        CenterX = ArrowWidth / 2.0,
                        CenterY = ArrowLength / 2.0
                    },
                    new TranslateTransform3D
                    {
                        OffsetX =
                            LabeledArrowDiameter / 2.0 - ArrowWidth / 2.0,
                        OffsetY =
                            LabeledArrowDiameter / 2.0 - ArrowLength / 2.0,
                        OffsetZ = offsetZ
                    }
                }
            };

            return labeledArrowTransform;
        }

        #endregion

        #region Properties

        public double YAxisLabeledArrowOffsetZ { get; set; } =
            DefaultYAxisLabeledArrowOffsetZ;

        public double TrueNorthLabeledArrowOffsetZ { get; set; } =
            DefaultTrueNorthLabeledArrowOffsetZ;

        public double ArrowWidth { get; set; } = DefaultArrowWidth;

        public double ArrowLength { get; set; } = DefaultArrowLength;

        public double LabelWidth { get; set; } = DefaultLabelWidth;

        public double LabelHeight { get; set; } = DefaultLabelHeight;

        public double LabelFontSize { get; set; } = DefaultLabelFontSize;

        public double LabeledArrowDiameter =>
            new DenseVector(
                new[]
                {
                    Math.Max(ArrowWidth, LabelWidth),
                    ArrowLength + LabelHeight * 2.0
                }
            ).L2Norm();

        public Brush YAxisArrowFillBrush { get; set; } =
            DefaultYAxisArrowFillBrush;

        public Brush YAxisLabelFillBrush { get; set; } =
            DefaultYAxisLabelFillBrush;

        public Brush TrueNorthArrowFillBrush { get; set; } =
             DefaultTrueNorthArrowFillBrush;

        public Brush TrueNorthLabelFillBrush { get; set; } =
            DefaultTrueNorthLabelFillBrush;

        public Material YAxisArrowFillMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(YAxisArrowFillBrush);

        public Material TrueNorthArrowFillMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(TrueNorthArrowFillBrush);

        #endregion

        #region Constants

        public const double DefaultYAxisLabeledArrowOffsetZ = 0.0;
        public const double DefaultTrueNorthLabeledArrowOffsetZ = 0.01;

        public const double DefaultArrowWidth = 1.0;
        public const double DefaultArrowLength = 3.0;

        public const double DefaultLabelWidth = 1.0;
        public const double DefaultLabelHeight = 1.0;
        public const double DefaultLabelFontSize = 1.0;

        public static readonly Brush DefaultYAxisArrowFillBrush =
            Brushes.Gainsboro;
        public static readonly Brush DefaultYAxisLabelFillBrush =
            Brushes.Gainsboro;

        public static readonly Brush DefaultTrueNorthArrowFillBrush =
            Brushes.DimGray;
        public static readonly Brush DefaultTrueNorthLabelFillBrush =
            Brushes.DimGray;

        #endregion
    }
}
