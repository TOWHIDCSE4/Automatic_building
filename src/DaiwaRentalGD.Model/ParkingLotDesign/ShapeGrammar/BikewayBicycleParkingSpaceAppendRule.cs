using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The rule for appending a bicycle parking space at the end of
    /// a bikeway tile.
    /// </summary>
    [Serializable]
    public class BikewayBicycleParkingSpaceAppendRule :
        SGRule<BikewaySGState, BikewayBicycleParkingSpaceAppendMarker>
    {
        #region Constructors

        public BikewayBicycleParkingSpaceAppendRule() : base()
        { }

        protected BikewayBicycleParkingSpaceAppendRule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public override IReadOnlyList<ISGMarker> Rewrite(
            BikewaySGState state,
            BikewayBicycleParkingSpaceAppendMarker marker
        )
        {
            var plc = state.ParkingLot.ParkingLotComponent;

            var plrc = state.ParkingLotRequirementsComponent;
            var bikewayBuilder = state.BikewayBuilder;

            var bt = marker.BikewayTile;

            if (plc.NumOfBicycleParkingSpaces >=
                plrc.BicycleParkingSpaceMaxTotal)
            {
                return NoMarker;
            }

            bikewayBuilder.AppendBicycleParkingSpace(bt, marker.Side);

            return NoMarker;
        }

        #endregion
    }
}
