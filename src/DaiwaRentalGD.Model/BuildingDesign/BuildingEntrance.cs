using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Defines an entrance to the building.
    /// </summary>
    [Serializable]
    public class BuildingEntrance : ISerializable
    {
        #region Constructors

        public BuildingEntrance()
        { }

        public BuildingEntrance(BuildingEntrance be)
        {
            EntrancePoint = new Point(be.EntrancePoint);
            EntranceDirection = be.EntranceDirection.Clone();
        }

        protected BuildingEntrance(
            SerializationInfo info, StreamingContext context
        )
        {
            _entrancePoint =
                info.GetValue<Point>(nameof(EntrancePoint));

            _entranceDirection = new DenseVector(
                info.GetValue<List<double>>(nameof(EntranceDirection))
                .ToArray()
            );
        }

        #endregion

        #region Methods

        public void Transform(TrsTransform3D transform)
        {
            EntrancePoint.Transform(transform);

            EntranceDirection = transform.TransformDir(EntranceDirection);
        }

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(EntrancePoint), _entrancePoint);
            info.AddValue(
                nameof(EntranceDirection), _entranceDirection.ToList()
            );
        }

        #endregion

        #region Properties

        public Point EntrancePoint
        {
            get => _entrancePoint;

            set
            {
                _entrancePoint =
                    value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public Vector<double> EntranceDirection
        {
            get => _entranceDirection;

            set
            {
                _entranceDirection =
                    value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public TrsTransform3D EntranceTransform
        {
            get
            {
                double entranceAngle = MathUtils.GetAngle2D(
                    new DenseVector(new[] { 0.0, 1.0, 0.0 }),
                    EntranceDirection
                );

                var transform = new TrsTransform3D
                {
                    Tx = EntrancePoint.X,
                    Ty = EntrancePoint.Y,
                    Tz = EntrancePoint.Z,
                    Rz = entranceAngle
                };

                return transform;
            }
        }

        public static Point DefaultEntrancePoint => new Point();

        public static Vector<double> DefaultEntranceDirection =>
            new DenseVector(new[] { 1.0, 0.0, 0.0 });

        #endregion

        #region Member variables

        private Point _entrancePoint = DefaultEntrancePoint;

        private Vector<double> _entranceDirection = DefaultEntranceDirection;

        #endregion
    }
}
