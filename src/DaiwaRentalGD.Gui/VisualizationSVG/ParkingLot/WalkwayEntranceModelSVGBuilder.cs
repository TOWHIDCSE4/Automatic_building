using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ParkingLotDesign;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Point = DaiwaRentalGD.Geometries.Point;

namespace DaiwaRentalGD.Gui.VisualizationSVG
{
    public class WalkwayEntranceModelSVGBuilder : ModelSVGBuilder
    {
        #region Constructors

        public WalkwayEntranceModelSVGBuilder()
        { }
        public WalkwayEntranceModelSVGBuilder(WalkwayEntrance walkwayEntrance) : base()
        {
            _walkwayEntrance = walkwayEntrance;
            CreateModel();
        }
        #endregion

        #region Methods
        private void CreateModel()
        {
            var walkwayEntranceTransfrom = _walkwayEntrance.WalkwayEntranceComponent.Transform;
            AddSvgTransforms(walkwayEntranceTransfrom);
            var markPlan = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(-0.2, -1.0, 0.0),
                    new Point(0.2, -1.0, 0.0)
                }
            );

            var markModel = CreateSvgPath(markPlan, _walkwayEntranceBackgroundColor, _walkwayEntranceBackgroundColor);
            AddSvgTransforms(markModel, ConvertToSvgTransforms(new TrsTransform3D
            {
                Sx = MarkScale,
                Sy = MarkScale,
                Sz = MarkScale
            }));
            AddChild(markModel);
        }

        #endregion

        #region Properties
        private WalkwayEntrance _walkwayEntrance;
        const double MarkScale = 1.0;
        private Color _walkwayEntranceBackgroundColor = Color.LimeGreen;

        #endregion


        #region Constants

        #endregion
    }
}