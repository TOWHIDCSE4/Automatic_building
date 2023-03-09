using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Physics;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// Create a visibility graph for walkway generation.
    /// </summary>
    [Serializable]
    public class WalkwayVisibilityGraphCreator : IWorkspaceItem
    {
        #region Constructors

        public WalkwayVisibilityGraphCreator()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected WalkwayVisibilityGraphCreator(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            WalkwayTileComponentTemplate =
                reader.GetValue<WalkwayTileComponent>(
                    nameof(WalkwayTileComponentTemplate)
                );
            Epsilon = reader.GetValue<double>(nameof(Epsilon));
        }

        #endregion

        #region Methods

        public IReadOnlyList<Polygon> GetCollisionPolygons(ParkingLot pl)
        {
            var plc = pl.ParkingLotComponent;
            bool bOverlapWithDriveway = pl.ParkingLotComponent.ParkingLotDesigner.WalkwayDesignerComponent.OverlapWithDriveways;
            var csos = plc.GetCollisionObjects(
                bOverlapWithDriveway ? ParkingLotComponent.WalkwayOverlapWithDrivewayTypes : ParkingLotComponent.WalkwayOverlapTypes
            );

            var cps = new List<Polygon>();

            foreach (var cso in csos)
            {
                var csoCps =
                    cso.GetComponent<CollisionBody2DComponent>()
                    ?.GetWorldCollisionPolygons() ??
                    new List<Polygon>();

                csoCps = csoCps
                    .Where(cp => cp.Points.All(p => p.Z == 0.0))
                    .ToList();

                cps.AddRange(csoCps);
            }

            return cps;
        }

        public Polygon GetSitePolygon(ParkingLot pl)
        {
            var site = pl.ParkingLotComponent.Site;

            var sitePolygon = new Polygon(site.SiteComponent.Boundary);
            sitePolygon.OffsetEdges(Epsilon);

            var siteTransform = site.TransformComponent.Transform;
            sitePolygon.Transform(siteTransform);

            return sitePolygon;
        }

        public IReadOnlyList<WalkwayGraphVertex>
            GetCollisionVertices(Polygon cp)
        {
            var vertices = new List<WalkwayGraphVertex>();

            var offsetCp = new Polygon(cp);
            offsetCp.OffsetEdges(
                WalkwayTileComponentTemplate.Width / 2.0 + Epsilon
            );

            for (int pointIndex = 0; pointIndex < offsetCp.Points.Count;
                ++pointIndex)
            {
                if (offsetCp.GetInteriorAngle(pointIndex) > Math.PI)
                {
                    continue;
                }

                var point = new Point(offsetCp.Points[pointIndex]);

                int edgeIndex0 =
                    (pointIndex - 1 + offsetCp.NumOfEdges) %
                    offsetCp.NumOfEdges;

                int edgeIndex1 = pointIndex;

                var edgeNormal0 = offsetCp.GetEdgeNormal(edgeIndex0);
                var edgeNormal1 = offsetCp.GetEdgeNormal(edgeIndex1);

                var vertex = new WalkwayGraphVertex
                {
                    Type = WalkwayGraphVertexType.Normal,
                    Point = point,
                    EdgeNormal0 = edgeNormal0,
                    EdgeNormal1 = edgeNormal1
                };
                vertices.Add(vertex);
            }

            return vertices;
        }

        public IReadOnlyList<WalkwayGraphVertex>
            GetSiteVertices(Polygon sitePolygon)
        {
            var vertices = new List<WalkwayGraphVertex>();

            var offsetSitePolygon = new Polygon(sitePolygon);
            offsetSitePolygon.OffsetEdges(
                -(WalkwayTileComponentTemplate.Width / 2.0 + Epsilon)
            );

            foreach (var point in offsetSitePolygon.Points)
            {
                var vertex = new WalkwayGraphVertex
                {
                    Point = point,
                    Type = WalkwayGraphVertexType.Normal
                };
                vertices.Add(vertex);
            }

            return vertices;
        }

        public IReadOnlyList<WalkwayGraphVertex> GetCollisionAndSiteVertices(
            IReadOnlyList<Polygon> cps, Polygon sitePolygon
        )
        {
            var vertices = new List<WalkwayGraphVertex>();

            foreach (var cp in cps)
            {
                var collisionVertices = GetCollisionVertices(cp);
                vertices.AddRange(collisionVertices);
            }

            var siteVertices = GetSiteVertices(sitePolygon);
            vertices.AddRange(siteVertices);

            return vertices;
        }

        private Tuple<WalkwayGraphVertex, WalkwayGraphVertex>
            GetWalkwayEntranceVertexPair(WalkwayEntrance we)
        {
            var wec = we.WalkwayEntranceComponent;

            var weParentWorldTf =
                we.Parent?.GetComponent<TransformComponent>()
                ?.GetWorldTransform() ?? new TrsTransform3D();

            var transform0 = new TrsTransform3D(wec.Transform);
            transform0.SetTranslateLocal(wec.Width / 2.0, 0.0, 0.0);
            var point0 = new Point(transform0.GetTranslate());
            point0.Transform(weParentWorldTf);
            var vertex0 = new WalkwayGraphVertex
            {
                Point = point0,
                Type = WalkwayGraphVertexType.Source
            };

            var transform1 = new TrsTransform3D(transform0);
            transform1.SetTranslateLocal(
                0.0, WalkwayTileComponentTemplate.Width, 0.0
            );
            var point1 = new Point(transform1.GetTranslate());
            point1.Transform(weParentWorldTf);
            var vertex1 = new WalkwayGraphVertex
            {
                Point = point1,
                Type = WalkwayGraphVertexType.Normal
            };

            var vertexPair =
                new Tuple<WalkwayGraphVertex, WalkwayGraphVertex>(
                    vertex0, vertex1
                );

            return vertexPair;
        }

        public IReadOnlyList<Tuple<WalkwayGraphVertex, WalkwayGraphVertex>>
            GetBuildingEntranceVertexPairs(Building building)
        {
            var vertexPairs =
                new List<Tuple<WalkwayGraphVertex, WalkwayGraphVertex>>();

            var buildingTf = building.TransformComponent.GetWorldTransform();

            foreach (var be in building.BuildingComponent.Entrances)
            {
                var bePoint0 = new Point(be.EntrancePoint);
                bePoint0.Transform(buildingTf);

                var beVertex0 = new WalkwayGraphVertex
                {
                    Type = WalkwayGraphVertexType.Destination,
                    Point = bePoint0
                };

                var bePoint1Tf = new TrsTransform3D(be.EntranceTransform);
                bePoint1Tf.SetTranslateLocal(
                    0.0, -WalkwayTileComponentTemplate.Width / 2.0, 0.0
                );
                var bePoint1 = new Point();
                bePoint1.Transform(bePoint1Tf);
                bePoint1.Transform(buildingTf);

                var beVertex1 = new WalkwayGraphVertex
                {
                    Type = WalkwayGraphVertexType.Normal,
                    Point = bePoint1
                };

                var vertexPair =
                    new Tuple<WalkwayGraphVertex, WalkwayGraphVertex>(
                        beVertex0, beVertex1
                    );

                vertexPairs.Add(vertexPair);
            }

            return vertexPairs;
        }

        private void RemoveInvalidVertices(
            WalkwayGraph wg,
            IReadOnlyList<Polygon> cps, Polygon sitePolygon
        )
        {
            foreach (var vertex in wg.Vertices.ToList())
            {
                var point = vertex.Point;

                if (!sitePolygon.IsPointInside2D(point))
                {
                    wg.RemoveVertex(vertex);
                    continue;
                }

                foreach (var cp in cps)
                {
                    if (cp.IsPointInside2D(point))
                    {
                        wg.RemoveVertex(vertex);
                        break;
                    }
                }
            }
        }

        public bool IsEdgeValid(
            WalkwayGraphVertex vertex0, WalkwayGraphVertex vertex1,
            IReadOnlyList<Polygon> cps, Polygon sitePolygon
        )
        {
            // Reduced visibility graph

            if (vertex1.Type != WalkwayGraphVertexType.Destination &&
                vertex1.EdgeNormal0 != null &&
                vertex1.EdgeNormal1 != null)
            {
                var edgeDir =
                    new LineSegment(vertex0.Point, vertex1.Point)
                    .Direction;

                double dotProduct0 = vertex1.EdgeNormal0.DotProduct(edgeDir);
                double dotProduct1 = vertex1.EdgeNormal1.DotProduct(edgeDir);

                if (dotProduct0 < Epsilon && dotProduct1 < Epsilon)
                {
                    return false;
                }
            }

            var edgePolygon = new Polygon(
                new[] { vertex0.Point, vertex1.Point }
            );

            if (!sitePolygon.DoesContain2D(edgePolygon, Epsilon))
            {
                return false;
            }

            foreach (var cp in cps)
            {
                if (edgePolygon.DoesOverlap2D(cp, Epsilon))
                {
                    return false;
                }
            }

            return true;
        }

        public WalkwayGraph Create(ParkingLot pl)
        {
            var wg = new WalkwayGraph { ParkingLot = pl };

            // Source vertices

            var wevPairs =
                pl.ParkingLotComponent.WalkwayEntrances
                .Select(we => GetWalkwayEntranceVertexPair(we))
                .ToList();

            foreach (var wevPair in wevPairs)
            {
                wg.AddVertex(wevPair.Item1);
                wg.AddVertex(wevPair.Item2);
            }

            // Destination vertices

            var bvPairs = GetBuildingEntranceVertexPairs(
                pl.ParkingLotComponent.Building
            );

            foreach (var bvPair in bvPairs)
            {
                wg.AddVertex(bvPair.Item1);
                wg.AddVertex(bvPair.Item2);
            }

            // Normal vertices

            var cps = GetCollisionPolygons(pl);
            var sitePolygon = GetSitePolygon(pl);
            var csvs = GetCollisionAndSiteVertices(cps, sitePolygon);

            foreach (var csv in csvs)
            {
                wg.AddVertex(csv);
            }

            RemoveInvalidVertices(wg, cps, sitePolygon);

            for (int vertexIndex0 = 0; vertexIndex0 < wg.Vertices.Count;
                ++vertexIndex0)
            {
                for (int vertexIndex1 = 0; vertexIndex1 < wg.Vertices.Count;
                    ++vertexIndex1)
                {
                    if (vertexIndex1 == vertexIndex0)
                    {
                        continue;
                    }

                    var vertex0 = wg.Vertices[vertexIndex0];
                    var vertex1 = wg.Vertices[vertexIndex1];

                    if (!IsEdgeValid(vertex0, vertex1, cps, sitePolygon))
                    {
                        continue;
                    }

                    var edge = wg.AddEdge(vertex0, vertex1);

                    edge.Weight = vertex0.Point.GetDistance(vertex1.Point);
                }
            }

            return wg;
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new[] { WalkwayTileComponentTemplate };

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(
                nameof(WalkwayTileComponentTemplate),
                WalkwayTileComponentTemplate
            );
            writer.AddValue(nameof(Epsilon), Epsilon);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public WalkwayTileComponent WalkwayTileComponentTemplate { get; } =
            new WalkwayTileComponent();

        public double Epsilon { get; set; } = DefaultEpsilon;

        #endregion

        #region Constants

        public const double DefaultEpsilon = 1e-6;

        #endregion
    }
}
