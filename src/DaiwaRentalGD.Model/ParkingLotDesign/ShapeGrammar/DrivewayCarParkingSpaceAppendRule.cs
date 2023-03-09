using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The rule for appending a car parking space at the end of
    /// a driveway tile.
    /// </summary>
    [Serializable]
    public class DrivewayCarParkingSpaceAppendRule :
        SGRule<DrivewaySGState, DrivewayCarParkingSpaceAppendMarker>
    {
        #region Constructors

        public DrivewayCarParkingSpaceAppendRule() : base()
        { }

        protected DrivewayCarParkingSpaceAppendRule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public override IReadOnlyList<ISGMarker> Rewrite(
            DrivewaySGState state, DrivewayCarParkingSpaceAppendMarker marker
        )
        {
            var plc = state.ParkingLot.ParkingLotComponent;

            var plrc = state.ParkingLotRequirementsComponent;
            var drivewayBuilder = state.DrivewayBuilder;

            var dt = marker.DrivewayTile;

            if (plc.NumOfCarParkingSpaces >= plrc.CarParkingSpaceMaxTotal)
            {
                return NoMarker;
            }

            drivewayBuilder.AppendCarParkingSpace(dt, marker.Side);

            return NoMarker;
        }

        #endregion
    }
}
