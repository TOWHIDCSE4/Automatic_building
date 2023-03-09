using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A pathfinding algorithm that computes a <see cref="WalkwayPath"/>
    /// from a <see cref="WalkwayGraph"/>.
    /// </summary>
    [Serializable]
    public class WalkwayPathfinder : IWorkspaceItem
    {
        #region Constructors

        public WalkwayPathfinder()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected WalkwayPathfinder(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            Epsilon = reader.GetValue<double>(nameof(Epsilon));
        }

        #endregion

        #region Methods

        private double GetHeuristic(
            WalkwayGraphVertex vertex, WalkwayGraphVertex destinationVertex
        )
        {
            // Euclidean distance is used for heuristic.

            var point = vertex.Point;
            var destinationPoint = destinationVertex.Point;

            double distance = point.GetDistance(destinationPoint);
            return distance;
        }

        private WalkwayPath CreateInitialWalkwayPath(
            WalkwayGraph graph,
            WalkwayGraphVertex sourceVertex,
            WalkwayGraphVertex destinationVertex
        )
        {
            var wp = new WalkwayPath
            {
                Graph = graph,
                SourceVertex = sourceVertex,
                DestinationVertex = destinationVertex
            };

            foreach (var vertex in graph.Vertices)
            {
                var wpn = new WalkwayPathNode
                {
                    Vertex = vertex,

                    GScore = double.PositiveInfinity,
                    HScore = GetHeuristic(vertex, destinationVertex),

                    Parent = null
                };

                wp.VertexNodeDict[vertex] = wpn;
            }

            var sourceWpn = wp.VertexNodeDict[sourceVertex];
            sourceWpn.GScore = 0.0;

            return wp;
        }

        private double GetEdgeTurnAngle(
            WalkwayGraphVertex parentVertex,
            WalkwayGraphEdge outEdge
        )
        {
            if (parentVertex == null)
            {
                return 0.0;
            }

            var parentEdgeDir =
                new LineSegment(
                    parentVertex.Point,
                    outEdge.Vertex0.Point
                ).Direction;

            var outEdgeDir =
                new LineSegment(
                    outEdge.Vertex0.Point,
                    outEdge.Vertex1.Point
                ).Direction;

            double turnAngle =
                MathUtils.GetAngle2D(parentEdgeDir, outEdgeDir);

            return turnAngle;
        }

        private bool IsWalkwayTilePlacementValid(
            WalkwayPath wp, WalkwayGraphVertex parentVertex,
            WalkwayGraphEdge edge
        )
        {
            var plc = wp.Graph.ParkingLot.ParkingLotComponent;

            if (parentVertex == null)
            {
                var wt = wp.CreateWalkwayTile(null, edge);

                bool isValid = plc.IsPlacementValid(wt);

                plc.RemoveWalkwayTile(wt);

                return isValid;
            }
            else
            {
                var previousEdge = edge.Vertex0.GetInEdge(parentVertex);
                var previousWt = wp.CreateWalkwayTile(null, previousEdge);

                var wt = wp.CreateWalkwayTile(previousWt, edge);

                bool isValid = plc.IsPlacementValid(wt);

                plc.RemoveWalkwayTile(wt);
                plc.RemoveWalkwayTile(previousWt);

                return isValid;
            }
        }

        public WalkwayPath FindPath(
            WalkwayGraph graph,
            WalkwayGraphVertex sourceVertex,
            WalkwayGraphVertex destinationVertex
        )
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (sourceVertex == null)
            {
                throw new ArgumentNullException(nameof(sourceVertex));
            }

            if (destinationVertex == null)
            {
                throw new ArgumentNullException(nameof(destinationVertex));
            }

            if (!graph.Vertices.Contains(sourceVertex))
            {
                throw new ArgumentException(
                    $"{nameof(sourceVertex)} is not in {nameof(graph)}",
                    nameof(sourceVertex)
                );
            }

            if (!graph.Vertices.Contains(destinationVertex))
            {
                throw new ArgumentException(
                    $"{nameof(destinationVertex)} is not in {nameof(graph)}",
                    nameof(destinationVertex)
                );
            }

            var wp = CreateInitialWalkwayPath(
                graph, sourceVertex, destinationVertex
            );

            var sourceWpn = wp.VertexNodeDict[sourceVertex];

            wp.OpenList.Add(sourceWpn);

            while (wp.OpenList.Any())
            {
                var wpn = wp.OpenList.First();
                wp.OpenList.Remove(wpn);

                wp.ClosedList.Add(wpn);

                var vertex = wpn.Vertex;

                if (vertex == destinationVertex)
                {
                    wp.IsValid = true;
                    break;
                }

                var parentVertex = wpn.Parent?.Vertex;

                foreach (var outEdge in vertex.OutEdges)
                {
                    double edgeTurnAngle =
                        GetEdgeTurnAngle(parentVertex, outEdge);

                    if (Math.Abs(edgeTurnAngle) > Math.PI / 2.0 + Epsilon)
                    {
                        continue;
                    }

                    if (!IsWalkwayTilePlacementValid(
                        wp, parentVertex, outEdge
                    ))
                    {
                        continue;
                    }

                    var outVertex = outEdge.Vertex1;
                    var outWpn = wp.VertexNodeDict[outVertex];

                    if (wp.ClosedList.Contains(outWpn))
                    {
                        continue;
                    }

                    double currentOutG = outWpn.GScore;
                    double newOutG = wpn.GScore + outEdge.Weight;

                    if (newOutG < currentOutG)
                    {
                        wp.OpenList.Remove(outWpn);

                        outWpn.GScore = newOutG;
                        outWpn.Parent = wpn;

                        wp.OpenList.Add(outWpn);
                    }
                }
            }

            return wp;
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>();

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Epsilon), Epsilon);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public double Epsilon { get; set; } = DefaultEpsilon;

        #endregion

        #region Constants

        public const double DefaultEpsilon = 1e-6;

        #endregion
    }
}
