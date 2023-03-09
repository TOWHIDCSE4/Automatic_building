using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Utilities for working with geometries.
    /// </summary>
    [Serializable]
    public class GeometryUtils
    {
        #region Methods

        /// <summary>
        /// Converts a <see cref="Polygon"/> into a <see cref="Mesh"/>.
        /// </summary>
        /// 
        /// <param name="polygon">
        /// The polygon to be converted.
        /// </param>
        /// 
        /// <returns>
        /// The <see cref="Mesh'"/> converted from the <see cref="Polygon"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="polygon"/> is <see langword="null"/>.
        /// </exception>
        public static Mesh CovnertToMesh(Polygon polygon)
        {
            if (polygon == null)
            {
                throw new ArgumentNullException(nameof(polygon));
            }

            Mesh mesh = new Mesh();

            mesh.AddPolygon(polygon);

            return mesh;
        }

        /// <summary>
        /// Creates a <see cref="Mesh"/> by
        /// extruding a <see cref="Polygon"/> along its normal direction.
        /// </summary>
        /// 
        /// <param name="polygon">
        /// The polygon to be extruded.
        /// </param>
        /// <param name="distance">
        /// The distance to extrude the polygon along its normal direction.
        /// </param>
        /// 
        /// <returns>
        /// The <see cref="Mesh"/> created by extruding
        /// the <see cref="Polygon"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="polygon"/> is <see langword="null"/>.
        /// </exception>
        public static Mesh Extrude(Polygon polygon, double distance)
        {
            if (polygon == null)
            {
                throw new ArgumentNullException(nameof(polygon));
            }

            Mesh mesh = new Mesh();

            var polygon0Points =
                polygon.Points.Select(point => new Point(point));

            if (distance >= 0.0)
            {
                polygon0Points = polygon0Points.Reverse();
            }

            Polygon polygon0 = new Polygon(polygon0Points);
            mesh.AddPolygon(polygon0);

            var polygon1Points =
                polygon0Points.Select(point => new Point(point)).Reverse();
            Polygon polygon1 = new Polygon(polygon1Points);

            Vector<double> offset = polygon.Normal * distance;
            TrsTransform3D polygon1Transform = new TrsTransform3D
            {
                Tx = offset[0],
                Ty = offset[1],
                Tz = offset[2]
            };
            polygon1.Transform(polygon1Transform);

            mesh.AddPolygon(polygon1);

            for (int pointIndex = 0; pointIndex < polygon.Points.Count;
                ++pointIndex)
            {
                int sideFaceVertex0 = (pointIndex + 1) % polygon.Points.Count;

                int sideFaceVertex1 = pointIndex;

                int sideFaceVertex2 =
                    (polygon.Points.Count - 1 - sideFaceVertex1)
                    % polygon.Points.Count + polygon.Points.Count;

                int sideFaceVertex3 =
                    (polygon.Points.Count - 1 - sideFaceVertex0)
                    % polygon.Points.Count + polygon.Points.Count;

                List<int> sideFaceVertices = new List<int>
                {
                    sideFaceVertex0,
                    sideFaceVertex1,
                    sideFaceVertex2,
                    sideFaceVertex3
                };

                mesh.AddFace(sideFaceVertices);
            }

            return mesh;
        }

        /// <summary>
        /// Creates a mesh representing a box.
        /// </summary>
        /// 
        /// <param name="sizeX">Box size in X Dimension.</param>
        /// <param name="sizeY">Box size in Y Dimension.</param>
        /// <param name="sizeZ">Box size in Z Dimension.</param>
        /// <param name="isCenterPivot">
        /// If <see langword="true"/>, the box will be centered at origin.
        /// Otherwise, its corner with the minimum coordinates will be
        /// at the origin.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="Mesh"/> reprensenting the box specified.
        /// </returns>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="sizeX"/>, <paramref name="sizeY"/> or
        /// <paramref name="sizeZ"/> is negative.
        /// </exception>
        public static Mesh CreateBoxMesh(
            double sizeX, double sizeY, double sizeZ,
            bool isCenterPivot
        )
        {
            if (sizeX < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sizeX),
                    $"{nameof(sizeX)} cannot be negative"
                );
            }

            if (sizeY < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sizeY),
                    $"{nameof(sizeY)} cannot be negative"
                );
            }

            if (sizeZ < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sizeZ),
                    $"{nameof(sizeZ)} cannot be negative"
                );
            }

            Polygon bottomRectangle = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(sizeX, 0.0, 0.0),
                    new Point(sizeX, sizeY, 0.0),
                    new Point(0.0, sizeY, 0.0)
                }
            );

            Mesh boxMesh = Extrude(bottomRectangle, sizeZ);

            if (isCenterPivot)
            {
                var transform = new TrsTransform3D
                {
                    Tx = -sizeX * 0.5,
                    Ty = -sizeY * 0.5,
                    Tz = -sizeZ * 0.5
                };
                boxMesh.Transform(transform);
            }

            return boxMesh;
        }

        #endregion
    }
}
