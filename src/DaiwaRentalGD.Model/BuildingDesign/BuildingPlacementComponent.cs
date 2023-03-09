using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component for setting the placement of a building on a site.
    /// </summary>
    [Serializable]
    public class BuildingPlacementComponent : Component
    {
        #region Constructors

        public BuildingPlacementComponent()
        { }

        protected BuildingPlacementComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _building = reader.GetValue<Building>(nameof(Building));

            _site = reader.GetValue<Site>(nameof(Site));

            _buildingOrientationMode =
                reader.GetValue<BuildingOrientationMode>(
                    nameof(BuildingOrientationMode)
                );

            _buildingX = reader.GetValue<double>(nameof(BuildingX));

            _buildingY = reader.GetValue<double>(nameof(BuildingY));

            _buildingMinTNAngle =
                reader.GetValue<double>(nameof(BuildingMinTNAngle));

            _buildingMaxTNAngle =
                reader.GetValue<double>(nameof(BuildingMaxTNAngle));

            _buildingTNAngle =
                reader.GetValue<double>(nameof(BuildingTNAngle));
        }

        #endregion

        #region Methods

        private void Place(Building building)
        {
            var buildingTc = building.TransformComponent;

            buildingTc.Transform.Tx = BuildingX;
            buildingTc.Transform.Ty = BuildingY;

            buildingTc.Transform.Rz = BuildingActualAngle;
        }

        protected override void Update()
        {
            if (Site == null)
            {
                return;
            }

            if (Building == null)
            {
                return;
            }

            NotifyPropertyChanged(nameof(BuildingMinX));
            NotifyPropertyChanged(nameof(BuildingMaxX));
            NotifyPropertyChanged(nameof(BuildingMinY));
            NotifyPropertyChanged(nameof(BuildingMaxY));

            Place(Building);
        }

        public static double ConvertRadiansToDegrees(double angleRadians)
        {
            return angleRadians / Math.PI * 180.0;
        }

        public static double ConvertDegreesToRadians(double angleDegrees)
        {
            return angleDegrees / 180.0 * Math.PI;
        }

        public IReadOnlyList<double> GetNorthPropertyEdgeTNAngles()
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            var sc = Site.SiteComponent;

            var trueNorthDir = sc.TrueNorthDirection;
            var northEdgeIndices = sc.GetNorthEdgeIndices(false);

            var northPropertyEdgeTNAngles = new List<double>();

            foreach (var northEdgeIndex in northEdgeIndices)
            {
                if (sc.BoundaryEdgeTypes[northEdgeIndex] !=
                    SiteEdgeType.Property)
                {
                    continue;
                }

                var northEdgeNormal =
                    sc.Boundary.GetEdgeNormal(northEdgeIndex);

                double northPropertyEdgeTNAngle =
                    MathUtils.GetAngle2D(trueNorthDir, northEdgeNormal);

                if (northPropertyEdgeTNAngle < BuildingMinTNAngle)
                {
                    continue;
                }
                if (northPropertyEdgeTNAngle > BuildingMaxTNAngle)
                {
                    continue;
                }

                northPropertyEdgeTNAngles.Add(northPropertyEdgeTNAngle);
            }

            return northPropertyEdgeTNAngles;
        }

        public IReadOnlyList<double> GetRoadEdgeTNAngles()
        {
            if (Site == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Site)} is not set"
                );
            }

            var sc = Site.SiteComponent;

            var trueNorthDir = sc.TrueNorthDirection;

            var roadEdgeTNAngles = new List<double>();

            foreach (int roadEdgeIndex in sc.RoadEdgeIndices)
            {
                var roadEdgeNormal = sc.Boundary.GetEdgeNormal(roadEdgeIndex);

                double roadEdgeTNAngle =
                    MathUtils.GetAngle2D(trueNorthDir, roadEdgeNormal);

                if (roadEdgeTNAngle >= BuildingMinTNAngle &&
                    roadEdgeTNAngle <= BuildingMaxTNAngle)
                {
                    roadEdgeTNAngles.Add(roadEdgeTNAngle);
                }

                double roadEdgeTNAngle1 = roadEdgeTNAngle + Math.PI;

                if (roadEdgeTNAngle1 >= BuildingMinTNAngle &&
                    roadEdgeTNAngle1 <= BuildingMaxTNAngle)
                {
                    roadEdgeTNAngles.Add(roadEdgeTNAngle1);
                }

                double roadEdgeTNAngle0 = roadEdgeTNAngle - Math.PI;

                if (roadEdgeTNAngle0 >= BuildingMinTNAngle &&
                    roadEdgeTNAngle0 <= BuildingMaxTNAngle)
                {
                    roadEdgeTNAngles.Add(roadEdgeTNAngle0);
                }
            }

            return roadEdgeTNAngles;
        }

        public double GetAlignedTNAngle(double tnAngle)
        {
            var targetTNAngles = new List<double>();

            targetTNAngles.AddRange(GetNorthPropertyEdgeTNAngles());
            targetTNAngles.AddRange(GetRoadEdgeTNAngles());

            double alignedTNAngle =
                GetAlignedTNAngle(tnAngle, targetTNAngles);

            return alignedTNAngle;
        }

        private double GetAlignedTNAngle(
            double tnAngle, IReadOnlyList<double> targetTNAngles
        )
        {
            double alignedTNAngle = tnAngle;
            double minAbsAngleDiff = double.MaxValue;

            foreach (var targetTNAngle in targetTNAngles)
            {
                double absAngleDiff = Math.Abs(tnAngle - targetTNAngle);

                if (absAngleDiff < minAbsAngleDiff)
                {
                    minAbsAngleDiff = absAngleDiff;
                    alignedTNAngle = targetTNAngle;
                }
            }

            return alignedTNAngle;
        }

        public double GetAlignedAngle(double angle)
        {
            double tnAngle = angle - TrueNorthAngle;

            double alignedTNAngle = GetAlignedTNAngle(tnAngle);

            double alignedAngle = alignedTNAngle + TrueNorthAngle;

            return alignedAngle;
        }



        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(Building).Append(Site);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Building), _building);
            writer.AddValue(nameof(Site), _site);

            writer.AddValue(
                nameof(BuildingOrientationMode), _buildingOrientationMode
            );

            writer.AddValue(nameof(BuildingX), _buildingX);
            writer.AddValue(nameof(BuildingY), _buildingY);

            writer.AddValue(nameof(BuildingMinTNAngle), _buildingMinTNAngle);
            writer.AddValue(nameof(BuildingMaxTNAngle), _buildingMaxTNAngle);
            writer.AddValue(nameof(BuildingTNAngle), _buildingTNAngle);
        }

        #endregion

        #region Properties

        public Building Building
        {
            get => _building;
            set
            {
                _building = value;
                NotifyPropertyChanged();
            }
        }

        public Site Site
        {
            get => _site;
            set
            {
                _site = value;
                NotifyPropertyChanged();

                NotifyPropertyChanged(nameof(BuildingMinX));
                NotifyPropertyChanged(nameof(BuildingMaxX));
                NotifyPropertyChanged(nameof(BuildingMinY));
                NotifyPropertyChanged(nameof(BuildingMaxY));
            }
        }

        #region Location

        public double BuildingMinX
        {
            get => Site?.SiteComponent.Boundary.GetBBox().MinX ?? 0.0;
        }

        public double BuildingMaxX
        {
            get => Site?.SiteComponent.Boundary.GetBBox().MaxX ?? 0.0;
        }

        public double BuildingX
        {
            get => _buildingX;
            set
            {
                _buildingX = value;
                NotifyPropertyChanged();
            }
        }

        public double BuildingMinY
        {
            get => Site?.SiteComponent.Boundary.GetBBox().MinY ?? 0.0;
        }

        public double BuildingMaxY
        {
            get => Site?.SiteComponent.Boundary.GetBBox().MaxY ?? 0.0;
        }

        public double BuildingY
        {
            get => _buildingY;
            set
            {
                _buildingY = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Orientation

        public BuildingOrientationMode BuildingOrientationMode
        {
            get => _buildingOrientationMode;
            set
            {
                _buildingOrientationMode = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(BuildingActualAngle));
                NotifyPropertyChanged(nameof(BuildingActualAngleDegrees));
                NotifyPropertyChanged(nameof(BuildingActualTNAngle));
                NotifyPropertyChanged(nameof(BuildingActualTNAngleDegrees));
            }
        }

        public double BuildingActualAngle
        {
            get
            {
                switch (BuildingOrientationMode)
                {
                    case BuildingOrientationMode.Free:

                        return BuildingAngle;

                    case BuildingOrientationMode.Aligned:

                        return GetAlignedAngle(BuildingAngle);

 
                    default:

                        throw new InvalidOperationException(
                            $"{nameof(BuildingOrientationMode)} " +
                            "is not supported"
                        );
                }
            }
        }

        public double BuildingActualAngleDegrees =>
            ConvertRadiansToDegrees(BuildingActualAngle);

        public double BuildingActualTNAngle =>
            BuildingActualAngle - TrueNorthAngle;

        public double BuildingActualTNAngleDegrees =>
            ConvertRadiansToDegrees(BuildingActualTNAngle);

        #endregion

        #region Free orientation (in true north space)

        public double BuildingMinTNAngle
        {
            get => _buildingMinTNAngle;
            set
            {
                BuildingTNAngle =
                    Math.Max(BuildingTNAngle, BuildingMinTNAngle);

                _buildingMinTNAngle = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(BuildingMinTNAngleDegrees));
                NotifyPropertyChanged(nameof(BuildingMinAngle));
                NotifyPropertyChanged(nameof(BuildingMinAngleDegrees));
            }
        }

        public double BuildingMinTNAngleDegrees
        {
            get => ConvertRadiansToDegrees(BuildingMinTNAngle);
            set => BuildingMinTNAngle = ConvertDegreesToRadians(value);
        }

        public double BuildingMaxTNAngle
        {
            get => _buildingMaxTNAngle;
            set
            {
                BuildingTNAngle =
                    Math.Min(BuildingTNAngle, BuildingMaxTNAngle);

                _buildingMaxTNAngle = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(BuildingMaxTNAngleDegrees));
                NotifyPropertyChanged(nameof(BuildingMaxAngle));
                NotifyPropertyChanged(nameof(BuildingMaxAngleDegrees));
            }
        }

        public double BuildingMaxTNAngleDegrees
        {
            get => ConvertRadiansToDegrees(BuildingMaxTNAngle);
            set => BuildingMaxTNAngle = ConvertDegreesToRadians(value);
        }

        public double BuildingTNAngle
        {
            get => _buildingTNAngle;
            set
            {
                if (
                    value < BuildingMinTNAngle ||
                    value > BuildingMaxTNAngle)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(BuildingTNAngle)} must be between " +
                        $"{nameof(BuildingMinTNAngle)} (inclusive) and " +
                        $"{nameof(BuildingMaxTNAngle)} (inclusive)"
                    );
                }

                _buildingTNAngle = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(BuildingTNAngleDegrees));
                NotifyPropertyChanged(nameof(BuildingAngle));
                NotifyPropertyChanged(nameof(BuildingAngleDegrees));
                NotifyPropertyChanged(nameof(BuildingActualAngle));
                NotifyPropertyChanged(nameof(BuildingActualAngleDegrees));
                NotifyPropertyChanged(nameof(BuildingActualTNAngle));
                NotifyPropertyChanged(nameof(BuildingActualTNAngleDegrees));
            }
        }

        public double BuildingTNAngleDegrees
        {
            get => ConvertRadiansToDegrees(BuildingTNAngle);
            set => BuildingTNAngle = ConvertDegreesToRadians(value);
        }

        #endregion

        #region Free orientation (in map space)

        public double BuildingMinAngle
        {
            get => BuildingMinTNAngle + TrueNorthAngle;
            set => BuildingMinTNAngle = value - TrueNorthAngle;
        }

        public double BuildingMinAngleDegrees
        {
            get => ConvertRadiansToDegrees(BuildingMinAngle);
            set => BuildingMinAngle = ConvertDegreesToRadians(value);
        }

        public double BuildingMaxAngle
        {
            get => BuildingMaxTNAngle + TrueNorthAngle;
            set => BuildingMaxTNAngle = value - TrueNorthAngle;
        }

        public double BuildingMaxAngleDegrees
        {
            get => ConvertRadiansToDegrees(BuildingMaxAngle);
            set => BuildingMaxAngle = ConvertDegreesToRadians(value);
        }

        public double BuildingAngle
        {
            get => BuildingTNAngle + TrueNorthAngle;
            set => BuildingTNAngle = value - TrueNorthAngle;
        }

        public double BuildingAngleDegrees
        {
            get => ConvertRadiansToDegrees(BuildingAngle);
            set => BuildingAngle = ConvertDegreesToRadians(value);
        }

        private double TrueNorthAngle =>
            Site?.SiteComponent.TrueNorthAngle ?? 0.0;

        #endregion

        #endregion

        #region Member variables

        private Building _building;

        private Site _site;

        private BuildingOrientationMode _buildingOrientationMode =
            DefaultBuildingOrientationMode;

        private double _buildingX = 0.0;
        private double _buildingY = 0.0;

        private double _buildingMinTNAngle = DefaultBuildingMinTNAngle;
        private double _buildingMaxTNAngle = DefaultBuildingMaxTNAngle;
        private double _buildingTNAngle = 0.0;

        #endregion

        #region Constants

        public const BuildingOrientationMode DefaultBuildingOrientationMode =
            BuildingOrientationMode.Aligned;

        public const double DefaultBuildingMinTNAngle = -Math.PI * 0.5;
        public const double DefaultBuildingMaxTNAngle = Math.PI * 0.5;

        #endregion
    }
}
