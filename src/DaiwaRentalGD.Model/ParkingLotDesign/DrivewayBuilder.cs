using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// Build driveways with given driveway tiles on a parking lot.
    /// </summary>
    [Serializable]
    public class DrivewayBuilder : IWorkspaceItem
    {
        #region Constructors

        public DrivewayBuilder()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected DrivewayBuilder(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            ParkingLot = reader.GetValue<ParkingLot>(nameof(ParkingLot));
            DrivewayTileComponentTemplate =
                reader.GetValue<DrivewayTileComponent>(
                    nameof(DrivewayTileComponentTemplate)
                );
            CarParkingAreaComponentTemplate =
                reader.GetValue<CarParkingAreaComponent>(
                    nameof(CarParkingAreaComponentTemplate)
                );
            _angleEpsilon = reader.GetValue<double>(nameof(AngleEpsilon));
        }

        #endregion

        #region Methods

        public bool ForwardDriveway(DrivewayTile dt)
        {
            if (ParkingLot == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ParkingLot)} is not set"
                );
            }

            if (dt == null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            var plc = ParkingLot.ParkingLotComponent;
            var dtc = dt.DrivewayTileComponent;

            if (dtc.ForwardDrivewayTile != null)
            {
                return false;
            }

            double steerAngle = GetDrivewaySteerAngle(dt, plc.Site);

            if (Math.Abs(steerAngle) > AngleEpsilon)
            {
                var isDtAdded = ForwardDrivewayByAdd(dt, steerAngle);
                if (isDtAdded) { return true; }
            }

            return ForwardDrivewayByExpand(dt);
        }

        private double GetDrivewaySteerAngle(DrivewayTile dt, Site site)
        {
            var dtc = dt.DrivewayTileComponent;

            var steerTf = dtc.GetForwardSideTransform(dtc.Width / 2.0);

            var steerPoint = new Point();
            steerPoint.Transform(steerTf);

            var currentForwardDir = steerTf.TransformDir(
                new DenseVector(new[] { 0.0, 1.0, 0.0 })
            );

            var vectorPair = SiteVectorField.GetVectorPair(site, steerPoint);

            double cos1 = currentForwardDir.DotProduct(vectorPair.Item1);
            double cos2 = currentForwardDir.DotProduct(vectorPair.Item2);

            Vector<double> newForwardDir =
                Math.Abs(cos1) >= Math.Abs(cos2) ?
                vectorPair.Item1 : vectorPair.Item2;

            if (newForwardDir.DotProduct(currentForwardDir) < 0.0)
            {
                newForwardDir = -newForwardDir;
            }

            double steerAngle =
                MathUtils.GetAngle2D(currentForwardDir, newForwardDir);

            return steerAngle;
        }

        private double GetCarParkingAreaSafeOffset(DrivewayTile dt)
        {
            var dtc = dt.DrivewayTileComponent;
            double cpaLength = CarParkingAreaComponentTemplate.Length;

            double cpaSafeOffset =
                cpaLength * Math.Abs(Math.Sin(dtc.SteerAngle));

            return cpaSafeOffset;
        }

        private double GetCarParkingAreaSafeOffset(
            DrivewayTile dt, WayTileSide side
        )
        {
            var dtc = dt.DrivewayTileComponent;
            double steerAngle = dtc.SteerAngle;

            if (steerAngle == 0.0)
            {
                return 0.0;
            }
            if (steerAngle > 0.0 && side == WayTileSide.Right)
            {
                return 0.0;
            }
            else if (steerAngle < 0.0 && side == WayTileSide.Left)
            {
                return 0.0;
            }
            else
            {
                return GetCarParkingAreaSafeOffset(dt);
            }
        }

        private bool ForwardDrivewayByExpand(DrivewayTile dt)
        {
            var plc = ParkingLot.ParkingLotComponent;
            var dtc = dt.DrivewayTileComponent;

            dtc.Length += CarParkingAreaComponentTemplate.SpaceWidth;

            if (!plc.IsPlacementValid(dt))
            {
                dtc.Length -= CarParkingAreaComponentTemplate.SpaceWidth;

                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ForwardDrivewayByAdd(DrivewayTile dt, double steerAngle)
        {
            var plc = ParkingLot.ParkingLotComponent;
            var dtc = dt.DrivewayTileComponent;

            DrivewayTile newDt = CreateDrivewayTile();
            newDt.DrivewayTileComponent.SteerAngle = steerAngle;

            double cpaSafeOffset = GetCarParkingAreaSafeOffset(dt);
            newDt.DrivewayTileComponent.Length += cpaSafeOffset;

            plc.AddDrivewayTile(newDt);
            dtc.ForwardDrivewayTile = newDt;

            if (!plc.IsPlacementValid(newDt))
            {
                plc.RemoveDrivewayTile(newDt);

                return false;
            }
            else
            {
                return true;
            }
        }

        public CarParkingArea AppendCarParkingSpace(
            DrivewayTile dt, WayTileSide side
        )
        {
            if (ParkingLot == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ParkingLot)} is not set"
                );
            }

            if (dt == null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            return
                AppendCarParkingSpaceByExpand(dt, side) ??
                AppendCarParkingSpaceByAdd(dt, side);
        }

        private CarParkingArea AppendCarParkingSpaceByExpand(
            DrivewayTile dt, WayTileSide side
        )
        {
            var plc = ParkingLot.ParkingLotComponent;
            var dtc = dt.DrivewayTileComponent;
            var cpaac = dtc.GetCarParkingAreaAnchorComponent(side);

            var cpa = cpaac.CarParkingAreas.LastOrDefault().Value;
            if (cpa == null)
            {
                return null;
            }

            var cpac = cpa.CarParkingAreaComponent;
            cpac.NumOfSpaces += 1;


            if (cpaac.GetActualMax() > dtc.Length)
            {
                cpac.NumOfSpaces -= 1;
                return null;
            }
            else if (!plc.IsPlacementValid(cpa))
            {
                cpac.NumOfSpaces -= 1;
                return null;
            }
            else
            {
                return cpa;
            }
        }

        private CarParkingArea AppendCarParkingSpaceByAdd(
            DrivewayTile dt, WayTileSide side
        )
        {
            var plc = ParkingLot.ParkingLotComponent;
            var dtc = dt.DrivewayTileComponent;
            var cpaac = dtc.GetCarParkingAreaAnchorComponent(side);

            var cpa = CreateCarParkingArea();

            double cpaSafeOffset = GetCarParkingAreaSafeOffset(dt, side);

            double cpaOffset = cpaac.GetActualMax();

            while (cpaOffset < dtc.Length)
            {
                try
                {
                    dtc.AddCarParkingArea(cpa, side, cpaOffset);

                    if (cpaac.GetActualMax() > dtc.Length)
                    {
                        dtc.RemoveCarParkingArea(cpa); ;
                    }
                    else if (!plc.IsPlacementValid(cpa))
                    {
                        dtc.RemoveCarParkingArea(cpa);
                    }
                    else
                    {
                        return cpa;
                    }

                    dtc.AddCarParkingArea(
                        cpa, side, cpaOffset + cpaSafeOffset
                    );

                    if (cpaac.GetActualMax() > dtc.Length)
                    {
                        dtc.RemoveCarParkingArea(cpa); ;
                    }
                    else if (!plc.IsPlacementValid(cpa))
                    {
                        dtc.RemoveCarParkingArea(cpa);
                    }
                    else
                    {
                        return cpa;
                    }
                }
                catch (ArgumentException)
                { }

                cpaOffset += CarParkingAreaComponentTemplate.SpaceWidth;
            }

            return null;
        }

        public CarParkingArea RemoveCarParkingSpace(
            DrivewayTile dt, WayTileSide side
        )
        {
            if (ParkingLot == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ParkingLot)} is not set"
                );
            }

            if (dt == null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            var dtc = dt.DrivewayTileComponent;
            var cpaac = dtc.GetCarParkingAreaAnchorComponent(side);

            var lastCpa = cpaac.CarParkingAreas.LastOrDefault().Value;

            if (lastCpa == null) { return null; }

            var lastCpac = lastCpa.CarParkingAreaComponent;

            if (lastCpac.NumOfSpaces == 1)
            {
                dtc.RemoveCarParkingArea(lastCpa);
            }
            else
            {
                lastCpac.NumOfSpaces -= 1;
            }

            return lastCpa;
        }

        public DrivewayTile TurnDriveway(DrivewayTile dt, WayTileSide side)
        {
            if (ParkingLot == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ParkingLot)} is not set"
                );
            }

            if (dt == null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            var plc = ParkingLot.ParkingLotComponent;
            var dtc = dt.DrivewayTileComponent;
            var cpaac = dtc.GetCarParkingAreaAnchorComponent(side);
            double dtWidth = DrivewayTileComponentTemplate.Width;

            double newDtOffset = dtc.Length - dtWidth;
            if (newDtOffset < 0.0) { return null; }

            int numOfRemovedCpss = 0;

            while (cpaac.GetActualMax() > newDtOffset)
            {
                RemoveCarParkingSpace(dt, side);
                ++numOfRemovedCpss;
            }

            DrivewayTile newDt = CreateDrivewayTile();

            plc.AddDrivewayTile(newDt);
            dtc.SetDrivewayTile(newDt, side);

            if (!plc.IsPlacementValid(newDt))
            {
                plc.RemoveDrivewayTile(newDt);

                while (numOfRemovedCpss > 0)
                {
                    AppendCarParkingSpace(dt, side);
                    --numOfRemovedCpss;
                }

                return null;
            }

            return newDt;
        }

        public bool PruneDrivewayTile(DrivewayTile dt)
        {
            if (dt == null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            var dtc = dt.DrivewayTileComponent;

            double dtFitLength = dtc.GetFitLength();

            if (dtc.Length == dtFitLength)
            {
                return false;
            }
            else
            {
                dtc.Length = dtFitLength;
                return true;
            }
        }

        public DrivewayTile CreateDrivewayTile()
        {
            var dt = new DrivewayTile();

            dt.DrivewayTileComponent.Width =
                DrivewayTileComponentTemplate.Width;

            dt.DrivewayTileComponent.Length =
                DrivewayTileComponentTemplate.Length;

            return dt;
        }

        public CarParkingArea CreateCarParkingArea()
        {
            var cps = new CarParkingArea();

            cps.CarParkingAreaComponent.SpaceWidth =
                CarParkingAreaComponentTemplate.SpaceWidth;
            cps.CarParkingAreaComponent.SpaceLength =
                CarParkingAreaComponentTemplate.SpaceLength;

            return cps;
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new IWorkspaceItem[]
            {
                ParkingLot,
                DrivewayTileComponentTemplate,
                CarParkingAreaComponentTemplate
            };

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(ParkingLot), ParkingLot);
            writer.AddValue(
                nameof(DrivewayTileComponentTemplate),
                DrivewayTileComponentTemplate
            );
            writer.AddValue(
                nameof(CarParkingAreaComponentTemplate),
                CarParkingAreaComponentTemplate
            );
            writer.AddValue(nameof(AngleEpsilon), _angleEpsilon);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public ParkingLot ParkingLot { get; set; }

        public SiteVectorField SiteVectorField =>
            ParkingLot?.ParkingLotComponent.SiteVectorField;

        public DrivewayTileComponent DrivewayTileComponentTemplate
        { get; } = new DrivewayTileComponent();

        public CarParkingAreaComponent CarParkingAreaComponentTemplate
        { get; } = new CarParkingAreaComponent();

        public double AngleEpsilon
        {
            get => _angleEpsilon;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(AngleEpsilon)} cannot be negative"
                    );
                }

                _angleEpsilon = value;
            }
        }

        #endregion

        #region Member variables

        private double _angleEpsilon = DefaultAngleEpsilon;

        #endregion

        #region Constants

        public const double DefaultAngleEpsilon = 1e-6;

        #endregion
    }
}
