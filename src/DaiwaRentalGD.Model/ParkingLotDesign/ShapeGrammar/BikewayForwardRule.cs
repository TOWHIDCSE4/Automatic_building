using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The rule for growing an existing bikeway in forward direction.
    /// </summary>
    [Serializable]
    public class BikewayForwardRule :
        SGRule<BikewaySGState, BikewayForwardMarker>
    {
        #region Constructors

        public BikewayForwardRule() : base()
        { }

        protected BikewayForwardRule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        private IReadOnlyList<ISGMarker> GetForwardMarkers(
            BikewayTile bt, double maxLength
        )
        {
            var markers = new List<ISGMarker>
            {
                new BikewayForwardMarker
                {
                    BikewayTile = bt,
                    MaxLength = maxLength
                },
                new BikewayBicycleParkingSpaceAppendMarker
                {
                    BikewayTile = bt,
                    Side = WayTileSide.Left
                },
                new BikewayBicycleParkingSpaceAppendMarker
                {
                    BikewayTile = bt,
                    Side = WayTileSide.Right
                }
            };

            return markers;
        }

        public override IReadOnlyList<ISGMarker> Rewrite(
            BikewaySGState state, BikewayForwardMarker marker
        )
        {
            var plc = state.ParkingLot.ParkingLotComponent;

            var plrc = state.ParkingLotRequirementsComponent;
            var bikewayBuilder = state.BikewayBuilder;

            var bt = marker.BikewayTile;
            var btc = bt.BikewayTileComponent;

            if (plc.NumOfBicycleParkingSpaces >=
                plrc.BicycleParkingSpaceMaxTotal)
            {
                return new[]
                {
                    new BikewayPruneMarker
                    {
                        BikewayTile = bt
                    }
                };
            }

            if (btc.Length > marker.MaxLength)
            {
                return new[]
                {
                    new BikewayPruneMarker
                    {
                        BikewayTile = bt
                    }
                };
            }

            bool isBtUpdated = bikewayBuilder.ForwardBikeway(bt);

            if (isBtUpdated)
            {
                return GetForwardMarkers(bt, marker.MaxLength);
            }
            else
            {
                return new[]
                {
                    new BikewayPruneMarker
                    {
                        BikewayTile = bt
                    }
                };
            }
        }

        #endregion
    }
}
