using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Model.ParkingLotDesign;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component that organizes the various parts of a parking lot
    /// on a site.
    /// </summary>
    [Serializable]
    public class ParkingLotComponent : Component
    {
        #region Constructors

        public ParkingLotComponent() : base()
        { }

        protected ParkingLotComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _building = reader.GetValue<Building>(nameof(Building));

            SiteVectorField =
                reader.GetValue<SiteVectorField>(nameof(SiteVectorField));

            _roadsides.AddRange(
                reader.GetValues<Roadside>(nameof(Roadsides))
            );

            _walkwayEntrances.AddRange(
                reader.GetValues<WalkwayEntrance>(nameof(WalkwayEntrances))
            );

            _walkwayTiles.AddRange(
                reader.GetValues<WalkwayTile>(nameof(WalkwayTiles))
            );

            WalkwayGraph =
                reader.GetValue<WalkwayGraph>(nameof(WalkwayGraph));

            _walkwayPaths.AddRange(
                reader.GetValues<WalkwayPath>(nameof(WalkwayPaths))
            );

            _drivewayEntrances.AddRange(
                reader.GetValues<DrivewayEntrance>(nameof(DrivewayEntrances))
            );

            _drivewayTiles.AddRange(
                reader.GetValues<DrivewayTile>(nameof(DrivewayTiles))
            );

            _bikewayTiles.AddRange(
                reader.GetValues<BikewayTile>(nameof(BikewayTiles))
            );
        }

        #endregion

        #region Methods

        #region Roadside

        public void UpdateRoadsides()
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            foreach (var roadside in _roadsides.ToList())
            {
                _roadsides.Remove(roadside);

                SceneObject.RemoveChild(roadside);
                SceneObject.Scene?.RemoveSceneObject(roadside);
            }

            int numOfRoadEdges = Site.SiteComponent.RoadEdges.Count;

            for (int roadEdgeIndexIndex = 0;
                roadEdgeIndexIndex < numOfRoadEdges;
                ++roadEdgeIndexIndex)
            {
                var roadside = new Roadside();

                SceneObject.AddChild(roadside);
                _roadsides.Add(roadside);

                roadside.RoadsideComponent.RoadEdgeIndexIndex =
                    roadEdgeIndexIndex;
            }

            NotifyPropertyChanged(nameof(MaxRoadsideIndex));
        }

        #endregion

        #region Walkway

        public bool AddWalkwayEntrance(WalkwayEntrance we)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            if (we == null)
            {
                throw new ArgumentNullException(nameof(we));
            }

            var wec = we.WalkwayEntranceComponent;

            int maxRoadEdgeIndex =
                Site.SiteComponent.RoadEdgeIndices.Count - 1;

            if (wec.RoadEdgeIndexIndex > maxRoadEdgeIndex)
            {
                return false;
            }

            SceneObject.AddChild(we);

            _walkwayEntrances.Add(we);

            return true;
        }

        public bool RemoveWalkwayEntrance(WalkwayEntrance we)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            bool isRemoved = _walkwayEntrances.Remove(we);

            if (!isRemoved) { return false; }

            SceneObject.RemoveChild(we);
            SceneObject.Scene?.RemoveSceneObject(we);

            return true;
        }

        public void ClearWalkwayEntrances()
        {
            foreach (var we in WalkwayEntrances.ToList())
            {
                RemoveWalkwayEntrance(we);
            }
        }

        public void AddWalkwayTile(WalkwayTile wt)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            if (wt == null)
            {
                throw new ArgumentNullException(nameof(wt));
            }

            SceneObject.AddChild(wt);
            _walkwayTiles.Add(wt);
        }

        public bool RemoveWalkwayTile(WalkwayTile wt)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            bool isRemoved = _walkwayTiles.Remove(wt);

            if (!isRemoved) { return false; }

            wt.WalkwayTileComponent.Disconnect();

            SceneObject.RemoveChild(wt);
            SceneObject.Scene?.RemoveSceneObject(wt);

            return true;
        }

        public void ClearWalkways()
        {
            foreach (var wt in WalkwayTiles.ToList())
            {
                RemoveWalkwayTile(wt);
            }

            _walkwayPaths.Clear();
            WalkwayGraph = null;
        }

        #endregion

        #region Driveway

        public bool AddDrivewayEntrance(DrivewayEntrance de)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            if (de == null)
            {
                throw new ArgumentNullException(nameof(de));
            }

            var dec = de.DrivewayEntranceComponent;

            int maxRoadEdgeIndex =
                Site.SiteComponent.RoadEdgeIndices.Count - 1;

            if (dec.RoadEdgeIndexIndex > maxRoadEdgeIndex)
            {
                return false;
            }

            SceneObject.AddChild(de);

            _drivewayEntrances.Add(de);

            return true;
        }

        public bool RemoveDrivewayEntrance(DrivewayEntrance de)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            bool isRemoved = _drivewayEntrances.Remove(de);

            if (!isRemoved) { return false; }

            SceneObject.RemoveChild(de);
            SceneObject.Scene?.RemoveSceneObject(de);

            return true;
        }

        public void ClearDrivewayEntrances()
        {
            foreach (var de in DrivewayEntrances.ToList())
            {
                RemoveDrivewayEntrance(de);
            }
        }

        public void AddDrivewayTile(DrivewayTile dt)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            if (dt == null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            SceneObject.AddChild(dt);
            _drivewayTiles.Add(dt);
        }

        public bool RemoveDrivewayTile(DrivewayTile dt)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            bool isRemoved = _drivewayTiles.Remove(dt);

            if (!isRemoved) { return false; }

            dt.DrivewayTileComponent.Disconnect();

            SceneObject.RemoveChild(dt);
            SceneObject.Scene?.RemoveSceneObject(dt);

            return true;
        }

        public void ClearDrivewayTiles()
        {
            foreach (var dt in DrivewayTiles.ToList())
            {
                RemoveDrivewayTile(dt);
            }
        }

        #endregion

        #region Bikeway

        public void AddBikewayTile(BikewayTile bt)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            if (bt == null)
            {
                throw new ArgumentNullException(nameof(bt));
            }

            SceneObject.AddChild(bt);
            _bikewayTiles.Add(bt);
        }

        public bool RemoveBikewayTile(BikewayTile bt)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            bool isRemoved = _bikewayTiles.Remove(bt);

            if (!isRemoved) { return false; }

            bt.BikewayTileComponent.Disconnect();

            SceneObject.RemoveChild(bt);
            SceneObject.Scene?.RemoveSceneObject(bt);

            return true;
        }

        public void ClearBikewayTiles()
        {
            foreach (var bt in BikewayTiles.ToList())
            {
                RemoveBikewayTile(bt);
            }
        }

        #endregion

        #region Collision

        public IReadOnlyList<SceneObject> GetCollisionObjects()
        {
            List<SceneObject> cos = new List<SceneObject>();

            if (Building != null)
            {
                cos.Add(Building);
            }
            cos.AddRange(WalkwayTiles);
            cos.AddRange(CarParkingAreas);
            cos.AddRange(DrivewayTiles);
            cos.AddRange(BikewayTiles);
            cos.AddRange(BicycleParkingAreas);

            return cos;
        }

        public IReadOnlyList<SceneObject> GetCollisionObjects(
            IReadOnlyList<Type> overlapTypes
        )
        {
            if (overlapTypes == null)
            {
                throw new ArgumentNullException(nameof(overlapTypes));
            }

            return GetCollisionObjects()
                .Where(so => !overlapTypes.Contains(so.GetType()))
                .ToList();
        }

        #endregion

        #region Placement

        public bool DoesSiteContain(SceneObject so)
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            if (so == null)
            {
                throw new ArgumentNullException(nameof(so));
            }

            return Site.CollisionBody2DComponent.DoesContain(so);
        }

        public bool IsPlacementValid(
            SceneObject so, IReadOnlyList<Type> overlapTypes
        )
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            if (so == null)
            {
                throw new ArgumentNullException(nameof(so));
            }

            if (overlapTypes == null)
            {
                throw new ArgumentNullException(nameof(overlapTypes));
            }

            var cbc = so.GetComponent<CollisionBody2DComponent>();
            if (cbc == null) { return true; }

            if (!DoesSiteContain(so)) { return false; }

            var cos = GetCollisionObjects(overlapTypes);

            bool doesOverlapAny = cbc.DoesOverlapAny(cos);
            return !doesOverlapAny;
        }

        public bool IsPlacementValid(WalkwayTile wt)
        {
            bool overlapWithDriveway = _parkingLotDesigner.WalkwayDesignerComponent.OverlapWithDriveways;
            return IsPlacementValid(wt, overlapWithDriveway ? WalkwayOverlapWithDrivewayTypes : WalkwayOverlapTypes);
        }

        public bool IsPlacementValid(CarParkingArea cpa)
        {
            return IsPlacementValid(cpa, CarParkingAreaOverlapTypes);
        }

        public bool IsPlacementValid(DrivewayTile dt)
        {
            return IsPlacementValid(dt, DrivewayOverlapTypes);
        }

        public bool IsPlacementValid(BikewayTile bt)
        {
            return IsPlacementValid(bt, BikewayOverlapTypes);
        }

        public bool IsPlacementValid(BicycleParkingArea bpa)
        {
            return IsPlacementValid(bpa, BicycleParkingAreaOverlapTypes);
        }

        #endregion

        #region Scene

        public void Reset()
        {
            UpdateRoadsides();

            ClearWalkwayEntrances();
            ClearWalkways();

            ClearDrivewayEntrances();
            ClearDrivewayTiles();

            ClearBikewayTiles();
        }

        protected override void Update()
        {
            Reset();
        }

        #endregion

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(Building)
            .Concat(Roadsides)
            .Concat(WalkwayEntrances)
            .Concat(WalkwayTiles)
            .Append(WalkwayGraph)
            .Concat(WalkwayPaths)
            .Concat(DrivewayEntrances)
            .Concat(DrivewayTiles)
            .Concat(BikewayTiles);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Building), _building);
            writer.AddValue(nameof(SiteVectorField), SiteVectorField);
            writer.AddValues(nameof(Roadsides), _roadsides);
            writer.AddValues(nameof(WalkwayEntrances), _walkwayEntrances);
            writer.AddValues(nameof(WalkwayTiles), _walkwayTiles);
            writer.AddValue(nameof(WalkwayGraph), WalkwayGraph);
            writer.AddValues(nameof(WalkwayPaths), _walkwayPaths);
            writer.AddValues(nameof(DrivewayEntrances), _drivewayEntrances);
            writer.AddValues(nameof(DrivewayTiles), _drivewayTiles);
            writer.AddValues(nameof(BikewayTiles), _bikewayTiles);
        }

        #endregion

        #region Properties

        public Site Site => SceneObject?.Parent as Site;

        public Building Building
        {
            get => _building;
            set
            {
                _building = value;
                NotifyPropertyChanged();
            }
        }

        public ParkingLotDesigner ParkingLotDesigner
        {
            get => _parkingLotDesigner;
            set
            {
                _parkingLotDesigner = value;
                NotifyPropertyChanged();
            }
        }

        public SiteVectorField SiteVectorField { get; } =
            new SiteVectorField();

        #region Roadsides

        public IReadOnlyList<Roadside> Roadsides => _roadsides;

        public int MaxRoadsideIndex => Roadsides.Count - 1;

        #endregion

        #region Walkways

        public IReadOnlyList<WalkwayEntrance> WalkwayEntrances =>
            _walkwayEntrances;

        public IReadOnlyList<WalkwayTile> WalkwayTiles => _walkwayTiles;

        public WalkwayGraph WalkwayGraph { get; internal set; }

        public IReadOnlyList<WalkwayPath> WalkwayPaths => _walkwayPaths;

        #endregion

        #region Driveways

        public IReadOnlyList<DrivewayEntrance> DrivewayEntrances =>
            _drivewayEntrances;

        public IReadOnlyList<DrivewayTile> DrivewayTiles => _drivewayTiles;

        #endregion

        #region Car parking

        public IReadOnlyList<CarParkingArea> CarParkingAreas
        {
            get
            {
                var dtCpas = DrivewayTiles.SelectMany(
                    dt => dt.DrivewayTileComponent.CarParkingAreas
                ).ToList();

                var rbCpas = Roadsides.SelectMany(
                    rb => rb.RoadsideComponent.CarParkingAreas
                ).ToList();

                var cpas = dtCpas.Concat(rbCpas).ToList();

                return cpas;
            }
        }

        public int NumOfCarParkingSpaces =>
            CarParkingAreas
            .Select(cpa => cpa.CarParkingAreaComponent.NumOfSpaces)
            .Sum();

        #endregion

        #region Bicycle parking

        public IReadOnlyList<BikewayTile> BikewayTiles => _bikewayTiles;

        public IReadOnlyList<BicycleParkingArea> BicycleParkingAreas
        {
            get
            {
                var bpas = BikewayTiles.SelectMany(
                    bt => bt.BikewayTileComponent.BicycleParkingAreas
                ).ToList();

                return bpas;
            }
        }

        public int NumOfBicycleParkingSpaces =>
            BicycleParkingAreas
            .Select(bpa => bpa.BicycleParkingAreaComponent.NumOfSpaces)
            .Sum();

        #endregion

        #endregion

        #region Member variables

        private Building _building;
        private ParkingLotDesigner _parkingLotDesigner;

        #region Roadsides

        private readonly List<Roadside> _roadsides = new List<Roadside>();

        #endregion

        #region Walkways

        private readonly List<WalkwayEntrance> _walkwayEntrances =
            new List<WalkwayEntrance>();

        private readonly List<WalkwayTile> _walkwayTiles =
            new List<WalkwayTile>();

        internal readonly List<WalkwayPath> _walkwayPaths =
            new List<WalkwayPath>();

        #endregion

        #region Driveways

        private readonly List<DrivewayEntrance> _drivewayEntrances =
            new List<DrivewayEntrance>();

        private readonly List<DrivewayTile> _drivewayTiles =
            new List<DrivewayTile>();

        #endregion

        #region Bicycle parking

        private readonly List<BikewayTile> _bikewayTiles =
            new List<BikewayTile>();

        #endregion

        #endregion

        #region Constants

        public static IReadOnlyList<Type> WalkwayOverlapTypes { get; } =
            new List<Type>
            {
                typeof(WalkwayTile)
            };

        public static IReadOnlyList<Type> WalkwayOverlapWithDrivewayTypes { get; } =
            new List<Type>
            {
                typeof(WalkwayTile),
                typeof(DrivewayTile)
            };

        public static IReadOnlyList<Type> DrivewayOverlapTypes { get; } =
            new List<Type>();

        public static IReadOnlyList<Type> CarParkingAreaOverlapTypes { get; } =
            new List<Type>();

        public static IReadOnlyList<Type> BikewayOverlapTypes { get; } =
            new List<Type>
            {
                typeof(WalkwayTile),
                typeof(DrivewayTile)
            };

        public static IReadOnlyList<Type> BicycleParkingAreaOverlapTypes { get; } =
            new List<Type>();

        #endregion
    }
}
