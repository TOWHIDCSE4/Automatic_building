using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Gui.Utilities.Configs;
using DaiwaRentalGD.Model.BuildingDesign;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    public class UnitModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public UnitModelSVGBuilder()
        { }
        public UnitModelSVGBuilder(Unit unit, bool isShowedLabel = true) : base()
        {
            _unit = unit;
            CreateModel();

            if (isShowedLabel)
                CreateUnitLabel();
        }

        #endregion
        #region Methods

        public void CreateModel()
        {
            var unitTransform = _unit.TransformComponent.Transform;
            AddSvgTransforms(unitTransform);

            var unitComponent = _unit.UnitComponent;
            CreateRoomsModel(unitComponent);
        }
        private void CreateRoomsModel(UnitComponent uc)
        {
            var roomsModel = new ModelSVGBuilder();
            for (int roomIndex = 0; roomIndex < uc.RoomPlans.Count;
                ++roomIndex)
            {
                var roomPlan = uc.RoomPlans[roomIndex];
                roomsModel.AddChild(CreateSvgPath(roomPlan, RoomSolidBrush, RoomWireframeBrush));
            }
            AddChild(roomsModel);
        }

        private void CreateUnitLabel()
        {
            if (!(_unit.UnitComponent is CatalogUnitComponent cuc))
            {
                return;
            }

            var textGroup = CreateSvgText(cuc);

            // Get transforms.
            var trans = new Svg.Transforms.SvgTransformCollection();
            trans.Add(new Svg.Transforms.SvgTranslate((float)Configs.UnitBuilding.LabelOffsetX * svgRatio, (float)Configs.UnitBuilding.LabelOffsetY * svgRatio));
            trans.Add(new Svg.Transforms.SvgScale(1, -1));
            trans.Add(new Svg.Transforms.SvgRotate(90));
            AddSvgTransforms(textGroup, trans);

            // Add to parent.
            AddChild(textGroup);

        }

        private SvgText CreateSvgText(CatalogUnitComponent cuc)
        {
            var textGroup = new SvgText()
            {
                FontSize = 11
            };
            textGroup.Transforms = new Svg.Transforms.SvgTransformCollection();

            var lineHeight = new SvgUnitCollection();
            lineHeight.Add(new SvgUnit(SvgUnitType.Em, 1));

            var lineHeight2 = new SvgUnitCollection();
            lineHeight2.Add(new SvgUnit(SvgUnitType.Em, 2));

            var startX = new SvgUnitCollection();
            startX.Add(0);

            var tspan1 = new SvgTextSpan()
            {
                X = startX,
                Y = lineHeight,
                Text = cuc.EntryName.FullName
            };

            var tspan2 = new SvgTextSpan()
            {
                X = startX,
                Y = lineHeight2,
                Text = cuc.PlanName
            };

            textGroup.Children.Add(tspan1);
            textGroup.Children.Add(tspan2);

            return textGroup;
        }
        #endregion

        #region Properties
        private Unit _unit;
        public Color RoomSolidBrush { get; set; } =
         DefaultRoomSolidBrush;

        public Color RoomWireframeBrush { get; set; } =
            DefaultRoomWireframeBrush;
        #endregion

        #region Constants
        public static readonly Color DefaultRoomSolidBrush =
        Color.White;
        public static readonly Color DefaultRoomWireframeBrush =
            Color.DimGray;
        #endregion
    }
}
