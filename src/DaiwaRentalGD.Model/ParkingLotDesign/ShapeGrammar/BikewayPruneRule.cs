using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The rule for pruning an existing bikeway tile.
    /// </summary>
    [Serializable]
    public class BikewayPruneRule : SGRule<BikewaySGState, BikewayPruneMarker>
    {
        #region Constructors

        public BikewayPruneRule() : base()
        { }

        protected BikewayPruneRule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public override IReadOnlyList<ISGMarker> Rewrite(
            BikewaySGState state, BikewayPruneMarker marker
        )
        {
            var pl = state.ParkingLot;
            var plc = pl.ParkingLotComponent;

            var bikewayBuilder = state.BikewayBuilder;

            var bt = marker.BikewayTile;
            var btc = bt.BikewayTileComponent;

            var isBtPruned = bikewayBuilder.PruneBikewayTile(bt);

            if (!isBtPruned)
            {
                return NoMarker;
            }

            if (btc.Length == 0.0)
            {
                var previousBt = btc.PreviousBikewayTile;

                plc.RemoveBikewayTile(bt);

                if (previousBt != null)
                {
                    return new List<ISGMarker>
                    {
                        new BikewayPruneMarker
                        {
                            BikewayTile = previousBt
                        }
                    };
                }
            }

            return NoMarker;
        }

        #endregion
    }
}
