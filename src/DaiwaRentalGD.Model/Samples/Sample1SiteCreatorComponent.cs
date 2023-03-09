using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// Sample
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.SiteCreatorComponent"/> #1.
    /// </summary>
    [Serializable]
    public class Sample1SiteCreatorComponent : SampleSiteCreatorComponent
    {
        #region Constructors

        public Sample1SiteCreatorComponent() : base()
        {
            Name = Sample1SiteCreatorName;
        }

        protected Sample1SiteCreatorComponent(
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
                    new Point(20.0, 0.0, 0.0),
                    new Point(20.0, 35.0, 0.0),
                    new Point(5.0, 30.0, 0.0)
                }
            );

            site.SiteComponent
                .SetBoundaryEdgeType(0, SiteEdgeType.Road);
            site.SiteComponent
                .SetBoundaryEdgeType(1, SiteEdgeType.Road);
            site.SiteComponent
                .SetBoundaryEdgeType(2, SiteEdgeType.Road);
            site.SiteComponent
                .SetBoundaryEdgeType(3, SiteEdgeType.Property);

            site.SiteComponent.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(-1.0, -7.0, 0.0),
                    new Point(27.0, -7.0, 0.0)
                )
            );
            site.SiteComponent.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(27.0, -7.0, 0.0),
                    new Point(28.0, 47.0, 0.0)
                )
            );
            site.SiteComponent.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(28.0, 47.0, 0.0),
                    new Point(-1.0, 37.0, 0.0)
                )
            );

            site.SiteComponent.TrueNorthAngle = TrueNorthAngle;
        }

        #endregion

        #region Constants

        public const string Sample1SiteCreatorName = "Site 1";

        #endregion
    }
}
