namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Interface for a type that has a bounding box.
    /// </summary>
    public interface IBBox
    {
        #region Methods

        /// <summary>
        /// Gets the bounding box of this object.
        /// </summary>
        /// 
        /// <returns>
        /// The bounding box of this object.
        /// </returns>
        BBox GetBBox();

        #endregion
    }
}
