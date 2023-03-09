using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.ParkingLotDesign;
using Svg;
using Svg.Transforms;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.BuildingComponent"/>.
    /// </summary>
    public class BuildingModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public BuildingModelSVGBuilder()
        { }
        public BuildingModelSVGBuilder(Building building)
        {
            _building = building;
            CreateModel();
        }

        #endregion

        #region Methods        

        public void CreateModel()
        {
            var buildingTransform = _building.TransformComponent.Transform;
            AddSvgTransforms(buildingTransform);
            CreateUnitsModel();
            CreateStaircasesModel();
            CreateCorridorsModel();
            CreateBalconiesModel();
            CreateEntrancesModel();
            CreateRoofsModel();
        }
        private void CreateUnitsModel()
        {
            var bc = _building.BuildingComponent;

            for (int floor = 0; floor < bc.NumOfFloors; ++floor)
            {

                for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
                {
                    Unit unit = bc.GetUnit(floor, stack);

                    if (unit == null)
                    {
                        continue;
                    }
                    var isShowedUnitLabel = floor == 0;
                    UnitModelSVGBuilder unitModel = new UnitModelSVGBuilder(unit, isShowedUnitLabel);
                    AddChild(unitModel);
                }
            }
        }
        private void CreateStaircasesModel()
        {
            foreach (var staircase in _building.BuildingComponent.Staircases)
            {
                if (staircase != null)
                {
                    StaircaseModelSVGBuilder staircaseModel = new StaircaseModelSVGBuilder(staircase);
                    AddChild(staircaseModel);
                }
            }
        }
        private void CreateCorridorsModel()
        {
            foreach (var corridor in _building.BuildingComponent.Corridors)
            {
                if (corridor != null)
                {
                    CorridorModelSVGBuilder corridorModel = new CorridorModelSVGBuilder(corridor);
                    AddChild(corridorModel);
                }
            }
        }
        private void CreateBalconiesModel()
        {
            foreach (var balcony in _building.BuildingComponent.Balconies)
            {
                if (balcony != null)
                {
                    BalconyModelSVGBuilder balconyModel = new BalconyModelSVGBuilder(balcony);
                    AddChild(balconyModel);
                }
            }
        }
        private void CreateRoofsModel()
        {
            foreach (var roof in _building.BuildingComponent.Roofs)
            {
                if (roof != null)
                {
                    RoofModelSVGBuilder roofModel = new RoofModelSVGBuilder(roof);
                    AddChild(roofModel);
                }
            }
        }
        private void CreateEntrancesModel()
        {
            foreach (BuildingEntrance entrance in
                _building.BuildingComponent.Entrances)
            {
                BuildingEntranceModelSVGBuilder entranceModel = new BuildingEntranceModelSVGBuilder(entrance);
                AddChild(entranceModel);
            }
        }
        #endregion

        #region Properties
        private Building _building;
        #endregion

        #region Constants

        #endregion
    }
}
