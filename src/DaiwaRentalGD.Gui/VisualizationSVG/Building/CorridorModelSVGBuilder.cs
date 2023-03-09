using DaiwaRentalGD.Model.BuildingDesign;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    public class CorridorModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public CorridorModelSVGBuilder()
        { }
        public CorridorModelSVGBuilder(Corridor corridor)
        {
            _corridor = corridor;
            CreateModel();
        }


        #endregion

        #region Methods

        public void CreateModel()
        {
            var corridorTransform = _corridor.TransformComponent.Transform;
            AddSvgTransforms(corridorTransform);
            var corridorPlan = _corridor.CorridorComponent.Plan;
            var corridorModel = CreateSvgPath(corridorPlan,SolidBrush,WireframeBrush);
            AddChild(corridorModel);
        } 

        #endregion

        #region Properties

        private Corridor _corridor;
        public Color SolidBrush { get; set; } = DefaultSolidBrush;

        public Color WireframeBrush { get; set; } = DefaultWireframeBrush;
        #endregion

        #region Constants
        public static readonly Color DefaultSolidBrush =
           Color.LightSteelBlue;

        public static readonly Color DefaultWireframeBrush =
            Color.DarkSlateGray;


        #endregion
    }
}
