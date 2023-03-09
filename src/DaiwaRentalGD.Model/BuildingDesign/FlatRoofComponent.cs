using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that describes a flat roof over a unit.
    /// </summary>
    [Serializable]
    public class FlatRoofComponent : RoofComponent
    {
        #region Constructors

        public FlatRoofComponent() : base()
        { }

        protected FlatRoofComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _height = reader.GetValue<double>(nameof(Height));
            _eaveLength = reader.GetValue<double>(nameof(EaveLength));
        }

        #endregion

        #region Methods

        public override Polygon GetPlan()
        {
            if (Unit == null)
            {
                throw new InvalidOperationException(
                    $"{Unit} is not set"
                );
            }

            var uc = Unit.UnitComponent;

            var unitBbox = uc.GetBBox();

            Polygon roofPlan = new Polygon(
                new[]
                {
                    new Point(
                        unitBbox.MinX,
                        unitBbox.MinY - EaveLength,
                        0.0
                    ),
                    new Point(
                        unitBbox.MaxX,
                        unitBbox.MinY - EaveLength,
                        0.0
                    ),
                    new Point(
                        unitBbox.MaxX,
                        unitBbox.MaxY + EaveLength,
                        0.0
                    ),
                    new Point(
                        unitBbox.MinX,
                        unitBbox.MaxY + EaveLength,
                        0.0
                    ),
                }
            );

            return roofPlan;
        }

        public override Mesh GetMesh()
        {
            Polygon roofPlan = GetPlan();

            Mesh roofMesh = GeometryUtils.Extrude(roofPlan, Height);

            return roofMesh;
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Height), _height);
            writer.AddValue(nameof(EaveLength), _eaveLength);
        }

        #endregion

        #region Properties

        public double Height
        {
            get => _height;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(Height)} must be positive"
                    );
                }

                _height = value;
                NotifyPropertyChanged();
            }
        }

        public double EaveLength
        {
            get => _eaveLength;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(EaveLength)} cannot be negative"
                    );
                }

                _eaveLength = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private double _height = DefaultHeight;
        private double _eaveLength = DefaultEaveLength;

        #endregion

        #region Constants

        public const double DefaultHeight = 1.0;
        public const double DefaultEaveLength = 1.0;

        #endregion
    }
}
