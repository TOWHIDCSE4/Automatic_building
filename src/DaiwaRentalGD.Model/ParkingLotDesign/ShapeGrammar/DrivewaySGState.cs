using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ShapeGrammar;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign.ShapeGrammar
{
    /// <summary>
    /// The state that the driveway shape grammar works on.
    /// </summary>
    [Serializable]
    public class DrivewaySGState : ISGState
    {
        #region Constructors

        public DrivewaySGState()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected DrivewaySGState(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            _markers.AddRange(reader.GetValues<ISGMarker>(nameof(Markers)));

            _plrc = reader.GetValue<ParkingLotRequirementsComponent>(
                nameof(ParkingLotRequirementsComponent)
            );

            DrivewayBuilder =
                reader.GetValue<DrivewayBuilder>(nameof(DrivewayBuilder));
        }

        #endregion

        #region Methods

        public void Reset()
        {
            Markers.Clear();

            AddDrivewayBeginMarkers();
        }

        private void AddDrivewayBeginMarkers()
        {
            if (ParkingLot == null) { return; }

            var des = ParkingLot.ParkingLotComponent.DrivewayEntrances;

            foreach (var de in des)
            {
                var marker = new DrivewayBeginMarker
                {
                    DrivewayEntrance = de
                };
                Markers.Add(marker);
            }
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>()
            .Concat(Markers)
            .Append(ParkingLotRequirementsComponent)
            .Append(DrivewayBuilder);

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValues(nameof(Markers), _markers);
            writer.AddValue(nameof(ParkingLotRequirementsComponent), _plrc);
            writer.AddValue(nameof(DrivewayBuilder), DrivewayBuilder);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public IList<ISGMarker> Markers => _markers;

        public ParkingLot ParkingLot
        {
            get => DrivewayBuilder.ParkingLot;
            set => DrivewayBuilder.ParkingLot = value;
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

        public DrivewayBuilder DrivewayBuilder { get; } =
            new DrivewayBuilder();

        #endregion

        #region Member variables

        private readonly List<ISGMarker> _markers = new List<ISGMarker>();

        private ParkingLotRequirementsComponent _plrc =
            new ParkingLotRequirementsComponent();

        #endregion
    }
}
