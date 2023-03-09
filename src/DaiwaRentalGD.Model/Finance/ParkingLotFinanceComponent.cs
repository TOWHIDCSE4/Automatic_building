using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ParkingLotDesign;
using DaiwaRentalGD.Model.Zoning;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Finance
{
    /// <summary>
    /// Calculates the financial metrics of the parking lot.
    /// </summary>
    [Serializable]
    public class ParkingLotFinanceComponent : Component
    {
        #region Constructors

        public ParkingLotFinanceComponent() : base()
        { }

        protected ParkingLotFinanceComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _parkingLot = reader.GetValue<ParkingLot>(nameof(ParkingLot));

            _landUseEvaluator =
                reader.GetValue<LandUseEvaluator>(nameof(LandUseEvaluator));

            _costYenPerSqm = reader.GetValue<double>(nameof(CostYenPerSqm));
            _costYen = reader.GetValue<double>(nameof(CostYen));

            _revenueYenPerCarParkingSpacePerMonth = reader.GetValue<double>(
                nameof(RevenueYenPerCarParkingSpacePerMonth)
            );
            _revenueYenPerMonth =
                reader.GetValue<double>(nameof(RevenueYenPerMonth));
            _revenueYenPerYear =
                reader.GetValue<double>(nameof(RevenueYenPerYear));
        }

        #endregion

        #region Methods

        public double GetCostYen()
        {
            return CostYenPerSqm * ParkingLotGrossArea;
        }

        public double GetRevenueYenPerMonth()
        {
            double revenueYenPerMonth =
                RevenueYenPerCarParkingSpacePerMonth *
                NumOfCarParkingSpaces;

            return revenueYenPerMonth;
        }

        public double GetRevenueYenPerYear()
        {
            return GetRevenueYenPerMonth() * NumOfMonthsPerYear;
        }

        protected override void Update()
        {
            CostYen = GetCostYen();
            RevenueYenPerMonth = GetRevenueYenPerMonth();
            RevenueYenPerYear = GetRevenueYenPerYear();
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(ParkingLot).Append(LandUseEvaluator);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(ParkingLot), _parkingLot);

            writer.AddValue(nameof(LandUseEvaluator), _landUseEvaluator);

            writer.AddValue(nameof(CostYenPerSqm), _costYenPerSqm);
            writer.AddValue(nameof(CostYen), _costYen);

            writer.AddValue(
                nameof(RevenueYenPerCarParkingSpacePerMonth),
                _revenueYenPerCarParkingSpacePerMonth
            );
            writer.AddValue(nameof(RevenueYenPerMonth), _revenueYenPerMonth);
            writer.AddValue(nameof(RevenueYenPerYear), _revenueYenPerYear);
        }

        #endregion

        #region Properties

        public ParkingLot ParkingLot
        {
            get => _parkingLot;
            set
            {
                _parkingLot = value;
                NotifyPropertyChanged();
            }
        }

        public LandUseEvaluator LandUseEvaluator
        {
            get => _landUseEvaluator;
            set
            {
                _landUseEvaluator = value;
                NotifyPropertyChanged();
            }
        }

        #region Cost

        public double ParkingLotGrossArea
        {
            get
            {
                if (LandUseEvaluator == null)
                {
                    return 0.0;
                }

                var bcrc = LandUseEvaluator.BuildingCoverageRatioComponent;

                double parkingLotGrossArea =
                    bcrc.SiteArea - bcrc.BuildingArea;

                return parkingLotGrossArea;
            }
        }

        public double CostYenPerSqm
        {
            get => _costYenPerSqm;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(CostYenPerSqm)} cannot be negative"
                    );
                }

                _costYenPerSqm = value;
                NotifyPropertyChanged();
            }
        }

        public double CostYen
        {
            get => _costYen;
            private set
            {
                _costYen = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Revenue

        public int NumOfCarParkingSpaces =>
            ParkingLot?.ParkingLotComponent.NumOfCarParkingSpaces ?? 0;

        public double RevenueYenPerCarParkingSpacePerMonth
        {
            get => _revenueYenPerCarParkingSpacePerMonth;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(RevenueYenPerCarParkingSpacePerMonth)} " +
                        "cannot be negative"
                    );
                }

                _revenueYenPerCarParkingSpacePerMonth = value;
                NotifyPropertyChanged();
            }
        }

        public double RevenueYenPerMonth
        {
            get => _revenueYenPerMonth;
            private set
            {
                _revenueYenPerMonth = value;
                NotifyPropertyChanged();
            }
        }

        public double RevenueYenPerYear
        {
            get => _revenueYenPerYear;
            private set
            {
                _revenueYenPerYear = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Member variables

        private ParkingLot _parkingLot;

        private LandUseEvaluator _landUseEvaluator;

        private double _costYenPerSqm = DefaultCostYenPerSqm;
        private double _costYen;

        private double _revenueYenPerCarParkingSpacePerMonth =
            DefaultRevenueYenPerCarParkingSpacePerMonth;
        private double _revenueYenPerMonth;
        private double _revenueYenPerYear;

        #endregion

        #region Constants

        public const int NumOfMonthsPerYear = 12;

        public const double DefaultCostYenPerSqm = 10000.0;

        public const double DefaultRevenueYenPerCarParkingSpacePerMonth =
            10000.0;

        #endregion
    }
}
