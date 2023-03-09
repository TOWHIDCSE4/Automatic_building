using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// Sample
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.SiteCreatorComponent"/> #3.
    /// </summary>
    [Serializable]
    public class Sample3SiteCreatorComponent : SampleSiteCreatorComponent
    {
        #region Constructors

        public Sample3SiteCreatorComponent() : base()
        {
            Name = Sample3SiteCreatorName;
        }

        protected Sample3SiteCreatorComponent(
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
                    new Point(40.0, 25.0, 0.0),
                    new Point(0.0, 25.0, 0.0)
                }
            );

            site.SiteComponent
                .SetBoundaryEdgeType(0, SiteEdgeType.Road);
            site.SiteComponent
                .SetBoundaryEdgeType(1, SiteEdgeType.Road);
            site.SiteComponent
                .SetBoundaryEdgeType(2, SiteEdgeType.Property);
            site.SiteComponent
                .SetBoundaryEdgeType(3, SiteEdgeType.Property);

            site.SiteComponent.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(-2.0, -5.0, 0.0),
                    new Point(45.0, -5.0, 0.0)
                )
            );
            site.SiteComponent.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(45.0, -5.0, 0.0),
                    new Point(45.0, 27.0, 0.0)
                )
            );

            site.SiteComponent.TrueNorthAngle = TrueNorthAngle;
        }

        #endregion

        #region Constants

        public const string Sample3SiteCreatorName = "Site 3";

        #endregion
    }
}
