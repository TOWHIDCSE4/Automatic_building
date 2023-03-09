using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that describes a staircase.
    /// </summary>
    [Serializable]
    public class StaircaseComponent : Component
    {
        #region Constructors

        public StaircaseComponent() : base()
        { }

        protected StaircaseComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _width = reader.GetValue<double>(nameof(Width));
            _length = reader.GetValue<double>(nameof(Length));
            _height = reader.GetValue<double>(nameof(Height));
            Entrance = reader.GetValue<BuildingEntrance>(nameof(Entrance));
        }

        #endregion

        #region Methods

        public Polygon GetPlan()
        {
            var plan = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(Width, 0.0, 0.0),
                    new Point(Width, Length, 0.0),
                    new Point(0.0, Length, 0.0),
                }
            );

            return plan;
        }

        public Mesh GetMesh()
        {
            Polygon plan = GetPlan();

            Mesh mesh = GeometryUtils.Extrude(plan, Height);

            return mesh;
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Width), _width);
            writer.AddValue(nameof(Length), _length);
            writer.AddValue(nameof(Height), _height);
            writer.AddValue(nameof(Entrance), Entrance);
        }

        #endregion

        #region Properties

        public double Width
        {
            get => _width;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(Width)} must be positive"
                    );
                }

                _width = value;
                NotifyPropertyChanged();
            }
        }

        public double Length
        {
            get => _length;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(Length)} must be positive"
                    );
                }

                _length = value;
                NotifyPropertyChanged();
            }
        }

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

        public BuildingEntrance Entrance { get; } = new BuildingEntrance
        {
            EntrancePoint = new Point(DefaultWidthP.M / 2.0, 0.0, 0.0),
            EntranceDirection = new DenseVector(new[] { 0.0, 1.0, 0.0 })
        };

        #endregion

        #region Member variables

        private double _width = DefaultWidthP.M;
        private double _length = DefaultLengthP.M;
        private double _height = DefaultHeight;

        #endregion

        #region Constants

        public static readonly LengthP DefaultWidthP = new LengthP(3.0);
        public static readonly LengthP DefaultLengthP = new LengthP(4.0);
        public const double DefaultHeight = 1.5;

        #endregion
    }
}
