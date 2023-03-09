using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that describes a corridor.
    /// </summary>
    [Serializable]
    public class CorridorComponent : Component
    {
        #region Constructors

        public CorridorComponent() : base()
        { }

        protected CorridorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _plan = reader.GetValue<Polygon>(nameof(Plan));
            _height = reader.GetValue<double>(nameof(Height));
            Entrance = reader.GetValue<BuildingEntrance>(nameof(Entrance));
        }

        #endregion

        #region Methods

        public Mesh GetMesh()
        {
            Mesh mesh = GeometryUtils.Extrude(Plan, Height);

            return mesh;
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Plan), _plan);
            writer.AddValue(nameof(Height), _height);
            writer.AddValue(nameof(Entrance), Entrance);
        }

        #endregion

        #region Properties

        public Polygon Plan
        {
            get => _plan;
            set
            {
                _plan = value ??
                    throw new ArgumentNullException(nameof(value));
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

        public BuildingEntrance Entrance { get; } = new BuildingEntrance();

        #endregion

        #region Member variables

        private Polygon _plan = new Polygon();

        private double _height = DefaultHeight;

        #endregion

        #region Constants

        public const double DefaultHeight = 1.5;

        #endregion
    }
}
