using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Gui.Utilities.Configs;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.SiteDesign;
using Svg.Transforms;
using System;
using System.Drawing;
using static DaiwaRentalGD.Gui.Utilities.Configs.Configs;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    public class ScaleBarModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public ScaleBarModelSVGBuilder(Site site)
        {
            _site = site;
            CreateModel();
        }
        #endregion

        #region Methods        


        private void CreateModel()
        {
            CreateYAxisBpxLineAndLabel(_site);
        }

        private void CreateYAxisBpxLineAndLabel(Site site)
        {
            var sbox = site.SiteComponent.GetBBoxOfBounddary();
            AddSvgTransforms(new SvgTranslate((float)sbox.MinX * svgRatio, (float)sbox.MinY * svgRatio - 2.0f * svgRatio));


            var scaleBarLineModel = new ModelSVGBuilder();
            AddChild(scaleBarLineModel);

            //float scaleBarLineWidth = ((float)sbox.MaxX - (float)sbox.MinX);
            float scaleBarLineWidth = 20;
            var scaleBarLineWidthCelling = Math.Ceiling(scaleBarLineWidth);

            Polygon scaleBarFullPolygon = new Polygon(
                  new[]
                  {
                        new Geometries.Point(0.0, 0.0, 0.0),
                        new Geometries.Point(scaleBarLineWidthCelling, 0.0, 0.0),
                        new Geometries.Point(scaleBarLineWidthCelling, 0.6, 0.0),
                        new Geometries.Point(0.0, 0.6, 0.0)
                  });

            var scaleBarFrame = CreateSvgPath(scaleBarFullPolygon, Color.White, Color.Gray);
            var scaleBarFrameTransforms = new SvgTransformCollection();
            scaleBarFrameTransforms.Add(new SvgTranslate(0, -0.15f * svgRatio));
            AddSvgTransforms(scaleBarFrame, scaleBarFrameTransforms);
            scaleBarLineModel.AddChild(scaleBarFrame);

            var scaleBarWidth1 = (float)Math.Ceiling(scaleBarLineWidth / 4);
            float StartX = 0, StartY = 0.3f, EndX = scaleBarWidth1, EndY = 0.3f;
            var scaleBar = CreateSvgLine(StartX, StartY, EndX, EndY, ScaleBar.ScaleBarColor);
            scaleBarLineModel.AddChild(scaleBar);

            var scaleBarWidth2 = (float)Math.Ceiling(scaleBarLineWidth / 2);
            StartX = scaleBarWidth1; StartY = 0; EndX = scaleBarWidth2; EndY = 0;
            var scaleBar2 = CreateSvgLine(StartX, StartY, EndX, EndY, ScaleBar.ScaleBarColor);
            scaleBarLineModel.AddChild(scaleBar2);

            var scaleBarWidth3 = (float)Math.Ceiling(scaleBarLineWidth / 1);
            StartX = scaleBarWidth2; StartY = 0.3f; EndX = scaleBarWidth3; EndY = 0.3f;
            var scaleBar3 = CreateSvgLine(StartX, StartY, EndX, EndY, ScaleBar.ScaleBarColor);
            scaleBarLineModel.AddChild(scaleBar3);


            // Create label for scale bar.
            var labelModel = new ModelSVGBuilder();
            labelModel.AddSvgTransforms(new SvgScale(1, -1));
            AddChild(labelModel);

            var labelMarginLeft = ScaleBar.LabelMarginLeft * svgRatio;
            var labelMarginTop = ScaleBar.LabelMarginTop * svgRatio;

            var label1 = CreateSvgText($"0 m", ScaleBar.FontSize, Color.Black);
            var labelTransforms1 = new SvgTransformCollection();       
            labelTransforms1.Add(new SvgTranslate(labelMarginLeft, labelMarginTop));
            AddSvgTransforms(label1, labelTransforms1);
            labelModel.AddChild(label1);

            var label2 = CreateSvgText($"{scaleBarWidth1} m", ScaleBar.FontSize, Color.Black);
            var labelTransforms2 = new SvgTransformCollection();    
            labelTransforms2.Add(new SvgTranslate(scaleBarWidth1 * svgRatio + labelMarginLeft, labelMarginTop));
            AddSvgTransforms(label2, labelTransforms2);
            labelModel.AddChild(label2);

            var label3 = CreateSvgText($"{scaleBarWidth2} m", ScaleBar.FontSize, Color.Black);
            var labelTransforms3 = new SvgTransformCollection();   
            labelTransforms3.Add(new SvgTranslate(scaleBarWidth2 * svgRatio + labelMarginLeft, labelMarginTop));
            AddSvgTransforms(label3, labelTransforms3);
            labelModel.AddChild(label3);

            var label4 = CreateSvgText($"{scaleBarWidth3} m", ScaleBar.FontSize, Color.Black);
            var labelTransforms4 = new SvgTransformCollection();          
            labelTransforms4.Add(new SvgTranslate(scaleBarWidth3 * svgRatio + labelMarginLeft, labelMarginTop));           
            AddSvgTransforms(label4, labelTransforms4);
            labelModel.AddChild(label4);     

        }
        public Svg.SvgLine CreateSvgLine(float StartX, float StartY, float EndX, float EndY, Color stroke)
        {
            var line = new Svg.SvgLine()
            {
                StartX = (float)(StartX * svgRatio),
                StartY = (float)(StartY * svgRatio),
                EndX = (float)(EndX * svgRatio),
                EndY = (float)(EndY * svgRatio),
                Stroke = new Svg.SvgColourServer(stroke),
                StrokeWidth = 5
            };

            line.Transforms = new SvgTransformCollection();
            return line;
        }

        #endregion

        #region Properties
        private Site _site;

        #endregion

        #region Constants
        #endregion

    }



}
