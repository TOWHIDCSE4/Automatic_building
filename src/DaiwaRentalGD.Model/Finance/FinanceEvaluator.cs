using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Finance
{
    /// <summary>
    /// Calculates financial information of a model.
    /// </summary>
    [Serializable]
    public class FinanceEvaluator : SceneObject
    {
        #region Constructors

        public FinanceEvaluator() : base()
        {
            InitializeComponents();
        }

        protected FinanceEvaluator(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _unitFinanceComponent = reader.GetValue<UnitFinanceComponent>(
                nameof(UnitFinanceComponent)
            );

            _parkingLotFinanceComponent =
                reader.GetValue<ParkingLotFinanceComponent>(
                    nameof(ParkingLotFinanceComponent)
                );

            _summaryFinanceComponent =
                reader.GetValue<SummaryFinanceComponent>(
                    nameof(SummaryFinanceComponent)
                );

            _financeDataJsonComponent =
                reader.GetValue<FinanceDataJsonComponent>(
                    nameof(FinanceDataJsonComponent)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(UnitFinanceComponent);
            AddComponent(ParkingLotFinanceComponent);
            AddComponent(SummaryFinanceComponent);
            AddComponent(FinanceDataJsonComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(UnitFinanceComponent), _unitFinanceComponent
            );

            writer.AddValue(
                nameof(ParkingLotFinanceComponent),
                _parkingLotFinanceComponent
            );

            writer.AddValue(
                nameof(SummaryFinanceComponent), _summaryFinanceComponent
            );

            writer.AddValue(
                nameof(FinanceDataJsonComponent), _financeDataJsonComponent
            );
        }

        #endregion

        #region Properties

        public UnitFinanceComponent UnitFinanceComponent
        {
            get => _unitFinanceComponent;
            set
            {
                ReplaceComponent(_unitFinanceComponent, value);
                _unitFinanceComponent = value;

                NotifyPropertyChanged();
            }
        }

        public ParkingLotFinanceComponent ParkingLotFinanceComponent
        {
            get => _parkingLotFinanceComponent;
            set
            {
                ReplaceComponent(_parkingLotFinanceComponent, value);
                _parkingLotFinanceComponent = value;

                NotifyPropertyChanged();
            }
        }

        public SummaryFinanceComponent SummaryFinanceComponent
        {
            get => _summaryFinanceComponent;
            set
            {
                ReplaceComponent(_summaryFinanceComponent, value);
                _summaryFinanceComponent = value;

                NotifyPropertyChanged();
            }
        }

        public FinanceDataJsonComponent FinanceDataJsonComponent
        {
            get => _financeDataJsonComponent;
            set
            {
                ReplaceComponent(_financeDataJsonComponent, value);
                _financeDataJsonComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private UnitFinanceComponent _unitFinanceComponent =
            new UnitFinanceComponent();

        private ParkingLotFinanceComponent _parkingLotFinanceComponent =
            new ParkingLotFinanceComponent();

        private SummaryFinanceComponent _summaryFinanceComponent =
            new SummaryFinanceComponent();

        private FinanceDataJsonComponent _financeDataJsonComponent
            = new FinanceDataJsonComponent();

        #endregion
    }
}
