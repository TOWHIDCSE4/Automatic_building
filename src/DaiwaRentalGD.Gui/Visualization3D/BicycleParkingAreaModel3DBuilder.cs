using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.BicycleParkingArea"/>.
    /// </summary>
    public class BicycleParkingAreaModel3DBuilder
    {
        #region Constructors

        public BicycleParkingAreaModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(BicycleParkingArea cpa)
        {
            var bpaTransform = new Transform3DGroup
            {
                Children =
                {
                    Viewport3DUtils.ConvertToTransform3D(
                        cpa.BicycleParkingAreaComponent.GetTransform()
                    ),
                    Viewport3DUtils.ConvertToTransform3D(
                        cpa.TransformComponent.Transform
                    )
                }
            };

            var bpaPlanSolidModel = CreatePlanSolidModel(cpa);

            var bpaPlanWireframeModel = CreatePlanWireframeModel(cpa);

            var bpaMarkModel = CreateMarkModel(cpa);

            Model3DGroup bpaModel = new Model3DGroup
            {
                Transform = bpaTransform,
                Children =
                {
                    bpaPlanSolidModel,
                    bpaPlanWireframeModel,
                    bpaMarkModel
                }
            };

            return bpaModel;
        }

        private Model3D CreatePlanSolidModel(BicycleParkingArea bpa)
        {
            var bpaPlan = bpa.BicycleParkingAreaComponent.GetPlan();

            var bpaPlanMesh = GeometryUtils.Extrude(
                bpaPlan, BicycleParkingAreaHeight
            );
            var bpaPlanGeometry =
                Viewport3DUtils.ConvertToGeometry3D(bpaPlanMesh);

            var bpaPlanModel = new GeometryModel3D
            {
                Geometry = bpaPlanGeometry,
                Material = BicycleParkingAreaSolidMaterial,
            };

            return bpaPlanModel;
        }

        public Model3D CreatePlanWireframeModel(BicycleParkingArea bpa)
        {
            var bpaPlan = bpa.BicycleParkingAreaComponent.GetPlan();

            var bpaPlanMesh = GeometryUtils.Extrude(
                bpaPlan, BicycleParkingAreaHeight
            );

            var bpaPlanWireframeGeometry =
                Viewport3DUtils.CreateWireframeGeometry3D(
                    bpaPlanMesh, BicycleParkingAreaWireframeThickness
                );

            var bpaPlanWireframeModel = new GeometryModel3D
            {
                Geometry = bpaPlanWireframeGeometry,
                Material = BicycleParkingAreaWireframeMaterial
            };

            return bpaPlanWireframeModel;
        }

        public Model3D CreateMarkModel(BicycleParkingArea bpa)
        {
            double bpaWidth = bpa.BicycleParkingAreaComponent.Width;
            double bpaLength = bpa.BicycleParkingAreaComponent.Length;

            string bpaMarkText = GetMarkText(bpa);

            TextBlock bpaMarkTextBlock = new TextBlock
            {
                Text = bpaMarkText,
                FontSize = BicycleParkingAreaTextFontSize,
                Foreground = BicycleParkingAreaTextFillBrush
            };

            Model3D bpaMarkModel = Viewport3DUtils.CreateTextModel3D(
                bpaMarkTextBlock,
                bpaLength * BicycleParkingAreaTextScale,
                // Use bpaLength * BicycleParkingAreaTextScale
                // so that the text will not scale with the width
                bpaLength * BicycleParkingAreaTextScale,
                out double actualSizeX,
                out double actualSizeY
            );

            double bpaMarkOffsetX = (bpaWidth - actualSizeX) / 2.0;
            double bpaMarkOffsetY = (bpaLength - actualSizeY) / 2.0;

            bpaMarkModel.Transform = new TranslateTransform3D
            {
                OffsetX = bpaMarkOffsetX,
                OffsetY = bpaMarkOffsetY,
                OffsetZ = BicycleParkingAreaTextOffsetZ
            };

            return bpaMarkModel;
        }

        private string GetMarkText(BicycleParkingArea bpa)
        {
            string bpaMarkText = string.Format(
                "B x{0,2}",
                bpa.BicycleParkingAreaComponent.NumOfSpaces
            );

            return bpaMarkText;
        }

        #endregion

        #region Properties

        public double BicycleParkingAreaHeight { get; set; } =
            DefaultBicycleParkingAreaHeight;

        public double BicycleParkingAreaWireframeThickness { get; set; } =
            DefaultBicycleParkingAreaWireframeThickness;

        public double BicycleParkingAreaTextOffsetZ =>
            BicycleParkingAreaHeight + BicycleParkingAreaWireframeThickness;

        public double BicycleParkingAreaTextFontSize =>
            DefaultBicycleParkingAreaTextFontSize;

        public double BicycleParkingAreaTextScale =>
            DefaultBicycleParkingAreaTextScale;

        public Brush BicycleParkingAreaSolidBrush { get; set; } =
            DefaultBicycleParkingAreaSolidBrush;

        public Brush BicycleParkingAreaWireframeBrush { get; set; } =
            DefaultBicycleParkingAreaWireframeBrush;

        public Brush BicycleParkingAreaTextFillBrush { get; set; } =
            DefaultBicycleParkingAreaTextFillBrush;

        public Material BicycleParkingAreaSolidMaterial =>
            new DiffuseMaterial
            {
                Brush = BicycleParkingAreaSolidBrush
            };

        public Material BicycleParkingAreaWireframeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(
                BicycleParkingAreaWireframeBrush
            );

        #endregion

        #region Constants

        public const double DefaultBicycleParkingAreaHeight = 0.01;

        public const double DefaultBicycleParkingAreaWireframeThickness =
            0.03;

        public const double DefaultBicycleParkingAreaTextFontSize = 2.0;

        public const double DefaultBicycleParkingAreaTextScale = 0.8;

        public static readonly Brush DefaultBicycleParkingAreaSolidBrush =
            Brushes.Azure;

        public static readonly Brush DefaultBicycleParkingAreaWireframeBrush =
            Brushes.SlateGray;

        public static readonly Brush DefaultBicycleParkingAreaTextFillBrush =
            Brushes.SlateGray;

        #endregion
    }
}
