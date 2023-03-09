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
    /// The component for placing car parking areas on roadsides.
    /// </summary>
    [Serializable]
    public class RoadsideCarParkingAreaDesignerComponent : Component
    {
        #region Constructors

        public RoadsideCarParkingAreaDesignerComponent() : base()
        { }

        protected RoadsideCarParkingAreaDesignerComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            CarParkingAreaComponentTemplate =
                reader.GetValue<CarParkingAreaComponent>(
                    nameof(CarParkingAreaComponentTemplate)
                );

            _parkingLot = reader.GetValue<ParkingLot>(nameof(ParkingLot));

            _roadsideStartIndex =
                reader.GetValue<int>(nameof(RoadsideStartIndex));

            var roadsideCarParkingAreaParamsList =
                reader.GetValues<RoadsideCarParkingAreaParams>(
                    nameof(RoadsideCarParkingAreaParamsList)
                );

            foreach (var rcpaParams in roadsideCarParkingAreaParamsList)
            {
                _roadsideCarParkingAreaParamsList.Add(rcpaParams);
            }
        }

        #endregion

        #region Methods

        protected override void Update()
        {
            if (ParkingLot == null)
            {
                return;
            }

            AlignRoadsides();

            var numOfRoadsides =
                ParkingLot.ParkingLotComponent.Roadsides.Count;

            for (int roadsideIndex = 0; roadsideIndex < numOfRoadsides;
                ++roadsideIndex)
            {
                int shiftedRoadsideIndex =
                    (roadsideIndex + RoadsideStartIndex) % numOfRoadsides;

                UpdateRoadside(shiftedRoadsideIndex);
            }

            ParkingLotRequirementsComponent?.OnCarParkingStatsUpdated();
        }

        public void AlignRoadsides()
        {
            var plc = ParkingLot.ParkingLotComponent;
            var roadsides = plc.Roadsides;

            while (_roadsideCarParkingAreaParamsList.Count > roadsides.Count)
            {
                _roadsideCarParkingAreaParamsList.RemoveAt(
                    _roadsideCarParkingAreaParamsList.Count - 1
                );
            }

            while (_roadsideCarParkingAreaParamsList.Count < roadsides.Count)
            {
                _roadsideCarParkingAreaParamsList.Add(
                    new RoadsideCarParkingAreaParams()
                );
            }

            for (int roadsideIndex = 0; roadsideIndex < roadsides.Count;
                ++roadsideIndex)
            {
                var roadside = roadsides[roadsideIndex];

                double maxOffset = roadside.RoadsideComponent.RoadEdge.Length;

                var rcpap = _roadsideCarParkingAreaParamsList[roadsideIndex];

                rcpap.MaxOffset = maxOffset;
            }

            RoadsideStartIndex =
                Math.Max(
                    Math.Min(RoadsideStartIndex, plc.MaxRoadsideIndex),
                    0
                );
        }

        private void UpdateRoadside(int roadsideIndex)
        {
            var plc = ParkingLot.ParkingLotComponent;
            var roadsideComp = plc.Roadsides[roadsideIndex].RoadsideComponent;

            roadsideComp.ClearCarParkingAreas();

            var roadsideCpaParams =
                _roadsideCarParkingAreaParamsList[roadsideIndex];

            if (!roadsideCpaParams.IsEnabled)
            {
                return;
            }

            double roadsideOffset = roadsideCpaParams.Offset;

            double roadsideLength = roadsideComp.RoadEdge.Length;
            double cpsWidth = CarParkingAreaComponentTemplate.SpaceWidth;

            double currentOffset = roadsideOffset % roadsideLength;
            CarParkingArea currentCpa = null;

            while (currentOffset + cpsWidth <= roadsideLength)
            {
                if (plc.NumOfCarParkingSpaces >= CarParkingSpaceMaxTotal)
                {
                    break;
                }

                if (currentCpa == null)
                {
                    var cpa = CreateCarParkingArea();

                    roadsideComp.AddCarParkingArea(cpa, currentOffset);

                    if (plc.IsPlacementValid(cpa))
                    {
                        currentCpa = cpa;
                    }
                    else
                    {
                        roadsideComp.RemoveCarParkingArea(cpa);
                    }
                }
                else
                {
                    currentCpa.CarParkingAreaComponent.NumOfSpaces += 1;

                    if (!plc.IsPlacementValid(currentCpa))
                    {
                        currentCpa.CarParkingAreaComponent.NumOfSpaces -= 1;
                        currentCpa = null;
                    }
                }

                currentOffset += cpsWidth;
            }
        }

        public CarParkingArea CreateCarParkingArea()
        {
            var cpa = new CarParkingArea();

            cpa.CarParkingAreaComponent.SpaceWidth =
                CarParkingAreaComponentTemplate.SpaceWidth;
            cpa.CarParkingAreaComponent.SpaceLength =
                CarParkingAreaComponentTemplate.SpaceLength;

            return cpa;
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(CarParkingAreaComponentTemplate)
            .Append(ParkingLot);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(CarParkingAreaComponentTemplate),
                CarParkingAreaComponentTemplate
            );

            writer.AddValue(nameof(ParkingLot), _parkingLot);

            writer.AddValue(nameof(RoadsideStartIndex), _roadsideStartIndex);

            writer.AddValues(
                nameof(RoadsideCarParkingAreaParamsList),
                _roadsideCarParkingAreaParamsList
            );
        }

        #endregion

        #region Properties

        public ParkingLotRequirementsComponent ParkingLotRequirementsComponent
            => SceneObject?.GetComponent<ParkingLotRequirementsComponent>();

        public double CarParkingSpaceMaxTotal =>
            ParkingLotRequirementsComponent?.CarParkingSpaceMaxTotal ??
            DefaultCarParkingSpaceMaxTotal;

        public CarParkingAreaComponent CarParkingAreaComponentTemplate
        { get; } = new CarParkingAreaComponent();

        public ParkingLot ParkingLot
        {
            get => _parkingLot;
            set
            {
                _parkingLot = value;
                NotifyPropertyChanged();
            }
        }

        public int RoadsideStartIndex
        {
            get => _roadsideStartIndex;
            set
            {
                _roadsideStartIndex = value;
                NotifyPropertyChanged();
            }
        }

        public IReadOnlyList<RoadsideCarParkingAreaParams>
            RoadsideCarParkingAreaParamsList =>
            _roadsideCarParkingAreaParamsList;

        #endregion

        #region Member variables

        private ParkingLot _parkingLot;

        private int _roadsideStartIndex = DefaultRoadsideStartIndex;

        private readonly ObservableCollection<RoadsideCarParkingAreaParams>
            _roadsideCarParkingAreaParamsList =
            new ObservableCollection<RoadsideCarParkingAreaParams>();

        #endregion

        #region Constants

        public const int DefaultRoadsideStartIndex = 0;

        public const double DefaultCarParkingSpaceMaxTotal =
            Double.PositiveInfinity;

        #endregion
    }
}
