using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeA;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeB;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeC;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign
{
    /// <summary>
    /// View model for unit catalog inputs.
    /// </summary>
    public class UnitCatalogInputsViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public UnitCatalogInputsViewModel(GDModelScene gdms) : base(gdms)
        {
            UpdateEntryViewModels();

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
                UnitCatalogJsonComponent.Load();

                LoadMessage = LoadSuccessMessage;
            }
            catch
            {
                LoadMessage = LoadFailMessage;
                return;
            }

            UpdateEntryViewModels();
        }

        private void UpdateEntryViewModels()
        {
            _entryViewModels = new List<UnitCatalogEntryViewModel>();

            foreach (var entry in UnitCatalogComponent.Entries)
            {
                var createEntryViewModel =
                    _createEntryViewModelDict[entry.GetType()];

                var entryViewModel = createEntryViewModel(entry);

                _entryViewModels.Add(entryViewModel);
            }

            NotifyPropertyChanged(nameof(EntryViewModels));
            NotifyPropertyChanged(nameof(TypeAEntryViewModels));
            NotifyPropertyChanged(nameof(TypeBEntryViewModels));
            NotifyPropertyChanged(nameof(TypeCEntryViewModels));

            UpdateGDModelScene();
        }

        private void UpdateSelectedEntryViewportViewModel()
        {
            if (SelectedEntryViewModel == null)
            {
                SelectedEntryViewportViewModel = null;
            }
            else
            {
                SelectedEntryViewportViewModel =
                    new UnitCatalogEntryViewportViewModel(
                        SelectedEntryViewModel.CatalogUnitComponent
                    );
            }
        }

        #endregion

        #region Properties

        private UnitCatalogComponent UnitCatalogComponent =>
            GDModelScene.UnitCatalog.UnitCatalogComponent;

        private UnitCatalogJsonComponent UnitCatalogJsonComponent =>
            GDModelScene.UnitCatalog.UnitCatalogJsonComponent;

        public string UnitCatalogJsonFileFullPath =>
            UnitCatalogJsonComponent.JsonFileFullPath;

        public IReadOnlyList<UnitCatalogEntryViewModel> EntryViewModels =>
            _entryViewModels;

        public IReadOnlyList<TypeAUnitCatalogEntryViewModel>
            TypeAEntryViewModels =>
            EntryViewModels.OfType<TypeAUnitCatalogEntryViewModel>().ToList();

        public IReadOnlyList<TypeBUnitCatalogEntryViewModel>
            TypeBEntryViewModels =>
            EntryViewModels.OfType<TypeBUnitCatalogEntryViewModel>().ToList();

        public IReadOnlyList<TypeCUnitCatalogEntryViewModel>
            TypeCEntryViewModels =>
            EntryViewModels.OfType<TypeCUnitCatalogEntryViewModel>().ToList();

        public UnitCatalogEntryViewModel SelectedEntryViewModel
        {
            get => _selectedEntryViewModel;
            set
            {
                _selectedEntryViewModel = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(SelectedEntryDisplayName));

                UpdateSelectedEntryViewportViewModel();
            }
        }

        public string SelectedEntryDisplayName =>
            string.Format(
                "Unit Catalog Entry {0}",
                SelectedEntryViewModel?.FullName ?? "not selected"
            );

        public UnitCatalogEntryViewportViewModel
            SelectedEntryViewportViewModel
        {
            get => _selectedEntryViewportViewModel;
            set
            {
                _selectedEntryViewportViewModel = value;

                NotifyPropertyChanged();
            }
        }

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

        private UnitCatalogEntryViewModel _selectedEntryViewModel;

        private List<UnitCatalogEntryViewModel> _entryViewModels =
            new List<UnitCatalogEntryViewModel>();

        private UnitCatalogEntryViewportViewModel
            _selectedEntryViewportViewModel;

        private static readonly
            Dictionary
            <Type, Func<CatalogUnitComponent, UnitCatalogEntryViewModel>>
            _createEntryViewModelDict =
            new Dictionary
            <Type, Func<CatalogUnitComponent, UnitCatalogEntryViewModel>>
            {
                {
                    typeof(TypeAUnitComponent),
                    (uc) => new TypeAUnitCatalogEntryViewModel(
                        (TypeAUnitComponent)uc
                    )
                },
                {
                    typeof(TypeBUnitComponent),
                    (uc) => new TypeBUnitCatalogEntryViewModel(
                        (TypeBUnitComponent)uc
                    )
                },
                {
                    typeof(TypeCUnitComponent),
                    (uc) => new TypeCUnitCatalogEntryViewModel(
                        (TypeCUnitComponent)uc
                    )
                }
            };

        #endregion

        #region Constants

        public const string LoadSuccessMessage =
            "Unit catalog is loaded.";

        public const string LoadFailMessage =
            "Failed to load unit catalog.";

        #endregion
    }
}
