using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.ParkingLotDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.ParkingLotDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign
    /// .ParkingLotRequirementsComponent"/>.
    /// </summary>
    public class ParkingLotRequirementsViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public ParkingLotRequirementsViewModel(GDModelScene gdms) : base(gdms)
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
                ParkingRequirementsJsonComponent.Load();

                LoadMessage = LoadSuccessMessage;
            }
            catch
            {
                LoadMessage = LoadFailMessage;
                return;
            }

            UpdateGDModelScene();

            NotifyPropertyChanged(nameof(UnitRequirementsList));

            NotifyPropertyChanged(nameof(CarParkingSpaceMinTotal));
            NotifyPropertyChanged(nameof(CarParkingSpaceMaxTotal));
            NotifyPropertyChanged(nameof(AutoCarParkingSpaceMinTotal));
            NotifyPropertyChanged(nameof(AutoCarParkingSpaceMaxTotal));

            NotifyPropertyChanged(nameof(BicycleParkingSpaceMinTotal));
            NotifyPropertyChanged(nameof(BicycleParkingSpaceMaxTotal));
            NotifyPropertyChanged(nameof(AutoBicycleParkingSpaceMinTotal));
            NotifyPropertyChanged(nameof(AutoBicycleParkingSpaceMaxTotal));
        }

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(OverrideCarParkingSpaceMinTotal));
            NotifyPropertyChanged(nameof(AutoCarParkingSpaceMinTotal));
            NotifyPropertyChanged(nameof(UseOverrideCarParkingSpaceMinTotal));
            NotifyPropertyChanged(nameof(CarParkingSpaceMinTotal));

            NotifyPropertyChanged(nameof(OverrideCarParkingSpaceMaxTotal));
            NotifyPropertyChanged(nameof(AutoCarParkingSpaceMaxTotal));
            NotifyPropertyChanged(nameof(UseOverrideCarParkingSpaceMaxTotal));
            NotifyPropertyChanged(nameof(CarParkingSpaceMaxTotal));

            NotifyPropertyChanged(
                nameof(OverrideBicycleParkingSpaceMinTotal)
            );
            NotifyPropertyChanged(
                nameof(AutoBicycleParkingSpaceMinTotal)
            );
            NotifyPropertyChanged(
                nameof(UseOverrideBicycleParkingSpaceMinTotal)
            );
            NotifyPropertyChanged(
                nameof(BicycleParkingSpaceMinTotal)
            );

            NotifyPropertyChanged(
                nameof(OverrideBicycleParkingSpaceMaxTotal)
            );
            NotifyPropertyChanged(
                nameof(AutoBicycleParkingSpaceMaxTotal)
            );
            NotifyPropertyChanged(
                nameof(UseOverrideBicycleParkingSpaceMaxTotal)
            );
            NotifyPropertyChanged(
                nameof(BicycleParkingSpaceMaxTotal)
            );
        }

        #endregion

        #region Properties

        private ParkingLotRequirementsComponent
            ParkingLotRequirementsComponent =>
            GDModelScene?.ParkingLotDesigner.ParkingLotRequirementsComponent;

        private ParkingRequirementsJsonComponent
            ParkingRequirementsJsonComponent =>
            GDModelScene?.ParkingLotDesigner.ParkingRequirementsJsonComponent;

        public string ParkingRequirementsJsonFileFullPath =>
            ParkingRequirementsJsonComponent?.JsonFileFullPath;

        public IReadOnlyList<UnitParkingRequirements> UnitRequirementsList
        {
            get
            {
                var uprList = ParkingLotRequirementsComponent
                    .UnitRequirementsTable.Values
                    .OrderBy(upr => upr.NumOfBedrooms)
                    .ToList().AsReadOnly();

                return uprList;
            }
        }

        #region Car parking

        public double? OverrideCarParkingSpaceMinTotal
        {
            get => ParkingLotRequirementsComponent?
                .OverrideCarParkingSpaceMinTotal;
            set
            {
                if (ParkingLotRequirementsComponent == null)
                {
                    return;
                }

                ParkingLotRequirementsComponent
                    .OverrideCarParkingSpaceMinTotal = (double)value;

                UpdateGDModelScene();
            }
        }

        public double? AutoCarParkingSpaceMinTotal =>
            ParkingLotRequirementsComponent?.AutoCarParkingSpaceMinTotal;

        public bool? UseOverrideCarParkingSpaceMinTotal
        {
            get => ParkingLotRequirementsComponent?
                .UseOverrideCarParkingSpaceMinTotal;
            set
            {
                if (ParkingLotRequirementsComponent == null)
                {
                    return;
                }

                ParkingLotRequirementsComponent
                    .UseOverrideCarParkingSpaceMinTotal = (bool)value;

                UpdateGDModelScene();
            }
        }

        public double? CarParkingSpaceMinTotal =>
            ParkingLotRequirementsComponent?.CarParkingSpaceMinTotal;

        public double? OverrideCarParkingSpaceMaxTotal
        {
            get => ParkingLotRequirementsComponent?
                .OverrideCarParkingSpaceMaxTotal;
            set
            {
                if (ParkingLotRequirementsComponent == null)
                {
                    return;
                }

                ParkingLotRequirementsComponent
                    .OverrideCarParkingSpaceMaxTotal = (double)value;

                UpdateGDModelScene();
            }
        }

        public double? AutoCarParkingSpaceMaxTotal =>
            ParkingLotRequirementsComponent?.AutoCarParkingSpaceMaxTotal;

        public bool? UseOverrideCarParkingSpaceMaxTotal
        {
            get => ParkingLotRequirementsComponent?
                .UseOverrideCarParkingSpaceMaxTotal;
            set
            {
                if (ParkingLotRequirementsComponent == null)
                {
                    return;
                }

                ParkingLotRequirementsComponent
                    .UseOverrideCarParkingSpaceMaxTotal = (bool)value;

                UpdateGDModelScene();
            }
        }

        public double? CarParkingSpaceMaxTotal =>
            ParkingLotRequirementsComponent?.CarParkingSpaceMaxTotal;

        #endregion

        #region Bicycle parking

        public double? OverrideBicycleParkingSpaceMinTotal
        {
            get => ParkingLotRequirementsComponent?
                .OverrideBicycleParkingSpaceMinTotal;
            set
            {
                if (ParkingLotRequirementsComponent == null)
                {
                    return;
                }

                ParkingLotRequirementsComponent
                    .OverrideBicycleParkingSpaceMinTotal = (double)value;

                UpdateGDModelScene();
            }
        }

        public double? AutoBicycleParkingSpaceMinTotal =>
            ParkingLotRequirementsComponent?.AutoBicycleParkingSpaceMinTotal;

        public bool? UseOverrideBicycleParkingSpaceMinTotal
        {
            get => ParkingLotRequirementsComponent?
                .UseOverrideBicycleParkingSpaceMinTotal;
            set
            {
                if (ParkingLotRequirementsComponent == null)
                {
                    return;
                }

                ParkingLotRequirementsComponent
                    .UseOverrideBicycleParkingSpaceMinTotal = (bool)value;

                UpdateGDModelScene();
            }
        }

        public double? BicycleParkingSpaceMinTotal =>
            ParkingLotRequirementsComponent?.BicycleParkingSpaceMinTotal;

        public double? OverrideBicycleParkingSpaceMaxTotal
        {
            get => ParkingLotRequirementsComponent?
                .OverrideBicycleParkingSpaceMaxTotal;
            set
            {
                if (ParkingLotRequirementsComponent == null)
                {
                    return;
                }

                ParkingLotRequirementsComponent
                    .OverrideBicycleParkingSpaceMaxTotal = (double)value;

                UpdateGDModelScene();
            }
        }

        public double? AutoBicycleParkingSpaceMaxTotal =>
            ParkingLotRequirementsComponent?.AutoBicycleParkingSpaceMaxTotal;

        public bool? UseOverrideBicycleParkingSpaceMaxTotal
        {
            get => ParkingLotRequirementsComponent?
                .UseOverrideBicycleParkingSpaceMaxTotal;
            set
            {
                if (ParkingLotRequirementsComponent == null)
                {
                    return;
                }

                ParkingLotRequirementsComponent
                    .UseOverrideBicycleParkingSpaceMaxTotal = (bool)value;

                UpdateGDModelScene();
            }
        }

        public double? BicycleParkingSpaceMaxTotal =>
            ParkingLotRequirementsComponent?.BicycleParkingSpaceMaxTotal;

        #endregion

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
            "Parking requirements data is loaded.";

        public const string LoadFailMessage =
            "Failed to load parking requirements data.";

        #endregion
    }
}
