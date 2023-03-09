using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// Sample
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.SiteCreatorComponent"/> #2.
    /// </summary>
    [Serializable]
    public class Sample2SiteCreatorComponent : SampleSiteCreatorComponent
    {
        #region Constructors

        public Sample2SiteCreatorComponent() : base()
        {
            Name = Sample2SiteCreatorName;
        }

        protected Sample2SiteCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public override void UpdateSite(Site site)
        {
            site.SiteComponent.ClearSite();

            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(40.0, 0.0, 0.0),
                    new Point(40.0, 35.0, 0.0),
                    new Point(30.0, 35.0, 0.0),
                    new Point(15.0, 30.0, 0.0),
                    new Point(0.0, 30.0, 0.0)
                }
            );

            site.SiteComponent
                .SetBoundaryEdgeType(0, SiteEdgeType.Road);
            site.SiteComponent
                .SetBoundaryEdgeType(1, SiteEdgeType.Property);
            site.SiteComponent
                .SetBoundaryEdgeType(2, SiteEdgeType.Road);
            site.SiteComponent
                .SetBoundaryEdgeType(3, SiteEdgeType.Road);
            site.SiteComponent
                .SetBoundaryEdgeType(4, SiteEdgeType.Road);
            site.SiteComponent
                .SetBoundaryEdgeType(5, SiteEdgeType.Property);

            site.SiteComponent.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(-1.0, -7.0, 0.0),
                    new Point(45.0, -7.0, 0.0)
                )
            );
            site.SiteComponent.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(40.0, 42.0, 0.0),
                    new Point(30.0, 42.0, 0.0)
                )
            );
            site.SiteComponent.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(30.0, 42.0, 0.0),
                    new Point(15.0, 37.0, 0.0)
                )
            );
            site.SiteComponent.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(15.0, 37.0, 0.0),
                    new Point(0.0, 37.0, 0.0)
                )
            );

            site.SiteComponent.TrueNorthAngle = TrueNorthAngle;
        }

        #endregion

        #region Constants

        public const string Sample2SiteCreatorName = "Site 2";

        #endregion
    }
}
