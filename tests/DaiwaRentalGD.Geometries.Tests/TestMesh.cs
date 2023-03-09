using System;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestMesh
    {
        #region Constructors

        public TestMesh(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConstructor_Default()
        {
            Mesh mesh = new Mesh();

            Assert.Empty(mesh.Points);
            Assert.Empty(mesh.Faces);
        }

        [Fact]
        public void TestConstructor_Copy()
        {
            Mesh mesh0 = new Mesh();

            mesh0.AddPoint(new Point(0.0, 0.0, 0.0));
            mesh0.AddPoint(new Point(2.0, 0.0, 0.0));
            mesh0.AddPoint(new Point(2.0, 3.0, 0.0));
            mesh0.AddFace(new[] { 0, 1, 2 });

            Mesh mesh1 = new Mesh(mesh0);

            Assert.Equal(mesh0.Points, mesh1.Points);
            Assert.Equal(mesh0.Faces, mesh1.Faces);

            for (
                int pointIndex = 0; pointIndex < mesh0.Points.Count;
                ++pointIndex
            )
            {
                Assert.NotSame(
                    mesh0.Points[pointIndex],
                    mesh1.Points[pointIndex]
                );
            }

            for (
                int faceIndex = 0; faceIndex < mesh0.Faces.Count;
                ++faceIndex
            )
            {
                Assert.NotSame(
                    mesh0.Faces[faceIndex],
                    mesh1.Faces[faceIndex]
                );
            }
        }

        [Fact]
        public void AddPoint()
        {
            Mesh mesh = new Mesh();

            Point point0 = new Point(0.0, 0.0, 0.0);
            Point point1 = new Point(2.0, 0.0, 0.0);
            Point point2 = new Point(2.0, 2.0, 0.0);
            Point point3 = new Point(0.0, 2.0, 0.0);

            mesh.AddPoint(point0);
            mesh.AddPoint(point1);
            mesh.AddPoint(point2);
            mesh.AddPoint(point3);

            Assert.Equal(
                new[] { point0, point1, point2, point3 },
                mesh.Points
            );
        }

        [Fact]
        public void AddPoint_NullPoint()
        {
            Mesh mesh = new Mesh();

            Point point0 = new Point(0.0, 1.0, 2.0);
            mesh.AddPoint(point0);

            Assert.Throws<ArgumentNullException>(
                () => { mesh.AddPoint(null); }
            );

            Assert.Equal(
                new[] { point0 },
                mesh.Points
            );
        }

        [Fact]
        public void AddFace()
        {
            Mesh mesh = new Mesh();

            mesh.AddPoint(new Point(0.0, 0.0, 0.0));
            mesh.AddPoint(new Point(2.0, 0.0, 0.0));
            mesh.AddPoint(new Point(2.0, 2.0, 0.0));
            mesh.AddPoint(new Point(0.0, 2.0, 0.0));
            mesh.AddPoint(new Point(2.0, 0.0, 1.0));
            mesh.AddPoint(new Point(0.0, 0.0, 1.0));

            mesh.AddFace(new[] { 0, 1, 2, 3 });
            mesh.AddFace(new[] { 0, 1, 4, 5 });

            Assert.Equal(
                new[]
                {
                    new[] { 0, 1, 2, 3 },
                    new[] { 0, 1, 4, 5 }
                },
                mesh.Faces
            );
        }

        [Fact]
        public void AddFace_InvalidFaceVertex()
        {
            Mesh mesh = new Mesh();

            mesh.AddPoint(new Point(0.0, 0.0, 0.0));
            mesh.AddPoint(new Point(2.0, 0.0, 0.0));
            mesh.AddPoint(new Point(2.0, 2.0, 0.0));
            mesh.AddPoint(new Point(0.0, 2.0, 0.0));
            mesh.AddPoint(new Point(2.0, 0.0, 1.0));
            mesh.AddPoint(new Point(0.0, 0.0, 1.0));

            mesh.AddFace(new[] { 0, 1, 2, 3 });

            Assert.ThrowsAny<ArgumentException>(
                () => { mesh.AddFace(new[] { 0, 1, 4, 10 }); }
            );

            Assert.ThrowsAny<ArgumentException>(
                () => { mesh.AddFace(new[] { -1, 1, 4, 5 }); }
            );

            Assert.Equal(
                new[]
                {
                    new[] { 0, 1, 2, 3 }
                },
                mesh.Faces
            );
        }

        [Fact]
        public void AddPolygon()
        {
            Mesh mesh = new Mesh();

            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 2.0, 0.0),
                    new Point(0.0, 2.0, 0.0)
                }
            );
            Polygon polygon1 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 1.0),
                    new Point(0.0, 0.0, 1.0)
                }
            );

            mesh.AddPolygon(polygon0);
            mesh.AddPolygon(polygon1);

            Assert.Equal(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 2.0, 0.0),
                    new Point(0.0, 2.0, 0.0),
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 1.0),
                    new Point(0.0, 0.0, 1.0)
                },
                mesh.Points
            );
            Assert.Equal(
                new[]
                {
                    new[] { 0, 1, 2, 3 },
                    new[] { 4, 5, 6, 7 }
                },
                mesh.Faces
            );
        }

        [Fact]
        public void AddPolygon_NullPolygon()
        {
            Mesh mesh = new Mesh();

            Polygon polygon0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 2.0, 0.0),
                    new Point(0.0, 2.0, 0.0)
                }
            );

            mesh.AddPolygon(polygon0);

            Assert.Throws<ArgumentNullException>(
                () => { mesh.AddPolygon(null); }
            );

            Assert.Equal(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(2.0, 0.0, 0.0),
                    new Point(2.0, 2.0, 0.0),
                    new Point(0.0, 2.0, 0.0)
                },
                mesh.Points
            );
            Assert.Equal(
                new[]
                {
                    new[] { 0, 1, 2, 3 }
                },
                mesh.Faces
            );
        }

        [Fact]
        public void TestGetPolygon()
        {
            Mesh mesh = new Mesh();

            Point point0 = new Point(0.0, 0.0, 0.0);
            Point point1 = new Point(2.0, 0.0, 0.0);
            Point point2 = new Point(2.0, 2.0, 0.0);
            Point point3 = new Point(0.0, 2.0, 0.0);
            Point point4 = new Point(2.0, 0.0, 1.0);
            Point point5 = new Point(0.0, 0.0, 1.0);

            mesh.AddPoint(point0);
            mesh.AddPoint(point1);
            mesh.AddPoint(point2);
            mesh.AddPoint(point3);
            mesh.AddPoint(point4);
            mesh.AddPoint(point5);

            mesh.AddFace(new[] { 0, 1, 2, 3 });
            mesh.AddFace(new[] { 0, 1, 4, 5 });

            Assert.Equal(
                new Polygon(
                    new[] { point0, point1, point2, point3 }
                ),
                mesh.GetPolygon(0)
            );
            Assert.Equal(
                new Polygon(
                    new[] { point0, point1, point4, point5 }
                ),
                mesh.GetPolygon(1)
            );
        }

        [Fact]
        public void TestGetEdges()
        {
            Mesh mesh = new Mesh();

            Point point0 = new Point(0.0, 0.0, 0.0);
            Point point1 = new Point(2.0, 0.0, 0.0);
            Point point2 = new Point(2.0, 2.0, 0.0);
            Point point3 = new Point(0.0, 2.0, 0.0);
            Point point4 = new Point(2.0, 0.0, 1.0);
            Point point5 = new Point(0.0, 0.0, 1.0);

            mesh.AddPoint(point0);
            mesh.AddPoint(point1);
            mesh.AddPoint(point2);
            mesh.AddPoint(point3);
            mesh.AddPoint(point4);
            mesh.AddPoint(point5);

            mesh.AddFace(new[] { 0, 1, 2, 3 });
            mesh.AddFace(new[] { 0, 1, 4, 5 });

            Assert.Equal(
                new[]
                {
                    new Tuple<int, int>(0, 1),
                    new Tuple<int, int>(0, 3),
                    new Tuple<int, int>(0, 5),
                    new Tuple<int, int>(1, 2),
                    new Tuple<int, int>(1, 4),
                    new Tuple<int, int>(2, 3),
                    new Tuple<int, int>(4, 5)
                },
                mesh.GetEdges()
            );
        }

        [Fact]
        public void TestGetPolygon_InvalidFaceIndex()
        {
            Mesh mesh = new Mesh();

            Point point0 = new Point(0.0, 0.0, 0.0);
            Point point1 = new Point(2.0, 0.0, 0.0);
            Point point2 = new Point(2.0, 2.0, 0.0);
            Point point3 = new Point(0.0, 2.0, 0.0);
            Point point4 = new Point(2.0, 0.0, 1.0);
            Point point5 = new Point(0.0, 0.0, 1.0);

            mesh.AddPoint(point0);
            mesh.AddPoint(point1);
            mesh.AddPoint(point2);
            mesh.AddPoint(point3);
            mesh.AddPoint(point4);
            mesh.AddPoint(point5);

            mesh.AddFace(new[] { 0, 1, 2, 3 });
            mesh.AddFace(new[] { 0, 1, 4, 5 });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { mesh.GetPolygon(-1); }
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { mesh.GetPolygon(5); }
            );
        }

        [Fact]
        public void Transform()
        {
            Mesh mesh = new Mesh();

            Point point0 = new Point(0.0, 0.0, 0.0);
            Point point1 = new Point(2.0, 0.0, 0.0);
            Point point2 = new Point(2.0, 2.0, 0.0);
            Point point3 = new Point(0.0, 2.0, 0.0);
            Point point4 = new Point(2.0, 0.0, 1.0);
            Point point5 = new Point(0.0, 0.0, 1.0);

            mesh.AddPoint(point0);
            mesh.AddPoint(point1);
            mesh.AddPoint(point2);
            mesh.AddPoint(point3);
            mesh.AddPoint(point4);
            mesh.AddPoint(point5);

            mesh.AddFace(new[] { 0, 1, 2, 3 });
            mesh.AddFace(new[] { 0, 1, 4, 5 });

            ITransform3D tf = new TrsTransform3D
            {
                Tx = 10.0,
                Ty = 20.0
            };
            mesh.Transform(tf);

            Assert.Equal(
                new[]
                {
                    new Point(10.0, 20.0, 0.0),
                    new Point(12.0, 20.0, 0.0),
                    new Point(12.0, 22.0, 0.0),
                    new Point(10.0, 22.0, 0.0),
                    new Point(12.0, 20.0, 1.0),
                    new Point(10.0, 20.0, 1.0)
                },
                mesh.Points
            );

            Assert.Equal(
                new[]
                {
                    new[] { 0, 1, 2, 3 },
                    new[] { 0, 1, 4, 5 }
                },
                mesh.Faces
            );
        }

        [Fact]
        public void TestGetBBox()
        {
            Mesh mesh = new Mesh();

            Point point0 = new Point(0.0, 0.0, 0.0);
            Point point1 = new Point(2.0, 0.0, 0.0);
            Point point2 = new Point(2.0, 2.0, 0.0);
            Point point3 = new Point(0.0, 2.0, 0.0);
            Point point4 = new Point(2.0, 0.0, 1.0);
            Point point5 = new Point(0.0, 0.0, 1.0);

            mesh.AddPoint(point0);
            mesh.AddPoint(point1);
            mesh.AddPoint(point2);
            mesh.AddPoint(point3);
            mesh.AddPoint(point4);
            mesh.AddPoint(point5);

            mesh.AddFace(new[] { 0, 1, 2, 3 });

            Assert.Equal(
                BBox.FromMinAndMax(0.0, 0.0, 0.0, 2.0, 2.0, 1.0),
                mesh.GetBBox()
            );
        }

        [Fact]
        public void TestSerialization()
        {
            var mesh = new Mesh();

            mesh.AddPoint(new Point(0.0, 0.0, 0.0));
            mesh.AddPoint(new Point(1.0, 0.0, 0.0));
            mesh.AddPoint(new Point(1.0, 1.0, 0.0));
            mesh.AddPoint(new Point(0.0, 0.0, 1.0));

            mesh.AddFace(new[] { 0, 2, 1 });
            mesh.AddFace(new[] { 0, 1, 3 });
            mesh.AddFace(new[] { 1, 2, 3 });
            mesh.AddFace(new[] { 2, 0, 3 });

            var meshItem = new HelperWorkspaceItem<Mesh>(mesh);

            var workspace = new JsonWorkspace();
            workspace.Save(meshItem);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedMeshItem =
                workspaceCopy.Load<HelperWorkspaceItem<Mesh>>(
                    meshItem.ItemInfo.Uid
                );

            Assert.Equal(mesh, deserializedMeshItem.Data);
        }

        #endregion

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
