using DaiwaRentalGD.Model.ParkingLotDesign;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    public class WalkwayTileModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public WalkwayTileModelSVGBuilder()
        { }
        public WalkwayTileModelSVGBuilder(WalkwayTile walkwayTile)
        {
            _walkwayTile = walkwayTile;
            CreateModel();
        }

        #endregion

        #region Methods

        public void CreateModel()
        {
            AddSvgTransforms(_walkwayTile.TransformComponent.Transform);
            var walkwayTilePlan = _walkwayTile.WalkwayTileComponent.GetPlan();
            var walkwayTileModel = CreateSvgPath(walkwayTilePlan, _walkwayTileBackgroundColor, _isShowedWireframe ? _walkwayTileStrokeColor : _walkwayTileBackgroundColor);
            AddChild(walkwayTileModel);
        }
        #endregion

        #region Properties
        private WalkwayTile _walkwayTile;
        private bool _isShowedWireframe { get; set; } = false;

        private Color _walkwayTileStrokeColor = Color.DimGray;
        private Color _walkwayTileBackgroundColor = Color.Gainsboro;
        #endregion

        #region Constants
        public const double DefaultWireframeThickness = 0.03;

        #endregion
    }
}
