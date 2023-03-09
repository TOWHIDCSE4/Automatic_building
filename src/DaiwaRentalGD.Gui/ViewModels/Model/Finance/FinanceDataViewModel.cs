using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model.Finance
{
    /// <summary>
    /// View model for data related to finance.
    /// </summary>
    public class FinanceDataViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public FinanceDataViewModel(GDModelScene gdms) : base(gdms)
        { }

        #endregion

        #region Methods

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(TotalRevenueYenPerYear));
            NotifyPropertyChanged(nameof(BuildingRevenueYenPerYear));
            NotifyPropertyChanged(nameof(ParkingLotRevenueYenPerYear));

            NotifyPropertyChanged(nameof(TotalCostYen));
            NotifyPropertyChanged(nameof(BuildingCostYen));
            NotifyPropertyChanged(nameof(ParkingLotCostYen));

            NotifyPropertyChanged(nameof(GrossRorPerYear));
        }

        #endregion

        #region Properties

        public double? TotalRevenueYenPerYear =>
            GDModelScene?.FinanceEvaluator.SummaryFinanceComponent
            .TotalRevenueYenPerYear;

        public double? BuildingRevenueYenPerYear =>
            GDModelScene?.FinanceEvaluator.SummaryFinanceComponent
            .BuildingRevenueYenPerYear;

        public double? ParkingLotRevenueYenPerYear =>
            GDModelScene?.FinanceEvaluator.ParkingLotFinanceComponent
            .RevenueYenPerYear;

        public double? TotalCostYen =>
            GDModelScene?.FinanceEvaluator.SummaryFinanceComponent
            .TotalCostYen;

        public double? BuildingCostYen =>
            GDModelScene?.FinanceEvaluator.SummaryFinanceComponent
            .BuildingCostYen;


        public double? ParkingLotCostYen =>
            GDModelScene?.FinanceEvaluator.ParkingLotFinanceComponent
            .CostYen;

        public double? GrossRorPerYear =>
            GDModelScene?.FinanceEvaluator.SummaryFinanceComponent
            .GrossRorPerYear;

        #endregion
    }
}
