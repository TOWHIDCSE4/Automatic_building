using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Finance
{
    /// <summary>
    /// Calculates the summary financial information of the model.
    /// </summary>
    [Serializable]
    public class SummaryFinanceComponent : Component
    {
        #region Constructors

        public SummaryFinanceComponent() : base()
        { }

        protected SummaryFinanceComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _building = reader.GetValue<Building>(nameof(Building));

            _totalRevenueYenPerYear =
                reader.GetValue<double>(nameof(TotalRevenueYenPerYear));

            _buildingRevenueYenPerYear =
                reader.GetValue<double>(nameof(BuildingRevenueYenPerYear));

            _totalCostYen = reader.GetValue<double>(nameof(TotalCostYen));

            _buildingCostYen =
                reader.GetValue<double>(nameof(BuildingCostYen));

            _grossRorPerYear =
                reader.GetValue<double>(nameof(GrossRorPerYear));
        }

        #endregion

        #region Methods

        #region Cost

        public double GetBuildingCostYen()
        {
            if (Building == null || UnitFinanceComponent == null)
            {
                return 0.0;
            }

            return UnitFinanceComponent.GetBuildingCostYen(Building);
        }

        public double GetTotalCostYen()
        {
            double totalCostYen = 0.0;

            totalCostYen += GetBuildingCostYen();
            totalCostYen += ParkingLotFinanceComponent?.CostYen ?? 0.0;

            return totalCostYen;
        }

        #endregion

        #region Revenue

        public double GetBuildingRevenueYenPerMonth()
        {
            if (Building == null || UnitFinanceComponent == null)
            {
                return 0.0;
            }

            return
                UnitFinanceComponent.GetBuildingRevenueYenPerMonth(Building);
        }

        public double GetBuildingRevenueYenPerYear()
        {
            return GetBuildingRevenueYenPerMonth() * NumOfMonthsPerYear;
        }

        public double GetTotalRevenueYenPerMonth()
        {
            double totalRevenueYenPerMonth = 0.0;

            totalRevenueYenPerMonth += GetBuildingRevenueYenPerMonth();
            totalRevenueYenPerMonth +=
                ParkingLotFinanceComponent?.RevenueYenPerMonth ?? 0.0;

            return totalRevenueYenPerMonth;
        }

        public double GetTotalRevenueYenPerYear()
        {
            return GetTotalRevenueYenPerMonth() * NumOfMonthsPerYear;
        }

        #endregion

        #region Gross Rate of Return (RoR)

        public double GetGrossRorPerMonth()
        {
            double totalCostYen = GetTotalCostYen();

            double totalRevenueYenPerMonth = GetTotalRevenueYenPerMonth();

            double grossRorPerMonth = totalRevenueYenPerMonth / totalCostYen;
            return grossRorPerMonth;
        }

        public double GetGrossRorPerYear()
        {
            return GetGrossRorPerMonth() * NumOfMonthsPerYear;
        }

        #endregion

        protected override void Update()
        {
            TotalRevenueYenPerYear = double.NaN;
            BuildingRevenueYenPerYear = double.NaN;

            TotalCostYen = double.NaN;
            BuildingCostYen = double.NaN;

            GrossRorPerYear = double.NaN;

            if (Building != null)
            {
                TotalRevenueYenPerYear = GetTotalRevenueYenPerYear();
                BuildingRevenueYenPerYear = GetBuildingRevenueYenPerYear();

                TotalCostYen = GetTotalCostYen();
                BuildingCostYen = GetBuildingCostYen();

                GrossRorPerYear = GetGrossRorPerYear();
            }
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(Building);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Building), _building);

            writer.AddValue(
                nameof(TotalRevenueYenPerYear), _totalRevenueYenPerYear
            );

            writer.AddValue(
                nameof(BuildingRevenueYenPerYear), _buildingRevenueYenPerYear
            );

            writer.AddValue(nameof(TotalCostYen), _totalCostYen);

            writer.AddValue(nameof(BuildingCostYen), _buildingCostYen);

            writer.AddValue(nameof(GrossRorPerYear), _grossRorPerYear);
        }

        #endregion

        #region Properties

        public UnitFinanceComponent UnitFinanceComponent
        {
            get => SceneObject?.GetComponent<UnitFinanceComponent>();
        }

        public ParkingLotFinanceComponent ParkingLotFinanceComponent =>
            SceneObject?.GetComponent<ParkingLotFinanceComponent>();

        public Building Building
        {
            get => _building;
            set
            {
                _building = value;
                NotifyPropertyChanged();
            }
        }

        public double TotalRevenueYenPerYear
        {
            get => _totalRevenueYenPerYear;
            private set
            {
                _totalRevenueYenPerYear = value;
                NotifyPropertyChanged();
            }
        }

        public double BuildingRevenueYenPerYear
        {
            get => _buildingRevenueYenPerYear;
            private set
            {
                _buildingRevenueYenPerYear = value;
                NotifyPropertyChanged();
            }
        }

        public double TotalCostYen
        {
            get => _totalCostYen;
            private set
            {
                _totalCostYen = value;
                NotifyPropertyChanged();
            }
        }

        public double BuildingCostYen
        {
            get => _buildingCostYen;
            private set
            {
                _buildingCostYen = value;
                NotifyPropertyChanged();
            }
        }

        public double GrossRorPerYear
        {
            get => _grossRorPerYear;
            private set
            {
                _grossRorPerYear = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private Building _building;

        private double _totalRevenueYenPerYear = double.NaN;
        private double _buildingRevenueYenPerYear = double.NaN;

        private double _totalCostYen = double.NaN;
        private double _buildingCostYen = double.NaN;

        private double _grossRorPerYear = double.NaN;

        #endregion

        #region Constants

        public const int NumOfMonthsPerYear = 12;

        #endregion
    }
}
