using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The rule for growing an existing driveway in forward direction.
    /// </summary>
    [Serializable]
    public class DrivewayForwardRule :
        SGRule<DrivewaySGState, DrivewayForwardMarker>
    {
        #region Constructors

        public DrivewayForwardRule() : base()
        { }

        protected DrivewayForwardRule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        private IReadOnlyList<ISGMarker> GetForwardMarkers(DrivewayTile dt)
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

        public IReadOnlyList<ISGMarker> GetTurnTileMarkers(DrivewayTile dt)
        {
            var markers = new List<ISGMarker>
            {
                new DrivewayTurnMarker
                {
                    DrivewayTile = dt,
                    Side = WayTileSide.Left,
                },
                new DrivewayTurnMarker
                {
                    DrivewayTile = dt,
                    Side = WayTileSide.Right,
                },
                new DrivewayPruneMarker
                {
                    DrivewayTile = dt
                }
            };

            return markers;
        }

        public override IReadOnlyList<ISGMarker> Rewrite(
            DrivewaySGState state, DrivewayForwardMarker marker
        )
        {
            var plc = state.ParkingLot.ParkingLotComponent;

            var plrc = state.ParkingLotRequirementsComponent;
            var drivewayBuilder = state.DrivewayBuilder;

            var dt = marker.DrivewayTile;

            if (plc.NumOfCarParkingSpaces >= plrc.CarParkingSpaceMaxTotal)
            {
                return new List<ISGMarker>
                {
                    new DrivewayPruneMarker
                    {
                        DrivewayTile = dt
                    }
                };
            }

            bool isDtUpdated = drivewayBuilder.ForwardDriveway(dt);

            if (isDtUpdated)
            {
                var forwardDt = dt.DrivewayTileComponent.ForwardDrivewayTile;
                return GetForwardMarkers(forwardDt ?? dt);
            }
            else
            {
                return GetTurnTileMarkers(dt);
            }
        }

        #endregion
    }
}
