using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The state that the bikeway shape grammar works on.
    /// </summary>
    [Serializable]
    public class BikewaySGState : ISGState
    {
        #region Constructors

        public BikewaySGState()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected BikewaySGState(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            _markers.AddRange(reader.GetValues<ISGMarker>(nameof(Markers)));

            _plrc = reader.GetValue<ParkingLotRequirementsComponent>(
                nameof(ParkingLotRequirementsComponent)
            );

            BikewayBuilder =
                reader.GetValue<BikewayBuilder>(nameof(BikewayBuilder));
        }

        #endregion

        #region Methods

        public void Reset()
        {
            Markers.Clear();

            AddBikewayBeginMarkers();
        }

        private void AddBikewayBeginMarkers()
        {
            if (ParkingLot == null)
            {
                return;
            }

            var plc = ParkingLot.ParkingLotComponent;

            foreach (var wt in plc.WalkwayTiles)
            {
                Markers.Add(
                    new BikewayBeginMarker
                    {
                        WalkwayTile = wt
                    }
                );
            }
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>()
            .Concat(Markers)
            .Append(ParkingLotRequirementsComponent)
            .Append(BikewayBuilder);

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValues(nameof(Markers), _markers);
            writer.AddValue(nameof(ParkingLotRequirementsComponent), _plrc);
            writer.AddValue(nameof(BikewayBuilder), BikewayBuilder);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public IList<ISGMarker> Markers => _markers;

        public ParkingLot ParkingLot
        {
            get => BikewayBuilder.ParkingLot;
            set => BikewayBuilder.ParkingLot = value;
        }

        public ParkingLotRequirementsComponent ParkingLotRequirementsComponent
        {
            get => _plrc;
            set
            {
                _plrc = value ??
                    throw new ArgumentNullException(nameof(value));
            }
        }

        public BikewayBuilder BikewayBuilder { get; } =
            new BikewayBuilder();

        #endregion

        #region Member variables

        private readonly List<ISGMarker> _markers = new List<ISGMarker>();

        private ParkingLotRequirementsComponent _plrc =
            new ParkingLotRequirementsComponent();

        #endregion
    }
}
