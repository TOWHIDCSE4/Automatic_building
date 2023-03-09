using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using LineSegment = DaiwaRentalGD.Geometries.LineSegment;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Utilities for working with geometries in <see cref="Viewport3D"/>.
    /// </summary>
    public class Viewport3DUtils
    {
        #region Methods

        #region Conversion

        public static Point3D ConvertToPoint3D(Point point)
        {
            return new Point3D(point.X, point.Y, point.Z);
        }

        public static Transform3D ConvertToTransform3D(ITransform3D transform)
        {
            Matrix<double> matrix = transform.Matrix;

            Matrix3D matrix3D = new Matrix3D(
                matrix[0, 0], matrix[1, 0], matrix[2, 0], 0.0,
                matrix[0, 1], matrix[1, 1], matrix[2, 1], 0.0,
                matrix[0, 2], matrix[1, 2], matrix[2, 2], 0.0,
                matrix[0, 3], matrix[1, 3], matrix[2, 3], 1.0
            );

            Transform3D transform3D = new MatrixTransform3D(matrix3D);

            return transform3D;
        }

        public static MeshGeometry3D ConvertToGeometry3D(Mesh mesh)
        {
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            for (int faceIndex = 0; faceIndex < mesh.Faces.Count; ++faceIndex)
            {
                Polygon polygon = mesh.GetPolygon(faceIndex);

                var polygonTriangleIndicesList = polygon.Trianglate();

                int triangleIndexOffset = positions.Count;

                foreach (Point point in polygon.Points)
                {
                    Point3D point3D = ConvertToPoint3D(point);
                    positions.Add(point3D);
                }

                foreach (var polygonTriangleIndices in
                    polygonTriangleIndicesList)
                {
                    triangleIndices.Add(
                        polygonTriangleIndices.Item1 + triangleIndexOffset
                    );
                    triangleIndices.Add(
                        polygonTriangleIndices.Item2 + triangleIndexOffset
                    );
                    triangleIndices.Add(
                        polygonTriangleIndices.Item3 + triangleIndexOffset
                    );
                }
            }

            var mg3d = new MeshGeometry3D
            {
                Positions = positions,
                TriangleIndices = triangleIndices
            };

            return mg3d;
        }

        #endregion

        #region Creation

        public static MeshGeometry3D CreateLineGeometry3D(
            Point point0, Point point1, double thickness,
            double epsilon = DefaultEpsilon
        )
        {
            var lineSegment = new LineSegment(point0, point1);

            return CreateLinesGeometry3D(
                new[] { lineSegment }, thickness, epsilon
            );
        }

        public static MeshGeometry3D CreateLinesGeometry3D(
            IEnumerable<LineSegment> lineSegments, double thickness,
            double epsilon = DefaultEpsilon
        )
        {
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            foreach (LineSegment ls in lineSegments)
            {
                Point point0 = ls.Point0;
                Point point1 = ls.Point1;

                Vector<double> dirX =
                    (point1.Vector - point0.Vector).Normalize(2.0);

                double xWorldZDotProduct = dirX.DotProduct(
                    new DenseVector(new[] { 0.0, 0.0, 1.0 })
                );
                Vector<double> dirUp;
                if (Math.Abs(Math.Abs(xWorldZDotProduct) - 1.0) <= epsilon)
                {
                    dirUp = new DenseVector(new[] { 0.0, 1.0, 0.0 });
                }
                else
                {
                    dirUp = new DenseVector(new[] { 0.0, 0.0, 1.0 });
                }

                Vector<double> dirY =
                    MathUtils.CrossProduct(dirUp, dirX).Normalize(2.0);
                Vector<double> dirZ =
                    MathUtils.CrossProduct(dirX, dirY).Normalize(2.0);

                Vector<double> v0 =
                    point0.Vector + (-dirY - dirZ) * thickness * 0.5;
                Vector<double> v1 =
                    point1.Vector + (-dirY - dirZ) * thickness * 0.5;
                Vector<double> v2 =
                    point1.Vector + (dirY - dirZ) * thickness * 0.5;
                Vector<double> v3 =
                    point0.Vector + (dirY - dirZ) * thickness * 0.5;
                Vector<double> v4 =
                    point0.Vector + (-dirY + dirZ) * thickness * 0.5;
                Vector<double> v5 =
                    point1.Vector + (-dirY + dirZ) * thickness * 0.5;
                Vector<double> v6 =
                    point1.Vector + (dirY + dirZ) * thickness * 0.5;
                Vector<double> v7 =
                    point0.Vector + (dirY + dirZ) * thickness * 0.5;

                Point3DCollection lsPositions = new Point3DCollection
                {
                    // 0, 1, 2, 3
                    new Point3D(v0[0], v0[1], v0[2]),
                    new Point3D(v1[0], v1[1], v1[2]),
                    new Point3D(v5[0], v5[1], v5[2]),
                    new Point3D(v4[0], v4[1], v4[2]),

                    // 4, 5, 6, 7
                    new Point3D(v1[0], v1[1], v1[2]),
                    new Point3D(v2[0], v2[1], v2[2]),
                    new Point3D(v6[0], v6[1], v6[2]),
                    new Point3D(v5[0], v5[1], v5[2]),

                    // 8, 9, 10, 11
                    new Point3D(v2[0], v2[1], v2[2]),
                    new Point3D(v3[0], v3[1], v3[2]),
                    new Point3D(v7[0], v7[1], v7[2]),
                    new Point3D(v6[0], v6[1], v6[2]),

                    // 12, 13, 14, 15
                    new Point3D(v3[0], v3[1], v3[2]),
                    new Point3D(v0[0], v0[1], v0[2]),
                    new Point3D(v4[0], v4[1], v4[2]),
                    new Point3D(v7[0], v7[1], v7[2]),

                    // 16, 17, 18, 19
                    new Point3D(v1[0], v1[1], v1[2]),
                    new Point3D(v0[0], v0[1], v0[2]),
                    new Point3D(v3[0], v3[1], v3[2]),
                    new Point3D(v2[0], v2[1], v2[2]),

                    // 20, 21, 22, 23
                    new Point3D(v4[0], v4[1], v4[2]),
                    new Point3D(v5[0], v5[1], v5[2]),
                    new Point3D(v6[0], v6[1], v6[2]),
                    new Point3D(v7[0], v7[1], v7[2])
                };

                var lsTriangleIndices = new Int32Collection
                {
                    0, 1, 2, 0, 2, 3,
                    4, 5, 6, 4, 6, 7,
                    8, 9, 10, 8, 10, 11,
                    12, 13, 14, 12, 14, 15,
                    16, 17, 18, 16, 18, 19,
                    20, 21, 22, 20, 22, 23
                };

                for (int triangleIndexIndex = 0;
                    triangleIndexIndex < lsTriangleIndices.Count;
                    ++triangleIndexIndex)
                {
                    lsTriangleIndices[triangleIndexIndex] += positions.Count;
                }

                foreach (Point3D point in lsPositions)
                {
                    positions.Add(point);
                }

                foreach (int triangleIndex in lsTriangleIndices)
                {
                    triangleIndices.Add(triangleIndex);
                }
            }

            var mg3d = new MeshGeometry3D
            {
                Positions = positions,
                TriangleIndices = triangleIndices
            };

            return mg3d;
        }

        public static Geometry3D CreateWireframeGeometry3D(
            Mesh mesh, double thickness, double epsilon = DefaultEpsilon
        )
        {

            var lineSegments = new List<LineSegment>();
            var lineSegmentSet = new HashSet<LineSegment>();

            foreach (var edge in mesh.GetEdges())
            {
                Point point0 = mesh.Points[edge.Item1];
                Point point1 = mesh.Points[edge.Item2];

                var ls01 = new LineSegment(point0, point1);
                var ls10 = new LineSegment(point1, point0);

                if (lineSegmentSet.Contains(ls01))
                {
                    continue;
                }

                lineSegments.Add(ls01);

                lineSegmentSet.Add(ls01);
                lineSegmentSet.Add(ls10);
            }

            return CreateLinesGeometry3D(lineSegments, thickness, epsilon);
        }

        public static Model3D CreateTextModel3D(
            TextBlock textBlock,
            double maxSizeX, double maxSizeY,
            out double actualSizeX, out double actualSizeY
        )
        {
            var availableSize = new System.Windows.Size(
                double.PositiveInfinity,
                double.PositiveInfinity
            );

            textBlock.Measure(availableSize);

            System.Windows.Size desiredSize = textBlock.DesiredSize;

            double aspectRatio = desiredSize.Width / desiredSize.Height;

            if (maxSizeX / maxSizeY >= aspectRatio)
            {
                actualSizeY = maxSizeY;
                actualSizeX = actualSizeY * aspectRatio;
            }
            else
            {
                actualSizeX = maxSizeX;
                actualSizeY = actualSizeX / aspectRatio;
            }

            double longSide = Math.Max(desiredSize.Width, desiredSize.Height);
            double normalizedSizeX = desiredSize.Width / longSide;
            double normalizedSizeY = desiredSize.Height / longSide;

            MeshGeometry3D geometry = new MeshGeometry3D
            {
                Positions = new Point3DCollection
                {
                    new Point3D(0.0, 0.0, 0.0),
                    new Point3D(actualSizeX, 0.0, 0.0),
                    new Point3D(actualSizeX, actualSizeY, 0.0),
                    new Point3D(0.0, actualSizeY, 0.0)
                },
                TriangleIndices = new Int32Collection
                {
                    0, 1, 2, 0, 2, 3
                },
                TextureCoordinates = new PointCollection
                {
                    new System.Windows.Point(
                        0.0, normalizedSizeY
                    ),
                    new System.Windows.Point(
                        normalizedSizeX, normalizedSizeY
                    ),
                    new System.Windows.Point(
                        normalizedSizeX, 0.0
                    ),
                    new System.Windows.Point(
                        0.0, 0.0
                    )
                }
            };

            DiffuseMaterial material = new DiffuseMaterial
            {
                Brush = new VisualBrush { Visual = textBlock }
            };

            GeometryModel3D gm3d = new GeometryModel3D
            {
                Geometry = geometry,
                Material = material
            };

            return gm3d;
        }

        public static Material CreateEmissiveMaterial(Brush brush)
        {
            // Direct uses of EmissiveMaterial might not yield
            // ideal result.
            // Reference: https://ikriv.com/blog/?p=296

            var material = new MaterialGroup
            {
                Children =
                {
                    new DiffuseMaterial
                    {
                        Brush = Brushes.Black
                    },
                    new EmissiveMaterial
                    {
                        Brush = brush
                    }
                }
            };

            return material;
        }

        #endregion

        #endregion

        #region Constants

        public const double DefaultEpsilon = 1e-6;

        #endregion
    }
}
