using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.Zoning;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> for
    /// <see cref="DaiwaRentalGD.Model.Zoning.SlantPlanes"/>.
    /// </summary>
    public class SlantPlanesModel3DBuilder
    {
        #region Constructors

        public SlantPlanesModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel(SlantPlanes slantPlanes)
        {
            var planesModel = CreateAllPlanesModel(slantPlanes);
            var violationsModel = CreateAllViolationsModel(slantPlanes);

            var model = new Model3DGroup
            {
                Children =
                {
                    planesModel,
                    violationsModel
                }
            };

            return model;
        }

        public Model3D CreateAllPlanesModel(SlantPlanes slantPlanes)
        {
            var adjacentSiteSlantPlanesModel =
                CreateAdjacentSiteSlantPlanesModel(slantPlanes);

            var roadSlantPlanesModel =
                CreateRoadSlantPlanesModel(slantPlanes);

            var northSlantPlanesModel =
                CreateNorthSlantPlanesModel(slantPlanes);

            var absoluteHeightPlanesModel =
              CreateAbsoluteHeightPlanesModel(slantPlanes);

            var planesModel = new Model3DGroup
            {
                Children =
                {
                    adjacentSiteSlantPlanesModel,
                    roadSlantPlanesModel,
                    northSlantPlanesModel,
                    absoluteHeightPlanesModel
                }
            };

            return planesModel;
        }

        public Model3D CreateAllViolationsModel(SlantPlanes slantPlanes)
        {
            var adjacentSiteSlantPlanesViolationsModel =
                CreateAdjacentSiteSlantPlanesViolationsModel(slantPlanes);

            var roadSlantPlanesViolationsModel =
                CreateRoadSlantPlanesViolationsModel(slantPlanes);

            var northSlantPlanesViolationsModel =
                CreateNorthSlantPlanesViolationsModel(slantPlanes);

            var absoluteHeightPlanesViolationsModel =
               CreateAbsoluteHeightPlanesViolationsModel(slantPlanes);

            var violationsModel = new Model3DGroup
            {
                Children =
                {
                    adjacentSiteSlantPlanesViolationsModel,
                    roadSlantPlanesViolationsModel,
                    northSlantPlanesViolationsModel,
                    absoluteHeightPlanesViolationsModel
                }
            };

            return violationsModel;
        }

        public Model3D CreateAdjacentSiteSlantPlanesModel(
            SlantPlanes slantPlanes
        )
        {
            if (!DoesShowAdjacentSiteSlantPlanes)
            {
                return new Model3DGroup();
            }

            AdjacentSiteSlantPlanesComponent asspc =
                slantPlanes.AdjacentSiteSlantPlanesComponent;

            IReadOnlyList<Polygon> planes =
                asspc.PlanesByEdge.Values
                .SelectMany(edgePlanes => edgePlanes)
                .ToList();

            var adjacentSiteSlantPlanesModel = CreatePlanesModel(
                planes,
                AdjacentSiteSlantPlanesFillMaterial,
                AdjacentSiteSlantPlanesWireframeMaterial
            );

            return adjacentSiteSlantPlanesModel;
        }

        public Model3D CreateAdjacentSiteSlantPlanesViolationsModel(
            SlantPlanes slantPlanes
        )
        {
            if (!DoesShowAdjacentSiteSlantPlanesViolations)
            {
                return new Model3DGroup();
            }

            AdjacentSiteSlantPlanesComponent asspc =
                slantPlanes.AdjacentSiteSlantPlanesComponent;

            var points =
                asspc.Violations
                .Select(violation => violation.Point)
                .ToList();

            var adjacentSiteSlantPlanesViolationsModel =
                CreateViolationsModel(
                    points,
                    AdjacentSiteSlantPlanesViolationPointMaterial
                );

            return adjacentSiteSlantPlanesViolationsModel;
        }

        public Model3D CreateRoadSlantPlanesModel(
            SlantPlanes slantPlanes
        )
        {
            if (!DoesShowRoadSlantPlanes)
            {
                return new Model3DGroup();
            }

            RoadSlantPlanesComponent rspc =
                slantPlanes.RoadSlantPlanesComponent;

            IReadOnlyList<Polygon> planes =
                rspc.PlanesByEdge.Values
                .SelectMany(edgePlanes => edgePlanes)
                .ToList();

            var roadSlantPlanesModel = CreatePlanesModel(
                planes,
                RoadSlantPlanesFillMaterial,
                RoadSlantPlanesWireframeMaterial
            );

            return roadSlantPlanesModel;
        }

        public Model3D CreateRoadSlantPlanesViolationsModel(
            SlantPlanes slantPlanes
        )
        {
            if (!DoesShowRoadSlantPlanesViolations)
            {
                return new Model3DGroup();
            }

            RoadSlantPlanesComponent rspc =
                slantPlanes.RoadSlantPlanesComponent;

            var points =
                rspc.Violations
                .Select(violation => violation.Point)
                .ToList();

            var roadSlantPlanesViolationsModel = CreateViolationsModel(
                points, RoadSlantPlanesViolationPointMaterial
            );
            return roadSlantPlanesViolationsModel;
        }

        public Model3D CreateNorthSlantPlanesModel(
            SlantPlanes slantPlanes
        )
        {
            if (!DoesShowNorthSlantPlanes)
            {
                return new Model3DGroup();
            }

            NorthSlantPlanesComponent rspc =
                slantPlanes.NorthSlantPlanesComponent;

            IReadOnlyList<Polygon> planes =
                rspc.PlanesByEdge.Values
                .SelectMany(edgePlanes => edgePlanes)
                .ToList();

            var northSlantPlanesModel = CreatePlanesModel(
                planes,
                NorthSlantPlanesFillMaterial,
                NorthSlantPlanesWireframeMaterial
            );

            return northSlantPlanesModel;
        }

        public Model3D CreateNorthSlantPlanesViolationsModel(
            SlantPlanes slantPlanes
        )
        {
            if (!DoesShowNorthSlantPlanesViolations)
            {
                return new Model3DGroup();
            }

            NorthSlantPlanesComponent rspc =
                slantPlanes.NorthSlantPlanesComponent;

            var violations = rspc.Violations;
            var points =
                violations
                .Select(violation => violation.Point)
                .ToList();

            var northSlantPlanesViolationsModel = CreateViolationsModel(
                points, NorthSlantPlanesViolationPointMaterial
            );
            return northSlantPlanesViolationsModel;
        }


        public Model3D CreateAbsoluteHeightPlanesModel(
         SlantPlanes slantPlanes
     )
        {
            if (!DoesShowAbsoluteHeightPlanes)
            {
                return new Model3DGroup();
            }

            AbsoluteHeightSlantPlanesComponent aspc =
            slantPlanes.AbsoluteHeightSlantPlanesComponent;

            IReadOnlyList<Polygon> planes =
                aspc.PlanesByEdge.Values
                .SelectMany(edgePlanes => edgePlanes)
                .ToList();

            var absoluteHeightPlanesModel = CreatePlanesModel(
                planes,
                AbsoluteHeightPlanesFillMaterial,
                AbsoluteHeightPlanesWireframeMaterial
            );

            return absoluteHeightPlanesModel;
        }

        public Model3D CreateAbsoluteHeightPlanesViolationsModel(
            SlantPlanes slantPlanes
        )
        {
            if (!DoesShowAbsoluteHeightPlanesViolations)
            {
                return new Model3DGroup();
            }

            AbsoluteHeightSlantPlanesComponent aspc =
                slantPlanes.AbsoluteHeightSlantPlanesComponent;

            var points =
                aspc.Violations
                .Select(violation => violation.Point)
                .ToList();

            var absoluteHeightPlanesViolationsModel = CreateViolationsModel(
                points, AbsoluteHeightPlanesViolationPointMaterial
            );
            return absoluteHeightPlanesViolationsModel;
        }

        public Model3D CreatePlanesModel(
            IReadOnlyList<Polygon> planes,
            Material fillMaterial, Material wireframeMaterial
        )
        {
            var planesModel = new Model3DGroup();

            foreach (Polygon plane in planes)
            {
                var planeMesh = GeometryUtils.CovnertToMesh(plane);

                Geometry3D planeFillGeometry =
                    Viewport3DUtils.ConvertToGeometry3D(planeMesh);

                Model3D planeFillModel = new GeometryModel3D
                {
                    Geometry = planeFillGeometry,
                    Material = fillMaterial,
                };

                Geometry3D planeWireframeGeometry =
                    Viewport3DUtils.CreateWireframeGeometry3D(
                        planeMesh, SlantPlaneWireframeThickness
                    );

                Model3D planeWireframeModel = new GeometryModel3D
                {
                    Geometry = planeWireframeGeometry,
                    Material = wireframeMaterial
                };

                planesModel.Children.Add(planeFillModel);
                planesModel.Children.Add(planeWireframeModel);
            }

            return planesModel;
        }

        public Model3D CreateViolationsModel(
            IReadOnlyList<Point> points, Material pointMaterial
        )
        {
            var violationsModel = new Model3DGroup();

            foreach (var point in points)
            {
                Mesh pointMesh = GeometryUtils.CreateBoxMesh(
                    SlantPlaneViolationPointSize,
                    SlantPlaneViolationPointSize,
                    SlantPlaneViolationPointSize,
                    true
                );

                Geometry3D pointGeometry =
                    Viewport3DUtils.ConvertToGeometry3D(pointMesh);

                Model3D pointModel = new GeometryModel3D
                {
                    Geometry = pointGeometry,
                    Material = pointMaterial,
                    Transform = new TranslateTransform3D
                    {
                        OffsetX = point.X,
                        OffsetY = point.Y,
                        OffsetZ = point.Z
                    }
                };

                violationsModel.Children.Add(pointModel);
            }

            return violationsModel;
        }

        #endregion

        #region Properties

        public bool DoesShowAdjacentSiteSlantPlanes { get; set; } =
            DefaultDoesShowAdjacentSiteSlantPlanes;

        public bool DoesShowAdjacentSiteSlantPlanesViolations { get; set; } =
            DefaultDoesShowAdjacentSiteSlantPlanesViolations;

        public bool DoesShowRoadSlantPlanes { get; set; } =
            DefaultDoesShowRoadSlantPlanes;

        public bool DoesShowRoadSlantPlanesViolations { get; set; } =
            DefaultDoesShowRoadSlantPlanesViolations;

        public bool DoesShowNorthSlantPlanes { get; set; } =
            DefaultDoesShowNorthSlantPlanes;

        public bool DoesShowNorthSlantPlanesViolations { get; set; } =
            DefaultDoesShowNorthSlantPlanesViolations;

        public bool DoesShowAbsoluteHeightPlanes { get; set; } =
       DefaultDoesShowRoadSlantPlanes;

        public bool DoesShowAbsoluteHeightPlanesViolations { get; set; } =
            DefaultDoesShowRoadSlantPlanesViolations;

        public double SlantPlaneWireframeThickness { get; set; } =
            DefaultSlantPlanesWireframeThickness;

        public double SlantPlaneViolationPointSize { get; set; } =
            DefaultSlantPlaneViolationPointSize;

        public Brush AdjacentSiteSlantPlanesFillBrush { get; set; } =
            DefaultAdjacentSiteSlantPlanesFillBrush;

        public Brush AdjacentSiteSlantPlanesWireframeBrush { get; set; } =
            DefaultAdjacentSiteSlantPlanesWireframeBrush;

        public Brush AdjacentSiteSlantPlanesViolationPointBrush
        { get; set; } = DefaultAdjacentSiteSlantPlanesViolationPointBrush;

        public Brush RoadSlantPlanesFillBrush { get; set; } =
            DefaultRoadSlantPlanesFillBrush;

        public Brush RoadSlantPlanesWireframeBrush { get; set; } =
            DefaultRoadSlantPlanesWireframeBrush;

        public Brush RoadSlantPlanesViolationPointBrush
        { get; set; } = DefaultRoadSlantPlanesViolationPointBrush;

        public Brush NorthSlantPlanesFillBrush { get; set; } =
            DefaultNorthSlantPlanesFillBrush;

        public Brush NorthSlantPlanesWireframeBrush { get; set; } =
            DefaultNorthSlantPlanesWireframeBrush;

        public Brush NorthSlantPlanesViolationPointBrush
        { get; set; } = DefaultNorthSlantPlanesViolationPointBrush;

        public Brush AbsoluteHeightPlanesFillBrush { get; set; } =
          DefaultAbsoluteHeightPlanesFillBrush;

        public Brush AbsoluteHeightPlanesWireframeBrush { get; set; } =
            DefaultAbsoluteHeightPlanesWireframeBrush;

        public Brush AbsoluteHeightPlanesViolationPointBrush
        { get; set; } = DefaultAbsoluteHeightPlanesViolationPointBrush;

        public Material AdjacentSiteSlantPlanesFillMaterial => null;

        public Material AdjacentSiteSlantPlanesWireframeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(
                AdjacentSiteSlantPlanesWireframeBrush
            );

        public Material AdjacentSiteSlantPlanesViolationPointMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(
                AdjacentSiteSlantPlanesViolationPointBrush
            );

        public Material RoadSlantPlanesFillMaterial => null;

        public Material RoadSlantPlanesWireframeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(
                RoadSlantPlanesWireframeBrush
            );

        public Material RoadSlantPlanesViolationPointMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(
                RoadSlantPlanesViolationPointBrush
            );

        public Material NorthSlantPlanesFillMaterial => null;

        public Material NorthSlantPlanesWireframeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(
                NorthSlantPlanesWireframeBrush
            );

        public Material NorthSlantPlanesViolationPointMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(
                NorthSlantPlanesViolationPointBrush
            );

        public Material AbsoluteHeightPlanesFillMaterial => null;

        public Material AbsoluteHeightPlanesWireframeMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(
                AbsoluteHeightPlanesWireframeBrush
            );

        public Material AbsoluteHeightPlanesViolationPointMaterial =>
            Viewport3DUtils.CreateEmissiveMaterial(
                AbsoluteHeightPlanesViolationPointBrush
            );

        #endregion

        #region Constants

        public const bool DefaultDoesShowAdjacentSiteSlantPlanes = false;
        public const bool DefaultDoesShowAdjacentSiteSlantPlanesViolations =
            false;

        public const bool DefaultDoesShowRoadSlantPlanes = false;
        public const bool DefaultDoesShowRoadSlantPlanesViolations = false;

        public const bool DefaultDoesShowNorthSlantPlanes = false;
        public const bool DefaultDoesShowNorthSlantPlanesViolations = false;

        public const double DefaultSlantPlanesWireframeThickness = 0.05;
        public const double DefaultSlantPlaneViolationPointSize = 0.2;

        public static readonly Brush
            DefaultAdjacentSiteSlantPlanesFillBrush =
            Brushes.Transparent;

        public static readonly Brush
            DefaultAdjacentSiteSlantPlanesWireframeBrush =
            Brushes.Orange;

        public static readonly Brush
            DefaultAdjacentSiteSlantPlanesViolationPointBrush =
            Brushes.Orange;

        public static readonly Brush
            DefaultRoadSlantPlanesFillBrush =
            Brushes.Transparent;

        public static readonly Brush
            DefaultRoadSlantPlanesWireframeBrush =
            Brushes.LightGreen;

        public static readonly Brush
            DefaultRoadSlantPlanesViolationPointBrush =
            Brushes.LightGreen;

        public static readonly Brush
            DefaultNorthSlantPlanesFillBrush =
            Brushes.Transparent;

        public static readonly Brush
            DefaultNorthSlantPlanesWireframeBrush =
            Brushes.Purple;

        public static readonly Brush
            DefaultNorthSlantPlanesViolationPointBrush =
            Brushes.Purple;

        public static readonly Brush
       DefaultAbsoluteHeightPlanesFillBrush =
       Brushes.Transparent;

        public static readonly Brush
            DefaultAbsoluteHeightPlanesWireframeBrush =
            Brushes.Red;

        public static readonly Brush
            DefaultAbsoluteHeightPlanesViolationPointBrush =
            Brushes.Red;

        #endregion
    }
}
