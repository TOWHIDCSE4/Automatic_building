using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign.TypeB
{
    /// <summary>
    /// A component that describes a Type B unit.
    /// </summary>
    [Serializable]
    public class TypeBUnitComponent : CatalogUnitComponent
    {
        #region Constructors

        public TypeBUnitComponent() : base()
        { }

        public TypeBUnitComponent(TypeBUnitComponent uc) : base(uc)
        {
            LayoutType = uc.LayoutType;

            StaircaseAnchorPoint = new Point(uc.StaircaseAnchorPoint);
            CorridorAnchorPoint = new Point(uc.CorridorAnchorPoint);

            BalconyAnchorPoint = new Point(uc.BalconyAnchorPoint);
            BalconyLength = uc.BalconyLength;
        }

        protected TypeBUnitComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            LayoutType =
                reader.GetValue<TypeBUnitLayoutType>(nameof(LayoutType));

            StaircaseAnchorPoint = new Point(
                reader.GetValue<List<double>>(nameof(StaircaseAnchorPoint))
            );

            CorridorAnchorPoint = new Point(
                reader.GetValue<List<double>>(nameof(CorridorAnchorPoint))
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
            TypeBUnitComponent uc = new TypeBUnitComponent(this);
            return uc;
        }

        public TrsTransform3D GetStaircaseTransform(Staircase staircase)
        {
            if (LayoutType == TypeBUnitLayoutType.Basic)
            {
                return null;
            }
            else
            {
                var sc = staircase.StaircaseComponent;

                var saPoint = new Point(StaircaseAnchorPoint);

                saPoint.Transform(
                    SceneObject?.GetComponent<TransformComponent>().Transform
                );

                var staircaseTf = new TrsTransform3D
                {
                    Rz = Math.PI,
                    Tx = saPoint.X + sc.Width,
                    Ty = saPoint.Y + sc.Length,
                    Tz = saPoint.Z
                };

                return staircaseTf;
            }
        }

        public TrsTransform3D GetCorridorTransform(Corridor corridor)
        {
            var saPoint = new Point(CorridorAnchorPoint);

            saPoint.Transform(
                SceneObject?.GetComponent<TransformComponent>().Transform
            );

            var corridorTf = new TrsTransform3D
            {
                Tx = saPoint.X,
                Ty = saPoint.Y,
                Tz = saPoint.Z
            };

            return corridorTf;
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

            writer.AddValue(nameof(LayoutType), LayoutType);
            writer.AddValue(
                nameof(StaircaseAnchorPoint), StaircaseAnchorPoint.AsList()
            );
            writer.AddValue(
                nameof(CorridorAnchorPoint), CorridorAnchorPoint.AsList()
            );
            writer.AddValue(
                nameof(BalconyAnchorPoint), BalconyAnchorPoint.AsList()
            );
            writer.AddValue(nameof(BalconyLength), _balconyLength);
        }

        #endregion

        #region Properties

        public TypeBUnitLayoutType LayoutType { get; set; }

        public Point StaircaseAnchorPoint
        {
            get => _staircaseAnchorPoint;
            set => _staircaseAnchorPoint = value ??
                throw new ArgumentNullException(nameof(value));
        }

        public Point CorridorAnchorPoint
        {
            get => _corridorAnchorPoint;
            set => _corridorAnchorPoint = value ??
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

        private Point _corridorAnchorPoint = new Point();

        private Point _balconyAnchorPoint = new Point();

        private double _balconyLength = BalconyComponent.DefaultLengthP.M;

        #endregion

        #region Constants

        public const int MainType = 2;

        public const int BasicUnitVariationType = 1;

        public const int StairUnitVariationType = 9;

        #endregion
    }
}
