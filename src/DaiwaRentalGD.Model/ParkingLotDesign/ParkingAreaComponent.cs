using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component representing parking area consisting of
    /// one or more parking spaces for certain types of vehicles.
    /// </summary>
    [Serializable]
    public class ParkingAreaComponent : Component
    {
        #region Constructors

        public ParkingAreaComponent() : base()
        { }

        protected ParkingAreaComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _spaceWidth = reader.GetValue<double>(nameof(SpaceWidth));
            _spaceLength = reader.GetValue<double>(nameof(SpaceLength));
            _numOfSpaces = reader.GetValue<int>(nameof(NumOfSpaces));
            _isWidthDirectionInverted =
                reader.GetValue<bool>(nameof(IsWidthDirectionInverted));
        }

        #endregion

        #region Methods

        public Polygon GetPlan()
        {
            Polygon plan = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(Width, 0.0, 0.0),
                    new Point(Width, Length, 0.0),
                    new Point(0.0, Length, 0.0)
                }
            );

            return plan;
        }

        public Polygon GetTransformedPlan()
        {
            var plan = GetPlan();

            plan.Transform(GetTransform());

            return plan;
        }

        public Polygon GetSpacePlan()
        {
            Polygon spacePlan = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(SpaceWidth, 0.0, 0.0),
                    new Point(SpaceWidth, Length, 0.0),
                    new Point(0.0, Length, 0.0)
                }
            );

            return spacePlan;
        }

        public Polygon GetTransformedSpacePlan(int spaceIndex)
        {
            if (spaceIndex < 0 || spaceIndex >= NumOfSpaces)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(spaceIndex),
                    $"{nameof(spaceIndex)} must be between " +
                    $"0 (inclusive) and {nameof(NumOfSpaces)} (exclusive)"
                );
            }

            var spacePlan = GetSpacePlan();

            spacePlan.Transform(GetSpaceTransform(spaceIndex));
            spacePlan.Transform(GetTransform());

            return spacePlan;
        }

        public void UpdateCollisionBody2D()
        {
            var cbc = SceneObject?.GetComponent<CollisionBody2DComponent>();

            if (cbc == null) { return; }

            cbc.ClearCollisionPolygons();

            var collisionPolygon = GetTransformedPlan();
            collisionPolygon.OffsetEdges(-cbc.Epsilon);

            cbc.AddCollisionPolygon(collisionPolygon);
        }

        public TrsTransform3D GetSpaceTransform(int spaceIndex)
        {
            var spaceTransform = new TrsTransform3D
            {
                Tx = SpaceWidth * spaceIndex
            };

            return spaceTransform;
        }

        public TrsTransform3D GetTransform()
        {
            if (IsWidthDirectionInverted)
            {
                return new TrsTransform3D
                {
                    Tx = Width,
                    Rz = Math.PI
                };
            }
            else
            {
                return new TrsTransform3D();
            }
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(SpaceWidth), _spaceWidth);
            writer.AddValue(nameof(SpaceLength), _spaceLength);
            writer.AddValue(nameof(NumOfSpaces), _numOfSpaces);
            writer.AddValue(
                nameof(IsWidthDirectionInverted), _isWidthDirectionInverted
            );
        }

        #endregion

        #region Properties

        public double SpaceWidth
        {
            get => _spaceWidth;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SpaceWidth)} cannot be negative"
                    );
                }

                _spaceWidth = value;
                UpdateCollisionBody2D();
            }
        }

        public double SpaceLength
        {
            get => _spaceLength;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SpaceLength)} cannot be negative"
                    );
                }

                _spaceLength = value;
                UpdateCollisionBody2D();
            }
        }

        public int NumOfSpaces
        {
            get => _numOfSpaces;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(NumOfSpaces)} must be positive"
                    );
                }

                _numOfSpaces = value;
                UpdateCollisionBody2D();
            }
        }

        public bool IsWidthDirectionInverted
        {
            get => _isWidthDirectionInverted;
            set
            {
                _isWidthDirectionInverted = value;
                UpdateCollisionBody2D();
            }
        }

        public double Width => SpaceWidth * NumOfSpaces;

        public double Length => SpaceLength;

        #endregion

        #region Member variables

        private double _spaceWidth = DefaultSpaceWidth;
        private double _spaceLength = DefaultSpaceLength;
        private int _numOfSpaces = DefaultNumOfSpaces;
        private bool _isWidthDirectionInverted =
            DefaultIsWidthDirectionInverted;

        #endregion

        #region Constants

        public const double DefaultSpaceWidth = 1.0;
        public const double DefaultSpaceLength = 2.0;
        public const int DefaultNumOfSpaces = 1;
        public const bool DefaultIsWidthDirectionInverted = false;

        #endregion
    }
}
