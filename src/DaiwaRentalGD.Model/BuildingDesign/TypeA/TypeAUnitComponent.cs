using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeA
{
    /// <summary>
    /// A component that describes a Type A unit.
    /// </summary>
    [Serializable]
    public class TypeAUnitComponent : CatalogUnitComponent
    {
        #region Constructors

        public TypeAUnitComponent() : base()
        { }

        public TypeAUnitComponent(TypeAUnitComponent uc) : base(uc)
        {
            PositionType = uc.PositionType;
            EntranceType = uc.EntranceType;

            StaircaseAnchorPoint = new Point(uc.StaircaseAnchorPoint);

            BalconyAnchorPoint = new Point(uc.BalconyAnchorPoint);
            BalconyLength = uc.BalconyLength;
        }

        protected TypeAUnitComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            PositionType =
                reader.GetValue<TypeAUnitPositionType>(nameof(PositionType));

            EntranceType =
                reader.GetValue<TypeAUnitEntranceType>(nameof(EntranceType));

            StaircaseAnchorPoint = new Point(
                reader.GetValue<List<double>>(nameof(StaircaseAnchorPoint))
            );

            BalconyAnchorPoint = new Point(
                reader.GetValue<List<double>>(nameof(BalconyAnchorPoint))
            );

            _balconyLength = reader.GetValue<double>(nameof(BalconyLength));
        }

        #endregion

        #region Methods

        public override CatalogUnitComponent Copy()
        {
            TypeAUnitComponent uc = new TypeAUnitComponent(this);
            return uc;
        }

        public void Mirror()
        {
            BBox roomPlansBbox = BBox.GetBBox(RoomPlans);

            double roomSizeX = roomPlansBbox.SizeX;

            var plans = new List<Polygon>();
            plans.AddRange(RoomPlans);

            foreach (Polygon plan in plans)
            {
                foreach (Point point in plan.Points)
                {
                    point.X = roomSizeX - point.X;
                }
                plan.Flip();
            }

            BalconyAnchorPoint.X =
                roomSizeX - BalconyAnchorPoint.X - BalconyLength;
        }

        public TrsTransform3D GetStaircaseTransform(Staircase staircase)
        {
            var sc = staircase.StaircaseComponent;

            var saPoint = new Point(StaircaseAnchorPoint);

            saPoint.Transform(
                SceneObject?.GetComponent<TransformComponent>().Transform
            );

            var staircaseTf = new TrsTransform3D();

            if (EntranceType == TypeAUnitEntranceType.North)
            {
                if (PositionType == TypeAUnitPositionType.Basic)
                {
                    staircaseTf.Rz = Math.PI;
                    staircaseTf.Tx = saPoint.X + sc.Width;
                    staircaseTf.Ty = saPoint.Y + sc.Length;
                    staircaseTf.Tz = saPoint.Z;
                }
                else
                {
                    staircaseTf.Rz = Math.PI;
                    staircaseTf.Tx = saPoint.X;
                    staircaseTf.Ty = saPoint.Y + sc.Length;
                    staircaseTf.Tz = saPoint.Z;
                }
            }
            else
            {
                if (PositionType == TypeAUnitPositionType.Basic)
                {
                    staircaseTf.Tx = saPoint.X;
                    staircaseTf.Ty = saPoint.Y - sc.Length;
                    staircaseTf.Tz = saPoint.Z;
                }
                else
                {
                    staircaseTf.Tx = saPoint.X - sc.Width;
                    staircaseTf.Ty = saPoint.Y - sc.Length;
                    staircaseTf.Tz = saPoint.Z;
                }
            }

            return staircaseTf;
        }

        public TrsTransform3D GetBalconyTransform(Balcony balcony)
        {
            var baPoint = new Point(BalconyAnchorPoint);

            baPoint.Transform(
                SceneObject?.GetComponent<TransformComponent>().Transform
            );

            var balconyTf = new TrsTransform3D
            {
                Rz = Math.PI,
                Tx = baPoint.X + BalconyLength,
                Ty = baPoint.Y,
                Tz = baPoint.Z
            };

            return balconyTf;
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(PositionType), PositionType);
            writer.AddValue(nameof(EntranceType), EntranceType);
            writer.AddValue(
                nameof(StaircaseAnchorPoint), StaircaseAnchorPoint.AsList()
            );
            writer.AddValue(
                nameof(BalconyAnchorPoint), BalconyAnchorPoint.AsList()
            );
            writer.AddValue(nameof(BalconyLength), _balconyLength);
        }

        #endregion

        #region Properties

        public TypeAUnitPositionType PositionType { get; set; }

        public TypeAUnitEntranceType EntranceType { get; set; }

        public Point StaircaseAnchorPoint
        {
            get => _staircaseAnchorPoint;
            set => _staircaseAnchorPoint = value ??
                throw new ArgumentNullException(nameof(value));
        }

        public Point BalconyAnchorPoint
        {
            get => _balconyAnchorPoint;
            set => _balconyAnchorPoint = value ??
                throw new ArgumentNullException(nameof(value));
        }

        public double BalconyLength
        {
            get => _balconyLength;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(BalconyLength)} must be positive"
                    );
                }

                _balconyLength = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private Point _staircaseAnchorPoint = new Point();

        private Point _balconyAnchorPoint = new Point();

        private double _balconyLength = BalconyComponent.DefaultLengthP.M;

        #endregion

        #region Constants

        public const int MainType = 1;

        public const int BasicUnitVariationType = 1;

        public const int EndUnitVariationType = 9;

        #endregion
    }
}
