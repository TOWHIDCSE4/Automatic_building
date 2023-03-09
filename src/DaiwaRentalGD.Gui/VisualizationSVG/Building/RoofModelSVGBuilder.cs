using DaiwaRentalGD.Model.BuildingDesign;
using System.Drawing;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    /// <summary>
    /// Builds for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.Roof"/>.
    /// </summary>
    public class RoofModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public RoofModelSVGBuilder()
        { }
        public RoofModelSVGBuilder(Roof roof)
        {
            _roof = roof;
            CreateModel();
        }
        #endregion

        #region Methods

        public void CreateModel()
        {
            var roofTransform = _roof.TransformComponent.Transform;
            AddSvgTransforms(roofTransform);
            var roofplan = _roof.RoofComponent.GetPlan();
            var roofModel = CreateSvgPath(roofplan, SolidBrush, WireframeBrush);
            AddChild(roofModel);
        }
        #endregion

        #region Properties
        private Roof _roof;
        public Color SolidBrush { get; set; } = DefaultSolidBrush;

        public Color WireframeBrush { get; set; } = DefaultWireframeBrush;
        #endregion

        #region Constants
        //opacity
        //100% - FF
        //95% - F2
        //90% - E6
        //85% - D9
        //80% - CC
        //75% - BF
        //70% - B3
        //65% - A6
        //60% - 99
        //55% - 8C
        //50% - 80
        //45% - 73
        //40% - 66
        //35% - 59
        //30% - 4D
        //25% - 40
        //20% - 33
        //15% - 26
        //10% - 1A
        //5% - 0D
        //0% - 00
        public static readonly Color DefaultSolidBrush = ColorTranslator.FromHtml("#FFFFFFFF");

        public static readonly Color DefaultWireframeBrush =
            Color.DimGray;

        #endregion
    }
}
