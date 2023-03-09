using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using MathNet.Numerics.LinearAlgebra;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A vector field on a site plane.
    /// </summary>
    [Serializable]
    public class SiteVectorField : ISerializable
    {
        #region Constructors

        public SiteVectorField()
        { }

        protected SiteVectorField(
            SerializationInfo info, StreamingContext context
        )
        { }

        #endregion

        #region Methods

        public Tuple<Vector<double>, Vector<double>>
            GetVectorPair(Site site, Point point)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            var boundary = site.SiteComponent.Boundary;

            var closestPointInfo = boundary.GetClosestPointOnEdge(point);
            var closestEdgeIndex = closestPointInfo.Item1;
            var closestEdge = boundary.GetEdge(closestEdgeIndex);

            var closestEdgeDir = closestEdge.Direction;
            var closestEdgeNormal = boundary.GetEdgeNormal(closestEdgeIndex);

            return new Tuple<Vector<double>, Vector<double>>(
                closestEdgeDir, closestEdgeNormal
            );
        }

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        { }

        #endregion
    }
}
