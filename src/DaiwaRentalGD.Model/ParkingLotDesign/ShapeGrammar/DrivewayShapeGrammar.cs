using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// Shape grammar for generating driveways and their
    /// connected car parking areas.
    /// </summary>
    [Serializable]
    public class DrivewayShapeGrammar : ShapeGrammar<DrivewaySGState>
    {
        #region Constructors

        public DrivewayShapeGrammar() : base()
        {
            InitializeRules();
        }

        protected DrivewayShapeGrammar(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            DrivewayBeginRule =
                reader.GetValue<DrivewayBeginRule>(nameof(DrivewayBeginRule));

            DrivewayForwardRule = reader.GetValue<DrivewayForwardRule>(
                nameof(DrivewayForwardRule)
            );

            DrivewayTurnRule =
                reader.GetValue<DrivewayTurnRule>(nameof(DrivewayTurnRule));

            DrivewayPruneRule =
                reader.GetValue<DrivewayPruneRule>(nameof(DrivewayPruneRule));

            DrivewayCarParkingSpaceAppendRule =
                reader.GetValue<DrivewayCarParkingSpaceAppendRule>(
                    nameof(DrivewayCarParkingSpaceAppendRule)
                );
        }

        #endregion

        #region Methods

        private void InitializeRules()
        {
            AddRule(DrivewayCarParkingSpaceAppendRule);
            AddRule(DrivewayForwardRule);
            AddRule(DrivewayTurnRule);
            AddRule(DrivewayBeginRule);
            AddRule(DrivewayPruneRule);
        }

        public override bool IsTerminated(DrivewaySGState state)
        {
            return false;
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(DrivewayBeginRule), DrivewayBeginRule);
            writer.AddValue(nameof(DrivewayForwardRule), DrivewayForwardRule);
            writer.AddValue(nameof(DrivewayTurnRule), DrivewayTurnRule);
            writer.AddValue(nameof(DrivewayPruneRule), DrivewayPruneRule);
            writer.AddValue(
                nameof(DrivewayCarParkingSpaceAppendRule),
                DrivewayCarParkingSpaceAppendRule
            );
        }

        public void setShapeGrammerRule_AllowDrivewayTuring(bool enable)
        {

            if (enable)
            {
                int ix = GetRuleIndex(DrivewayTurnRule);
                if (ix < 0)
                {
                    int ixToInsert = GetRuleIndex(DrivewayBeginRule);
                    InsertRule(ixToInsert, DrivewayTurnRule);
                }
            }
            else
            {
                int ix = GetRuleIndex(DrivewayTurnRule);
                if (ix >= 0) { 
                    RemoveRule(DrivewayTurnRule);
                }
            }
        }


        #endregion

        #region Properties

        public DrivewayBeginRule DrivewayBeginRule
        { get; } = new DrivewayBeginRule();

        public DrivewayForwardRule DrivewayForwardRule
        { get; } = new DrivewayForwardRule();

        public DrivewayTurnRule DrivewayTurnRule
        { get; } = new DrivewayTurnRule();

        public DrivewayPruneRule DrivewayPruneRule
        { get; } = new DrivewayPruneRule();

        public DrivewayCarParkingSpaceAppendRule
            DrivewayCarParkingSpaceAppendRule
        { get; } = new DrivewayCarParkingSpaceAppendRule();

        #endregion
    }
}
