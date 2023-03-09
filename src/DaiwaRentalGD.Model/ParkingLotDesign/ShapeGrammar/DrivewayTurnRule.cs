using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The rule for growing an existing driveway in left or right direction.
    /// </summary>
    [Serializable]
    public class DrivewayTurnRule :
        SGRule<DrivewaySGState, DrivewayTurnMarker>
    {
        #region Constructors

        public DrivewayTurnRule() : base()
        { }

        protected DrivewayTurnRule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public IReadOnlyList<ISGMarker> GetForwardMarkers(DrivewayTile dt)
        {
            var markers = new List<ISGMarker>
            {
                new DrivewayForwardMarker
                {
                    DrivewayTile = dt
                },
                new DrivewayCarParkingSpaceAppendMarker
                {
                    DrivewayTile = dt,
                    Side = WayTileSide.Left
                },
                new DrivewayCarParkingSpaceAppendMarker
                {
                    DrivewayTile = dt,
                    Side = WayTileSide.Right
                }
            };

            return markers;
        }

        public override IReadOnlyList<ISGMarker> Rewrite(
            DrivewaySGState state, DrivewayTurnMarker marker
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

            var newDt = drivewayBuilder.TurnDriveway(dt, marker.Side);

            if (newDt == null)
            {
                return NoMarker;
            }
            else
            {
                return GetForwardMarkers(newDt);
            }
        }

        #endregion
    }
}
