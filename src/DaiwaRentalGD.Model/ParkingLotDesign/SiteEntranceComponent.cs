using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Scene;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component representing an entrance on the site boundary.
    /// </summary>
    [Serializable]
    public class SiteEntranceComponent : Component
    {
        #region Constructors

        public SiteEntranceComponent() : base()
        { }

        protected SiteEntranceComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _roadEdgeIndexIndex =
                reader.GetValue<int>(nameof(RoadEdgeIndexIndex));

            _roadEdgeSafeParam =
                reader.GetValue<double>(nameof(RoadEdgeSafeParam));

            _width = reader.GetValue<double>(nameof(Width));
        }

        #endregion

        #region Methods

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(RoadEdgeIndexIndex), _roadEdgeIndexIndex);
            writer.AddValue(nameof(RoadEdgeSafeParam), _roadEdgeSafeParam);
            writer.AddValue(nameof(Width), _width);
        }

        #endregion

        #region Properties

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
                            $"{nameof(RoadEdgeIndexIndex)} cannot be negative"
                        );
                    }
                }
                else
                {
                    if (
                        value < 0 ||
                        value >= Site.SiteComponent.RoadEdgeIndices.Count
                    )
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(value),
                            $"{nameof(RoadEdgeIndexIndex)} must be " +
                            "between 0 (inclusive) and the number of " +
                            "road edges on the boundary (exclusive)"
                        );
                    }
                }

                _roadEdgeIndexIndex = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(RoadEdgeLength));
                NotifyPropertyChanged(nameof(RoadEdgeSafeLength));
                NotifyPropertyChanged(nameof(RoadEdgeDistance));
                NotifyPropertyChanged(nameof(Point));
                NotifyPropertyChanged(nameof(Direction));
                NotifyPropertyChanged(nameof(Transform));
            }
        }

        public double RoadEdgeSafeParam
        {
            get => _roadEdgeSafeParam;
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(RoadEdgeSafeParam)} must be between " +
                        "0.0 (inclusive) and 1.0 (inclusive)"
                    );
                }

                _roadEdgeSafeParam = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(RoadEdgeDistance));
                NotifyPropertyChanged(nameof(Point));
                NotifyPropertyChanged(nameof(Transform));
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(Width)} cannot be negative"
                    );
                }

                _width = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(RoadEdgeSafeLength));
                NotifyPropertyChanged(nameof(RoadEdgeDistance));
                NotifyPropertyChanged(nameof(Point));
                NotifyPropertyChanged(nameof(Transform));
            }
        }

        public double RoadEdgeLength
        {
            get
            {
                if (Site == null)
                {
                    throw new InvalidOperationException(
                        "The site entrance is not attached to a site"
                    );
                }

                Polygon boundary = Site.SiteComponent.Boundary;

                int roadEdgeIndex =
                    Site.SiteComponent.RoadEdgeIndices[RoadEdgeIndexIndex];

                LineSegment roadEdge = boundary.GetEdge(roadEdgeIndex);

                return roadEdge.Length;
            }
        }

        public double RoadEdgeSafeLength
        {
            get
            {
                double safeLength = Math.Max(0.0, RoadEdgeLength - Width);
                return safeLength;
            }
        }

        public double RoadEdgeDistance =>
            RoadEdgeSafeLength * RoadEdgeSafeParam;

        public Point Point
        {
            get
            {
                if (Site == null)
                {
                    throw new InvalidOperationException(
                        "The site entrance is not attached to a site"
                    );
                }

                Polygon boundary = Site.SiteComponent.Boundary;

                int roadEdgeIndex =
                    Site.SiteComponent.RoadEdgeIndices[RoadEdgeIndexIndex];

                LineSegment roadEdge = boundary.GetEdge(roadEdgeIndex);

                Point point =
                    roadEdge.GetPointByLength(RoadEdgeDistance);

                return point;
            }
        }

        public Vector<double> Direction
        {
            get
            {
                if (Site == null)
                {
                    throw new InvalidOperationException(
                        "The site entrance is not attached to a site"
                    );
                }

                Polygon boundary = Site.SiteComponent.Boundary;

                int roadEdgeIndex =
                    Site.SiteComponent.RoadEdgeIndices[RoadEdgeIndexIndex];

                var direction = -boundary.GetEdgeNormal(roadEdgeIndex);
                return direction;
            }
        }

        public TrsTransform3D Transform
        {
            get
            {
                var transform = new TrsTransform3D();

                transform.SetTranslate(Point.Vector);
                transform.Rz = MathUtils.GetAngle2D(
                    new DenseVector(new[] { 0.0, 1.0, 0.0 }),
                    Direction
                );

                return transform;
            }
        }

        #endregion

        #region Member variables

        private int _roadEdgeIndexIndex;
        private double _roadEdgeSafeParam;
        private double _width;

        #endregion
    }
}
