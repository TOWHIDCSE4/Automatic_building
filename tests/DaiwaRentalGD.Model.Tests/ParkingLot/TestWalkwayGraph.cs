using System;
using DaiwaRentalGD.Model.ParkingLotDesign;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.ParkingLot
{
    public class TestWalkwayGraph
    {
        [Fact]
        public void TestConstructor()
        {
            var wg = new WalkwayGraph();

            Assert.Empty(wg.Vertices);
            Assert.Empty(wg.Edges);
        }

        [Fact]
        public void TestHasVertex()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();
            var vertex2 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);

            Assert.True(wg.HasVertex(vertex0));
            Assert.True(wg.HasVertex(vertex1));
            Assert.False(wg.HasVertex(vertex2));
            Assert.False(wg.HasVertex(null));
        }

        [Fact]
        public void TestAddVertex()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);

            Assert.Equal(
                new[] { vertex0, vertex1 },
                wg.Vertices
            );
        }

        [Fact]
        public void TestAddVertex_Null()
        {
            var wg = new WalkwayGraph();

            Assert.Throws<ArgumentNullException>(
                () => { wg.AddVertex(null); }
            );

            Assert.Empty(wg.Vertices);
        }

        [Fact]
        public void TestAddVertex_VertexExists()
        {
            var wg = new WalkwayGraph();

            var vertex = new WalkwayGraphVertex();
            wg.AddVertex(vertex);

            Assert.ThrowsAny<ArgumentException>(
                () => { wg.AddVertex(vertex); }
            );

            Assert.Equal(
                new[] { vertex },
                wg.Vertices
            );
        }

        [Fact]
        public void TestRemoveVertex()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();
            var vertex2 = new WalkwayGraphVertex();
            var vertex3 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);
            wg.AddVertex(vertex2);

            var edge01 = wg.AddEdge(vertex0, vertex1);
            var edge10 = wg.AddEdge(vertex1, vertex0);
            var edge12 = wg.AddEdge(vertex1, vertex2);
            var edge21 = wg.AddEdge(vertex2, vertex1);
            var edge20 = wg.AddEdge(vertex2, vertex0);

            Assert.Equal(
                new[] { edge10, edge20 },
                vertex0.InEdges
            );
            Assert.Equal(
                new[] { edge01 },
                vertex0.OutEdges
            );
            Assert.Equal(
                new[] { edge01, edge21 },
                vertex1.InEdges
            );
            Assert.Equal(
                new[] { edge10, edge12 },
                vertex1.OutEdges
            );
            Assert.Equal(
                new[] { edge12 },
                vertex2.InEdges
            );
            Assert.Equal(
                new[] { edge21, edge20 },
                vertex2.OutEdges
            );

            Assert.Equal(vertex0, edge01.Vertex0);
            Assert.Equal(vertex1, edge01.Vertex1);

            Assert.Equal(vertex1, edge10.Vertex0);
            Assert.Equal(vertex0, edge10.Vertex1);

            Assert.Equal(vertex1, edge12.Vertex0);
            Assert.Equal(vertex2, edge12.Vertex1);

            Assert.Equal(vertex2, edge21.Vertex0);
            Assert.Equal(vertex1, edge21.Vertex1);

            Assert.Equal(vertex2, edge20.Vertex0);
            Assert.Equal(vertex0, edge20.Vertex1);

            Assert.Equal(
                new[] { vertex0, vertex1, vertex2 },
                wg.Vertices
            );
            Assert.Equal(
                new[] { edge01, edge10, edge12, edge21, edge20 },
                wg.Edges
            );

            Assert.True(wg.RemoveVertex(vertex1));
            Assert.False(wg.RemoveVertex(vertex3));
            Assert.False(wg.RemoveVertex(null));

            Assert.Equal(
                new[] { edge20 },
                vertex0.InEdges
            );
            Assert.Empty(vertex0.OutEdges);

            Assert.Empty(vertex1.InEdges);
            Assert.Empty(vertex1.OutEdges);

            Assert.Empty(vertex2.InEdges);
            Assert.Equal(
                new[] { edge20 },
                vertex2.OutEdges
            );

            Assert.Null(edge01.Vertex0);
            Assert.Null(edge01.Vertex1);

            Assert.Null(edge10.Vertex0);
            Assert.Null(edge10.Vertex1);

            Assert.Null(edge12.Vertex0);
            Assert.Null(edge12.Vertex1);

            Assert.Null(edge21.Vertex0);
            Assert.Null(edge21.Vertex1);

            Assert.Equal(vertex2, edge20.Vertex0);
            Assert.Equal(vertex0, edge20.Vertex1);

            Assert.Equal(
                new[] { vertex0, vertex2 },
                wg.Vertices
            );
            Assert.Equal(
                new[] { edge20 },
                wg.Edges
            );
        }

        [Fact]
        public void TestHasEdge()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();
            var vertex2 = new WalkwayGraphVertex();
            var vertex3 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);
            wg.AddVertex(vertex2);

            var edge01 = wg.AddEdge(vertex0, vertex1);
            var edge10 = wg.AddEdge(vertex1, vertex0);
            var edge12 = wg.AddEdge(vertex1, vertex2);
            var edge21 = wg.AddEdge(vertex2, vertex1);
            var edge20 = wg.AddEdge(vertex2, vertex0);

            Assert.True(wg.HasEdge(vertex0, vertex1));
            Assert.True(wg.HasEdge(vertex1, vertex0));
            Assert.True(wg.HasEdge(vertex1, vertex2));
            Assert.True(wg.HasEdge(vertex2, vertex1));
            Assert.True(wg.HasEdge(vertex2, vertex0));

            Assert.False(wg.HasEdge(vertex0, vertex2));

            Assert.False(wg.HasEdge(vertex0, vertex0));

            Assert.False(wg.HasEdge(null, vertex0));
            Assert.False(wg.HasEdge(vertex0, null));
            Assert.False(wg.HasEdge(null, null));

            Assert.False(wg.HasEdge(vertex0, vertex3));
            Assert.False(wg.HasEdge(vertex3, vertex0));
            Assert.False(wg.HasEdge(vertex3, vertex3));

            Assert.True(wg.HasEdge(edge01));
            Assert.True(wg.HasEdge(edge10));
            Assert.True(wg.HasEdge(edge12));
            Assert.True(wg.HasEdge(edge21));
            Assert.True(wg.HasEdge(edge20));

            Assert.False(wg.HasEdge(null));
        }

        [Fact]
        public void TestAddEdge()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();
            var vertex2 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);
            wg.AddVertex(vertex2);

            var edge01 = wg.AddEdge(vertex0, vertex1);
            var edge10 = wg.AddEdge(vertex1, vertex0);
            var edge12 = wg.AddEdge(vertex1, vertex2);
            var edge21 = wg.AddEdge(vertex2, vertex1);
            var edge20 = wg.AddEdge(vertex2, vertex0);

            Assert.Equal(
                new[] { edge10, edge20 },
                vertex0.InEdges
            );
            Assert.Equal(
                new[] { edge01 },
                vertex0.OutEdges
            );
            Assert.Equal(
                new[] { edge01, edge21 },
                vertex1.InEdges
            );
            Assert.Equal(
                new[] { edge10, edge12 },
                vertex1.OutEdges
            );
            Assert.Equal(
                new[] { edge12 },
                vertex2.InEdges
            );
            Assert.Equal(
                new[] { edge21, edge20 },
                vertex2.OutEdges
            );

            Assert.Equal(vertex0, edge01.Vertex0);
            Assert.Equal(vertex1, edge01.Vertex1);

            Assert.Equal(vertex1, edge10.Vertex0);
            Assert.Equal(vertex0, edge10.Vertex1);

            Assert.Equal(vertex1, edge12.Vertex0);
            Assert.Equal(vertex2, edge12.Vertex1);

            Assert.Equal(vertex2, edge21.Vertex0);
            Assert.Equal(vertex1, edge21.Vertex1);

            Assert.Equal(vertex2, edge20.Vertex0);
            Assert.Equal(vertex0, edge20.Vertex1);

            Assert.Equal(
                new[] { vertex0, vertex1, vertex2 },
                wg.Vertices
            );
            Assert.Equal(
                new[] { edge01, edge10, edge12, edge21, edge20 },
                wg.Edges
            );
        }

        [Fact]
        public void TestAddEdge_NullVertex()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();
            var vertex2 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);
            wg.AddVertex(vertex2);

            Assert.Throws<ArgumentNullException>(
                () => { wg.AddEdge(vertex0, null); }
            );
            Assert.Throws<ArgumentNullException>(
                () => { wg.AddEdge(null, vertex0); }
            );

            Assert.Empty(vertex0.InEdges);
            Assert.Empty(vertex0.OutEdges);

            Assert.Equal(
                new[] { vertex0, vertex1, vertex2 },
                wg.Vertices
            );
            Assert.Empty(wg.Edges);
        }

        [Fact]
        public void TestAddEdge_VertexDoesNotExist()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();
            var vertex2 = new WalkwayGraphVertex();
            var vertex3 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);
            wg.AddVertex(vertex2);

            Assert.ThrowsAny<ArgumentException>(
                () => { wg.AddEdge(vertex0, vertex3); }
            );
            Assert.ThrowsAny<ArgumentException>(
                () => { wg.AddEdge(vertex3, vertex0); }
            );

            Assert.Empty(vertex0.InEdges);
            Assert.Empty(vertex0.OutEdges);
            Assert.Empty(vertex3.InEdges);
            Assert.Empty(vertex3.OutEdges);

            Assert.Equal(
                new[] { vertex0, vertex1, vertex2 },
                wg.Vertices
            );
            Assert.Empty(wg.Edges);
        }

        [Fact]
        public void TestAddEdge_SameVertex()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();
            var vertex2 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);
            wg.AddVertex(vertex2);

            Assert.ThrowsAny<ArgumentException>(
                () => { wg.AddEdge(vertex0, vertex0); }
            );

            Assert.Empty(vertex0.InEdges);
            Assert.Empty(vertex0.OutEdges);

            Assert.Equal(
                new[] { vertex0, vertex1, vertex2 },
                wg.Vertices
            );
            Assert.Empty(wg.Edges);
        }

        [Fact]
        public void TestAddEdge_EdgeExists()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();
            var vertex2 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);
            wg.AddVertex(vertex2);

            var edge01 = wg.AddEdge(vertex0, vertex1);

            Assert.ThrowsAny<InvalidOperationException>(
                () => { wg.AddEdge(vertex0, vertex1); }
            );

            Assert.Empty(vertex0.InEdges);
            Assert.Equal(
                new[] { edge01 },
                vertex0.OutEdges
            );
            Assert.Equal(
                new[] { edge01 },
                vertex1.InEdges
            );
            Assert.Empty(vertex1.OutEdges);

            Assert.Equal(vertex0, edge01.Vertex0);
            Assert.Equal(vertex1, edge01.Vertex1);

            Assert.Equal(
                new[] { vertex0, vertex1, vertex2 },
                wg.Vertices
            );
            Assert.Equal(
                new[] { edge01 },
                wg.Edges
            );
        }

        [Fact]
        public void TestRemoveEdge_ByVertices()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();
            var vertex2 = new WalkwayGraphVertex();
            var vertex3 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);
            wg.AddVertex(vertex2);

            var edge01 = wg.AddEdge(vertex0, vertex1);
            var edge10 = wg.AddEdge(vertex1, vertex0);
            var edge12 = wg.AddEdge(vertex1, vertex2);
            var edge21 = wg.AddEdge(vertex2, vertex1);
            var edge20 = wg.AddEdge(vertex2, vertex0);

            Assert.Equal(
                new[] { edge10, edge20 },
                vertex0.InEdges
            );
            Assert.Equal(
                new[] { edge01 },
                vertex0.OutEdges
            );
            Assert.Equal(
                new[] { edge01, edge21 },
                vertex1.InEdges
            );
            Assert.Equal(
                new[] { edge10, edge12 },
                vertex1.OutEdges
            );
            Assert.Equal(
                new[] { edge12 },
                vertex2.InEdges
            );
            Assert.Equal(
                new[] { edge21, edge20 },
                vertex2.OutEdges
            );

            Assert.Equal(vertex0, edge01.Vertex0);
            Assert.Equal(vertex1, edge01.Vertex1);
            Assert.Equal(vertex1, edge10.Vertex0);
            Assert.Equal(vertex0, edge10.Vertex1);
            Assert.Equal(vertex1, edge12.Vertex0);
            Assert.Equal(vertex2, edge12.Vertex1);
            Assert.Equal(vertex2, edge21.Vertex0);
            Assert.Equal(vertex1, edge21.Vertex1);
            Assert.Equal(vertex2, edge20.Vertex0);
            Assert.Equal(vertex0, edge20.Vertex1);

            Assert.Equal(
                new[] { vertex0, vertex1, vertex2 },
                wg.Vertices
            );
            Assert.Equal(
                new[] { edge01, edge10, edge12, edge21, edge20 },
                wg.Edges
            );

            Assert.True(wg.RemoveEdge(vertex1, vertex2));
            Assert.False(wg.RemoveEdge(vertex1, vertex2));
            Assert.False(wg.RemoveEdge(null, vertex0));
            Assert.False(wg.RemoveEdge(vertex3, vertex0));

            Assert.Equal(
                new[] { edge01, edge21 },
                vertex1.InEdges
            );
            Assert.Equal(
                new[] { edge10 },
                vertex1.OutEdges
            );
            Assert.Empty(vertex2.InEdges);
            Assert.Equal(
                new[] { edge21, edge20 },
                vertex2.OutEdges
            );

            Assert.Equal(
                new[] { vertex0, vertex1, vertex2 },
                wg.Vertices
            );
            Assert.Equal(
                new[] { edge01, edge10, edge21, edge20 },
                wg.Edges
            );

            Assert.Null(edge12.Vertex0);
            Assert.Null(edge12.Vertex1);
        }

        [Fact]
        public void TestRemoveEdge_ByEdge()
        {
            var wg = new WalkwayGraph();

            var vertex0 = new WalkwayGraphVertex();
            var vertex1 = new WalkwayGraphVertex();
            var vertex2 = new WalkwayGraphVertex();

            wg.AddVertex(vertex0);
            wg.AddVertex(vertex1);
            wg.AddVertex(vertex2);

            var edge01 = wg.AddEdge(vertex0, vertex1);
            var edge10 = wg.AddEdge(vertex1, vertex0);
            var edge12 = wg.AddEdge(vertex1, vertex2);
            var edge21 = wg.AddEdge(vertex2, vertex1);
            var edge20 = wg.AddEdge(vertex2, vertex0);

            Assert.Equal(
                new[] { edge10, edge20 },
                vertex0.InEdges
            );
            Assert.Equal(
                new[] { edge01 },
                vertex0.OutEdges
            );
            Assert.Equal(
                new[] { edge01, edge21 },
                vertex1.InEdges
            );
            Assert.Equal(
                new[] { edge10, edge12 },
                vertex1.OutEdges
            );
            Assert.Equal(
                new[] { edge12 },
                vertex2.InEdges
            );
            Assert.Equal(
                new[] { edge21, edge20 },
                vertex2.OutEdges
            );

            Assert.Equal(vertex0, edge01.Vertex0);
            Assert.Equal(vertex1, edge01.Vertex1);
            Assert.Equal(vertex1, edge10.Vertex0);
            Assert.Equal(vertex0, edge10.Vertex1);
            Assert.Equal(vertex1, edge12.Vertex0);
            Assert.Equal(vertex2, edge12.Vertex1);
            Assert.Equal(vertex2, edge21.Vertex0);
            Assert.Equal(vertex1, edge21.Vertex1);
            Assert.Equal(vertex2, edge20.Vertex0);
            Assert.Equal(vertex0, edge20.Vertex1);

            Assert.Equal(
                new[] { vertex0, vertex1, vertex2 },
                wg.Vertices
            );
            Assert.Equal(
                new[] { edge01, edge10, edge12, edge21, edge20 },
                wg.Edges
            );

            Assert.True(wg.RemoveEdge(edge12));
            Assert.False(wg.RemoveEdge(edge12));
            Assert.False(wg.RemoveEdge(null));

            Assert.Equal(
                new[] { edge01, edge21 },
                vertex1.InEdges
            );
            Assert.Equal(
                new[] { edge10 },
                vertex1.OutEdges
            );
            Assert.Empty(vertex2.InEdges);
            Assert.Equal(
                new[] { edge21, edge20 },
                vertex2.OutEdges
            );

            Assert.Equal(
                new[] { vertex0, vertex1, vertex2 },
                wg.Vertices
            );
            Assert.Equal(
                new[] { edge01, edge10, edge21, edge20 },
                wg.Edges
            );

            Assert.Null(edge12.Vertex0);
            Assert.Null(edge12.Vertex1);
        }
    }
}
