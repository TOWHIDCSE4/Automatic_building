using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// Shape grammar for generating bikeways and their
    /// connected bicycle parking areas.
    /// </summary>
    [Serializable]
    public class BikewayShapeGrammar : ShapeGrammar<BikewaySGState>
    {
        #region Constructors

        public BikewayShapeGrammar() : base()
        {
            InitializeRules();
        }

        protected BikewayShapeGrammar(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            BikewayBeginRule =
                reader.GetValue<BikewayBeginRule>(nameof(BikewayBeginRule));

            BikewayForwardRule = reader.GetValue<BikewayForwardRule>(
                nameof(BikewayForwardRule)
            );

            BikewayPruneRule = reader.GetValue<BikewayPruneRule>(
                nameof(BikewayPruneRule)
            );

            BikewayBicycleParkingSpaceAppendRule =
                reader.GetValue<BikewayBicycleParkingSpaceAppendRule>(
                    nameof(BikewayBicycleParkingSpaceAppendRule)
                );
        }

        #endregion

        #region Methods

        private void InitializeRules()
        {
            AddRule(BikewayBicycleParkingSpaceAppendRule);
            AddRule(BikewayForwardRule);
            AddRule(BikewayBeginRule);
            AddRule(BikewayPruneRule);
        }

        public override bool IsTerminated(BikewaySGState state)
        {
            return false;
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(BikewayBeginRule), BikewayBeginRule);
            writer.AddValue(nameof(BikewayForwardRule), BikewayForwardRule);
            writer.AddValue(nameof(BikewayPruneRule), BikewayPruneRule);
            writer.AddValue(
                nameof(BikewayBicycleParkingSpaceAppendRule),
                BikewayBicycleParkingSpaceAppendRule
            );
        }

        #endregion

        #region Properties

        public BikewayBeginRule BikewayBeginRule
        { get; } = new BikewayBeginRule();

        public BikewayForwardRule BikewayForwardRule
        { get; } = new BikewayForwardRule();

        public BikewayPruneRule BikewayPruneRule
        { get; } = new BikewayPruneRule();

        public BikewayBicycleParkingSpaceAppendRule
            BikewayBicycleParkingSpaceAppendRule
        { get; } = new BikewayBicycleParkingSpaceAppendRule();

        #endregion
    }
}
