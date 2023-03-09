using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component for organizing car parking areas along a line.
    /// </summary>
    [Serializable]
    public class CarParkingAreaAnchorComponent : Component
    {
        #region Constructors

        public CarParkingAreaAnchorComponent() : base()
        { }

        protected CarParkingAreaAnchorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            var carParkingAreaPairs =
                reader.GetValues<KeyValuePair<double, CarParkingArea>>(
                    nameof(CarParkingAreas)
                );

            foreach (var pair in carParkingAreaPairs)
            {
                _carParkingAreas.Add(pair.Key, pair.Value);
                _carParkingAreaOffsets.Add(pair.Value, pair.Key);
            }

            _transform = reader.GetValue<TrsTransform3D>(nameof(Transform));
        }

        #endregion

        #region Methods

        public void AddCarParkingArea(CarParkingArea cpa, double offset)
        {
            if (SceneObject == null)
            {
                throw new InvalidOperationException(
                    "Cannot add car parking area when " +
                    $"{nameof(SceneObject)} is null"
                );
            }

            if (cpa == null)
            {
                throw new ArgumentNullException(nameof(cpa));
            }

            if (_carParkingAreaOffsets.ContainsKey(cpa))
            {
                throw new ArgumentException(
                    "The car parking area is already added",
                    nameof(cpa)
                );
            }

            if (_carParkingAreas.ContainsKey(offset))
            {
                throw new ArgumentException(
                    "A car parking area exists at offset",
                    nameof(offset)
                );
            }

            SceneObject.AddChild(cpa);

            _carParkingAreas.Add(offset, cpa);
            _carParkingAreaOffsets.Add(cpa, offset);

            cpa.TransformComponent.Transform = GetTransform(offset);
        }

        public bool RemoveCarParkingArea(CarParkingArea cpa)
        {
            if (SceneObject == null)
            {
                throw new InvalidOperationException(
                    "Cannot remove car parking area when " +
                    $"{nameof(SceneObject)} is null"
                );
            }

            if (!_carParkingAreaOffsets.ContainsKey(cpa))
            {
                return false;
            }

            double offset = _carParkingAreaOffsets[cpa];

            _carParkingAreas.Remove(offset);
            _carParkingAreaOffsets.Remove(cpa);

            SceneObject.RemoveChild(cpa);
            SceneObject.Scene?.RemoveSceneObject(cpa);

            return true;
        }

        public void ClearCarParkingAreas()
        {
            foreach (var cpa in CarParkingAreas.Values.ToList())
            {
                RemoveCarParkingArea(cpa);
            }
        }

        public TrsTransform3D GetTransform(double offset)
        {
            var transform = new TrsTransform3D(Transform);

            transform.SetTranslateLocal(offset, 0.0, 0.0);

            return transform;
        }

        public void UpdateCarParkingAreaTransforms()
        {
            foreach (double offset in _carParkingAreas.Keys)
            {
                var cpa = _carParkingAreas[offset];

                cpa.TransformComponent.Transform = GetTransform(offset);
            }
        }

        public double GetActualMin()
        {
            if (_carParkingAreas.Count == 0) { return 0.0; }

            return _carParkingAreas.Keys.Min();
        }

        public double GetActualMax()
        {
            if (_carParkingAreas.Count == 0) { return 0.0; }

            return
                _carParkingAreas
                .Select(
                    pair =>
                    pair.Key + pair.Value.CarParkingAreaComponent.Width
                ).Max();
        }

        public double GetActualLength()
        {
            return GetActualMax() - GetActualMin();
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Concat(_carParkingAreas.Values);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValues(
                nameof(CarParkingAreas), _carParkingAreas.ToList()
            );
            writer.AddValue(nameof(Transform), _transform);
        }

        #endregion

        #region Properties

        public IReadOnlyDictionary<double, CarParkingArea>
            CarParkingAreas => _carParkingAreas;

        public IReadOnlyDictionary<CarParkingArea, double>
            CarParkingAreaOffsets => _carParkingAreaOffsets;

        public TrsTransform3D Transform
        {
            get => _transform;
            set
            {
                _transform =
                    value ??
                    throw new ArgumentNullException(nameof(value));

                UpdateCarParkingAreaTransforms();
            }
        }

        #endregion

        #region Member variables

        private readonly SortedList<double, CarParkingArea>
            _carParkingAreas =
            new SortedList<double, CarParkingArea>();

        private readonly Dictionary<CarParkingArea, double>
            _carParkingAreaOffsets =
            new Dictionary<CarParkingArea, double>();

        private TrsTransform3D _transform = new TrsTransform3D();

        #endregion
    }
}
