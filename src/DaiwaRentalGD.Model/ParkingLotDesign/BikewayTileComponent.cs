using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component describing a tile on the bikeway.
    /// </summary>
    [Serializable]
    public class BikewayTileComponent : WayTileComponent
    {
        #region Constructors

        public BikewayTileComponent() : base()
        {
            Width = DefaultWidth;
            Length = DefaultLength;
        }

        protected BikewayTileComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public BicycleParkingAreaAnchorComponent
            GetBicycleParkingAreaAnchorComponent(WayTileSide side)
        {
            switch (side)
            {
                case WayTileSide.Left:

                    return LeftBicycleParkingAreaAnchorComponent;

                case WayTileSide.Right:

                    return RightBicycleParkingAreaAnchorComponent;

                default:

                    throw new ArgumentException(
                        $"{nameof(side)} must be " +
                        $"{nameof(WayTileSide.Left)} or " +
                        $"{nameof(WayTileSide.Right)}",
                        nameof(side)
                    );
            }
        }

        public void AddBicycleParkingArea(
            BicycleParkingArea bpa, WayTileSide side, double offset
        )
        {
            var leftBpaac = LeftBicycleParkingAreaAnchorComponent;
            if (leftBpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(LeftBicycleParkingAreaAnchorComponent)} " +
                    "is not set"
                );
            }

            var rightBpaac = RightBicycleParkingAreaAnchorComponent;
            if (rightBpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(RightBicycleParkingAreaAnchorComponent)} " +
                    "is not set"
                );
            }

            if (bpa == null)
            {
                throw new ArgumentNullException(nameof(bpa));
            }

            switch (side)
            {
                case WayTileSide.Left:

                    leftBpaac.AddBicycleParkingArea(bpa, offset);
                    break;

                case WayTileSide.Right:

                    bpa.BicycleParkingAreaComponent.IsWidthDirectionInverted =
                        true;
                    rightBpaac.AddBicycleParkingArea(bpa, offset);
                    break;

                default:

                    throw new ArgumentException(
                        $"{nameof(side)} must be " +
                        $"{nameof(WayTileSide.Left)} or " +
                        $"{nameof(WayTileSide.Right)}",
                        nameof(side)
                    );
            }
        }

        public bool RemoveBicycleParkingArea(BicycleParkingArea bpa)
        {
            var leftBpaac = LeftBicycleParkingAreaAnchorComponent;
            if (leftBpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(LeftBicycleParkingAreaAnchorComponent)} " +
                    "is not set"
                );
            }

            var rightBpaac = RightBicycleParkingAreaAnchorComponent;
            if (rightBpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(RightBicycleParkingAreaAnchorComponent)} " +
                    "is not set"
                );
            }

            if (leftBpaac.BicycleParkingAreaOffsets.ContainsKey(bpa))
            {
                leftBpaac.RemoveBicycleParkingArea(bpa);
                return true;
            }
            else if (rightBpaac.BicycleParkingAreaOffsets.ContainsKey(bpa))
            {
                rightBpaac.RemoveBicycleParkingArea(bpa);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasBicycleParkingArea(BicycleParkingArea bpa)
        {
            return
                LeftBicycleParkingAreaAnchorComponent
                .BicycleParkingAreaOffsets.ContainsKey(bpa) ||
                RightBicycleParkingAreaAnchorComponent
                .BicycleParkingAreaOffsets.ContainsKey(bpa);
        }

        public double GetFitLength()
        {
            if (ForwardBikewayTile != null)
            {
                return Length;
            }

            double leftActualMax =
                LeftBicycleParkingAreaAnchorComponent.GetActualMax();

            double rightActualMax =
                RightBicycleParkingAreaAnchorComponent.GetActualMax();

            double fitLength = Math.Max(leftActualMax, rightActualMax);
            return fitLength;
        }

        private void UpdateBicycleParkingAreaAnchorComponents()
        {
            var leftBpaac = LeftBicycleParkingAreaAnchorComponent;
            var rightBpaac = RightBicycleParkingAreaAnchorComponent;

            if (leftBpaac == null || rightBpaac == null)
            {
                return;
            }

            leftBpaac.Transform =
                GetLeftSideLocalTransform(0.0, Math.PI / 2.0);
            rightBpaac.Transform =
                GetRightSideLocalTransform(0.0, Math.PI / 2.0);
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            UpdateBicycleParkingAreaAnchorComponents();
        }

        #endregion

        #region Properties

        #region Connections

        public override SceneObject ForwardWayTile
        {
            get => base.ForwardWayTile;
            set
            {
                if (value != null && !(value is BikewayTile))
                {
                    throw new ArgumentException(
                        $"{nameof(value)} must be " +
                        $"an instance of {nameof(BikewayTile)} or null",
                        nameof(value)
                    );
                }

                base.ForwardWayTile = value;
            }
        }

        public override SceneObject LeftWayTile
        {
            get => base.LeftWayTile;
            set { }
        }

        public override SceneObject RightWayTile
        {
            get => base.RightWayTile;
            set { }
        }

        public BikewayTile ForwardBikewayTile
        {
            get => ForwardWayTile as BikewayTile;
            set => ForwardWayTile = value;
        }

        public BikewayTile PreviousBikewayTile =>
            PreviousWayTile as BikewayTile;

        #endregion

        public override double Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                UpdateBicycleParkingAreaAnchorComponents();
            }
        }

        public override double SteerAngle
        {
            get => base.SteerAngle;
            set
            {
                base.SteerAngle = value;
                UpdateBicycleParkingAreaAnchorComponents();
            }
        }

        #region Bicycle parking areas

        public BicycleParkingAreaAnchorComponent
            LeftBicycleParkingAreaAnchorComponent
        {
            get
            {
                var bpaacs =
                    SceneObject?
                    .GetComponents<BicycleParkingAreaAnchorComponent>();

                if (bpaacs == null) { return null; }
                if (bpaacs.Count < 2) { return null; }

                var leftBpaac = bpaacs[0];
                return leftBpaac;
            }
        }

        public BicycleParkingAreaAnchorComponent
            RightBicycleParkingAreaAnchorComponent
        {
            get
            {
                var bpaacs =
                    SceneObject?
                    .GetComponents<BicycleParkingAreaAnchorComponent>();

                if (bpaacs == null) { return null; }
                if (bpaacs.Count < 2) { return null; }

                var rightBpaac = bpaacs[1];
                return rightBpaac;
            }
        }

        public IReadOnlyList<BicycleParkingArea> BicycleParkingAreas
        {
            get
            {
                var leftBpas =
                    LeftBicycleParkingAreaAnchorComponent
                    ?.BicycleParkingAreas.Values ??
                    new List<BicycleParkingArea>();

                var rightBpas =
                    RightBicycleParkingAreaAnchorComponent
                    ?.BicycleParkingAreas.Values ??
                    new List<BicycleParkingArea>();

                var bpas = new List<BicycleParkingArea>();

                bpas.AddRange(leftBpas);
                bpas.AddRange(rightBpas);

                return bpas;
            }
        }

        #endregion

        #endregion

        #region Constants

        public new const double DefaultWidth = 2.0;

        public new const double DefaultLength =
            BicycleParkingAreaComponent.DefaultSpaceWidth *
            BicycleParkingAreaComponent.DefaultNumOfSpaces;

        #endregion
    }
}
