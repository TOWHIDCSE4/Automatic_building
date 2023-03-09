using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The rule for pruning an existing driveway tile.
    /// </summary>
    [Serializable]
    public class DrivewayPruneRule :
        SGRule<DrivewaySGState, DrivewayPruneMarker>
    {
        #region Constructors

        public DrivewayPruneRule() : base()
        { }

        protected DrivewayPruneRule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        private void RestoreCarParkingSpaces(
            ParkingLot pl, DrivewayTile dt,
            ParkingLotRequirementsComponent plrc,
            DrivewayBuilder drivewayBuilder
        )
        {
            var plc = pl.ParkingLotComponent;
            var dtc = dt.DrivewayTileComponent;
            var leftCpaac = dtc.LeftCarParkingAreaAnchorComponent;
            var rightCpaac = dtc.RightCarParkingAreaAnchorComponent;

            double cpsMaxTotal = plrc.CarParkingSpaceMaxTotal;

            while (true)
            {
                if (plc.NumOfCarParkingSpaces >= cpsMaxTotal)
                {
                    break;
                }

                double leftCpaacMax = leftCpaac.GetActualMax();
                double rightCpaacMax = rightCpaac.GetActualMax();

                var longerSide =
                    leftCpaacMax >= rightCpaacMax ?
                    WayTileSide.Left : WayTileSide.Right;

                var shorterSide =
                    leftCpaacMax < rightCpaacMax ?
                    WayTileSide.Left : WayTileSide.Right;

                var updatedCpa =
                    drivewayBuilder.AppendCarParkingSpace(dt, shorterSide) ??
                    drivewayBuilder.AppendCarParkingSpace(dt, longerSide);

                if (updatedCpa == null)
                {
                    break;
                }
            }
        }

        public override IReadOnlyList<ISGMarker> Rewrite(
            DrivewaySGState state, DrivewayPruneMarker marker
        )
        {
            var pl = state.ParkingLot;
            var plc = pl.ParkingLotComponent;

            var plrc = state.ParkingLotRequirementsComponent;
            var drivewayBuilder = state.DrivewayBuilder;

            var dt = marker.DrivewayTile;
            var dtc = dt.DrivewayTileComponent;

            RestoreCarParkingSpaces(pl, dt, plrc, drivewayBuilder);

            var isDtPruned = drivewayBuilder.PruneDrivewayTile(dt);

            if (!isDtPruned)
            {
                return NoMarker;
            }

            if (dtc.Length == 0.0)
            {
                var previousDt = dtc.PreviousDrivewayTile;

                plc.RemoveDrivewayTile(dt);

                if (previousDt != null)
                {
                    return new List<ISGMarker>
                    {
                        new DrivewayPruneMarker
                        {
                            DrivewayTile = previousDt
                        }
                    };
                }
            }

            return NoMarker;
        }

        #endregion
    }
}
