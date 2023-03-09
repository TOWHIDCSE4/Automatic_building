using DaiwaRentalGD.Model.BuildingDesign;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
   public class StaircaseModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public StaircaseModelSVGBuilder()
        { }
        public StaircaseModelSVGBuilder(Staircase staircase)
        {
            _staircase = staircase;
            CreateModel();
        }

        #endregion

        #region Methods

        public void CreateModel()
        {
            var staircaseTransform = _staircase.TransformComponent.Transform;
            AddSvgTransforms(staircaseTransform);
            var staircasePlan = _staircase.StaircaseComponent.GetPlan();
            var staircase = CreateSvgPath(staircasePlan, SolidBrush, WireframeBrush);
            AddChild(staircase);
        }


        #endregion

        #region Properties
        private Staircase _staircase;
        public Color SolidBrush { get; set; } = DefaultSolidBrush;

        public Color WireframeBrush { get; set; } = DefaultWireframeBrush;
        #endregion

        #region Constants

        public static readonly Color DefaultSolidBrush =
            Color.SteelBlue;

        public static readonly Color DefaultWireframeBrush =
            Color.DarkSlateGray;
        #endregion
    }
}
