using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A comparer for <see cref="WalkwayPathNode"/>.
    /// </summary>
    [Serializable]
    public class WalkwayPathNodeComparer :
        IComparer<WalkwayPathNode>, ISerializable
    {
        #region Constructors

        public WalkwayPathNodeComparer()
        { }

        protected WalkwayPathNodeComparer(
            SerializationInfo info, StreamingContext context
        )
        { }

        #endregion

        #region Methods

        public int Compare(WalkwayPathNode wpn0, WalkwayPathNode wpn1)
        {
            return wpn0.FScore.CompareTo(wpn1.FScore);
        }

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        { }

        #endregion
    }
}
