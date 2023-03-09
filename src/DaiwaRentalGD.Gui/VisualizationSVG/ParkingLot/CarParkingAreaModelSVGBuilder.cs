using System;
using System.Drawing;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Gui.Utilities.Configs;
using DaiwaRentalGD.Gui.Visualization3D;
using DaiwaRentalGD.Model.ParkingLotDesign;
using DaiwaRentalGD.Model.SiteDesign;
using Svg;
using Svg.Transforms;

namespace DaiwaRentalGD.Gui.VisualizationSVG.ParkingLot
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.Site"/>.
    /// </summary>
    public class CarParkingAreaModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public CarParkingAreaModelSVGBuilder()
        { }
        public CarParkingAreaModelSVGBuilder(CarParkingArea carParkingArea) : base()
        {
            _carParkingArea = carParkingArea;
            CreateModel();
        }
        #endregion

        #region Methods        

        private void CreateModel()
        {
            var areaTransform = _carParkingArea.TransformComponent.Transform;
            var areaComponentTransform = _carParkingArea.CarParkingAreaComponent.GetTransform();
            AddSvgTransforms(areaTransform);
            AddSvgTransforms(areaComponentTransform);

            for (int spaceIndex = 0; spaceIndex < _carParkingArea.CarParkingAreaComponent.NumOfSpaces; ++spaceIndex)
            {
                CreateModelSpace(spaceIndex);
            }
        }

        private void CreateModelSpace(int spaceIndex)
        {
            var spaceGroup = new SvgGroup();
            var spacePlan = _carParkingArea.CarParkingAreaComponent.GetSpacePlan();

            // Create space
            var space = CreateSvgPath(spacePlan, _carParkingSpaceBackgroundColor, _carParkingSpaceStroke);
            AddChild(spaceGroup, space);

            // Transform for space
            var tranform = _carParkingArea.CarParkingAreaComponent.GetSpaceTransform(spaceIndex);
            spaceGroup.Transforms = new SvgTransformCollection();
            AddSvgTransforms(spaceGroup, ConvertToSvgTransforms(tranform));

            // Create space mark(label).
            var textGroup = CreateSvgText("P", 30, Configs.CarParkingArea.CarParkingSpaceTextBgColor);
            textGroup.FontWeight = SvgFontWeight.Bold;
            textGroup.CustomAttributes.Add("alignment-baseline", "middle");

            var textTrans = new Svg.Transforms.SvgTransformCollection();
            double centerX = _carParkingArea.CarParkingAreaComponent.SpaceWidth / 2 * svgRatio;
            double centerY = _carParkingArea.CarParkingAreaComponent.SpaceLength / 2 * svgRatio * 0.9;
            textTrans.Add(new SvgTranslate((float)centerX, (float)centerY));
            textTrans.Add(new SvgScale(1, -1.8f));

            AddSvgTransforms(textGroup, textTrans);
            AddChild(spaceGroup, textGroup);

            // Add space group to parent.
            AddChild(spaceGroup);
        }


        #endregion

        #region Properties
        private CarParkingArea _carParkingArea;
        private Color _carParkingSpaceBackgroundColor = Color.LightGray;
        private Color _carParkingSpaceStroke = Color.SlateGray;
        private Color _carParkingSpaceTextColor = Color.White;
        const float CarParkingSpaceTextFontSize = 32f;
        #endregion

        #region Constants

        #endregion
    }
}
