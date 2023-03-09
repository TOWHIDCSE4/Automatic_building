using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Scene;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component that represents a roadside of the site boundary
    /// that can be used for car parking.
    /// </summary>
    [Serializable]
    public class RoadsideComponent : Component
    {
        #region Constructors

        public RoadsideComponent() : base()
        { }

        protected RoadsideComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _roadEdgeIndexIndex =
                reader.GetValue<int>(nameof(RoadEdgeIndexIndex));
        }

        #endregion

        #region Methods

        public void UpdateTransformComponent()
        {
            if (TransformComponent == null) { return; }

            if (RoadEdge == null) { return; }

            var transform = new TrsTransform3D();

            transform.SetTranslate(RoadEdge.Point0.Vector);

            transform.Rz = MathUtils.GetAngle2D(
                new DenseVector(new[] { 1.0, 0.0, 0.0 }),
                RoadEdge.Direction
            );

            TransformComponent.Transform = transform;
        }

        public void AddCarParkingArea(CarParkingArea cpa, double offset)
        {
            var cpaac = CarParkingAreaAnchorComponent;
            if (cpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CarParkingAreaAnchorComponent)} " +
                    "is not found"
                );
            }

            cpaac.AddCarParkingArea(cpa, offset);
        }

        public bool RemoveCarParkingArea(CarParkingArea cpa)
        {
            var cpaac = CarParkingAreaAnchorComponent;
            if (cpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CarParkingAreaAnchorComponent)} " +
                    "is not found"
                );
            }

            return cpaac.RemoveCarParkingArea(cpa);
        }

        public void ClearCarParkingAreas()
        {
            var cpaac = CarParkingAreaAnchorComponent;
            if (cpaac == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CarParkingAreaAnchorComponent)} " +
                    "is not found"
                );
            }

            cpaac.ClearCarParkingAreas();
        }

        protected override void OnSceneObjectParented()
        {
            base.OnSceneObjectParented();

            UpdateTransformComponent();
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(RoadEdgeIndexIndex), _roadEdgeIndexIndex);
        }

        #endregion

        #region Properties

        public TransformComponent TransformComponent =>
            SceneObject?.GetComponent<TransformComponent>();

        public CarParkingAreaAnchorComponent CarParkingAreaAnchorComponent =>
            SceneObject?.GetComponent<CarParkingAreaAnchorComponent>();

        public ParkingLot ParkingLot => SceneObject?.Parent as ParkingLot;

        public Site Site => ParkingLot?.ParkingLotComponent.Site;

        public int RoadEdgeIndexIndex
        {
            get => _roadEdgeIndexIndex;
            set
            {
                if (Site == null)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(value),
                            $"{nameof(value)} cannot be negative"
                        );
                    }
                }
                else
                {
                    if (
                        value < 0 ||
                        value >= Site.SiteComponent.RoadEdges.Count
                    )
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(value),
                            $"{nameof(value)} must be " +
                            "between 0 (inclusive) and the number of " +
                            "road edges on the site boundary (exclusive)"
                        );
                    }
                }

                _roadEdgeIndexIndex = value;

                ClearCarParkingAreas();
                UpdateTransformComponent();

                NotifyPropertyChanged();
            }
        }

        public LineSegment RoadEdge =>
            Site?.SiteComponent.RoadEdges[RoadEdgeIndexIndex];

        public IReadOnlyList<CarParkingArea> CarParkingAreas =>
            CarParkingAreaAnchorComponent
            ?.CarParkingAreas.Values.ToList() ??
            new List<CarParkingArea>();

        #endregion

        #region Member variables

        private int _roadEdgeIndexIndex;

        #endregion
    }
}
