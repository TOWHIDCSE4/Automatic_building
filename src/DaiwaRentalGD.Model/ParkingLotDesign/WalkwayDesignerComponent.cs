using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// The component for designing walkways on a parking lot.
    /// </summary>
    [Serializable]
    public class WalkwayDesignerComponent : Component
    {
        #region Constructors

        public WalkwayDesignerComponent() : base()
        {
            Initialize();
        }

        protected WalkwayDesignerComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _parkingLot = reader.GetValue<ParkingLot>(nameof(ParkingLot));

            _numOfWalkwayEntrances =
                reader.GetValue<int>(nameof(NumOfWalkwayEntrances));

            var walkwayEntrances =
                reader.GetValues<WalkwayEntrance>(nameof(WalkwayEntrances));

            foreach (var walkwayEntrance in walkwayEntrances)
            {
                _walkwayEntrances.Add(walkwayEntrance);
            }

            WalkwayEntranceComponentTemplate =
                reader.GetValue<WalkwayEntranceComponent>(
                    nameof(WalkwayEntranceComponentTemplate)
                );

            WalkwayVisibilityGraphCreator =
                reader.GetValue<WalkwayVisibilityGraphCreator>(
                    nameof(WalkwayVisibilityGraphCreator)
                );

            WalkwayPathfinder =
                reader.GetValue<WalkwayPathfinder>(nameof(WalkwayPathfinder));
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            AlignWalkwayEntrances();
        }

        protected override void Update()
        {
            if (ParkingLot == null)
            {
                return;
            }

            UpdateWalkwayEntranceRoadEdgeIndexIndices();
            AddWalkwayEntrances();
            CreateVisibilityGrpah();
            CreateWalkways();
        }

        private void AlignWalkwayEntrances()
        {
            while (_walkwayEntrances.Count > NumOfWalkwayEntrances)
            {
                _walkwayEntrances.RemoveAt(_walkwayEntrances.Count - 1);
            }

            while (_walkwayEntrances.Count < NumOfWalkwayEntrances)
            {
                _walkwayEntrances.Add(CreateWalkwayEntrance());
            }
        }

        private WalkwayEntrance CreateWalkwayEntrance()
        {
            var we = new WalkwayEntrance();

            we.WalkwayEntranceComponent.Width =
                WalkwayEntranceComponentTemplate.Width;

            return we;
        }

        private void UpdateWalkwayEntranceRoadEdgeIndexIndices()
        {
            int maxRoadsideIndex =
                ParkingLot.ParkingLotComponent.MaxRoadsideIndex;

            if (maxRoadsideIndex < 0)
            {
                return;
            }

            foreach (var we in WalkwayEntrances)
            {
                we.WalkwayEntranceComponent.RoadEdgeIndexIndex = Math.Min(
                    we.WalkwayEntranceComponent.RoadEdgeIndexIndex,
                    maxRoadsideIndex
                );
            }
        }

        private void AddWalkwayEntrances()
        {
            var plc = ParkingLot.ParkingLotComponent;

            foreach (var we in WalkwayEntrances)
            {
                plc.AddWalkwayEntrance(we);
            }
        }

        private void CreateVisibilityGrpah()
        {
            var wg = WalkwayVisibilityGraphCreator.Create(ParkingLot);

            ParkingLot.ParkingLotComponent.WalkwayGraph = wg;
        }

        private void CreateWalkways()
        {
            var plc = ParkingLot.ParkingLotComponent;
            var wg = plc.WalkwayGraph;

            var svs =
                wg.Vertices
                .Where(v => v.Type == WalkwayGraphVertexType.Source)
                .ToList();

            var dvs =
                wg.Vertices
                .Where(v => v.Type == WalkwayGraphVertexType.Destination)
                .ToList();

            for (int dvi = 0; dvi < dvs.Count - 1; ++dvi)
            {
                var dv = dvs[dvi];
                var dv1 = dvs[dvi + 1];
                var wp = WalkwayPathfinder.FindPath(wg, dv, dv1);

                if (!wp.IsValid)
                {
                    continue;
                }

                plc._walkwayPaths.Add(wp);
                wp.AddWalkwayTiles();
            }

            foreach (var sv in svs)
            {
                double shortestWpLength = double.PositiveInfinity;
                WalkwayPath shortestWp = null;

                foreach (var dv in dvs)
                {
                    var wp = WalkwayPathfinder.FindPath(wg, sv, dv);

                    if (!wp.IsValid)
                    {
                        continue;
                    };

                    if (wp.Length < shortestWpLength)
                    {
                        shortestWpLength = wp.Length;
                        shortestWp = wp;
                    }
                }

                if (shortestWp == null)
                {
                    continue;
                }

                plc._walkwayPaths.Add(shortestWp);
                shortestWp.AddWalkwayTiles();
            }
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(ParkingLot)
            .Concat(WalkwayEntrances)
            .Append(WalkwayEntranceComponentTemplate)
            .Append(WalkwayVisibilityGraphCreator)
            .Append(WalkwayPathfinder);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(ParkingLot), _parkingLot);
            writer.AddValue(
                nameof(NumOfWalkwayEntrances), _numOfWalkwayEntrances
            );
            writer.AddValues(nameof(WalkwayEntrances), _walkwayEntrances);
            writer.AddValue(
                nameof(WalkwayEntranceComponentTemplate),
                WalkwayEntranceComponentTemplate
            );
            writer.AddValue(
                nameof(WalkwayVisibilityGraphCreator),
                WalkwayVisibilityGraphCreator
            );
            writer.AddValue(nameof(WalkwayPathfinder), WalkwayPathfinder);
        }

        #endregion

        #region Properties

        public ParkingLot ParkingLot
        {
            get => _parkingLot;
            set
            {
                _parkingLot = value;
                NotifyPropertyChanged();
            }
        }
        public bool OverlapWithDriveways
        {
            get => _OverlapWithDriveways;
            set
            {
                _OverlapWithDriveways = value;
                _parkingLot.ParkingLotComponent.ParkingLotDesigner.setDesignOrder_Overlap(value);
                NotifyPropertyChanged();
            }
        }


        public int NumOfWalkwayEntrances
        {
            get => _numOfWalkwayEntrances;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(NumOfWalkwayEntrances)} must be positive"
                    );
                }

                _numOfWalkwayEntrances = value;

                AlignWalkwayEntrances();

                NotifyPropertyChanged();
            }
        }

        public IReadOnlyList<WalkwayEntrance> WalkwayEntrances =>
            _walkwayEntrances;

        public WalkwayEntranceComponent WalkwayEntranceComponentTemplate
        { get; } = new WalkwayEntranceComponent();

        public WalkwayVisibilityGraphCreator WalkwayVisibilityGraphCreator
        { get; } = new WalkwayVisibilityGraphCreator();

        public WalkwayPathfinder WalkwayPathfinder
        { get; } = new WalkwayPathfinder();

        #endregion

        #region Member variables

        private ParkingLot _parkingLot;
        private bool _OverlapWithDriveways = DefaultOverlapWithDriveways;

        private int _numOfWalkwayEntrances = DefaultNumOfWalkwayEntrances;

        private readonly ObservableCollection<WalkwayEntrance>
            _walkwayEntrances =
            new ObservableCollection<WalkwayEntrance>();

        #endregion

        #region Constants

        public const int DefaultNumOfWalkwayEntrances = 1;
        public const bool DefaultOverlapWithDriveways = false;

        #endregion
    }
}
