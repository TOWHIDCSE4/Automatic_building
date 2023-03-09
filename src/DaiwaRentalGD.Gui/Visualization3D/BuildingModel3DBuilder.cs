using System.Windows.Media.Media3D;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign.Building"/>.
    /// </summary>
    public class BuildingModel3DBuilder
    {
        #region Constructors

        public BuildingModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(Building building)
        {
            var buildingTransform =
                Viewport3DUtils.ConvertToTransform3D(
                    building.TransformComponent.Transform
                );

            var unitsModel = CreateUnitsModel(building);
            var staircasesModel = CreateStaircasesModel(building);
            var corridorsModel = CreateCorridorsModel(building);
            var balconiesModel = CreateBalconiesModel(building);
            var roofsModel = CreateRoofsModel(building);
            var entrancesModel = CreateEntrancesModel(building);

            var buildingModel = new Model3DGroup()
            {
                Transform = buildingTransform,
                Children =
                {
                    unitsModel,
                    staircasesModel,
                    corridorsModel,
                    balconiesModel,
                    roofsModel,
                    entrancesModel
                }
            };

            return buildingModel;
        }

        private Model3D CreateUnitsModel(Building building)
        {
            var unitsModel = new Model3DGroup();

            var bc = building.BuildingComponent;

            for (int floor = 0; floor < bc.NumOfFloors; ++floor)
            {
                UnitModel3DBuilder.DoesShowLabel = floor == 0;

                for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
                {
                    Unit unit = bc.GetUnit(floor, stack);

                    if (unit == null)
                    {
                        continue;
                    }

                    Model3DGroup unitModel =
                        UnitModel3DBuilder.CreateModel(unit);

                    unitsModel.Children.Add(unitModel);
                }
            }

            return unitsModel;
        }

        private Model3D CreateStaircasesModel(Building building)
        {
            var staircasesModel = new Model3DGroup();

            foreach (var staircase in building.BuildingComponent.Staircases)
            {
                if (staircase != null)
                {
                    var staircaseModel =
                        StaircaseModel3DBuilder.CreateModel(staircase);

                    staircasesModel.Children.Add(staircaseModel);
                }
            }

            return staircasesModel;
        }

        private Model3D CreateCorridorsModel(Building building)
        {
            var corridorsModel = new Model3DGroup();

            foreach (var corridor in building.BuildingComponent.Corridors)
            {
                if (corridor != null)
                {
                    var corridorModel =
                        CorridorModel3DBuilder.CreateModel(corridor);

                    corridorsModel.Children.Add(corridorModel);
                }
            }

            return corridorsModel;
        }

        private Model3D CreateBalconiesModel(Building building)
        {
            var balconiesModel = new Model3DGroup();

            foreach (var balcony in building.BuildingComponent.Balconies)
            {
                if (balcony != null)
                {
                    var balconyModel =
                        BalconyModel3DBuilder.CreateModel(balcony);

                    balconiesModel.Children.Add(balconyModel);
                }
            }

            return balconiesModel;
        }

        private Model3D CreateRoofsModel(Building building)
        {
            var roofsModel = new Model3DGroup();

            foreach (var roof in building.BuildingComponent.Roofs)
            {
                if (roof != null)
                {
                    var roofModel = RoofModel3DBuilder.CreateModel(roof);

                    roofsModel.Children.Add(roofModel);
                }
            }

            return roofsModel;
        }

        private Model3D CreateEntrancesModel(Building building)
        {
            var entrancesModel = new Model3DGroup();

            foreach (BuildingEntrance entrance in
                building.BuildingComponent.Entrances)
            {
                var entranceModel =
                    BuildingEntranceModel3DBuilder.CreateModel(entrance);

                entrancesModel.Children.Add(entranceModel);
            }

            return entrancesModel;
        }

        #endregion

        #region Properties

        public UnitModel3DBuilder UnitModel3DBuilder { get; } =
            new UnitModel3DBuilder();

        public StaircaseModel3DBuilder StaircaseModel3DBuilder { get; } =
            new StaircaseModel3DBuilder();

        public CorridorModel3DBuilder CorridorModel3DBuilder { get; } =
            new CorridorModel3DBuilder();

        public BalconyModel3DBuilder BalconyModel3DBuilder { get; } =
            new BalconyModel3DBuilder();

        public RoofModel3DBuilder RoofModel3DBuilder { get; } =
            new RoofModel3DBuilder();

        public BuildingEntranceModel3DBuilder BuildingEntranceModel3DBuilder
        { get; } = new BuildingEntranceModel3DBuilder();

        #endregion
    }
}
