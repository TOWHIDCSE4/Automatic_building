using DaiwaRentalGD.Model.ParkingLotDesign;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DaiwaRentalGD.Gui.VisualizationSVG.ParkingLot
{
    public class DrivewayTileModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public DrivewayTileModelSVGBuilder()
        { }
        public DrivewayTileModelSVGBuilder(DrivewayTile drivewayTile) : base()
        {
            _drivewayTile = drivewayTile;
            CreateModel();
        }
        #endregion

        #region Methods
        private void CreateModel()
        {
            var drivewayTileTransfrom = _drivewayTile.TransformComponent.Transform;
            AddSvgTransforms(drivewayTileTransfrom);

            // Title
            var drivewayTilePlan = _drivewayTile.DrivewayTileComponent.GetPlan();
            var drivewayModel = CreateSvgPath(drivewayTilePlan, _drivewayTileBackgroudColor, _isShowedDrivewayTileWireframe ? _drivewayTileStrokeColor : _drivewayTileBackgroudColor);
            AddChild(drivewayModel);

            // Left
            var leftAreas = _drivewayTile.DrivewayTileComponent.LeftCarParkingAreaAnchorComponent.CarParkingAreas.Values;
            foreach (CarParkingArea cpa in leftAreas)
            {
                var cpaModel = new CarParkingAreaModelSVGBuilder(cpa);
                AddChild(cpaModel);
            }

            // Right
            var rightAreas = _drivewayTile.DrivewayTileComponent.RightCarParkingAreaAnchorComponent.CarParkingAreas.Values;
            foreach (CarParkingArea cpa in rightAreas)
            {
                var bpaModel = new CarParkingAreaModelSVGBuilder(cpa);
                AddChild(bpaModel);
            }

        }

        #endregion

        #region Properties
        private DrivewayTile _drivewayTile;
        private bool _isShowedDrivewayTileWireframe = false;
        private Color _drivewayTileBackgroudColor = Color.Gainsboro;
        private Color _drivewayTileStrokeColor = Color.DimGray;
        #endregion

        #region Constants

        #endregion
    }
}
