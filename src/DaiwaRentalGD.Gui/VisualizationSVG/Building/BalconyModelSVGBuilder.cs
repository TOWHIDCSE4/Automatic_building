using DaiwaRentalGD.Model.BuildingDesign;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    public class BalconyModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public BalconyModelSVGBuilder()
        { }
        public BalconyModelSVGBuilder(Balcony balcony)
        {
            _balcony = balcony;
            CreateModel();
        }
        #endregion

        #region Methods

        public void CreateModel()
        {
            var balconyTransform = _balcony.TransformComponent.Transform;
            AddSvgTransforms(balconyTransform);
            var balconyplan = _balcony.BalconyComponent.GetPlan();
            var balconyModel = CreateSvgPath(balconyplan, SolidBrush, WireframeBrush);
            AddChild(balconyModel);
        }
        #endregion

        #region Properties

        private Balcony _balcony;
        public Color SolidBrush { get; set; } = DefaultSolidBrush;

        public Color WireframeBrush { get; set; } = DefaultWireframeBrush;
        #endregion

        #region Constants

        public static readonly Color DefaultSolidBrush =
            Color.Salmon;

        public static readonly Color DefaultWireframeBrush =
            Color.DarkRed;
        #endregion
    }
}
