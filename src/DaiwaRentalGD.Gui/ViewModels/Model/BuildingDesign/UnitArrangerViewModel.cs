using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign;

namespace DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign
{
    /// <summary>
    /// View model for
    /// <see cref="DaiwaRentalGD.Model.BuildingDesign
    /// .UnitArrangerComponent"/>.
    /// </summary>
    public class UnitArrangerViewModel : GDModelSceneViewModelBase
    {
        /// <summary>
        /// View model for a normalized entry index into unit catalog.
        /// </summary>
        public class NormalizedEntryIndexViewModel : GDModelSceneViewModelBase
        {
            #region Constructors

            internal NormalizedEntryIndexViewModel(
                UnitArrangerViewModel uavm, int eii
            ) : base(uavm.GDModelScene)
            {
                UnitArrangerViewModel = uavm ??
                    throw new ArgumentNullException(nameof(uavm));

                EntryIndexIndex = eii;
            }

            #endregion

            #region Methods

            protected override void NotifyAllGDModelScenePropertiesChanged()
            {
                NotifyPropertyChanged(nameof(NormalizedEntryIndex));
            }

            #endregion

            #region Properties

            public UnitArrangerViewModel UnitArrangerViewModel { get; }

            public int EntryIndexIndex { get; }

            public double? NormalizedEntryIndex
            {
                get => UnitArrangerViewModel.UnitArrangerComponent?
                    .NormalizedEntryIndices[EntryIndexIndex];
                set
                {
                    if (UnitArrangerViewModel.UnitArrangerComponent == null)
                    {
                        return;
                    }

                    UnitArrangerViewModel.UnitArrangerComponent
                        .SetNormalizedEntryIndex(
                            EntryIndexIndex, (double)value
                        );

                    UpdateGDModelScene();
                }
            }

            #endregion
        }

        #region Constructors

        public UnitArrangerViewModel(GDModelScene gdms) : base(gdms)
        {
            NormalizedEntryIndexViewModels = new ReadOnlyObservableCollection
                <NormalizedEntryIndexViewModel>(
                    _normalizedEntryIndexViewModels
                );

            UpdateNormalizedEntryIndexViewModels();
        }

        #endregion

        #region Methods

        private void ClearNormalizedEntryIndexViewModels()
        {
            foreach (var viewModel in _normalizedEntryIndexViewModels)
            {
                viewModel.IsActivated = false;
            }

            _normalizedEntryIndexViewModels.Clear();
        }

        private void UpdateNormalizedEntryIndexViewModels()
        {
            ClearNormalizedEntryIndexViewModels();

            for (int eii = 0;
                eii < UnitArrangerComponent.NormalizedEntryIndices.Count;
                ++eii)
            {
                var viewModel = new NormalizedEntryIndexViewModel(this, eii);

                _normalizedEntryIndexViewModels.Add(viewModel);
            }
        }

        protected override void NotifyAllGDModelScenePropertiesChanged()
        {
            NotifyPropertyChanged(nameof(HouseholdType));

            NotifyPropertyChanged(nameof(MinNumOfFloors));
            NotifyPropertyChanged(nameof(MaxNumOfFloors));
            NotifyPropertyChanged(nameof(NumOfFloors));

            NotifyPropertyChanged(nameof(MinNumOfUnitsPerFloor));
            NotifyPropertyChanged(nameof(MaxNumOfUnitsPerFloor));
            NotifyPropertyChanged(nameof(NumOfUnitsPerFloor));
        }

        #endregion

        #region Properties

        public override bool IsActivated
        {
            get => base.IsActivated;
            set
            {
                if (value == IsActivated)
                {
                    return;
                }

                base.IsActivated = value;

                if (value)
                {
                    UpdateNormalizedEntryIndexViewModels();
                }
                else
                {
                    ClearNormalizedEntryIndexViewModels();
                }
            }
        }

        public UnitArrangerComponent UnitArrangerComponent =>
            GDModelScene?.BuildingDesigner.UnitArrangerComponent;

        public IReadOnlyList<HouseholdType> AllHouseholdTypes =>
            Enum.GetValues(typeof(HouseholdType))
            .OfType<HouseholdType>().ToList();

        public HouseholdType? HouseholdType
        {
            get => UnitArrangerComponent?.HouseholdType;
            set
            {
                if (UnitArrangerComponent == null)
                {
                    return;
                }

                UnitArrangerComponent.HouseholdType = (HouseholdType)value;

                UpdateGDModelScene();
            }
        }

        public int? MinNumOfFloors
        {
            get => UnitArrangerComponent?.MinNumOfFloors;
            set
            {
                if (UnitArrangerComponent == null)
                {
                    return;
                }

                UnitArrangerComponent.MinNumOfFloors = (int)value;

                UpdateGDModelScene();
            }
        }

        public int? MaxNumOfFloors
        {
            get => UnitArrangerComponent?.MaxNumOfFloors;
            set
            {
                if (UnitArrangerComponent == null)
                {
                    return;
                }

                UnitArrangerComponent.MaxNumOfFloors = (int)value;

                UpdateGDModelScene();
            }
        }

        public int? NumOfFloors
        {
            get => UnitArrangerComponent?.NumOfFloors;
            set
            {
                if (UnitArrangerComponent == null)
                {
                    return;
                }

                UnitArrangerComponent.NumOfFloors = (int)value;

                UpdateGDModelScene();
            }
        }

        public int? MinNumOfUnitsPerFloor
        {
            get => UnitArrangerComponent?.MinNumOfUnitsPerFloor;
            set
            {
                if (UnitArrangerComponent == null)
                {
                    return;
                }

                UnitArrangerComponent.MinNumOfUnitsPerFloor = (int)value;

                UpdateGDModelScene();
            }
        }

        public int? MaxNumOfUnitsPerFloor
        {
            get => UnitArrangerComponent?.MaxNumOfUnitsPerFloor;
            set
            {
                if (UnitArrangerComponent == null)
                {
                    return;
                }

                UnitArrangerComponent.MaxNumOfUnitsPerFloor = (int)value;

                UpdateGDModelScene();
            }
        }

        public int? NumOfUnitsPerFloor
        {
            get => UnitArrangerComponent?.NumOfUnitsPerFloor;
            set
            {
                if (UnitArrangerComponent == null)
                {
                    return;
                }

                UnitArrangerComponent.NumOfUnitsPerFloor = (int)value;

                UpdateNormalizedEntryIndexViewModels();

                UpdateGDModelScene();
            }
        }

        public ReadOnlyObservableCollection<NormalizedEntryIndexViewModel>
            NormalizedEntryIndexViewModels
        { get; }

        #endregion

        #region Member variables

        private readonly ObservableCollection<NormalizedEntryIndexViewModel>
            _normalizedEntryIndexViewModels =
            new ObservableCollection<NormalizedEntryIndexViewModel>();

        #endregion
    }
}
