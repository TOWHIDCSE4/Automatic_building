using System;
using System.Drawing;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.VisualizationSVG.ParkingLot
{
    /// <summary>
    /// </summary>
    public class BikewayTileModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public BikewayTileModelSVGBuilder()
        { }
        public BikewayTileModelSVGBuilder(BikewayTile bikewayTile) : base()
        {
            _bikewayTile = bikewayTile;
            CreateModel();
        }
        #endregion

        #region Methods
        private void CreateModel()
        {
            var bikewayTileTransfrom = _bikewayTile.TransformComponent.Transform;
            AddSvgTransforms(bikewayTileTransfrom);
         
            // Tile
            var bikewayTilePlan = _bikewayTile.BikewayTileComponent.GetPlan();
            var bikewayModel = CreateSvgPath(bikewayTilePlan, _bikewayTileBackgroundColor, _isShowedWireframe ? _bikewayTileStrokeColor : _bikewayTileBackgroundColor);   
            AddChild(bikewayModel);

            // Left
            var leftAreas = _bikewayTile.BikewayTileComponent.LeftBicycleParkingAreaAnchorComponent.BicycleParkingAreas.Values;
            foreach (BicycleParkingArea bpa in leftAreas)
            {
                var bpaModel = new BicycleParkingAreaSVGBuilder(bpa);
                AddChild(bpaModel);
            }

            // Right
            var rightAreas = _bikewayTile.BikewayTileComponent.RightBicycleParkingAreaAnchorComponent.BicycleParkingAreas.Values;
            foreach (BicycleParkingArea bpa in rightAreas)
            {
                var bpaModel = new BicycleParkingAreaSVGBuilder(bpa);
                AddChild(bpaModel);
            }


        }

        #endregion

        #region Properties
        private BikewayTile _bikewayTile;

        private bool _isShowedWireframe { get; set; } = false;

        private Color _bikewayTileStrokeColor = Color.DimGray;

        private Color _bikewayTileBackgroundColor = Color.Gainsboro;
        #endregion

        #region Constants

        #endregion
    }
}
