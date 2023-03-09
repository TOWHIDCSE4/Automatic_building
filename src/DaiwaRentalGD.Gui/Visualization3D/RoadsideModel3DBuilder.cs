using System.Windows.Media.Media3D;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign.Roadside"/>.
    /// </summary>
    public class RoadsideModel3DBuilder
    {
        #region Constructors

        public RoadsideModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(Roadside roadside)
        {
            Model3DGroup roadsideModel = new Model3DGroup
            {
                Transform = Viewport3DUtils.ConvertToTransform3D(
                    roadside.TransformComponent.Transform
                )
            };

            var cpas = roadside.RoadsideComponent
                .CarParkingAreaAnchorComponent
                .CarParkingAreas.Values;

            foreach (CarParkingArea cpa in cpas)
            {
                var cpaModel = CarParkingAreaModel3DBuilder.CreateModel(cpa);

                roadsideModel.Children.Add(cpaModel);
            }

            return roadsideModel;
        }

        #endregion

        #region Properties

        public CarParkingAreaModel3DBuilder CarParkingAreaModel3DBuilder
        { get; } = new CarParkingAreaModel3DBuilder();

        #endregion
    }
}
