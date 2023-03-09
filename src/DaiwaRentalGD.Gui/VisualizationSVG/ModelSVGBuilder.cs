using System;
using System.Drawing;
using DaiwaRentalGD.Geometries;
using Svg;
using Svg.Pathing;
using LineSegment = DaiwaRentalGD.Geometries.LineSegment;
using Svg.Transforms;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    /// <summary>
    /// This is Model base.
    /// </summary>
    public class ModelSVGBuilder
    {
        #region Constructors

        public ModelSVGBuilder()
        {
            SvgGroup = new SvgGroup();
            SvgGroup.Transforms = new Svg.Transforms.SvgTransformCollection();
        }
        #endregion

        #region Methods
        protected virtual SvgPath CreateSvgPath(Polygon plan)
        {
            return CreateSvgPath(plan, Color.White, Color.LightGray);
        }

        protected virtual SvgPath CreateSvgPath(Polygon plan, Color fill, Color stroke)
        {
            var svgPath = new SvgPath();
            svgPath.Transforms = new Svg.Transforms.SvgTransformCollection();
            svgPath.PathData = new SvgPathSegmentList();
            svgPath.Fill = new SvgColourServer(fill);
            svgPath.Stroke = new SvgColourServer(stroke);
            svgPath.StrokeWidth = 1;
            var numOfLines = plan.NumOfEdges;
            if (numOfLines > 0)
            {
                var line = plan.GetEdge(0);
                var start = new PointF((float)(line.Point0.X * svgRatio), (float)(line.Point0.Y * svgRatio));
                SvgMoveToSegment svgStartMove = new SvgMoveToSegment(start);
                svgPath.PathData.Add(svgStartMove);
            }

            for (int index = 0; index < numOfLines; index++)
            {
                var line = plan.GetEdge(index);
                var start = new PointF((float)(line.Point0.X * svgRatio), (float)(line.Point0.Y * svgRatio));
                var end = new PointF((float)(line.Point1.X * svgRatio), (float)(line.Point1.Y * svgRatio));
                SvgLineSegment newline = new SvgLineSegment(start, end);
                svgPath.PathData.Add(newline);
            }
            return svgPath;
        }

        protected virtual SvgText CreateSvgText(string text, float fontSize, Color bgColor)
        {
            var textG = new SvgText
            {
                Text = text,
                FontSize = fontSize,
                Color = new SvgColourServer(bgColor),
                Fill = new SvgColourServer(bgColor),
                TextAnchor = Svg.SvgTextAnchor.Middle
            };
            textG.Transforms = new SvgTransformCollection();
            return textG;
        }

        protected virtual SvgLine CreateSvgLine(LineSegment lineSegment)
        {
            return CreateSvgLine(lineSegment, Color.LightGray);
        }

        protected virtual SvgLine CreateSvgLine(LineSegment lineSegment, Color stroke)
        {
            var line = new SvgLine()
            {
                StartX = (float)(lineSegment.Point0.X * svgRatio),
                StartY = (float)(lineSegment.Point0.Y * svgRatio),
                EndX = (float)(lineSegment.Point1.X * svgRatio),
                EndY = (float)(lineSegment.Point1.Y * svgRatio),
                Stroke = new SvgColourServer(stroke),
                StrokeWidth = 1
            };

            line.Transforms = new SvgTransformCollection();
            return line;
        }

        protected virtual SvgGroup CreateSvgLineGroup(Polygon plan)
        {
            var svgGroup = new SvgGroup();
            svgGroup.Transforms = new Svg.Transforms.SvgTransformCollection();
            var numOfLines = plan.NumOfEdges;
            for (int index = 0; index < numOfLines; index++)
            {
                var line = plan.GetEdge(index);
                var svgLine = CreateSvgLine(line);
                svgGroup.Children.Add(svgLine);
            }
            return svgGroup;
        }

        public virtual SvgTransformCollection ConvertToSvgTransforms(TrsTransform3D transform)
        {
            var trans = new Svg.Transforms.SvgTransformCollection();
            var degrees = getRotateAngle(transform);
            trans.Add(new Svg.Transforms.SvgTranslate((float)(transform.Tx * svgRatio), (float)(transform.Ty * svgRatio)));
            trans.Add(new Svg.Transforms.SvgScale((float)(transform.Sx), (float)(transform.Sy)));
            trans.Add(new Svg.Transforms.SvgRotate((float)degrees));
            return trans;
        }

        protected virtual SvgGroup CreateSvgGroup()
        {
            var group = new SvgGroup();
            group.Transforms = new SvgTransformCollection();
            return group;
        }

        public virtual void AddSvgTransforms(SvgTransformCollection trans)
        {
            foreach (var tran in trans)
            {
                SvgGroup.Transforms.Add(tran);
            }
        }
        public virtual void AddSvgTransforms(SvgTransform tran)
        {
            if (SvgGroup.Transforms == null)
                SvgGroup.Transforms = new SvgTransformCollection();
            SvgGroup.Transforms.Add(tran);
        }

        public virtual SvgTransformCollection AddSvgTransforms(TrsTransform3D transform)
        {
            var trans = ConvertToSvgTransforms(transform);
            foreach (var tran in trans)
            {
                SvgGroup.Transforms.Add(tran);
            }
            return trans;
        }

        public virtual void AddSvgTransforms(SvgElement element, SvgTransformCollection trans)
        {
            foreach (var tran in trans)
            {
                element.Transforms.Add(tran);
            }
        }

        public virtual void AddChild(ModelSVGBuilder child)
        {
            SvgGroup.Children.Add(child.SvgGroup);
        }
        public virtual void AddChild(SvgElement parent, SvgElement child)
        {
            parent.Children.Add(child);
        }

        public virtual void AddChild(SvgElement element)
        {
            SvgGroup.Children.Add(element);
        }

        private double getRotateAngle(TrsTransform3D transform)
        {
            var matrix = transform.Matrix;
            var radians = Math.Atan2(matrix.Row(1)[0], matrix.Row(0)[0]);
            var degrees = radians * 180 / Math.PI;
            return degrees;
        }
        #endregion

        #region Properties
        public SvgGroup SvgGroup { get; private set; }

        #endregion

        #region Constants

        public const float svgRatio = 20.0f;

        #endregion
    }
}
