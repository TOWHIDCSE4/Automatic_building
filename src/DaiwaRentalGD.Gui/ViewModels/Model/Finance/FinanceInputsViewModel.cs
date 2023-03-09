using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.Finance;

namespace DaiwaRentalGD.Gui.ViewModels.Model.Finance
{
    /// <summary>
    /// View model for finance-related components.
    /// </summary>
    public class FinanceInputsViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public FinanceInputsViewModel(GDModelScene gdms) : base(gdms)
        {
            InitializeCommands();
        }

        #endregion

        #region Methods

        private void InitializeCommands()
        {
            LoadDataCommand = new RelayCommand
            {
                ExecuteAction = LoadDataAction
            };
        }

        private void LoadDataAction(object parameter)
        {
            try
            {
                 FinanceDataJsonComponent.Load(); 

                LoadMessage = LoadSuccessMessage;
            }
            catch
            {
                LoadMessage = LoadFailMessage;
                return;
            }

            UpdateGDModelScene();

            NotifyPropertyChanged(nameof(UnitCostsAndRevenuesEntries));
            NotifyPropertyChanged(nameof(ParkingLotCostYenPerSqm));
            NotifyPropertyChanged(
                nameof(RevenueYenPerCarParkingSpacePerMonth)
            );
        }

        #endregion

        #region Properties

        private FinanceDataJsonComponent FinanceDataJsonComponent =>
            GDModelScene.FinanceEvaluator.FinanceDataJsonComponent;

        private UnitFinanceComponent UnitFinanceComponent =>
            GDModelScene.FinanceEvaluator.UnitFinanceComponent;

        private ParkingLotFinanceComponent ParkingLotFinanceComponent =>
            GDModelScene.FinanceEvaluator.ParkingLotFinanceComponent;

        public string FinanceDataJsonFileFullPath =>
            FinanceDataJsonComponent.JsonFileFullPath;

        public IReadOnlyList<UnitCostsAndRevenuesEntry> UnitCostsAndRevenuesEntries =>
            UnitFinanceComponent.CostAndrevenueEntries
            .OrderBy(
                ure => new Tuple<int, double>(
                    ure.NumOfBedrooms, ure.MaxArea
                )
            ).ToList().AsReadOnly();

        public double ParkingLotCostYenPerSqm =>
            ParkingLotFinanceComponent.CostYenPerSqm;

        public double RevenueYenPerCarParkingSpacePerMonth =>
            ParkingLotFinanceComponent.RevenueYenPerCarParkingSpacePerMonth;

        public string LoadMessage
        {
            get => _loadMessage;
            set
            {
                _loadMessage = value ??
                    throw new ArgumentNullException(nameof(value));

                NotifyPropertyChanged();
            }
        }

        public ICommand LoadDataCommand { get; private set; }

        #endregion

        #region Member variables

        private string _loadMessage = string.Empty;

        #endregion

        #region Constants

        public const string LoadSuccessMessage =
            "Financial data is loaded.";

        public const string LoadFailMessage =
            "Failed to load financial data.";

        #endregion
    }
}
