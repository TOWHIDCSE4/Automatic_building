using System.Windows.Media.Media3D;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.ParkingLot"/>.
    /// </summary>
    public class ParkingLotModel3DBuilder
    {
        #region Constructors

        public ParkingLotModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(ParkingLot parkingLot)
        {
            var parkingLotModel = new Model3DGroup
            {
                Transform = Viewport3DUtils.ConvertToTransform3D(
                    parkingLot.TransformComponent.Transform
                )
            };

            var plc = parkingLot.ParkingLotComponent;

            foreach (var walkwayEntrance in plc.WalkwayEntrances)
            {
                var weModel =
                    WalkwayEntranceModel3DBuilder
                    .CreateModel(walkwayEntrance);

                parkingLotModel.Children.Add(weModel);
            }

            foreach (var drivewayEntrance in plc.DrivewayEntrances)
            {
                var deModel =
                    DrivewayEntranceModel3DBuilder
                    .CreateModel(drivewayEntrance);

                parkingLotModel.Children.Add(deModel);
            }

            foreach (WalkwayTile walkwayTile in plc.WalkwayTiles)
            {
                var wtModel =
                    WalkwayTileModel3DBuilder.CreateModel(walkwayTile);

                parkingLotModel.Children.Add(wtModel);
            }

            if (DoesShowWalkwayGraph)
            {
                var wgModel =
                    WalkwayGraphModel3DBuilder.CreateModel(plc.WalkwayGraph);

                parkingLotModel.Children.Add(wgModel);
            }

            if (DoesShowWalkwayPaths)
            {
                foreach (var walkwayPath in plc.WalkwayPaths)
                {
                    var wpModel =
                        WalkwayPathModel3DBuilder.CreateModel(walkwayPath);

                    parkingLotModel.Children.Add(wpModel);
                }
            }

            if (DoesShowSiteVectorField)
            {
                var svfModel =
                    SiteVectorFieldModel3DBuilder.CreateModel(parkingLot);

                parkingLotModel.Children.Add(svfModel);
            }

            foreach (Roadside roadside in plc.Roadsides)
            {
                var roadsideModel =
                    RoadsideModel3DBuilder.CreateModel(roadside);

                parkingLotModel.Children.Add(roadsideModel);
            }

            foreach (DrivewayTile drivewayTile in plc.DrivewayTiles)
            {
                var dtModel =
                    DrivewayTileModel3DBuilder.CreateModel(drivewayTile);

                parkingLotModel.Children.Add(dtModel);
            }

            foreach (BikewayTile bikeway in plc.BikewayTiles)
            {
                var btModel = BikewayTileModel3DBuilder.CreateModel(bikeway);

                parkingLotModel.Children.Add(btModel);
            }

            return parkingLotModel;
        }

        #endregion

        #region Properties

        public WalkwayEntranceModel3DBuilder WalkwayEntranceModel3DBuilder
        { get; } = new WalkwayEntranceModel3DBuilder();

        public DrivewayEntranceModel3DBuilder DrivewayEntranceModel3DBuilder
        { get; } = new DrivewayEntranceModel3DBuilder();

        public WalkwayTileModel3DBuilder WalkwayTileModel3DBuilder
        { get; } = new WalkwayTileModel3DBuilder();

        public WalkwayGraphModel3DBuilder WalkwayGraphModel3DBuilder
        { get; } = new WalkwayGraphModel3DBuilder();

        public WalkwayPathModel3DBuilder WalkwayPathModel3DBuilder
        { get; } = new WalkwayPathModel3DBuilder();

        public SiteVectorFieldModel3DBuilder SiteVectorFieldModel3DBuilder
        { get; } = new SiteVectorFieldModel3DBuilder();

        public RoadsideModel3DBuilder RoadsideModel3DBuilder
        { get; } = new RoadsideModel3DBuilder();

        public DrivewayTileModel3DBuilder DrivewayTileModel3DBuilder
        { get; } = new DrivewayTileModel3DBuilder();

        public BikewayTileModel3DBuilder BikewayTileModel3DBuilder
        { get; } = new BikewayTileModel3DBuilder();

        #region Options

        public bool DoesShowWayTiles
        {
            get => _doesShowWayTiles;
            set
            {
                WalkwayTileModel3DBuilder.DoesShowWireframe = value;
                DrivewayTileModel3DBuilder.DoesShowWireframe = value;
                BikewayTileModel3DBuilder.DoesShowWireframe = value;

                _doesShowWayTiles = value;
            }
        }

        public bool DoesShowWalkwayGraph { get; set; } =
            DefaultDoesShowWalkwayGraph;

        public bool DoesShowWalkwayPaths { get; set; } =
            DefaultDoesShowWalkwayPaths;

        public bool DoesShowSiteVectorField { get; set; } =
            DefaultDoesShowSiteVectorField;

        #endregion

        #endregion

        #region Member variables

        private bool _doesShowWayTiles = DefaultDoesShowWayTiles;

        #endregion

        #region Constants

        public const bool DefaultDoesShowWayTiles = false;
        public const bool DefaultDoesShowWalkwayGraph = false;
        public const bool DefaultDoesShowWalkwayPaths = false;
        public const bool DefaultDoesShowSiteVectorField = false;

        #endregion
    }
}
