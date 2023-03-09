using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using MathNet.Numerics.LinearAlgebra.Complex;
using Svg.Transforms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media.Media3D;
using MathNet.Numerics.LinearAlgebra.Double;
using DaiwaRentalGD.Gui.Utilities.Configs;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    public class CompassModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors


        public CompassModelSVGBuilder(Site site)
        {
            _site = site;
            CreateModel();
        }
        #endregion

        #region Methods        

        private void CreateModel()
        {

            AddSvgTransforms(GetCompassTransform(_site));
            //CreateYAxisArrowAndLabel();
            CreateTrueNorthArrowAndLabel();
        }

        private SvgTransformCollection GetCompassTransform(Site site)
        {

            var scBbox = site.SiteComponent.GetBBox();

            double compassOffsetX = scBbox.MinX + (Configs.Compass.LabeledArrowDiameter / 2.0 - Configs.Compass.ArrowWidth / 2.0) - Configs.Compass.LabeledArrowDiameter;
            double compassOffsetY = scBbox.MinY + (Configs.Compass.LabeledArrowDiameter / 2.0 - Configs.Compass.ArrowLength / 2.0);

            var transforms = new Svg.Transforms.SvgTransformCollection();
            transforms.Add(new SvgTranslate((float)(compassOffsetX * svgRatio), (float)(compassOffsetY * svgRatio)));

            return transforms;
        }

        private void CreateTrueNorthArrowAndLabel()
        {
            var trueNorthGroup = CreateSvgGroup();

            // Transforms.   
            double trueNorthAngle = BuildingPlacementComponent.ConvertRadiansToDegrees(_site.SiteComponent.TrueNorthAngle);

            float centerX = (float)(Configs.Compass.ArrowWidth / 2.0) * svgRatio;
            float centerY = (float)(Configs.Compass.ArrowLength / 2.0) * svgRatio;
            trueNorthGroup.Transforms.Add(new SvgRotate((float)trueNorthAngle, centerX, centerY));

            // Arrow
            var arrowGroup = CreateSvgPath(Configs.Compass.TrueNorthArrow, Configs.Compass.TrueNorthArrowBgColor, Configs.Compass.TrueNorthArrowBgColor);
            AddChild(trueNorthGroup, arrowGroup);

            // Label
            var labelGroup = CreateSvgText("N", 16, Configs.Compass.TrueNorthLabelBgColor);
            var labelTransforms = new SvgTransformCollection();
            labelTransforms.Add(new SvgScale(1, -1));
            labelTransforms.Add(new SvgTranslate((float)(Configs.Compass.ArrowWidth) / 2.0f * svgRatio, ((float)Configs.Compass.ArrowLength + (float)Configs.Compass.LabelHeight / 4) * svgRatio * -1));
            AddSvgTransforms(labelGroup, labelTransforms);
            labelGroup.FontWeight = Svg.SvgFontWeight.Bold;
            AddChild(trueNorthGroup, labelGroup);

            // Add arrow and label to parent group.
            AddChild(trueNorthGroup);
        }

        private void CreateYAxisArrowAndLabel()
        {
            double trueNorthAngle = _site.SiteComponent.TrueNorthAngle;
            if (trueNorthAngle == 0)
                return;

            var yAxisNorthGroup = CreateSvgGroup();

            // Transfrom.
            float centerX = (float)(Configs.Compass.ArrowWidth / 2.0) * svgRatio;
            float centerY = (float)(Configs.Compass.ArrowLength / 2.0) * svgRatio;
            yAxisNorthGroup.Transforms.Add(new SvgRotate(0.0f, centerX, centerY));

            // Create arrow.
            var arrowGroup = CreateSvgPath(Configs.Compass.YAxis, Configs.Compass.YAxisArrowBgColor, Configs.Compass.YAxisArrowBgColor);
            AddChild(yAxisNorthGroup, arrowGroup);

            // Create label.
            var labelGroup = CreateSvgText("+Y", 16, Configs.Compass.YAxisLabelBgColor);
            var labelTransforms = new SvgTransformCollection();
            labelTransforms.Add(new SvgScale(1, -1));
            labelTransforms.Add(new SvgTranslate((float)(Configs.Compass.ArrowWidth) / 2.0f * svgRatio, ((float)Configs.Compass.ArrowLength + (float)Configs.Compass.LabelHeight / 4) * svgRatio * -1));
            AddSvgTransforms(labelGroup, labelTransforms);
            labelGroup.FontWeight = Svg.SvgFontWeight.Bold;
            AddChild(yAxisNorthGroup, labelGroup);

            // Add arrow and label to parent group.
            AddChild(yAxisNorthGroup);
        }

        #endregion

        #region Properties
        private Site _site;


        #endregion

        #region Constants




        #endregion
    }
}
