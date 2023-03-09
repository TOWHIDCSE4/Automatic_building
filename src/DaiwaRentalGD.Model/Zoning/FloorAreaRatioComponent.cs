using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Zoning
{
    /// <summary>
    /// Calculates the Floor Area Ratio (FAR) of a building on a site.
    /// </summary>
    [Serializable]
    public class FloorAreaRatioComponent : Component
    {
        #region Constructors

        public FloorAreaRatioComponent() : base()
        { }

        protected FloorAreaRatioComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _site = reader.GetValue<Site>(nameof(Site));

            _building = reader.GetValue<Building>(nameof(Building));

            _siteArea = reader.GetValue<double>(nameof(SiteArea));

            _totalFloorArea = reader.GetValue<double>(nameof(TotalFloorArea));

            _floorAreaRatio = reader.GetValue<double>(nameof(FloorAreaRatio));
        }

        #endregion

        #region Methods

        public double GetSiteArea(Site site)
        {
            Polygon siteBoundary = site.SiteComponent.Boundary;

            double siteArea = siteBoundary.Area;
            return siteArea;
        }

        public double GetUnitArea(Unit unit)
        {
            UnitComponent uc = unit.UnitComponent;

            double unitArea = 0.0;

            foreach (Polygon roomPlan in uc.RoomPlans)
            {
                unitArea += roomPlan.Area;
            }

            return unitArea;
        }

        public double GetTotalFloorArea(Building building)
        {
            var bc = building.BuildingComponent;

            double totalFloorArea = 0.0;

            foreach (Unit unit in bc.Units)
            {
                if (unit == null)
                {
                    continue;
                }

                totalFloorArea += GetUnitArea(unit);
            }

            foreach (var staircase in bc.Staircases)
            {
                if (staircase == null)
                {
                    continue;
                }

                double staircaseArea =
                    staircase.StaircaseComponent.GetPlan().Area;

                totalFloorArea += staircaseArea;
            }

            foreach (var corridor in bc.Corridors)
            {
                if (corridor == null)
                {
                    continue;
                }

                double corridorArea =
                    corridor.CorridorComponent.Plan.Area;

                totalFloorArea += corridorArea;
            }

            return totalFloorArea;
        }

        public double GetFloorAreaRatio(Site site, Building building)
        {
            double siteArea = GetSiteArea(site);

            if (siteArea == 0.0) { return 0.0; }

            double totalFloorArea = GetTotalFloorArea(building);

            double floorAreaRatio = totalFloorArea / siteArea;
            return floorAreaRatio;
        }

        protected override void Update()
        {
            SiteArea = double.NaN;
            TotalFloorArea = double.NaN;
            FloorAreaRatio = double.NaN;

            if (Site != null && Building != null)
            {
                SiteArea = GetSiteArea(Site);
                TotalFloorArea = GetTotalFloorArea(Building);

                FloorAreaRatio = GetFloorAreaRatio(Site, Building);
            }
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(Site).Append(Building);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Site), _site);

            writer.AddValue(nameof(Building), _building);

            writer.AddValue(nameof(SiteArea), _siteArea);

            writer.AddValue(nameof(TotalFloorArea), _totalFloorArea);

            writer.AddValue(nameof(FloorAreaRatio), _floorAreaRatio);
        }

        #endregion

        #region Properties

        public Site Site
        {
            get => _site;
            set
            {
                _site = value;
                NotifyPropertyChanged();
            }
        }

        public Building Building
        {
            get => _building;
            set
            {
                _building = value;
                NotifyPropertyChanged();
            }
        }

        public double SiteArea
        {
            get => _siteArea;
            private set
            {
                _siteArea = value;
                NotifyPropertyChanged();
            }
        }

        public double TotalFloorArea
        {
            get => _totalFloorArea;
            private set
            {
                _totalFloorArea = value;
                NotifyPropertyChanged();
            }
        }

        public double FloorAreaRatio
        {
            get => _floorAreaRatio;
            private set
            {
                _floorAreaRatio = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private Site _site;
        private Building _building;

        private double _siteArea = double.NaN;
        private double _totalFloorArea = double.NaN;
        private double _floorAreaRatio = double.NaN;

        #endregion
    }
}
