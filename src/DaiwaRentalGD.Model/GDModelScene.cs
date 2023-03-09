using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.Finance;
using DaiwaRentalGD.Model.ParkingLotDesign;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Model.Zoning;
using Workspaces.Core;

namespace DaiwaRentalGD.Model
{
    /// <summary>
    /// A scene representing the GD model. Provides convenient properties
    /// for accessing scene objects and manages the dependencies among them.
    /// </summary>
    [Serializable]
    public class GDModelScene : Scene.Scene
    {
        #region Constructors

        public GDModelScene() : base()
        {
            InitializeSceneObjects();
        }

        protected GDModelScene(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _unitCatalog = reader.GetValue<UnitCatalog>(nameof(UnitCatalog));            

            _site = reader.GetValue<Site>(nameof(Site));

            _building = reader.GetValue<Building>(nameof(Building));

            _parkingLot = reader.GetValue<ParkingLot>(nameof(ParkingLot));

            _siteDesigner =
                reader.GetValue<SiteDesigner>(nameof(SiteDesigner));

            _buildingDesigner =
                reader.GetValue<BuildingDesigner>(nameof(BuildingDesigner));

            _parkingLotDesigner = reader.GetValue<ParkingLotDesigner>(
                nameof(ParkingLotDesigner)
            );

            _setback = reader.GetValue<Setback>(nameof(Setback));

            _buildingPlacementValidation = reader.GetValue<BuildingPlacementValidation>(nameof(BuildingPlacementValidation));

            _slantPlanes = reader.GetValue<SlantPlanes>(nameof(SlantPlanes));

            _landUseEvaluator =
                reader.GetValue<LandUseEvaluator>(nameof(LandUseEvaluator));

            _financeEvaluator =
                reader.GetValue<FinanceEvaluator>(nameof(FinanceEvaluator));

            _unitCatalog.FilterUnitCatalogEntriesByFinance(_financeEvaluator.UnitFinanceComponent.CostAndrevenueEntries);
        }

        #endregion

        #region Methods

        private void InitializeSceneObjects()
        {
            UnitCatalog.FilterUnitCatalogEntriesByFinance(FinanceEvaluator.UnitFinanceComponent.CostAndrevenueEntries);

            AddSceneObject(UnitCatalog);

            AddSceneObject(Site);
            AddSceneObject(SiteDesigner);

            AddSceneObject(Building);
            AddSceneObject(BuildingDesigner);

            AddSceneObject(Setback);
            AddSceneObject(BuildingPlacementValidation);
            AddSceneObject(SlantPlanes);

            AddSceneObject(ParkingLot);
            AddSceneObject(ParkingLotDesigner);
            Site.AddChild(ParkingLot);

            AddSceneObject(LandUseEvaluator);
            AddSceneObject(FinanceEvaluator);

            OnSetUnitCatalog();

            OnSetSite();
            OnSetBuilding();
            OnSetParkingLot();

            OnSetSiteDesigner();
            OnSetBuildingDesigner();

            OnSetSetback();
            OnSetBuildingPlacementValidation();
            OnSetSlantPlanes();

            OnSetParkingLotDesigner();

            OnSetLandUseEvaluator();
            OnSetFinanceEvaluator();
        }

        private void OnSetUnitCatalog()
        {
            BuildingDesigner.UnitArrangerComponent.UnitCatalog = UnitCatalog;
        }

        private void OnSetSite()
        {
            SiteDesigner.SiteCreatorComponent.Site = Site;
            BuildingDesigner.BuildingPlacementComponent.Site = Site;
            Setback.SetbackComponent.Site = Site;

            SlantPlanes.AdjacentSiteSlantPlanesComponent.Site = Site;
            SlantPlanes.AbsoluteHeightSlantPlanesComponent.Site = Site;
            SlantPlanes.RoadSlantPlanesComponent.Site = Site;
            SlantPlanes.NorthSlantPlanesComponent.Site = Site;

            LandUseEvaluator.BuildingCoverageRatioComponent.Site = Site;
            LandUseEvaluator.FloorAreaRatioComponent.Site = Site;
        }

        private void OnSetBuilding()
        {
            BuildingDesigner.UnitArrangerComponent.Building = Building;
            BuildingDesigner.RoofCreatorComponent.Building = Building;
            BuildingDesigner.BuildingEntranceCreatorComponent.Building =
                Building;
            BuildingDesigner.BuildingEntranceCreatorComponent.Building =
                Building;

            ParkingLot.ParkingLotComponent.Building = Building;

            Setback.SetbackResolverComponent.Building = Building;

            SlantPlanes.AdjacentSiteSlantPlanesComponent.Building = Building;
            SlantPlanes.AbsoluteHeightSlantPlanesComponent.Building = Building;
            SlantPlanes.RoadSlantPlanesComponent.Building = Building;
            SlantPlanes.NorthSlantPlanesComponent.Building = Building;

            LandUseEvaluator.BuildingCoverageRatioComponent.Building =
                Building;
            LandUseEvaluator.FloorAreaRatioComponent.Building =
                Building;
            FinanceEvaluator.SummaryFinanceComponent.Building = Building;
        }

        private void OnSetParkingLot()
        {
            ParkingLot.ParkingLotComponent.Building = Building;
            ParkingLot.ParkingLotComponent.ParkingLotDesigner = ParkingLotDesigner;

            ParkingLotDesigner.ParkingLotRequirementsComponent
                .ParkingLot = ParkingLot;
            ParkingLotDesigner.WalkwayDesignerComponent
                .ParkingLot = ParkingLot;
            ParkingLotDesigner.RoadsideCarParkingAreaDesignerComponent
                .ParkingLot = ParkingLot;
            ParkingLotDesigner.DrivewayDesignerComponent
                .ParkingLot = ParkingLot;
            ParkingLotDesigner.BikewayDesignerComponent
                .ParkingLot = ParkingLot;

            FinanceEvaluator.ParkingLotFinanceComponent
                .ParkingLot = ParkingLot;
        }

        private void OnSetSiteDesigner()
        {
            SiteDesigner.SiteCreatorComponent.Site = Site;
        }

        private void OnSetBuildingDesigner()
        {
            BuildingDesigner.UnitArrangerComponent.UnitCatalog = UnitCatalog;
            BuildingDesigner.UnitArrangerComponent.Building = Building;
            BuildingDesigner.RoofCreatorComponent.Building = Building;

            BuildingDesigner.BuildingEntranceCreatorComponent.Building =
                Building;

            BuildingDesigner.BuildingPlacementComponent.Building = Building;
            BuildingDesigner.BuildingPlacementComponent.Site = Site;
        }

        private void OnSetParkingLotDesigner()
        {
            ParkingLotDesigner.ParkingLotRequirementsComponent
                .ParkingLot = ParkingLot;
            ParkingLotDesigner.WalkwayDesignerComponent
                .ParkingLot = ParkingLot;
            ParkingLotDesigner.RoadsideCarParkingAreaDesignerComponent
                .ParkingLot = ParkingLot;
            ParkingLotDesigner.DrivewayDesignerComponent
                .ParkingLot = ParkingLot;
            ParkingLotDesigner.BikewayDesignerComponent
                .ParkingLot = ParkingLot;
        }

        private void OnSetSetback()
        {
            Setback.SetbackComponent.Site = Site;

            Setback.SetbackResolverComponent.Building = Building;
        }

        private void OnSetBuildingPlacementValidation()
        {
            BuildingPlacementValidation.BuildingPlacementComponent.Site = Site;

            BuildingPlacementValidation.BuildingPlacementComponent.Building = Building;
        }

        private void OnSetSlantPlanes()
        {
            SlantPlanes.AdjacentSiteSlantPlanesComponent.Site = Site;
            SlantPlanes.AdjacentSiteSlantPlanesComponent.Building = Building;

            SlantPlanes.AbsoluteHeightSlantPlanesComponent.Site = Site;
            SlantPlanes.AbsoluteHeightSlantPlanesComponent.Building = Building;

            SlantPlanes.RoadSlantPlanesComponent.Site = Site;
            SlantPlanes.RoadSlantPlanesComponent.Building = Building;

            SlantPlanes.NorthSlantPlanesComponent.Site = Site;
            SlantPlanes.NorthSlantPlanesComponent.Building = Building;
        }

        private void OnSetLandUseEvaluator()
        {
            LandUseEvaluator.BuildingCoverageRatioComponent.Building =
                Building;
            LandUseEvaluator.BuildingCoverageRatioComponent.Site = Site;

            LandUseEvaluator.FloorAreaRatioComponent.Building = Building;
            LandUseEvaluator.FloorAreaRatioComponent.Site = Site;

            FinanceEvaluator.ParkingLotFinanceComponent.LandUseEvaluator =
                LandUseEvaluator;
        }

        private void OnSetFinanceEvaluator()
        {
            FinanceEvaluator.ParkingLotFinanceComponent.ParkingLot =
                ParkingLot;
            FinanceEvaluator.ParkingLotFinanceComponent.LandUseEvaluator =
                LandUseEvaluator;
            FinanceEvaluator.SummaryFinanceComponent.Building = Building;
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            _unitCatalog.FilterUnitCatalogEntriesByFinance(_financeEvaluator.UnitFinanceComponent.CostAndrevenueEntries);

            writer.AddValue(nameof(UnitCatalog), _unitCatalog);
            writer.AddValue(nameof(Site), _site);
            writer.AddValue(nameof(Building), _building);
            writer.AddValue(nameof(ParkingLot), _parkingLot);
            writer.AddValue(nameof(SiteDesigner), _siteDesigner);
            writer.AddValue(nameof(BuildingDesigner), _buildingDesigner);
            writer.AddValue(nameof(ParkingLotDesigner), _parkingLotDesigner);
            writer.AddValue(nameof(Setback), _setback);
            writer.AddValue(nameof(BuildingPlacementValidation), _buildingPlacementValidation);
            writer.AddValue(nameof(SlantPlanes), _slantPlanes);
            writer.AddValue(nameof(LandUseEvaluator), _landUseEvaluator);
            writer.AddValue(nameof(FinanceEvaluator), _financeEvaluator);
        }

        #endregion

        #region Properties

        public UnitCatalog UnitCatalog
        {
            get => _unitCatalog;
            set
            {
                ReplaceSceneObject(_unitCatalog, value);
                _unitCatalog = value;

                OnSetUnitCatalog();

                NotifyPropertyChanged();
            }
        }

        public Site Site
        {
            get => _site;
            set
            {
                ReplaceSceneObject(_site, value);
                _site = value;

                OnSetSite();

                NotifyPropertyChanged();
            }
        }

        public Building Building
        {
            get => _building;
            set
            {
                ReplaceSceneObject(_building, value);
                _building = value;

                OnSetBuilding();

                NotifyPropertyChanged();
            }
        }

        public ParkingLot ParkingLot
        {
            get => _parkingLot;
            set
            {
                ReplaceSceneObject(_parkingLot, value);
                _parkingLot = value;

                OnSetParkingLot();

                NotifyPropertyChanged();
            }
        }

        public SiteDesigner SiteDesigner
        {
            get => _siteDesigner;
            set
            {
                ReplaceSceneObject(_siteDesigner, value);
                _siteDesigner = value;

                OnSetSiteDesigner();

                NotifyPropertyChanged();
            }
        }

        public BuildingDesigner BuildingDesigner
        {
            get => _buildingDesigner;
            set
            {
                ReplaceSceneObject(_buildingDesigner, value);
                _buildingDesigner = value;

                OnSetBuildingDesigner();

                NotifyPropertyChanged();
            }
        }

        public ParkingLotDesigner ParkingLotDesigner
        {
            get => _parkingLotDesigner;
            set
            {
                ReplaceSceneObject(_parkingLotDesigner, value);
                _parkingLotDesigner = value;

                OnSetParkingLotDesigner();

                NotifyPropertyChanged();
            }
        }

        public Setback Setback
        {
            get => _setback;
            set
            {
                ReplaceSceneObject(_setback, value);
                _setback = value;

                OnSetSetback();

                NotifyPropertyChanged();
            }
        }

        public SlantPlanes SlantPlanes
        {
            get => _slantPlanes;
            set
            {
                ReplaceSceneObject(_slantPlanes, value);
                _slantPlanes = value;

                OnSetSlantPlanes();

                NotifyPropertyChanged();
            }
        }

        public LandUseEvaluator LandUseEvaluator
        {
            get => _landUseEvaluator;
            set
            {
                ReplaceSceneObject(_landUseEvaluator, value);
                _landUseEvaluator = value;

                OnSetLandUseEvaluator();

                NotifyPropertyChanged();
            }
        }

        public FinanceEvaluator FinanceEvaluator
        {
            get => _financeEvaluator;
            set
            {
                ReplaceSceneObject(_financeEvaluator, value);
                _financeEvaluator = value;

                OnSetFinanceEvaluator();

                NotifyPropertyChanged();
            }
        }

        public BuildingPlacementValidation BuildingPlacementValidation
        {
            get => _buildingPlacementValidation;
            set
            {
                ReplaceSceneObject(_buildingPlacementValidation, value);
                _buildingPlacementValidation = value;

                OnSetBuildingPlacementValidation();

                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Member variables

        private UnitCatalog _unitCatalog = new UnitCatalog();

        private Site _site = new Site();

        private Building _building = new Building();

        private ParkingLot _parkingLot = new ParkingLot();

        private SiteDesigner _siteDesigner = new SiteDesigner();

        private BuildingDesigner _buildingDesigner = new BuildingDesigner();

        private ParkingLotDesigner _parkingLotDesigner =
            new ParkingLotDesigner();

        private Setback _setback = new Setback();

        private BuildingPlacementValidation _buildingPlacementValidation = new BuildingPlacementValidation();

        private SlantPlanes _slantPlanes = new SlantPlanes();

        private LandUseEvaluator _landUseEvaluator = new LandUseEvaluator();

        private FinanceEvaluator _financeEvaluator = new FinanceEvaluator();

        #endregion
    }
}
