using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ShapeGrammar;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The rule for adding a bikeway tile.
    /// </summary>
    [Serializable]
    public class BikewayBeginRule : SGRule<BikewaySGState, BikewayBeginMarker>
    {
        #region Constructors

        public BikewayBeginRule() : base()
        { }

        protected BikewayBeginRule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public IReadOnlyList<ISGMarker> GetForwardMarkers(
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

        public IList<TrsTransform3D> GetBikewayTransforms(
            WalkwayTile wt, BikewayBuilder bikewayBuilder
        )
        {
            var btTransforms = new List<TrsTransform3D>();

            var wtc = wt.WalkwayTileComponent;

            var leftBottomTf = wtc.GetLeftSideTransform(0.0);
            btTransforms.Add(leftBottomTf);

            var leftTopTf = wtc.GetLeftSideTransform(wtc.Length, Math.PI);
            double btWidth = bikewayBuilder.BikewayTileComponentTemplate.Width;
            leftTopTf.SetTranslateLocal(-btWidth, 0.0, 0.0);
            btTransforms.Add(leftTopTf);

            return btTransforms;
        }

        public IReadOnlyList<ISGMarker> RewriteOne(
            BikewaySGState state, WalkwayTile wt, TrsTransform3D btTransform
        )
        {
            var plc = state.ParkingLot.ParkingLotComponent;

            var plrc = state.ParkingLotRequirementsComponent;
            var bikewayBuilder = state.BikewayBuilder;

            if (plc.NumOfBicycleParkingSpaces >=
                plrc.BicycleParkingSpaceMaxTotal)
            {
                return NoMarker;
            }

            var bt = bikewayBuilder.CreateBikewayTile();

            var btc = bt.BikewayTileComponent;
            var wtc = wt.WalkwayTileComponent;

            if (btc.Length > wtc.Length)
            {
                return NoMarker;
            }

            bt.TransformComponent.Transform = new TrsTransform3D(btTransform);

            plc.AddBikewayTile(bt);

            if (!plc.IsPlacementValid(bt))
            {
                plc.RemoveBikewayTile(bt);
                return NoMarker;
            }

            return GetForwardMarkers(bt, wtc.Length);
        }

        public override IReadOnlyList<ISGMarker> Rewrite(
            BikewaySGState state, BikewayBeginMarker marker
        )
        {
            var markers = new List<ISGMarker>();

            var wt = marker.WalkwayTile;
            var bikewayBuilder = state.BikewayBuilder;

            var btTransforms = GetBikewayTransforms(wt, bikewayBuilder);

            foreach (var btTransform in btTransforms)
            {
                var btMarkers = RewriteOne(state, wt, btTransform);

                markers.AddRange(btMarkers);
            }

            return markers;
        }

        #endregion
    }
}
