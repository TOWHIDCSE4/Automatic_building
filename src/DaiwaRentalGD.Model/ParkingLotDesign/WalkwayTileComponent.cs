using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component describing a tile on the walkway.
    /// </summary>
    [Serializable]
    public class WalkwayTileComponent : WayTileComponent
    {
        #region Constructors

        public WalkwayTileComponent() : base()
        {
            Width = DefaultWidth;
            Length = DefaultLength;
        }

        protected WalkwayTileComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Properties

        public override SceneObject ForwardWayTile
        {
            get => base.ForwardWayTile;
            set
            {
                if (value != null && !(value is WalkwayTile))
                {
                    throw new ArgumentException(
                        $"{nameof(value)} must be " +
                        $"an instance of {nameof(WalkwayTile)} or null",
                        nameof(value)
                    );
                }

                base.ForwardWayTile = value;
            }
        }

        public override SceneObject LeftWayTile
        {
            get => base.LeftWayTile;
            set
            {
                if (value != null && !(value is WalkwayTile))
                {
                    throw new ArgumentException(
                        $"{nameof(value)} must be " +
                        $"an instance of {nameof(WalkwayTile)} or null",
                        nameof(value)
                    );
                }

                base.LeftWayTile = value;
            }
        }

        public override SceneObject RightWayTile
        {
            get => base.RightWayTile;
            set
            {
                if (value != null && !(value is WalkwayTile))
                {
                    throw new ArgumentException(
                        $"{nameof(value)} must be " +
                        $"an instance of {nameof(WalkwayTile)} or null",
                        nameof(value)
                    );
                }

                base.RightWayTile = value;
            }
        }

        public WalkwayTile ForwardWalkwayTile
        {
            get => ForwardWayTile as WalkwayTile;
            set => ForwardWayTile = value;
        }

        public WalkwayTile LeftWalkwayTile
        {
            get => LeftWayTile as WalkwayTile;
            set => LeftWayTile = value;
        }

        public WalkwayTile RightWalkwayTile
        {
            get => RightWayTile as WalkwayTile;
            set => RightWayTile = value;
        }

        public WalkwayTile PreviousWalkwayTile =>
            PreviousWayTile as WalkwayTile;

        #endregion

        #region Constants

        public new const double DefaultWidth = 2.0;

        public new const double DefaultLength = 2.0;

        #endregion
    }
}
