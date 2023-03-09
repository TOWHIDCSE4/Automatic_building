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
    /// A component for organizing bicycle parking areas along a line.
    /// </summary>
    [Serializable]
    public class BicycleParkingAreaAnchorComponent : Component
    {
        #region Constructors

        public BicycleParkingAreaAnchorComponent() : base()
        { }

        protected BicycleParkingAreaAnchorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            var bicycleParkingAreaPairs =
                reader.GetValues<KeyValuePair<double, BicycleParkingArea>>(
                    nameof(BicycleParkingAreas)
                );

            foreach (var pair in bicycleParkingAreaPairs)
            {
                _bicycleParkingAreas.Add(pair.Key, pair.Value);
                _bicycleParkingAreaOffsets.Add(pair.Value, pair.Key);
            }

            _transform = reader.GetValue<TrsTransform3D>(nameof(Transform));
        }

        #endregion

        #region Methods

        public void AddBicycleParkingArea(
            BicycleParkingArea bpa, double offset
        )
        {
            if (SceneObject == null)
            {
                throw new InvalidOperationException(
                    "Cannot add bicycle parking area when " +
                    $"{nameof(SceneObject)} is null"
                );
            }

            if (bpa == null)
            {
                throw new ArgumentNullException(nameof(bpa));
            }

            if (_bicycleParkingAreaOffsets.ContainsKey(bpa))
            {
                throw new ArgumentException(
                    "The bicycle parking area is already added",
                    nameof(bpa)
                );
            }

            if (_bicycleParkingAreas.ContainsKey(offset))
            {
                throw new ArgumentException(
                    "A bicycle parking area exists at offset",
                    nameof(offset)
                );
            }

            SceneObject.AddChild(bpa);

            _bicycleParkingAreas.Add(offset, bpa);
            _bicycleParkingAreaOffsets.Add(bpa, offset);

            bpa.TransformComponent.Transform = GetTransform(offset);
        }

        public bool RemoveBicycleParkingArea(BicycleParkingArea bpa)
        {
            if (SceneObject == null)
            {
                throw new InvalidOperationException(
                    "Cannot remove bicycle parking area when " +
                    $"{nameof(SceneObject)} is null"
                );
            }

            if (!_bicycleParkingAreaOffsets.ContainsKey(bpa))
            {
                return false;
            }

            double offset = _bicycleParkingAreaOffsets[bpa];

            _bicycleParkingAreas.Remove(offset);
            _bicycleParkingAreaOffsets.Remove(bpa);

            SceneObject.RemoveChild(bpa);
            SceneObject.Scene?.RemoveSceneObject(bpa);

            return true;
        }

        public void ClearBicycleParkingAreas()
        {
            foreach (var bpa in BicycleParkingAreas.Values.ToList())
            {
                RemoveBicycleParkingArea(bpa);
            }
        }

        public TrsTransform3D GetTransform(double offset)
        {
            var transform = new TrsTransform3D(Transform);

            transform.SetTranslateLocal(offset, 0.0, 0.0);

            return transform;
        }

        public void UpdateBicycleParkingAreaTransforms()
        {
            foreach (double offset in _bicycleParkingAreas.Keys)
            {
                var bpa = _bicycleParkingAreas[offset];

                bpa.TransformComponent.Transform = GetTransform(offset);
            }
        }

        public double GetActualMin()
        {
            if (_bicycleParkingAreas.Count == 0) { return 0.0; }

            return _bicycleParkingAreas.Keys.Min();
        }

        public double GetActualMax()
        {
            if (_bicycleParkingAreas.Count == 0) { return 0.0; }

            return
                _bicycleParkingAreas
                .Select(
                    pair =>
                    pair.Key + pair.Value.BicycleParkingAreaComponent.Width
                ).Max();
        }

        public double GetActualLength()
        {
            return GetActualMax() - GetActualMin();
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Concat(_bicycleParkingAreas.Values);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValues(
                nameof(BicycleParkingAreas), _bicycleParkingAreas.ToList()
            );
            writer.AddValue(nameof(Transform), _transform);
        }

        #endregion

        #region Properties

        public IReadOnlyDictionary<double, BicycleParkingArea>
            BicycleParkingAreas => _bicycleParkingAreas;

        public IReadOnlyDictionary<BicycleParkingArea, double>
            BicycleParkingAreaOffsets => _bicycleParkingAreaOffsets;

        public TrsTransform3D Transform
        {
            get => _transform;
            set
            {
                _transform =
                    value ??
                    throw new ArgumentNullException(nameof(value));

                UpdateBicycleParkingAreaTransforms();
            }
        }

        #endregion

        #region Member variables

        private readonly SortedList<double, BicycleParkingArea>
            _bicycleParkingAreas =
            new SortedList<double, BicycleParkingArea>();

        private readonly Dictionary<BicycleParkingArea, double>
            _bicycleParkingAreaOffsets =
            new Dictionary<BicycleParkingArea, double>();

        private TrsTransform3D _transform = new TrsTransform3D();

        #endregion
    }
}
