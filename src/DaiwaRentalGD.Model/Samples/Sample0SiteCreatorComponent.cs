using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// Sample
    /// <see cref="DaiwaRentalGD.Model.SiteDesign.SiteCreatorComponent"/> #0.
    /// </summary>
    [Serializable]
    public class Sample0SiteCreatorComponent : SampleSiteCreatorComponent
    {
        #region Constructors

        public Sample0SiteCreatorComponent() : base()
        {
            Name = Sample0SiteCreatorName;
        }

        protected Sample0SiteCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public override void UpdateSite(Site site)
        {
            site.SiteComponent.ClearSite();

            /*
             * site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(40.0, 0.0, 0.0),
                    new Point(40.0, 35.0, 0.0),
                    new Point(16.0, 30.0, 0.0),
                    new Point(15.0, 20.0, 0.0),
                    new Point(0.0, 20.0, 0.0)
                }
            );
            */
            site.SiteComponent.Boundary = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(32.0, 0.0, 0.0),
                    new Point(32.0, 24.0, 0.0),
                    new Point(10.0, 23.0, 0.0),
                    new Point(9.0, 21.0, 0.0),
                    new Point(1.0, 21.0, 0.0)
                }
            );

            site.SiteComponent
                .SetBoundaryEdgeType(0, SiteEdgeType.Road);
            site.SiteComponent
                .SetBoundaryEdgeType(1, SiteEdgeType.Property);
            site.SiteComponent
                .SetBoundaryEdgeType(2, SiteEdgeType.Property);
            site.SiteComponent
                .SetBoundaryEdgeType(3, SiteEdgeType.Property);
            site.SiteComponent
                .SetBoundaryEdgeType(4, SiteEdgeType.Property);
            site.SiteComponent
                .SetBoundaryEdgeType(5, SiteEdgeType.Property);

            /*
                        site.SiteComponent.AddOppositeRoadEdge(
                            new LineSegment(
                                new Point(-1.0, -7.0, 0.0),
                                new Point(40.0, -7.0, 0.0)
                            )
                        );
            */
            site.SiteComponent.AddOppositeRoadEdge(
                new LineSegment(
                    new Point(-1.0, -7.0, 0.0),
                    new Point(32.0, -7.0, 0.0)
                )
            );

            site.SiteComponent.TrueNorthAngle = TrueNorthAngle;
        }

        #endregion

        #region Constants

        public const string Sample0SiteCreatorName = "Site 0";

        #endregion
    }
}
