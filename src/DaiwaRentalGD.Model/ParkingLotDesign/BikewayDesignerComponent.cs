using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// The component for designing bikeways on a parking lot.
    /// </summary>
    [Serializable]
    public class BikewayDesignerComponent : Component
    {
        #region Constructors

        public BikewayDesignerComponent() : base()
        { }

        protected BikewayDesignerComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _bikewayShapeGrammar = reader.GetValue<BikewayShapeGrammar>(
                nameof(BikewayShapeGrammar)
            );

            _bikewaySGState =
                reader.GetValue<BikewaySGState>(nameof(BikewaySGState));
        }

        #endregion

        #region Methods

        protected override void Update()
        {
            if (ParkingLot == null)
            {
                return;
            }

            _bikewaySGState.ParkingLotRequirementsComponent =
                ParkingLotRequirementsComponent;
            _bikewaySGState.Reset();

            _bikewayShapeGrammar.Run(_bikewaySGState);

            ParkingLotRequirementsComponent?.OnBicycleParkingStatsUpdated();
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(BikewayShapeGrammar)
            .Append(BikewaySGState);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(BikewayShapeGrammar), _bikewayShapeGrammar
            );
            writer.AddValue(
                nameof(BikewaySGState), _bikewaySGState
            );
        }

        #endregion

        #region Properties

        public ParkingLot ParkingLot
        {
            get => _bikewaySGState.ParkingLot;
            set
            {
                _bikewaySGState.ParkingLot = value;

                NotifyPropertyChanged();
            }
        }

        public ParkingLotRequirementsComponent ParkingLotRequirementsComponent
            => SceneObject?.GetComponent<ParkingLotRequirementsComponent>();

        public BikewayShapeGrammar BikewayShapeGrammar =>
            _bikewayShapeGrammar;

        public BikewaySGState BikewaySGState => _bikewaySGState;

        #endregion

        #region Member variables

        private readonly BikewayShapeGrammar _bikewayShapeGrammar =
            new BikewayShapeGrammar();

        private readonly BikewaySGState _bikewaySGState =
            new BikewaySGState();

        #endregion
    }
}
