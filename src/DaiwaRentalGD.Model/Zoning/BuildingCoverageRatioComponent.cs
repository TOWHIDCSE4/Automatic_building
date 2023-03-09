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
    /// Calculates the Building Coverage Ratio (BCR) of a building on a site.
    /// </summary>
    [Serializable]
    public class BuildingCoverageRatioComponent : Component
    {
        #region Constructors

        public BuildingCoverageRatioComponent() : base()
        { }

        protected BuildingCoverageRatioComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _site = reader.GetValue<Site>(nameof(Site));

            _building = reader.GetValue<Building>(nameof(Building));

            _siteArea = reader.GetValue<double>(nameof(SiteArea));

            _buildingArea = reader.GetValue<double>(nameof(BuildingArea));

            _buildingCoverageRatio =
                reader.GetValue<double>(nameof(BuildingCoverageRatio));
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

        public double GetBuildingArea(Building building)
        {
            var bc = building.BuildingComponent;

            if (bc.NumOfFloors == 0)
            {
                return 0.0;
            }

            double buildingArea = 0.0;

            foreach (Unit unit in bc.GetFloorUnits(0))
            {
                if (unit == null) { continue; }

                buildingArea += GetUnitArea(unit);
            }

            foreach (var staircase in bc.GetFloorStaircases(0))
            {
                if (staircase == null)
                {
                    continue;
                }

                double staircaseArea =
                    staircase.StaircaseComponent.GetPlan().Area;

                buildingArea += staircaseArea;
            }

            foreach (var corridor in bc.GetFloorCorridors(0))
            {
                if (corridor == null)
                {
                    continue;
                }

                double corridorArea =
                    corridor.CorridorComponent.Plan.Area;

                buildingArea += corridorArea;
            }

            foreach (var balcony in bc.GetFloorBalconies(0))
            {
                if (balcony == null)
                {
                    continue;
                }

                double balconyArea =
                    balcony.BalconyComponent.GetPlan().Area;

                buildingArea += balconyArea;
            }

            return buildingArea;
        }

        public double GetBuildingCoverageRatio(Site site, Building building)
        {
            double siteArea = GetSiteArea(site);

            if (siteArea == 0.0) { return 0.0; }

            double buildingArea = GetBuildingArea(building);

            double buildingCoverageRatio = buildingArea / siteArea;
            return buildingCoverageRatio;
        }

        protected override void Update()
        {
            SiteArea = double.NaN;
            BuildingArea = double.NaN;
            BuildingCoverageRatio = double.NaN;

            if (Site != null && Building != null)
            {
                SiteArea = GetSiteArea(Site);
                BuildingArea = GetBuildingArea(Building);

                BuildingCoverageRatio =
                    GetBuildingCoverageRatio(Site, Building);
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

            writer.AddValue(nameof(BuildingArea), _buildingArea);

            writer.AddValue(
                nameof(BuildingCoverageRatio),
                _buildingCoverageRatio
            );
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

        public double BuildingArea
        {
            get => _buildingArea;
            private set
            {
                _buildingArea = value;
                NotifyPropertyChanged();
            }
        }

        public double BuildingCoverageRatio
        {
            get => _buildingCoverageRatio;
            private set
            {
                _buildingCoverageRatio = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private Site _site;
        private Building _building;

        private double _siteArea = double.NaN;
        private double _buildingArea = double.NaN;
        private double _buildingCoverageRatio = double.NaN;

        #endregion
    }
}
