using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.CarParkingArea"/>.
    /// </summary>
    public class CarParkingAreaModel3DBuilder
    {
        #region Constructors

        public CarParkingAreaModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(CarParkingArea cpa)
        {
            var cpaModel = new Model3DGroup
            {
                Transform = new Transform3DGroup
                {
                    Children =
                    {
                        Viewport3DUtils.ConvertToTransform3D(
                            cpa.CarParkingAreaComponent.GetTransform()
                        ),
                        Viewport3DUtils.ConvertToTransform3D(
                            cpa.TransformComponent.Transform
                        )
                    }
                }
            };

            for (int spaceIndex = 0;
                spaceIndex < cpa.CarParkingAreaComponent.NumOfSpaces;
                ++spaceIndex)
            {
                var spaceModel = CreateSpaceModel(cpa, spaceIndex);

                cpaModel.Children.Add(spaceModel);
            }

            return cpaModel;
        }

        private Model3D CreateSpaceModel(CarParkingArea cpa, int spaceIndex)
        {
            var spacePlanModel = CreateSpacePlanSolidModel(cpa);

            var spacePlanWireframeModel = CreateSpacePlanWireframeModel(cpa);

            var spaceMarkModel = CreateSpaceMarkModel(cpa);

            var spaceTransform = Viewport3DUtils.ConvertToTransform3D(
                cpa.CarParkingAreaComponent.GetSpaceTransform(spaceIndex)
            );

            Model3DGroup spaceModel = new Model3DGroup
            {
                Children =
                {
                    spacePlanModel,
                    spacePlanWireframeModel,
                    spaceMarkModel
                },
                Transform = spaceTransform
            };

            return spaceModel;
        }

        private Model3D CreateSpacePlanSolidModel(CarParkingArea cpa)
        {
            var spacePlan = cpa.CarParkingAreaComponent.GetSpacePlan();

            var spacePlanMesh = GeometryUtils.Extrude(
                spacePlan, CarParkingSpaceHeight
            );

            var spacePlanGeometry =
                Viewport3DUtils.ConvertToGeometry3D(spacePlanMesh);

            var spacePlanModel = new GeometryModel3D
            {
                Geometry = spacePlanGeometry,
                Material = CarParkingSpaceSolidMaterial,
            };

            return spacePlanModel;
        }

        public Model3D CreateSpacePlanWireframeModel(CarParkingArea cpa)
        {
            var spacePlan = cpa.CarParkingAreaComponent.GetSpacePlan();

            var spacePlanMesh = GeometryUtils.Extrude(
                spacePlan, CarParkingSpaceHeight
            );

            var spacePlanWireframeGeometry =
                Viewport3DUtils.CreateWireframeGeometry3D(
                    spacePlanMesh, CarParkingSpaceWireframeThickness
                );

            var spacePlanWireframeModel = new GeometryModel3D
            {
                Geometry = spacePlanWireframeGeometry,
                Material = CarParkingSpaceWireframeMaterial
            };

            return spacePlanWireframeModel;
        }

        private Model3D CreateSpaceMarkModel(CarParkingArea cpa)
        {
            double cpsWidth = cpa.CarParkingAreaComponent.SpaceWidth;
            double cpsLength = cpa.CarParkingAreaComponent.SpaceLength;

            TextBlock spaceMarkTextBlock = new TextBlock
            {
                Text = "P",
                FontSize = CarParkingSpaceTextFontSize,
                FontWeight = FontWeights.Bold,
                Foreground = CarParkingSpaceTextFillBrush
            };

            Model3D spaceMarkModel = Viewport3DUtils.CreateTextModel3D(
                spaceMarkTextBlock,
                cpsWidth * CarParkingSpaceTextScale,
                cpsLength * CarParkingSpaceTextScale,
                out double actualSizeX,
                out double actualSizeY
            );

            double spaceMarkOffsetX = (cpsWidth - actualSizeX) / 2.0;
            double spaceMarkOffsetY = (cpsLength - actualSizeY) / 2.0;

            spaceMarkModel.Transform = new TranslateTransform3D
            {
                OffsetX = spaceMarkOffsetX,
                OffsetY = spaceMarkOffsetY,
                OffsetZ = CarParkingSpaceTextOffsetZ
            };

            return spaceMarkModel;
        }

        #endregion

        #region Properties

        public double CarParkingSpaceHeight { get; set; } =
            DefaultCarParkingSpaceHeight;

        public double CarParkingSpaceWireframeThickness { get; set; } =
            DefaultCarParkingSpaceWireframeThickness;

        public double CarParkingSpaceTextOffsetZ =>
            CarParkingSpaceHeight + CarParkingSpaceWireframeThickness;

        public double CarParkingSpaceTextFontSize { get; set; } =
            DefaultCarParkingSpaceTextFontSize;

        public double CarParkingSpaceTextScale { get; set; } =
            DefaultCarParkingSpaceTextScale;

        public Brush CarParkingSpaceSolidBrush { get; set; } =
            DefaultCarParkingSpaceSolidBrush;

        public Brush CarParkingSpaceWireframeBrush { get; set; } =
            DefaultCarParkingSpaceWireframeBrush;

        public Brush CarParkingSpaceTextFillBrush { get; set; } =
            DefaultCarParkingSpaceTextFillBrush;

        public Material CarParkingSpaceSolidMaterial => new DiffuseMaterial
        {
            Brush = CarParkingSpaceSolidBrush
        };

        public Material CarParkingSpaceWireframeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(
                CarParkingSpaceWireframeBrush
            );

        #endregion

        #region Constants

        public const double DefaultCarParkingSpaceHeight = 0.01;

        public const double DefaultCarParkingSpaceWireframeThickness = 0.03;

        public const double DefaultCarParkingSpaceTextFontSize = 2.0;

        public const double DefaultCarParkingSpaceTextScale = 0.6;

        public static readonly Brush DefaultCarParkingSpaceSolidBrush =
            Brushes.LightGray;

        public static readonly Brush DefaultCarParkingSpaceWireframeBrush =
            Brushes.SlateGray;

        public static readonly Brush DefaultCarParkingSpaceTextFillBrush =
            Brushes.White;

        #endregion
    }
}
