using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A vertex in the graph for walkway generation.
    /// </summary>
    [Serializable]
    public class WalkwayGraphVertex : IWorkspaceItem
    {
        #region Constructors

        public WalkwayGraphVertex()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected WalkwayGraphVertex(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            _inEdges.AddRange(
                reader.GetValues<WalkwayGraphEdge>(nameof(InEdges))
            );
            _outEdges.AddRange(
                reader.GetValues<WalkwayGraphEdge>(nameof(OutEdges))
            );

            Type = reader.GetValue<WalkwayGraphVertexType>(nameof(Type));

            Point = new Point(reader.GetValue<List<double>>(nameof(Point)));

            var edgeNormal0Values =
                reader.GetValue<List<double>>(nameof(EdgeNormal0));

            if (edgeNormal0Values != null)
            {
                EdgeNormal0 = new DenseVector(edgeNormal0Values.ToArray());
            }

            var edgeNormal1Values =
                reader.GetValue<List<double>>(nameof(EdgeNormal1));

            if (edgeNormal1Values != null)
            {
                EdgeNormal1 = new DenseVector(edgeNormal1Values.ToArray());
            }
        }

        #endregion

        #region Methods

        public WalkwayGraphEdge GetInEdge(WalkwayGraphVertex inVertex)
        {
            foreach (var inEdge in InEdges)
            {
                if (inEdge.Vertex0 == inVertex)
                {
                    return inEdge;
                }
            }

            return null;
        }

        public WalkwayGraphEdge GetOutEdge(WalkwayGraphVertex outVertex)
        {
            foreach (var outEdge in OutEdges)
            {
                if (outEdge.Vertex1 == outVertex)
                {
                    return outEdge;
                }
            }

            return null;
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>()
            .Concat(InEdges)
            .Concat(OutEdges);

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValues(nameof(InEdges), _inEdges);
            writer.AddValues(nameof(OutEdges), _outEdges);

            writer.AddValue(nameof(Type), Type);

            writer.AddValue(nameof(Point), Point.AsList());

            writer.AddValue(nameof(EdgeNormal0), EdgeNormal0?.ToList());
            writer.AddValue(nameof(EdgeNormal1), EdgeNormal1?.ToList());
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public IReadOnlyList<WalkwayGraphEdge> InEdges => _inEdges;

        public IReadOnlyList<WalkwayGraphEdge> OutEdges => _outEdges;

        public IReadOnlyList<WalkwayGraphVertex> InVertices =>
            _inEdges.Select(edge => edge.Vertex0).ToList();

        public IReadOnlyList<WalkwayGraphVertex> OutVertices =>
            _outEdges.Select(edge => edge.Vertex1).ToList();

        public WalkwayGraphVertexType Type { get; set; } =
            WalkwayGraphVertexType.Normal;

        public Point Point { get; set; }

        public Vector<double> EdgeNormal0 { get; set; }

        public Vector<double> EdgeNormal1 { get; set; }

        #endregion

        #region Member variables

        internal readonly List<WalkwayGraphEdge> _inEdges =
            new List<WalkwayGraphEdge>();

        internal readonly List<WalkwayGraphEdge> _outEdges =
            new List<WalkwayGraphEdge>();

        #endregion
    }
}
