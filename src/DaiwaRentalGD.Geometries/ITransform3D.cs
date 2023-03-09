using MathNet.Numerics.LinearAlgebra;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Interface for types that represent a 3D transform capable of
    /// transforming 3D vectors.
    /// </summary>
    public interface ITransform3D
    {
        #region Methods

        /// <summary>
        /// Transforms a 3D vector that represents a position.
        /// Does not change the provided 3D vector.
        /// </summary>
        /// 
        /// <param name="position3D">
        /// A 3D vector that represents a position.
        /// </param>
        /// 
        /// <returns>
        /// The transformed position.
        /// </returns>
        Vector<double> Transform(Vector<double> position3D);

        /// <summary>
        /// Transforms a 3D vector that represents a direction.
        /// Does not change the provided 3D vector.
        /// </summary>
        /// 
        /// <param name="dir3D">
        /// A 3D vector that represents a direction.
        /// </param>
        /// 
        /// <returns>
        /// The transformed direction.
        /// </returns>
        Vector<double> TransformDir(Vector<double> dir3D);

        /// <summary>
        /// The homogeneous matrix of this transform.
        /// </summary>
        Matrix<double> Matrix { get; }

        #endregion
    }
}
