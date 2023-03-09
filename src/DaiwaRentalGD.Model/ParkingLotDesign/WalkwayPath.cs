using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A walkway path computed from a <see cref="WalkwayGraph"/>.
    /// </summary>
    [Serializable]
    public class WalkwayPath : IWorkspaceItem
    {
        #region Constructors

        public WalkwayPath()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected WalkwayPath(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            Graph = reader.GetValue<WalkwayGraph>(nameof(Graph));
            SourceVertex = reader.GetValue<WalkwayGraphVertex>(
                nameof(SourceVertex)
            );
            DestinationVertex = reader.GetValue<WalkwayGraphVertex>(
                nameof(DestinationVertex)
            );

            IsValid = reader.GetValue<bool>(nameof(IsValid));

            WalkwayTileComponentTemplate =
                reader.GetValue<WalkwayTileComponent>(
                    nameof(WalkwayTileComponentTemplate)
                );

            var vertexNodePairs = reader.GetValues
                <KeyValuePair<WalkwayGraphVertex, WalkwayPathNode>>(
                    nameof(VertexNodeDict)
                );

            foreach (var pair in vertexNodePairs)
            {
                VertexNodeDict.Add(pair.Key, pair.Value);
            }

            var openList =
                reader.GetValues<WalkwayPathNode>(nameof(OpenList));

            foreach (var node in openList)
            {
                OpenList.Add(node);
            }

            var closedList =
                reader.GetValues<WalkwayPathNode>(nameof(ClosedList));

            foreach (var node in closedList)
            {
                ClosedList.Add(node);
            }
        }

        #endregion

        #region Methods

        public IReadOnlyList<WalkwayGraphVertex> GetVertices()
        {
            if (!IsValid)
            {
                return new List<WalkwayGraphVertex>();
            }

            var vertices = new List<WalkwayGraphVertex>();

            var currentWpn = VertexNodeDict[DestinationVertex];

            while (currentWpn != null)
            {
                vertices.Add(currentWpn.Vertex);
                currentWpn = currentWpn.Parent;
            }

            vertices.Reverse();

            return vertices;
        }

        public IReadOnlyList<WalkwayGraphEdge> GetEdges()
        {
            if (!IsValid)
            {
                return new List<WalkwayGraphEdge>();
            }

            var edges = new List<WalkwayGraphEdge>();

            var vertices = GetVertices();

            for (int vertexIndex = 0; vertexIndex < vertices.Count - 1;
                ++vertexIndex)
            {
                var vertex = vertices[vertexIndex];
                var outVertex = vertices[vertexIndex + 1];

                var edge = vertex.GetOutEdge(outVertex);

                edges.Add(edge);
            }

            return edges;
        }

        internal WalkwayTile CreateWalkwayTile(
            WalkwayTile previousWt,
            WalkwayGraphEdge edge
        )
        {
            var plc = Graph.ParkingLot.ParkingLotComponent;

            var wt = new WalkwayTile();

            plc.AddWalkwayTile(wt);

            var wtc = wt.WalkwayTileComponent;

            wtc.Width = WalkwayTileComponentTemplate.Width;
            wtc.Length = WalkwayTileComponentTemplate.Length;

            var point0 = edge.Vertex0.Point;
            var point1 = edge.Vertex1.Point;

            var edgeLineSegmemt = new LineSegment(point0, point1);

            var edgeDir = edgeLineSegmemt.Direction;
            var edgeLength = edgeLineSegmemt.Length;

            if (previousWt == null)
            {
                wtc.Length = edgeLength;

                double wtDirAngle = MathUtils.GetAngle2D(
                    new DenseVector(new[] { 0.0, 1.0, 0.0 }),
                    edgeDir
                );

                var wtTransform = new TrsTransform3D
                {
                    Tx = point0.X,
                    Ty = point0.Y,
                    Tz = point0.Z,
                    Rz = wtDirAngle
                };
                wtTransform.SetTranslateLocal(-wtc.Width / 2.0, 0.0, 0.0);

                wt.TransformComponent.Transform = wtTransform;

                return wt;
            }
            else
            {
                var previousWtc = previousWt.WalkwayTileComponent;

                var previousWtDir =
                    previousWtc
                    .GetForwardSideTransform(0.0)
                    .TransformDir(
                        new DenseVector(new[] { 0.0, 1.0, 0.0 })
                    );

                double angleDiff =
                    MathUtils.GetAngle2D(previousWtDir, edgeDir);
                angleDiff = Math.Max(
                    -Math.PI / 2.0,
                    Math.Min(Math.PI / 2.0, angleDiff)
                );

                double wtLengthOffset =
                    wtc.Width / 2.0 *
                    Math.Tan(Math.Abs(angleDiff / 2.0));

                wtc.SteerAngle = angleDiff;
                wtc.Length = Math.Max(0.0, edgeLength - wtLengthOffset);

                previousWtc.Length = Math.Max(
                    0.0,
                    previousWtc.Length - wtLengthOffset
                );

                previousWtc.ForwardWalkwayTile = wt;

                return wt;
            }
        }

        public IReadOnlyList<WalkwayTile> AddWalkwayTiles()
        {
            var walkwayTiles = new List<WalkwayTile>();

            WalkwayTile previousWt = null;

            foreach (var edge in GetEdges())
            {
                var wt = CreateWalkwayTile(previousWt, edge);

                walkwayTiles.Add(wt);

                previousWt = wt;
            }

            return walkwayTiles;
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>()
            .Append(Graph)
            .Append(SourceVertex)
            .Append(DestinationVertex)
            .Append(WalkwayTileComponentTemplate)
            .Concat(VertexNodeDict.Keys)
            .Concat(VertexNodeDict.Values)
            .Concat(OpenList)
            .Concat(ClosedList);

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Graph), Graph);
            writer.AddValue(nameof(SourceVertex), SourceVertex);
            writer.AddValue(nameof(DestinationVertex), DestinationVertex);

            writer.AddValue(nameof(IsValid), IsValid);

            writer.AddValue(
                nameof(WalkwayTileComponentTemplate),
                WalkwayTileComponentTemplate
            );

            writer.AddValues(nameof(VertexNodeDict), VertexNodeDict.ToList());

            writer.AddValues(nameof(OpenList), OpenList);
            writer.AddValues(nameof(ClosedList), ClosedList);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public WalkwayGraph Graph { get; internal set; }

        public WalkwayGraphVertex SourceVertex { get; internal set; }

        public WalkwayGraphVertex DestinationVertex { get; internal set; }

        public bool IsValid { get; internal set; }

        public double Length => GetEdges().Select(edge => edge.Length).Sum();

        public WalkwayTileComponent WalkwayTileComponentTemplate
        { get; } = new WalkwayTileComponent();

        internal Dictionary<WalkwayGraphVertex, WalkwayPathNode>
            VertexNodeDict
        { get; } = new Dictionary<WalkwayGraphVertex, WalkwayPathNode>();

        internal SortedSet<WalkwayPathNode> OpenList
        { get; } =
            new SortedSet<WalkwayPathNode>(new WalkwayPathNodeComparer());

        internal HashSet<WalkwayPathNode> ClosedList
        { get; } = new HashSet<WalkwayPathNode>();

        #endregion
    }
}
