using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A graph used for walkway generation.
    /// </summary>
    [Serializable]
    public class WalkwayGraph : IWorkspaceItem
    {
        #region Constructors

        public WalkwayGraph()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected WalkwayGraph(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            ParkingLot = reader.GetValue<ParkingLot>(nameof(ParkingLot));

            var vertices =
                reader.GetValues<WalkwayGraphVertex>(nameof(Vertices));

            foreach (var vertex in vertices)
            {
                _vertices.Add(vertex);
                _vertexSet.Add(vertex);
            }

            _edges.AddRange(
                reader.GetValues<WalkwayGraphEdge>(nameof(Edges))
            );
        }

        #endregion

        #region Methods

        #region Vertices

        public bool HasVertex(WalkwayGraphVertex vertex)
        {
            return _vertexSet.Contains(vertex);
        }

        public void AddVertex(WalkwayGraphVertex vertex)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            if (HasVertex(vertex))
            {
                throw new ArgumentException(
                    $"{nameof(vertex)} is already in " +
                    $"this {nameof(WalkwayGraph)}",
                    nameof(vertex)
                );
            }

            _vertices.Add(vertex);
            _vertexSet.Add(vertex);
        }

        public bool RemoveVertex(WalkwayGraphVertex vertex)
        {
            if (!HasVertex(vertex))
            {
                return false;
            }

            // Remove incoming edges
            foreach (var inEdge in vertex.InEdges.ToList())
            {
                inEdge.Vertex0._outEdges.Remove(inEdge);
                vertex._inEdges.Remove(inEdge);

                _edges.Remove(inEdge);

                inEdge.Vertex0 = null;
                inEdge.Vertex1 = null;
            }

            // Remove outgoing edges
            foreach (var outEdge in vertex.OutEdges.ToList())
            {
                outEdge.Vertex1._inEdges.Remove(outEdge);
                vertex._outEdges.Remove(outEdge);

                _edges.Remove(outEdge);

                outEdge.Vertex0 = null;
                outEdge.Vertex1 = null;
            }

            // Remove vertex
            _vertices.Remove(vertex);
            _vertexSet.Remove(vertex);

            return true;
        }

        #endregion

        #region Edges

        public bool HasEdge(
            WalkwayGraphVertex vertex0, WalkwayGraphVertex vertex1
        )
        {
            if (!HasVertex(vertex0))
            {
                return false;
            }

            if (!HasVertex(vertex1))
            {
                return false;
            }

            var outVertices0 = vertex0.OutVertices;
            return outVertices0.Contains(vertex1);
        }

        public bool HasEdge(WalkwayGraphEdge edge)
        {
            return _edges.Contains(edge);
        }

        public WalkwayGraphEdge AddEdge(
            WalkwayGraphVertex vertex0, WalkwayGraphVertex vertex1
        )
        {
            if (vertex0 == null)
            {
                throw new ArgumentNullException(nameof(vertex0));
            }

            if (vertex1 == null)
            {
                throw new ArgumentNullException(nameof(vertex1));
            }

            if (!HasVertex(vertex0))
            {
                throw new ArgumentException(
                    $"{nameof(vertex0)} is not in " +
                    $"this {nameof(WalkwayGraph)}",
                    nameof(vertex0)
                );
            }

            if (!HasVertex(vertex1))
            {
                throw new ArgumentException(
                    $"{nameof(vertex1)} is not in " +
                    $"this {nameof(WalkwayGraph)}",
                    nameof(vertex1)
                );
            }

            if (vertex0 == vertex1)
            {
                throw new ArgumentException(
                    $"{nameof(vertex1)} cannot be the same as " +
                    $"{nameof(vertex0)}",
                    nameof(vertex1)
                );
            }

            if (HasEdge(vertex0, vertex1))
            {
                throw new InvalidOperationException(
                    $"An edge exists from {nameof(vertex0)} " +
                    $"to {nameof(vertex1)}"
                );
            }

            var edge = new WalkwayGraphEdge
            {
                Vertex0 = vertex0,
                Vertex1 = vertex1
            };

            vertex0._outEdges.Add(edge);
            vertex1._inEdges.Add(edge);

            _edges.Add(edge);

            return edge;
        }

        public bool RemoveEdge(
            WalkwayGraphVertex vertex0, WalkwayGraphVertex vertex1
        )
        {
            if (!HasEdge(vertex0, vertex1))
            {
                return false;
            }

            var edge = vertex0.GetOutEdge(vertex1);
            return RemoveEdge(edge);
        }

        public bool RemoveEdge(WalkwayGraphEdge edge)
        {
            if (!HasEdge(edge))
            {
                return false;
            }

            edge.Vertex0._outEdges.Remove(edge);
            edge.Vertex1._inEdges.Remove(edge);

            edge.Vertex0 = null;
            edge.Vertex1 = null;

            _edges.Remove(edge);

            return true;
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>()
            .Append(ParkingLot)
            .Concat(Vertices)
            .Concat(Edges);

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(ParkingLot), ParkingLot);
            writer.AddValues(nameof(Vertices), _vertices);
            writer.AddValues(nameof(Edges), _edges);
        }

        #endregion

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public ParkingLot ParkingLot { get; set; }

        public IReadOnlyList<WalkwayGraphVertex> Vertices => _vertices;

        public IReadOnlyList<WalkwayGraphEdge> Edges => _edges;

        #endregion

        #region Member variables

        private readonly List<WalkwayGraphVertex> _vertices =
            new List<WalkwayGraphVertex>();

        private readonly HashSet<WalkwayGraphVertex> _vertexSet =
            new HashSet<WalkwayGraphVertex>();

        private readonly List<WalkwayGraphEdge> _edges =
            new List<WalkwayGraphEdge>();

        #endregion
    }
}
