using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeC
{
    /// <summary>
    /// A component that describes a Type C unit.
    /// </summary>
    [Serializable]
    public class TypeCUnitComponent : CatalogUnitComponent
    {
        #region Constructors

        public TypeCUnitComponent() : base()
        { }

        public TypeCUnitComponent(TypeCUnitComponent uc) : base(uc)
        {
            PositionType = uc.PositionType;
            EntranceType = uc.EntranceType;

            BalconyAnchorPoint = new Point(uc.BalconyAnchorPoint);
            BalconyLength = uc.BalconyLength;
        }

        protected TypeCUnitComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            PositionType =
                reader.GetValue<TypeCUnitPositionType>(nameof(PositionType));

            EntranceType =
                reader.GetValue<TypeCUnitEntranceType>(nameof(EntranceType));

            BalconyAnchorPoint = new Point(
                reader.GetValue<List<double>>(nameof(BalconyAnchorPoint))
            );

            _balconyLength = reader.GetValue<double>(nameof(BalconyLength));
        }

        #endregion

        #region Methods

        public override CatalogUnitComponent Copy()
        {
            TypeCUnitComponent uc = new TypeCUnitComponent(this);
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
                nameof(BalconyAnchorPoint), BalconyAnchorPoint.AsList()
            );
            writer.AddValue(nameof(BalconyLength), _balconyLength);
        }

        #endregion

        #region Properties

        public TypeCUnitPositionType PositionType { get; set; }

        public TypeCUnitEntranceType EntranceType { get; set; }

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

        private Point _balconyAnchorPoint = new Point();

        private double _balconyLength = BalconyComponent.DefaultLengthP.M;

        #endregion

        #region Constants

        public const int MainType = 3;

        #endregion
    }
}
