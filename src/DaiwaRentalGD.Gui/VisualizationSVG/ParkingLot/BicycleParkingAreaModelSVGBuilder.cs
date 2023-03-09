using System;
using System.Drawing;
using DaiwaRentalGD.Gui.Utilities.Configs;
using DaiwaRentalGD.Model.ParkingLotDesign;
using Svg;
using Svg.Transforms;

namespace DaiwaRentalGD.Gui.VisualizationSVG.ParkingLot
{
    /// <summary>
    /// </summary>
    public class BicycleParkingAreaSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public BicycleParkingAreaSVGBuilder()
        { }
        public BicycleParkingAreaSVGBuilder(BicycleParkingArea bicycleParkingArea) : base()
        {
            _bicycleParkingArea = bicycleParkingArea;
            CreateModel();
        }
        #endregion

        #region Methods        

        private void CreateModel()
        {           
            var areaTransform = _bicycleParkingArea.TransformComponent.Transform;
            var areaComponentTransform = _bicycleParkingArea.BicycleParkingAreaComponent.GetTransform();
            AddSvgTransforms(areaTransform);
            AddSvgTransforms(areaComponentTransform);

            var areaPlan = _bicycleParkingArea.BicycleParkingAreaComponent.GetPlan();
            var areaSvg = CreateSvgPath(areaPlan, _bikeParkingAreaBackgroundColor, _bikeParkingAreaStrokeColor);
            // Add area to parent
            AddChild(areaSvg);

            // Create space mark(label).
            var textGroup = CreateSvgText(GetMarkText(_bicycleParkingArea), 12, Configs.BicycleParkingArea.BicycleParkingAreaTextBgColor);
            textGroup.CustomAttributes.Add("alignment-baseline", "middle");

            // Create trans for bicycle area.
            var textTrans = new Svg.Transforms.SvgTransformCollection();
            double centerX = _bicycleParkingArea.BicycleParkingAreaComponent.SpaceWidth * 2 * svgRatio;
            double centerY = _bicycleParkingArea.BicycleParkingAreaComponent.SpaceLength / 2 * svgRatio * 0.9;
            textTrans.Add(new SvgTranslate((float)centerX, (float)centerY));
            textTrans.Add(new SvgScale(1, -1.8f));
            AddSvgTransforms(textGroup, textTrans);

            // Add text to parent
            AddChild(textGroup);

            
            // Currently, 3D don't show space wireframe.
            // This functionality may be implemented in the future.
            //for (int spaceIndex = 0; spaceIndex < _bicycleParkingArea.BicycleParkingAreaComponent.NumOfSpaces; ++spaceIndex)
            //{
            //    CreateModelSpace(spaceIndex);
            //}
        }

        // Currently, 3D don't show space wireframe.
        // This functionality may be implemented in the future.
        //private void CreateModelSpace(int spaceIndex)
        //{
        //    var spacePlan = _bicycleParkingArea.BicycleParkingAreaComponent.GetSpacePlan();
        //    var space = CreateSvgPath(spacePlan, _bikeParkingAreaBackgroundColor, Color.Transparent);

        //    var tranform = _bicycleParkingArea.BicycleParkingAreaComponent.GetSpaceTransform(spaceIndex);
        //    AddSvgTransforms(space, ConvertToSvgTransforms(tranform));
        //    AddChild(space);
        //}
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
        private BicycleParkingArea _bicycleParkingArea;

        private Color _bikeParkingAreaStrokeColor = Color.SlateGray;

        private Color _bikeParkingAreaBackgroundColor = Color.Azure;
        #endregion

        #region Constants

        #endregion
    }
}
