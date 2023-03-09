using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component representing a way tile that is the basic unit
    /// of a way.
    /// </summary>
    [Serializable]
    public class WayTileComponent : Component
    {
        #region Constructors

        public WayTileComponent() : base()
        { }

        protected WayTileComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _width = reader.GetValue<double>(nameof(Width));
            _length = reader.GetValue<double>(nameof(Length));
            _steerAngle = reader.GetValue<double>(nameof(SteerAngle));

            Side = reader.GetValue<WayTileSide>(nameof(Side));

            _forwardWayTile =
                reader.GetValue<SceneObject>(nameof(ForwardWayTile));
            _leftWayTile =
                reader.GetValue<SceneObject>(nameof(LeftWayTile));
            _rightWayTile =
                reader.GetValue<SceneObject>(nameof(RightWayTile));
            PreviousWayTile =
                reader.GetValue<SceneObject>(nameof(PreviousWayTile));
        }

        #endregion

        #region Methods

        #region Geometry

        public Polygon GetPlan()
        {
            if (SteerAngle > 0.0)
            {
                return GetLeftSteerPlan();
            }
            else if (SteerAngle < 0.0)
            {
                return GetRightSteerPlan();
            }
            else
            {
                return GetNoSteerPlan();
            }
        }

        private Polygon GetLeftSteerPlan()
        {
            Polygon plan = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(Width, 0.0, 0.0),
                    new Point(
                        Width,
                        Width * Math.Tan(SteerAngle / 2.0),
                        0.0
                    ),
                    new Point(
                        Width * Math.Cos(SteerAngle) -
                        Length * Math.Sin(SteerAngle),
                        Width * Math.Sin(SteerAngle) +
                        Length * Math.Cos(SteerAngle),
                        0.0
                    ),
                    new Point(
                        -Length * Math.Sin(SteerAngle),
                        Length * Math.Cos(SteerAngle),
                        0.0
                    )
                }
            );

            return plan;
        }

        private Polygon GetRightSteerPlan()
        {
            double unsignedSteerAngle = -SteerAngle;

            Polygon plan = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(Width, 0.0, 0.0),
                    new Point(
                        Width + Length * Math.Sin(unsignedSteerAngle),
                        Length * Math.Cos(unsignedSteerAngle),
                        0.0
                    ),
                    new Point(
                        Width - Width * Math.Cos(unsignedSteerAngle)
                        + Length * Math.Sin(unsignedSteerAngle),
                        Width * Math.Sin(unsignedSteerAngle) +
                        Length * Math.Cos(unsignedSteerAngle),
                        0.0
                    ),
                    new Point(
                        0.0,
                        Width * Math.Tan(unsignedSteerAngle / 2.0),
                        0.0
                    )
                }
            );

            return plan;
        }

        private Polygon GetNoSteerPlan()
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

        public void UpdateCollisionBody2D()
        {
            var cbc = SceneObject?.GetComponent<CollisionBody2DComponent>();

            if (cbc == null) { return; }

            cbc.ClearCollisionPolygons();

            var collisionPolygon = GetPlan();
            collisionPolygon.OffsetEdges(-cbc.Epsilon);

            cbc.AddCollisionPolygon(collisionPolygon);
        }

        #endregion

        #region Transforms

        public TrsTransform3D GetLeftSideLocalTransform(
            double localTy, double localRz = 0.0
        )
        {
            var transform = new TrsTransform3D();

            if (SteerAngle < 0.0)
            {
                double unsignedSteerAngle = -SteerAngle;

                transform.SetTranslateLocal(
                    Width - Width * Math.Cos(unsignedSteerAngle),
                    Width * Math.Sin(unsignedSteerAngle),
                    0.0
                );
            }

            transform.Rz += SteerAngle;

            transform.SetTranslateLocal(0.0, localTy, 0.0);
            transform.Rz += localRz;

            return transform;
        }

        public TrsTransform3D GetRightSideLocalTransform(
            double localTy, double localRz = 0.0
        )
        {
            var transform = new TrsTransform3D();

            if (SteerAngle > 0.0)
            {
                transform.SetTranslateLocal(
                    Width * Math.Cos(SteerAngle),
                    Width * Math.Sin(SteerAngle),
                    0.0
                );
            }
            else
            {
                transform.SetTranslateLocal(Width, 0.0, 0.0);
            }

            transform.Rz += SteerAngle;

            transform.SetTranslateLocal(0.0, localTy, 0.0);
            transform.Rz += localRz;

            return transform;
        }

        public TrsTransform3D GetForwardSideLocalTransform(
            double localTx, double localRz = 0.0
        )
        {
            var transform = GetLeftSideLocalTransform(Length);

            transform.SetTranslateLocal(localTx, 0.0, 0.0);
            transform.Rz += localRz;

            return transform;
        }

        public TrsTransform3D GetLeftSideTransform(
            double localTy, double localRz = 0.0
        )
        {
            var transform =
                Transform == null ?
                new TrsTransform3D() :
                new TrsTransform3D(Transform);

            if (SteerAngle < 0.0)
            {
                double unsignedSteerAngle = -SteerAngle;

                transform.SetTranslateLocal(
                    Width - Width * Math.Cos(unsignedSteerAngle),
                    Width * Math.Sin(unsignedSteerAngle),
                    0.0
                );
            }

            transform.Rz += SteerAngle;

            transform.SetTranslateLocal(0.0, localTy, 0.0);
            transform.Rz += localRz;

            return transform;
        }

        public TrsTransform3D GetRightSideTransform(
            double localTy, double localRz = 0.0
        )
        {
            var transform =
                Transform == null ?
                new TrsTransform3D() :
                new TrsTransform3D(Transform);

            if (SteerAngle > 0.0)
            {
                transform.SetTranslateLocal(
                    Width * Math.Cos(SteerAngle),
                    Width * Math.Sin(SteerAngle),
                    0.0
                );
            }
            else
            {
                transform.SetTranslateLocal(Width, 0.0, 0.0);
            }

            transform.Rz += SteerAngle;

            transform.SetTranslateLocal(0.0, localTy, 0.0);
            transform.Rz += localRz;

            return transform;
        }

        public TrsTransform3D GetForwardSideTransform(
            double localTx, double localRz = 0.0
        )
        {
            var transform = GetLeftSideTransform(Length);

            transform.SetTranslateLocal(localTx, 0.0, 0.0);
            transform.Rz += localRz;

            return transform;
        }

        #endregion

        #region Connections

        public void DisconnectPrevious()
        {
            var previousWtc =
                PreviousWayTile?.GetComponent<WayTileComponent>();

            PreviousWayTile = null;

            var side = Side;
            Side = WayTileSide.Forward;

            if (previousWtc == null) { return; }

            switch (side)
            {
                case WayTileSide.Forward:

                    previousWtc._forwardWayTile = null;
                    break;

                case WayTileSide.Left:

                    previousWtc._leftWayTile = null;
                    break;

                case WayTileSide.Right:

                    previousWtc._rightWayTile = null;
                    break;
            }
        }

        public void Disconnect()
        {
            DisconnectPrevious();

            ForwardWayTile = null;
            LeftWayTile = null;
            RightWayTile = null;
        }

        #endregion

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(ForwardWayTile)
            .Append(LeftWayTile)
            .Append(RightWayTile)
            .Append(PreviousWayTile);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Width), _width);
            writer.AddValue(nameof(Length), _length);
            writer.AddValue(nameof(SteerAngle), _steerAngle);

            writer.AddValue(nameof(Side), Side);
            writer.AddValue(nameof(ForwardWayTile), _forwardWayTile);
            writer.AddValue(nameof(LeftWayTile), _leftWayTile);
            writer.AddValue(nameof(RightWayTile), _rightWayTile);
            writer.AddValue(nameof(PreviousWayTile), PreviousWayTile);
        }

        #endregion

        #region Properties

        #region Geometry

        public virtual double Width
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
                UpdateCollisionBody2D();
            }
        }

        public virtual double Length
        {
            get => _length;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(Length)} cannot be negative"
                    );
                }

                _length = value;
                UpdateCollisionBody2D();
            }
        }

        public virtual double SteerAngle
        {
            get => _steerAngle;
            set
            {
                if (value < -Math.PI / 2.0 || value > Math.PI / 2.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SteerAngle)} must be between " +
                        "-Pi / 2 (inclusive) and Pi / 2 (inclusive)"
                    );
                }

                _steerAngle = value;
                UpdateCollisionBody2D();
            }
        }

        #endregion

        #region Transforms

        public TrsTransform3D Transform =>
            SceneObject?.GetComponent<TransformComponent>()?.Transform;

        public TrsTransform3D ForwardTileTransform =>
            GetForwardSideTransform(0.0, 0.0);

        public TrsTransform3D LeftTileTransform =>
            GetLeftSideTransform(Length - Width, Math.PI / 2.0);

        public TrsTransform3D RightTileTransform =>
            GetRightSideTransform(Length, -Math.PI / 2.0);

        #endregion

        #region Connections

        public WayTileSide Side
        { get; private set; } = WayTileSide.Forward;

        public virtual SceneObject ForwardWayTile
        {
            get => _forwardWayTile;
            set
            {
                _forwardWayTile?.GetComponent<WayTileComponent>()
                    .DisconnectPrevious();
                value?.GetComponent<WayTileComponent>()
                    .DisconnectPrevious();

                _forwardWayTile = value;

                var forwardWtc =
                    _forwardWayTile?.GetComponent<WayTileComponent>();

                if (forwardWtc != null)
                {
                    forwardWtc.PreviousWayTile = SceneObject;
                    forwardWtc.Side = WayTileSide.Forward;

                    var forwardTc =
                        _forwardWayTile.GetComponent<TransformComponent>();

                    if (forwardTc != null)
                    {
                        forwardTc.Transform = ForwardTileTransform;
                    }
                }
            }
        }

        public virtual SceneObject LeftWayTile
        {
            get => _leftWayTile;
            set
            {
                _leftWayTile?.GetComponent<WayTileComponent>()
                    .DisconnectPrevious();
                value?.GetComponent<WayTileComponent>()
                    .DisconnectPrevious();

                _leftWayTile = value;

                var leftWtc = _leftWayTile?.GetComponent<WayTileComponent>();

                if (leftWtc != null)
                {
                    leftWtc.PreviousWayTile = SceneObject;
                    leftWtc.Side = WayTileSide.Left;

                    var leftTc =
                        _leftWayTile.GetComponent<TransformComponent>();

                    if (leftTc != null)
                    {
                        leftTc.Transform = LeftTileTransform;
                    }
                }
            }
        }

        public virtual SceneObject RightWayTile
        {
            get => _rightWayTile;
            set
            {
                _rightWayTile?.GetComponent<WayTileComponent>()
                    .DisconnectPrevious();
                value?.GetComponent<WayTileComponent>()
                    .DisconnectPrevious();

                _rightWayTile = value;

                var rightWtc =
                    _rightWayTile?.GetComponent<WayTileComponent>();

                if (rightWtc != null)
                {
                    rightWtc.PreviousWayTile = SceneObject;
                    rightWtc.Side = WayTileSide.Right;

                    var rightTc =
                        _rightWayTile.GetComponent<TransformComponent>();

                    if (rightTc != null)
                    {
                        rightTc.Transform = RightTileTransform;
                    }
                }
            }
        }

        public SceneObject PreviousWayTile { get; private set; }

        #endregion

        #endregion

        #region Member variables

        private double _width = DefaultWidth;
        private double _length = DefaultLength;
        private double _steerAngle = DefaultSteerAngle;

        private SceneObject _forwardWayTile;
        private SceneObject _leftWayTile;
        private SceneObject _rightWayTile;

        #endregion

        #region Constants

        public const double DefaultWidth = 5.0;

        public const double DefaultLength = 2.5;

        public const double DefaultSteerAngle = 0.0;

        #endregion
    }
}
