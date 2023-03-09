using System;
using System.Linq;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Gui.VisualizationSVG.ParkingLot;
using ParkingLotDesign = DaiwaRentalGD.Model.ParkingLotDesign;
using Svg;
using Svg.Transforms;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    /// <summary>
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.ParkingLot"/>.
    /// </summary>
    public class ParkingLotModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public ParkingLotModelSVGBuilder()
        { }
        public ParkingLotModelSVGBuilder(ParkingLotDesign.ParkingLot parkingLot) : base()
        {
            _parkingLot = parkingLot;
            CreateModel();
        }
        #endregion

        #region Methods

        private void CreateModel()
        {
            var parkingLotTransform = _parkingLot.TransformComponent.Transform;
            AddSvgTransforms(parkingLotTransform);

            var parkingLotComponent = _parkingLot.ParkingLotComponent;

            // Roadside
            var roadsides = parkingLotComponent.Roadsides;
            foreach (var roadside in roadsides)
            {
                var roadsideModel = new RoadsideModelSVGBuilder(roadside);
                AddChild(roadsideModel);
            }           

            // DrivewayTile
            var drivewayTiles = parkingLotComponent.DrivewayTiles;
            foreach (var drivewayTile in drivewayTiles)
            {
                var drivewayTileModel = new DrivewayTileModelSVGBuilder(drivewayTile);
                AddChild(drivewayTileModel);
            }

            // DriveWayEntrance
            var drivewayentrances = parkingLotComponent.DrivewayEntrances;
            foreach (var drivewayentrance in drivewayentrances)
            {
                var drivewayentrancesModel = new DrivewayEntranceModelSVGBuilder(drivewayentrance);
                AddChild(drivewayentrancesModel);
            }

            // WalkwayTile
            var walkwayTiles = parkingLotComponent.WalkwayTiles;
            foreach (var walkwayTile in walkwayTiles)
            {
                var walkwayTileModel = new WalkwayTileModelSVGBuilder(walkwayTile);
                AddChild(walkwayTileModel);
            }

            // BikewayTile
            var bikewayTiles = parkingLotComponent.BikewayTiles;
            foreach (var bikeway in bikewayTiles)
            {
                var bikewayTileModel = new BikewayTileModelSVGBuilder(bikeway);
                AddChild(bikewayTileModel);
            }

            // WalkwayEntrance
            foreach (var walkwayEntrance in parkingLotComponent.WalkwayEntrances)
            {
                var walkwayEntranceModel = new WalkwayEntranceModelSVGBuilder(walkwayEntrance);
                AddChild(walkwayEntranceModel);
            }

        }


        #endregion

        #region Properties
        private ParkingLotDesign.ParkingLot _parkingLot;
        #endregion

        #region Constants

        #endregion
    }
}
