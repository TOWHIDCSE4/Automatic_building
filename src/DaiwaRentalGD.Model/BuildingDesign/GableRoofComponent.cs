using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that describes a gable roof over a unit.
    /// </summary>
    [Serializable]
    public class GableRoofComponent : RoofComponent
    {
        #region Constructors

        public GableRoofComponent() : base()
        { }

        protected GableRoofComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _eaveHeight = reader.GetValue<double>(nameof(EaveHeight));
            _eaveLength = reader.GetValue<double>(nameof(EaveLength));
            _slopeAngle = reader.GetValue<double>(nameof(SlopeAngle));
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

            BBox unitBbox = uc.GetBBox();

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

        public Vector<double> GetSizes()
        {
            if (Unit == null)
            {
                throw new InvalidOperationException(
                    $"{Unit} is not set"
                );
            }

            var uc = Unit.UnitComponent;

            BBox unitBbox = uc.GetBBox();

            double sizeX = unitBbox.SizeX;
            double sizeY = unitBbox.SizeY + EaveLength * 2.0;
            double sizeZ = sizeY / 2.0 * Math.Tan(SlopeAngle) + EaveHeight;

            Vector<double> sizes =
                new DenseVector(new[] { sizeX, sizeY, sizeZ });

            return sizes;
        }

        public Polygon GetSection()
        {
            if (Unit == null)
            {
                throw new InvalidOperationException(
                    $"{Unit} is not set"
                );
            }

            Vector<double> sizes = GetSizes();

            Polygon roofSection = new Polygon(
                new[]
                {
                    new Point(0.0, -EaveLength, 0.0),
                    new Point(0.0, -EaveLength + sizes[1], 0.0),
                    new Point(0.0, -EaveLength + sizes[1], EaveHeight),
                    new Point(0.0, sizes[1] / 2.0 - EaveLength, sizes[2]),
                    new Point(0.0, -EaveLength, EaveHeight)
                }
            );

            return roofSection;
        }

        public override Mesh GetMesh()
        {
            Polygon roofSection = GetSection();

            double unitSizeX = GetSizes()[0];

            Mesh roofMesh = GeometryUtils.Extrude(roofSection, unitSizeX);

            return roofMesh;
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(EaveHeight), _eaveHeight);
            writer.AddValue(nameof(EaveLength), _eaveLength);
            writer.AddValue(nameof(SlopeAngle), _slopeAngle);
        }

        #endregion

        #region Properties

        public double EaveHeight
        {
            get => _eaveHeight;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(EaveHeight)} cannot be negative"
                    );
                }

                _eaveHeight = value;
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

        public double SlopeAngle
        {
            get => _slopeAngle;
            set
            {
                if (value < 0.0 || value >= Math.PI / 2.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SlopeAngle)} must be between " +
                        "0.0 (inclusive) and Pi / 2 (exclusive)"
                    );
                }

                _slopeAngle = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private double _eaveHeight = DefaultEaveHeight;
        private double _eaveLength = DefaultEaveLength;
        private double _slopeAngle = DefaultSlopeAngle;

        #endregion

        #region Constants

        public const double DefaultEaveHeight = 0.25;
        public const double DefaultEaveLength = 0.74;
        public const double DefaultSlopeAngle = Math.PI / 6.0;

        #endregion
    }
}
