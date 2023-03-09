using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component describing a tile on the driveway.
    /// </summary>
    [Serializable]
    public class DrivewayTileComponent : WayTileComponent
    {
        #region Constructors

        public DrivewayTileComponent() : base()
        {
            Width = DefaultWidth;
            Length = DefaultLength;
        }

        protected DrivewayTileComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public void SetDrivewayTile(DrivewayTile dt, WayTileSide side)
        {
            switch (side)
            {
                case WayTileSide.Left:

                    LeftDrivewayTile = dt;
                    break;

                case WayTileSide.Right:

                    RightDrivewayTile = dt;
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

        public CarParkingAreaAnchorComponent
            GetCarParkingAreaAnchorComponent(WayTileSide side)
        {
            switch (side)
            {
                case WayTileSide.Left:

                    return LeftCarParkingAreaAnchorComponent;

                case WayTileSide.Right:

                    return RightCarParkingAreaAnchorComponent;

                default:

                    throw new ArgumentException(
                        $"{nameof(side)} must be " +
                        $"{nameof(WayTileSide.Left)} or " +
                        $"{nameof(WayTileSide.Right)}",
                        nameof(side)
                    );
            }
        }

        public void AddCarParkingArea(
            CarParkingArea cpa, WayTileSide side, double offset
        )
        {
            var leftCpaac = LeftCarParkingAreaAnchorComponent;
            if (leftCpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(LeftCarParkingAreaAnchorComponent)} " +
                    "is not set"
                );
            }

            var rightCpaac = RightCarParkingAreaAnchorComponent;
            if (rightCpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(RightCarParkingAreaAnchorComponent)} " +
                    "is not set"
                );
            }

            if (cpa == null)
            {
                throw new ArgumentNullException(nameof(cpa));
            }

            switch (side)
            {
                case WayTileSide.Left:

                    leftCpaac.AddCarParkingArea(cpa, offset);
                    break;

                case WayTileSide.Right:

                    cpa.CarParkingAreaComponent.IsWidthDirectionInverted =
                        true;
                    rightCpaac.AddCarParkingArea(cpa, offset);
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

        public bool RemoveCarParkingArea(CarParkingArea cpa)
        {
            var leftCpaac = LeftCarParkingAreaAnchorComponent;
            if (leftCpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(LeftCarParkingAreaAnchorComponent)} " +
                    "is not set"
                );
            }

            var rightCpaac = RightCarParkingAreaAnchorComponent;
            if (rightCpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(RightCarParkingAreaAnchorComponent)} " +
                    "is not set"
                );
            }

            if (leftCpaac.CarParkingAreaOffsets.ContainsKey(cpa))
            {
                leftCpaac.RemoveCarParkingArea(cpa);
                return true;
            }
            else if (rightCpaac.CarParkingAreaOffsets.ContainsKey(cpa))
            {
                rightCpaac.RemoveCarParkingArea(cpa);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasCarParkingArea(CarParkingArea cpa)
        {
            return
                LeftCarParkingAreaAnchorComponent
                .CarParkingAreaOffsets.ContainsKey(cpa) ||
                RightCarParkingAreaAnchorComponent
                .CarParkingAreaOffsets.ContainsKey(cpa);
        }

        public double GetFitLength()
        {
            if (ForwardDrivewayTile != null ||
                LeftDrivewayTile != null ||
                RightDrivewayTile != null)
            {
                return Length;
            }

            double leftActualMax =
                LeftCarParkingAreaAnchorComponent.GetActualMax();

            double rightActualMax =
                RightCarParkingAreaAnchorComponent.GetActualMax();

            double fitLength = Math.Max(leftActualMax, rightActualMax);
            return fitLength;
        }

        private void UpdateCarParkingAreaAnchorComponents()
        {
            var leftCpaac = LeftCarParkingAreaAnchorComponent;
            var rightCpaac = RightCarParkingAreaAnchorComponent;

            if (leftCpaac == null || rightCpaac == null)
            {
                return;
            }

            leftCpaac.Transform =
                GetLeftSideLocalTransform(0.0, Math.PI / 2.0);
            rightCpaac.Transform =
                GetRightSideLocalTransform(0.0, Math.PI / 2.0);
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            UpdateCarParkingAreaAnchorComponents();
        }

        #endregion

        #region Properties

        #region Connections

        public override SceneObject ForwardWayTile
        {
            get => base.ForwardWayTile;
            set
            {
                if (value != null && !(value is DrivewayTile))
                {
                    throw new ArgumentException(
                        $"{nameof(value)} must be " +
                        $"an instance of {nameof(DrivewayTile)} or null",
                        nameof(value)
                    );
                }

                base.ForwardWayTile = value;
            }
        }

        public override SceneObject LeftWayTile
        {
            get => base.LeftWayTile;
            set
            {
                if (value != null && !(value is DrivewayTile))
                {
                    throw new ArgumentException(
                        $"{nameof(value)} must be " +
                        $"an instance of {nameof(DrivewayTile)} or null",
                        nameof(value)
                    );
                }

                base.LeftWayTile = value;
            }
        }

        public override SceneObject RightWayTile
        {
            get => base.RightWayTile;
            set
            {
                if (value != null && !(value is DrivewayTile))
                {
                    throw new ArgumentException(
                        $"{nameof(value)} must be " +
                        $"an instance of {nameof(DrivewayTile)} or null",
                        nameof(value)
                    );
                }

                base.RightWayTile = value;
            }
        }

        public DrivewayTile ForwardDrivewayTile
        {
            get => ForwardWayTile as DrivewayTile;
            set => ForwardWayTile = value;
        }

        public DrivewayTile LeftDrivewayTile
        {
            get => LeftWayTile as DrivewayTile;
            set => LeftWayTile = value;
        }

        public DrivewayTile RightDrivewayTile
        {
            get => RightWayTile as DrivewayTile;
            set => RightWayTile = value;
        }

        public DrivewayTile PreviousDrivewayTile =>
            PreviousWayTile as DrivewayTile;

        #endregion

        public override double Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                UpdateCarParkingAreaAnchorComponents();
            }
        }

        public override double SteerAngle
        {
            get => base.SteerAngle;
            set
            {
                base.SteerAngle = value;
                UpdateCarParkingAreaAnchorComponents();
            }
        }

        #region Car parking areas

        public CarParkingAreaAnchorComponent
            LeftCarParkingAreaAnchorComponent
        {
            get
            {
                var cpaacs =
                    SceneObject?
                    .GetComponents<CarParkingAreaAnchorComponent>();

                if (cpaacs == null) { return null; }
                if (cpaacs.Count < 2) { return null; }

                var leftCpaac = cpaacs[0];
                return leftCpaac;
            }
        }

        public CarParkingAreaAnchorComponent
            RightCarParkingAreaAnchorComponent
        {
            get
            {
                var cpaacs =
                    SceneObject?
                    .GetComponents<CarParkingAreaAnchorComponent>();

                if (cpaacs == null) { return null; }
                if (cpaacs.Count < 2) { return null; }

                var rightCpaac = cpaacs[1];
                return rightCpaac;
            }
        }

        public IReadOnlyList<CarParkingArea> CarParkingAreas
        {
            get
            {
                var leftCpas =
                    LeftCarParkingAreaAnchorComponent
                    ?.CarParkingAreas.Values ??
                    new List<CarParkingArea>();

                var rightCpas =
                    RightCarParkingAreaAnchorComponent
                    ?.CarParkingAreas.Values ??
                    new List<CarParkingArea>();

                var cpas = new List<CarParkingArea>();

                cpas.AddRange(leftCpas);
                cpas.AddRange(rightCpas);

                return cpas;
            }
        }

        #endregion

        #endregion

        #region Constants

        public new const double DefaultWidth = 5.0;

        public new const double DefaultLength =
            CarParkingAreaComponent.DefaultSpaceWidth *
            CarParkingAreaComponent.DefaultNumOfSpaces;

        #endregion
    }
}
