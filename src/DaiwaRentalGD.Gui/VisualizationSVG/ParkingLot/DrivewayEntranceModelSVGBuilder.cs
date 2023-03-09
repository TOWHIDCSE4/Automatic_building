using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ParkingLotDesign;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Point = DaiwaRentalGD.Geometries.Point;

namespace DaiwaRentalGD.Gui.VisualizationSVG.ParkingLot
{
    public class DrivewayEntranceModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public DrivewayEntranceModelSVGBuilder()
        { }
        public DrivewayEntranceModelSVGBuilder(DrivewayEntrance drivewayEntrance) : base()
        {
            _drivewayEntrance = drivewayEntrance;
            CreateModel();
        }
        #endregion

        #region Methods
        private void CreateModel()
        {
            var walkwayEntranceTransfrom = _drivewayEntrance.DrivewayEntranceComponent.Transform;
            AddSvgTransforms(walkwayEntranceTransfrom);
            var plan = new Polygon(new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(-0.4, -1.0, 0.0),
                    new Point(0.4, -1.0, 0.0)
                }
            );

            var model = CreateSvgPath(plan, _drivewayEntranceBackgroundColor, _drivewayEntranceBackgroundColor);
            AddSvgTransforms(model, ConvertToSvgTransforms(new TrsTransform3D
            {
                Sx = MarkScale,
                Sy = MarkScale,
                Sz = MarkScale
            }));

            AddChild(model);
        }

        #endregion

        #region Properties
        private DrivewayEntrance _drivewayEntrance;
        private Color _drivewayEntranceBackgroundColor = Color.LimeGreen;
        const double MarkScale = 1;
        #endregion

        #region Constants

        #endregion
    }
}
