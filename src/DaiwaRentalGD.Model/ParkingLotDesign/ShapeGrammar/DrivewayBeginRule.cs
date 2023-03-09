using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.ShapeGrammar;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The rule for adding a driveway tile connected to a driveway entrance.
    /// </summary>
    [Serializable]
    public class DrivewayBeginRule :
        SGRule<DrivewaySGState, DrivewayBeginMarker>
    {
        #region Constructors

        public DrivewayBeginRule() : base()
        { }

        protected DrivewayBeginRule(
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
            DrivewaySGState state, DrivewayBeginMarker marker
        )
        {
            var plc = state.ParkingLot.ParkingLotComponent;

            var plrc = state.ParkingLotRequirementsComponent;
            var drivewayBuilder = state.DrivewayBuilder;

            var de = marker.DrivewayEntrance;

            if (plc.NumOfCarParkingSpaces >= plrc.CarParkingSpaceMaxTotal)
            {
                return NoMarker;
            }

            var dt = drivewayBuilder.CreateDrivewayTile();

            dt.TransformComponent.Transform =
                new TrsTransform3D(
                    de.DrivewayEntranceComponent.Transform
                );

            plc.AddDrivewayTile(dt);

            if (!plc.IsPlacementValid(dt))
            {
                plc.RemoveDrivewayTile(dt);
                return NoMarker;
            }

            return GetForwardMarkers(dt);
        }

        #endregion
    }
}
