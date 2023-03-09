using System;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Gui.Visualization3D;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign
{
    /// <summary>
    /// View model for viewing <see cref="CatalogUnitComponent"/>s
    /// (unit catalog entries) in a viewport.
    /// </summary>
    public class UnitCatalogEntryViewportViewModel : ViewModelBase
    {
        #region Constructors

        public UnitCatalogEntryViewportViewModel(CatalogUnitComponent cuc) :
            base()
        {
            if (cuc == null)
            {
                throw new ArgumentNullException(nameof(cuc));
            }

            Unit = CreateUnit(cuc);

            Model3D = CreateModel3D(Unit);
        }

        #endregion

        #region Methods

        private Unit CreateUnit(CatalogUnitComponent cuc)
        {
            var unit = new Unit
            {
                UnitComponent = cuc.Copy(),
            };

            var centerTranslate = -cuc.GetBBox().GetCenter().Vector;
            centerTranslate[2] = 0.0;

            unit.TransformComponent.Transform
                .SetTranslateLocal(centerTranslate);

            return unit;
        }

        private Model3D CreateModel3D(Unit unit)
        {
            var unitModelBuilder = new UnitModel3DBuilder();
            var unitModel = unitModelBuilder.CreateModel(unit);

            var lightsModelBuilder = new LightsModel3DBuilder();
            var lightsModel = lightsModelBuilder.CreateModel();

            var model = new Model3DGroup
            {
                Children =
                {
                    unitModel,
                    lightsModel
                }
            };

            return model;
        }

        #endregion

        #region Properties

        public Unit Unit { get; }

        public Model3D Model3D { get; private set; }

        #endregion
    }
}
