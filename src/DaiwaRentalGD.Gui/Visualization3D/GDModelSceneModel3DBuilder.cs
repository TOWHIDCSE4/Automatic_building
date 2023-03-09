using System.Windows.Media.Media3D;
using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.GDModelScene"/>.
    /// </summary>
    public class GDModelSceneModel3DBuilder
    {
        #region Constructors

        public GDModelSceneModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(GDModelScene gdms)
        {
            var lightsModel =
                LightsModel3DBuilder.CreateModel();

            var siteModel =
                SiteModel3DBuilder.CreateModel(gdms.Site);

            var compassModel =
                CompassModel3DBuilder.CreateModel(gdms.Site);

            var slantPlanesModel =
                SlantPlanesModel3DBuilder
                .CreateModel(gdms.SlantPlanes);

            var buildingModel =
                BuildingModel3DBuilder.CreateModel(gdms.Building);

            var parkingLotModel =
                ParkingLotModel3DBuilder
                .CreateModel(gdms.ParkingLot);

            var model = new Model3DGroup
            {
                Children =
                {
                    // The order of children partly considers
                    // the drawing of transparent objects
                    lightsModel,
                    siteModel,
                    compassModel,
                    parkingLotModel,
                    buildingModel,
                    slantPlanesModel
                }
            };

            return model;
        }

        #endregion

        #region Properties

        public LightsModel3DBuilder LightsModel3DBuilder { get; } =
            new LightsModel3DBuilder();

        public SiteModel3DBuilder SiteModel3DBuilder { get; } =
            new SiteModel3DBuilder();

        public CompassModel3DBuilder CompassModel3DBuilder { get; } =
            new CompassModel3DBuilder();

        public SlantPlanesModel3DBuilder SlantPlanesModel3DBuilder { get; } =
            new SlantPlanesModel3DBuilder();

        public BuildingModel3DBuilder BuildingModel3DBuilder { get; } =
            new BuildingModel3DBuilder();

        public ParkingLotModel3DBuilder ParkingLotModel3DBuilder { get; } =
            new ParkingLotModel3DBuilder();

        #endregion
    }
}
