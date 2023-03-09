using System;
using DaiwaRentalGD.Model.ParkingLotDesign;
using Svg;
using Svg.Transforms;
using LineSegment = DaiwaRentalGD.Geometries.LineSegment;

namespace DaiwaRentalGD.Gui.VisualizationSVG.ParkingLot
{
    /// <summary>
    /// This is Model base.
    /// </summary>
    public class RoadsideModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public RoadsideModelSVGBuilder()
        { }
        public RoadsideModelSVGBuilder(Roadside roadside) : base()
        {
            _roadside = roadside;
            CreateModel();
        }
        #endregion

        #region Methods
        private void CreateModel()
        {
            var roadSideTransform = _roadside.TransformComponent.Transform;
            AddSvgTransforms(roadSideTransform);

            var cpas = _roadside.RoadsideComponent.CarParkingAreaAnchorComponent.CarParkingAreas.Values;
            foreach (CarParkingArea cpa in cpas)
            {
                var carParkingArea = new CarParkingAreaModelSVGBuilder(cpa);
                AddChild(carParkingArea);
            }
        }

        #endregion

        #region Properties
        private Roadside _roadside;
        #endregion

        #region Constants

        #endregion
    }
}
