using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.Unit"/>.
    /// </summary>
    public class UnitModel3DBuilder
    {
        #region Constructors

        public UnitModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3DGroup CreateModel(Unit unit)
        {
            Model3DGroup unitModel = new Model3DGroup
            {
                Transform = Viewport3DUtils.ConvertToTransform3D(
                    unit.TransformComponent.Transform
                )
            };

            Model3D roomsModel = CreateRoomsModel(unit);
            unitModel.Children.Add(roomsModel);

            if (DoesShowLabel)
            {
                Model3D labelModel = CreateLabelModel(unit);

                if (labelModel != null)
                {
                    unitModel.Children.Add(labelModel);
                }
            }

            return unitModel;
        }

        private Model3D CreateRoomsModel(Unit unit)
        {
            var uc = unit.UnitComponent;

            var roomsModel = new Model3DGroup();

            for (int roomIndex = 0; roomIndex < uc.RoomPlans.Count;
                ++roomIndex)
            {
                var roomMesh = uc.GetRoomMesh(roomIndex);
                var roomModel = CreateRoomModel(roomMesh);

                roomsModel.Children.Add(roomModel);
            }

            return roomsModel;
        }

        private Model3D CreateRoomModel(Mesh roomMesh)
        {
            var roomSolidModel = CreateRoomSolidModel(roomMesh);

            var roomWireframeModel = CreateRoomWireframeModel(roomMesh);

            var roomModel = new Model3DGroup
            {
                Children =
                {
                    roomSolidModel,
                    roomWireframeModel
                }
            };
            return roomModel;
        }

        private Model3D CreateRoomSolidModel(Mesh roomMesh)
        {
            Geometry3D roomSolidGeometry =
                Viewport3DUtils.ConvertToGeometry3D(roomMesh);

            GeometryModel3D roomSolidModel = new GeometryModel3D
            {
                Geometry = roomSolidGeometry,
                Material = RoomSolidMaterial

            };

            return roomSolidModel;
        }

        private Model3D CreateRoomWireframeModel(Mesh roomMesh)
        {
            Geometry3D roomWireframeGeometry =
                Viewport3DUtils.CreateWireframeGeometry3D(
                    roomMesh, WireframeThickness
                );

            GeometryModel3D roomWireframeModel = new GeometryModel3D
            {
                Geometry = roomWireframeGeometry,
                Material = RoomWireframeMaterial
            };

            return roomWireframeModel;
        }

        private Model3D CreateLabelModel(Unit unit)
        {
            if (!(unit.UnitComponent is CatalogUnitComponent cuc))
            {
                return null;
            }

            string labelText = GetLabelText(cuc);

            TextBlock labelTextBlock = new TextBlock
            {
                Text = labelText,
                FontSize = LabelFontSize,
                Foreground = LabelTextFillBrush,
            };

            double maxTextBlockSizeX = double.PositiveInfinity;
            double maxTextBlockSizeY = Math.Min(
                unit.UnitComponent.GetBBox().SizeX,
                LabelMaxSizeX
            );

            Model3D labelModel = Viewport3DUtils.CreateTextModel3D(
                labelTextBlock,
                maxTextBlockSizeX, maxTextBlockSizeY,
                out double actualTextBlockSizeX,
                out double actualTextBlockSizeY
            );

            double buildingRotationZ =
                (unit.Parent as Building)?
                .TransformComponent.Transform.Rz ?? 0.0;

            double labelRotation = buildingRotationZ < 0.0 ? 180.0 : 0.0;

            var labelTransform = new Transform3DGroup
            {
                Children = new Transform3DCollection
                {
                    new RotateTransform3D(
                        new AxisAngleRotation3D(
                            new Vector3D(0.0, 0.0, 1.0),
                            labelRotation
                        ),
                        actualTextBlockSizeX / 2.0,
                        actualTextBlockSizeY / 2.0,
                        0.0
                    ),
                    new RotateTransform3D(
                        new AxisAngleRotation3D(
                            new Vector3D(0.0, 0.0, 1.0),
                            -90.0
                        )
                    ),
                    new TranslateTransform3D(
                        0.0, LabelOffsetY, LabelOffsetZ
                    )
                }
            };

            labelModel.Transform = labelTransform;

            return labelModel;
        }

        private string GetLabelText(CatalogUnitComponent cuc)
        {
            string labelText = string.Format(
                "{0}\n({1})",
                cuc.EntryName.FullName,
                cuc.PlanName
            );

            return labelText;
        }

        #endregion

        #region Properties

        public bool DoesShowLabel { get; set; } = DefaultDoesShowLabel;

        public double WireframeThickness { get; set; } =
            DefaultWireframeThickness;

        public double LabelOffsetY { get; set; } = DefaultLabelOffsetY;

        public double LabelOffsetZ { get; set; } = DefaultLabelOffsetZ;

        public double LabelFontSize { get; set; } = DefaultLabelFontSize;

        public double LabelMaxSizeX { get; set; } = DefaultLabelMaxSizeX;

        public Brush RoomSolidBrush { get; set; } =
            DefaultRoomSolidBrush;

        public Brush RoomWireframeBrush { get; set; } =
            DefaultRoomWireframeBrush;

        public Brush LabelTextFillBrush { get; set; } =
            DefaultLabelTextFillBrush;

        public Material RoomSolidMaterial => new DiffuseMaterial
        {
            Brush = RoomSolidBrush
        };

        public Material RoomWireframeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(RoomWireframeBrush);

        #endregion

        #region Constants

        public const bool DefaultDoesShowLabel = true;

        public const double DefaultWireframeThickness = 0.03;

        public const double DefaultLabelOffsetY = -2.0;
        public const double DefaultLabelOffsetZ = 0.1;
        public const double DefaultLabelFontSize = 1.0;
        public const double DefaultLabelMaxSizeX = 2.0;

        public static readonly Brush DefaultRoomSolidBrush =
            Brushes.White;
        public static readonly Brush DefaultRoomWireframeBrush =
            Brushes.DimGray;
        public static readonly Brush DefaultLabelTextFillBrush =
            Brushes.DimGray;

        #endregion
    }
}
