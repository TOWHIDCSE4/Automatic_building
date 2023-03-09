using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// Build bikeways with given bikeway tiles on a parking lot.
    /// </summary>
    [Serializable]
    public class BikewayBuilder : IWorkspaceItem
    {
        #region Constructors

        public BikewayBuilder()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected BikewayBuilder(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            ParkingLot = reader.GetValue<ParkingLot>(nameof(ParkingLot));
            BikewayTileComponentTemplate =
                reader.GetValue<BikewayTileComponent>(
                    nameof(BikewayTileComponentTemplate)
                );
            BicycleParkingAreaComponentTemplate =
                reader.GetValue<BicycleParkingAreaComponent>(
                    nameof(BicycleParkingAreaComponentTemplate)
                );
        }

        #endregion

        #region Methods

        public bool ForwardBikeway(BikewayTile bt)
        {
            if (ParkingLot == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ParkingLot)} is not set"
                );
            }

            if (bt == null)
            {
                throw new ArgumentNullException(nameof(bt));
            }

            var btc = bt.BikewayTileComponent;

            if (btc.ForwardBikewayTile != null)
            {
                return false;
            }

            return ForwardBikewayByExpand(bt);
        }

        private bool ForwardBikewayByExpand(BikewayTile bt)
        {
            var plc = ParkingLot.ParkingLotComponent;
            var btc = bt.BikewayTileComponent;
            double bpsWidth = BicycleParkingAreaComponentTemplate.SpaceWidth;

            btc.Length += bpsWidth;

            if (!plc.IsPlacementValid(bt))
            {
                btc.Length -= bpsWidth;

                return false;
            }
            else
            {
                return true;
            }
        }

        public BicycleParkingArea AppendBicycleParkingSpace(
            BikewayTile bt, WayTileSide side
        )
        {
            if (ParkingLot == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ParkingLot)} is not set"
                );
            }

            if (bt == null)
            {
                throw new ArgumentNullException(nameof(bt));
            }

            return
                AppendBicycleParkingSpaceByExpand(bt, side) ??
                AppendBicycleParkingSpaceByAdd(bt, side);
        }

        private BicycleParkingArea AppendBicycleParkingSpaceByExpand(
            BikewayTile bt, WayTileSide side
        )
        {
            var plc = ParkingLot.ParkingLotComponent;
            var btc = bt.BikewayTileComponent;
            var bpaac = btc.GetBicycleParkingAreaAnchorComponent(side);

            var bpa = bpaac.BicycleParkingAreas.LastOrDefault().Value;
            if (bpa == null)
            {
                return null;
            }

            var bpac = bpa.BicycleParkingAreaComponent;
            bpac.NumOfSpaces += 1;


            if (bpaac.GetActualMax() > btc.Length)
            {
                bpac.NumOfSpaces -= 1;
                return null;
            }
            else if (!plc.IsPlacementValid(bpa))
            {
                bpac.NumOfSpaces -= 1;
                return null;
            }
            else
            {
                return bpa;
            }
        }

        private BicycleParkingArea AppendBicycleParkingSpaceByAdd(
            BikewayTile bt, WayTileSide side
        )
        {
            var plc = ParkingLot.ParkingLotComponent;
            var btc = bt.BikewayTileComponent;
            var bpaac = btc.GetBicycleParkingAreaAnchorComponent(side);

            var bpa = CreateBicycleParkingArea();

            double bpaOffset = bpaac.GetActualMax();

            while (bpaOffset < btc.Length)
            {
                try
                {
                    btc.AddBicycleParkingArea(bpa, side, bpaOffset);

                    if (bpaac.GetActualMax() > btc.Length)
                    {
                        btc.RemoveBicycleParkingArea(bpa); ;
                    }
                    else if (!plc.IsPlacementValid(bpa))
                    {
                        btc.RemoveBicycleParkingArea(bpa);
                    }
                    else
                    {
                        return bpa;
                    }
                }
                catch (ArgumentException)
                { }

                bpaOffset += BicycleParkingAreaComponentTemplate.SpaceWidth;
            }

            return null;
        }

        public BicycleParkingArea RemoveBicycleParkingSpace(
            BikewayTile bt, WayTileSide side
        )
        {
            if (ParkingLot == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ParkingLot)} is not set"
                );
            }

            if (bt == null)
            {
                throw new ArgumentNullException(nameof(bt));
            }

            var btc = bt.BikewayTileComponent;
            var bpaac = btc.GetBicycleParkingAreaAnchorComponent(side);

            var lastBpa = bpaac.BicycleParkingAreas.LastOrDefault().Value;

            if (lastBpa == null)
            {
                return null;
            }

            var lastBpac = lastBpa.BicycleParkingAreaComponent;

            if (lastBpac.NumOfSpaces == 1)
            {
                btc.RemoveBicycleParkingArea(lastBpa);
            }
            else
            {
                lastBpac.NumOfSpaces -= 1;
            }

            return lastBpa;
        }

        public bool PruneBikewayTile(BikewayTile bt)
        {
            if (bt == null)
            {
                throw new ArgumentNullException(nameof(bt));
            }

            var btc = bt.BikewayTileComponent;

            double btFitLength = btc.GetFitLength();

            if (btc.Length == btFitLength)
            {
                return false;
            }
            else
            {
                btc.Length = btFitLength;
                return true;
            }
        }

        public BikewayTile CreateBikewayTile()
        {
            var bt = new BikewayTile();

            bt.BikewayTileComponent.Width =
                BikewayTileComponentTemplate.Width;

            bt.BikewayTileComponent.Length =
                BikewayTileComponentTemplate.Length;

            return bt;
        }

        public BicycleParkingArea CreateBicycleParkingArea()
        {
            var bpa = new BicycleParkingArea();

            bpa.BicycleParkingAreaComponent.SpaceWidth =
                BicycleParkingAreaComponentTemplate.SpaceWidth;
            bpa.BicycleParkingAreaComponent.SpaceLength =
                BicycleParkingAreaComponentTemplate.SpaceLength;

            return bpa;
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new IWorkspaceItem[]
            {
                ParkingLot,
                BikewayTileComponentTemplate,
                BicycleParkingAreaComponentTemplate
            };

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(ParkingLot), ParkingLot);
            writer.AddValue(
                nameof(BikewayTileComponentTemplate),
                BikewayTileComponentTemplate
            );
            writer.AddValue(
                nameof(BicycleParkingAreaComponentTemplate),
                BicycleParkingAreaComponentTemplate
            );
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public ParkingLot ParkingLot { get; set; }

        public BikewayTileComponent BikewayTileComponentTemplate
        { get; } = new BikewayTileComponent();

        public BicycleParkingAreaComponent BicycleParkingAreaComponentTemplate
        { get; } = new BicycleParkingAreaComponent();

        #endregion
    }
}
