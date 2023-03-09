using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Represents a 3D transform consisting of
    /// scale, rotation and translation (TRS),
    /// in this specified order.
    /// </summary>
    [Serializable]
    public class TrsTransform3D :
        ITransform3D, IEquatable<TrsTransform3D>, ISerializable
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="TrsTransform3D"/>.
        /// </summary>
        /// <param name="tx">Translation in X Dimension.</param>
        /// <param name="ty">Translation in Y Dimension.</param>
        /// <param name="tz">Translation in Z Dimension.</param>
        /// <param name="rx">Rotation around X Axis.</param>
        /// <param name="ry">Rotation around Y Axis.</param>
        /// <param name="rz">Rotation around Z Axis.</param>
        /// <param name="sx">Scaling in X Dimension.</param>
        /// <param name="sy">Scaling in Y Dimension.</param>
        /// <param name="sz">Scaling in Z Dimension.</param>
        public TrsTransform3D(
            double tx, double ty, double tz,
            double rx, double ry, double rz,
            double sx, double sy, double sz
        )
        {
            Tx = tx;
            Ty = ty;
            Tz = tz;

            Rx = rx;
            Ry = ry;
            Rz = rz;

            Sx = sx;
            Sy = sy;
            Sz = sz;
        }

        /// <summary>
        /// Creates an identity transform.
        /// </summary>
        public TrsTransform3D() : this(
            0.0, 0.0, 0.0,
            0.0, 0.0, 0.0,
            1.0, 1.0, 1.0
        )
        { }

        /// <summary>
        /// Creates a copy of a given <see cref="TrsTransform3D"/>.
        /// </summary>
        /// 
        /// <param name="transform">
        /// The transform to be copied.
        /// </param>
        public TrsTransform3D(TrsTransform3D transform) : this(
            transform.Tx, transform.Ty, transform.Tz,
            transform.Rx, transform.Ry, transform.Rz,
            transform.Sx, transform.Sy, transform.Sz
        )
        { }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// 
        /// <param name="info">
        /// Contains data of a serialized <see cref="TrsTransform3D"/>
        /// instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected TrsTransform3D(
            SerializationInfo info, StreamingContext context
        )
        {
            Tx = info.GetDouble(nameof(Tx));
            Ty = info.GetDouble(nameof(Ty));
            Tz = info.GetDouble(nameof(Tz));

            Rx = info.GetDouble(nameof(Rx));
            Ry = info.GetDouble(nameof(Ry));
            Rz = info.GetDouble(nameof(Rz));

            Sx = info.GetDouble(nameof(Sx));
            Sy = info.GetDouble(nameof(Sy));
            Sz = info.GetDouble(nameof(Sz));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the homogeneous matrix for a 3D translation.
        /// </summary>
        /// 
        /// <param name="tx">Translation in X Dimension.</param>
        /// <param name="ty">Translation in Y Dimension.</param>
        /// <param name="tz">Translation in Z Dimension.</param>
        /// 
        /// <returns>
        /// The homogeneous matrix for the 3D translation.
        /// </returns>
        public static Matrix<double> GetTranslateMatrix(
            double tx, double ty, double tz
        )
        {
            Matrix<double> tMatrix = new DenseMatrix(
                4, 4,
                new[]
                {
                    1.0, 0.0, 0.0, 0.0,
                    0.0, 1.0, 0.0, 0.0,
                    0.0, 0.0, 1.0, 0.0,
                    tx, ty, tz, 1.0
                }
            );

            return tMatrix;
        }

        /// <summary>
        /// Gets the homogeneous matrix for a 3D rotation.
        /// </summary>
        /// 
        /// <param name="rx">Rotation around X Axis.</param>
        /// <param name="ry">Rotation around Y Axis.</param>
        /// <param name="rz">Rotation around Z Axis.</param>
        /// 
        /// <returns>
        /// The homogeneous matrix for the 3D rotation.
        /// </returns>
        public static Matrix<double> GetRotationMatrix(
            double rx, double ry, double rz
        )
        {
            Matrix<double> rxMatrix = new DenseMatrix(
                4, 4,
                new[]
                {
                    1.0, 0.0, 0.0, 0.0,
                    0.0, Math.Cos(rx), Math.Sin(rx), 0.0,
                    0.0, -Math.Sin(rx), Math.Cos(rx), 0.0,
                    0.0, 0.0, 0.0, 1.0
                }
            );
            Matrix<double> ryMatrix = new DenseMatrix(
                4, 4,
                new[]
                {
                    Math.Cos(ry), 0.0, Math.Sin(ry), 0.0,
                    0.0, 1.0, 0.0, 0.0,
                    -Math.Sin(ry), 0.0, Math.Cos(ry), 0.0,
                    0.0, 0.0, 0.0, 1.0
                }
            );
            Matrix<double> rzMatrix = new DenseMatrix(
                4, 4,
                new[]
                {
                    Math.Cos(rz), Math.Sin(rz), 0.0, 0.0,
                    -Math.Sin(rz), Math.Cos(rz), 0.0, 0.0,
                    0.0, 0.0, 1.0, 0.0,
                    0.0, 0.0, 0.0, 1.0
                }
            );

            // Rotates around Z-Axis first, then Y-Axis, then X-Axis.
            Matrix<double> rMatrix = rxMatrix * ryMatrix * rzMatrix;
            return rMatrix;
        }

        /// <summary>
        /// Gets the homogeneous matrix for a 3D scaling.
        /// </summary>
        /// 
        /// <param name="sx">Scaling in X Dimension.</param>
        /// <param name="sy">Scaling in Y Dimension.</param>
        /// <param name="sz">Scaling in Z Dimension.</param>
        /// 
        /// <returns>
        /// The homogeneous matrix for the 3D scaling.
        /// </returns>
        public static Matrix<double> GetScaleMatrix(
            double sx, double sy, double sz
        )
        {
            Matrix<double> sMatrix = new DenseMatrix(
                4, 4,
                new[]
                {
                    sx, 0.0, 0.0, 0.0,
                    0.0, sy, 0.0, 0.0,
                    0.0, 0.0, sz, 0.0,
                    0.0, 0.0, 0.0, 1.0
                }
            );

            return sMatrix;
        }

        /// <summary>
        /// Gets a vector representing the translation part of
        /// this <see cref="TrsTransform3D"/>.
        /// </summary>
        /// 
        /// <returns>
        /// A vector representing the translation part of
        /// this <see cref="TrsTransform3D"/>.
        /// </returns>
        public Vector<double> GetTranslate()
        {
            return new DenseVector(new[] { Tx, Ty, Tz });
        }

        /// <summary>
        /// Sets the translation part of this <see cref="TrsTransform3D"/>.
        /// </summary>
        /// 
        /// <param name="translate">
        /// A vector representing the translation part to be set.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="translate"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="translate"/> is not a 3D vector.
        /// </exception>
        public void SetTranslate(Vector<double> translate)
        {
            if (translate == null)
            {
                throw new ArgumentNullException(nameof(translate));
            }

            if (translate.Count != 3)
            {
                throw new ArgumentException(
                    $"{nameof(translate)} must be a 3D vector",
                    nameof(translate)
                );
            }

            Tx = translate[0];
            Ty = translate[1];
            Tz = translate[2];
        }

        /// <summary>
        /// Sets the translation part of this <see cref="TrsTransform3D"/>
        /// using a local translation.
        /// </summary>
        /// 
        /// <param name="localTranslate">
        /// A vector representing a local translation to be used to
        /// set the translation part of this <see cref="TrsTransform3D"/>.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="localTranslate"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="localTranslate"/> is not a 3D vector.
        /// </exception>
        public void SetTranslateLocal(Vector<double> localTranslate)
        {
            if (localTranslate == null)
            {
                throw new ArgumentNullException(nameof(localTranslate));
            }

            if (localTranslate.Count != 3)
            {
                throw new ArgumentException(
                    $"{nameof(localTranslate)} must be a 3D vector",
                    nameof(localTranslate)
                );
            }

            var translate = Transform(localTranslate);

            Tx = translate[0];
            Ty = translate[1];
            Tz = translate[2];
        }

        /// <summary>
        /// Sets the translation part of this <see cref="TrsTransform3D"/>
        /// using a local translation.
        /// </summary>
        /// 
        /// <param name="localTx">Translation in local X Dimension.</param>
        /// <param name="localTy">Translation in local Y Dimension.</param>
        /// <param name="localTz">Translation in local Z Dimension.</param>
        public void SetTranslateLocal(
            double localTx, double localTy, double localTz
        )
        {
            var localTranslate =
                new DenseVector(new[] { localTx, localTy, localTz });

            SetTranslateLocal(localTranslate);
        }

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
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="position3D"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="position3D"/> is not a 3D vector.
        /// </exception>
        public Vector<double> Transform(Vector<double> position3D)
        {
            if (position3D == null)
            {
                throw new ArgumentNullException(nameof(position3D));
            }

            if (position3D.Count != 3)
            {
                throw new ArgumentException(
                    $"{nameof(position3D)} must be a 3D vector",
                    nameof(position3D)
                );
            }

            Vector<double> position4D = new DenseVector(4);
            position3D.CopySubVectorTo(position4D, 0, 0, 3);
            position4D[3] = 1.0;

            position4D = Matrix * position4D;

            Vector<double> transformedPosition3D = position4D.SubVector(0, 3);
            return transformedPosition3D;
        }

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
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dir3D"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="dir3D"/> is not a 3D vector.
        /// </exception>
        public Vector<double> TransformDir(Vector<double> dir3D)
        {
            if (dir3D == null)
            {
                throw new ArgumentNullException(nameof(dir3D));
            }

            if (dir3D.Count != 3)
            {
                throw new ArgumentException(
                    $"{nameof(dir3D)} must be a 3D vector",
                    nameof(dir3D)
                );
            }

            Vector<double> dir4D = new DenseVector(4);
            dir3D.CopySubVectorTo(dir4D, 0, 0, 3);
            dir4D[3] = 0.0;

            dir4D = Matrix * dir4D;

            Vector<double> transformedDir3D = dir4D.SubVector(0, 3);
            return transformedDir3D;
        }

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(Tx), Tx);
            info.AddValue(nameof(Ty), Ty);
            info.AddValue(nameof(Tz), Tz);

            info.AddValue(nameof(Rx), Rx);
            info.AddValue(nameof(Ry), Ry);
            info.AddValue(nameof(Rz), Rz);

            info.AddValue(nameof(Sx), Sx);
            info.AddValue(nameof(Sy), Sy);
            info.AddValue(nameof(Sz), Sz);
        }

        /// <summary>
        /// Determines whether this <see cref="TrsTransform3D"/> and
        /// a given object have the same value.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The object to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is
        /// a <see cref="TrsTransform3D"/> and has the same
        /// translation, rotation and scaling
        /// as this <see cref="TrsTransform3D"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj) =>
            Equals(obj as TrsTransform3D);

        /// <summary>
        /// Determines whether this <see cref="TrsTransform3D"/> and another
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="other">
        /// The other <see cref="TrsTransform3D"/> to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="other"/>
        /// has the same translation, rotation and scaling
        /// as this <see cref="TrsTransform3D"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool Equals(TrsTransform3D other)
        {
            return ReferenceEquals(this, other) || (
                other != null &&
                Tx == other.Tx &&
                Ty == other.Ty &&
                Tz == other.Tz &&
                Rx == other.Rx &&
                Ry == other.Ry &&
                Rz == other.Rz &&
                Sx == other.Sx &&
                Sy == other.Sy &&
                Sz == other.Sz
            );
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1755184145;

            hashCode = hashCode * -1521134295 + Tx.GetHashCode();
            hashCode = hashCode * -1521134295 + Ty.GetHashCode();
            hashCode = hashCode * -1521134295 + Tz.GetHashCode();
            hashCode = hashCode * -1521134295 + Rx.GetHashCode();
            hashCode = hashCode * -1521134295 + Ry.GetHashCode();
            hashCode = hashCode * -1521134295 + Rz.GetHashCode();
            hashCode = hashCode * -1521134295 + Sx.GetHashCode();
            hashCode = hashCode * -1521134295 + Sy.GetHashCode();
            hashCode = hashCode * -1521134295 + Sz.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// Determines whether two <see cref="TrsTransform3D"/>s
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="TrsTransform3D"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="TrsTransform3D"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have the same translation, rotation
        /// and scaling. Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(
            TrsTransform3D left, TrsTransform3D right
        ) => EqualityComparer<TrsTransform3D>.Default.Equals(left, right);

        /// <summary>
        /// Determines whether two <see cref="TrsTransform3D"/>s
        /// have different values.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="TrsTransform3D"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="TrsTransform3D"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> are different in translation, rotation or
        /// scaling. Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(
            TrsTransform3D left, TrsTransform3D right
        ) => !(left == right);

        #endregion

        #region Properties

        /// <summary>
        /// Translation in X Dimension.
        /// </summary>
        public double Tx { get; set; }

        /// <summary>
        /// Translation in Y Dimension.
        /// </summary>
        public double Ty { get; set; }

        /// <summary>
        /// Translation in Z Dimension.
        /// </summary>
        public double Tz { get; set; }

        /// <summary>
        /// Rotation around X Axis.
        /// </summary>
        public double Rx { get; set; }

        /// <summary>
        /// Rotation around Y Axis.
        /// </summary>
        public double Ry { get; set; }

        /// <summary>
        /// Rotation around Z Axis.
        /// </summary>
        public double Rz { get; set; }

        /// <summary>
        /// Scaling in X Dimension.
        /// </summary>
        public double Sx { get; set; }

        /// <summary>
        /// Scaling in Y Dimension.
        /// </summary>
        public double Sy { get; set; }

        /// <summary>
        /// Scaling in Z Dimension.
        /// </summary>
        public double Sz { get; set; }

        /// <summary>
        /// The homogeneous matrix of this transform.
        /// </summary>
        public Matrix<double> Matrix
        {
            get
            {
                _translateMatrix[0, 3] = Tx;
                _translateMatrix[1, 3] = Ty;
                _translateMatrix[2, 3] = Tz;

                _rotateXMatrix[1, 1] = Math.Cos(Rx);
                _rotateXMatrix[2, 1] = Math.Sin(Rx);
                _rotateXMatrix[1, 2] = -Math.Sin(Rx);
                _rotateXMatrix[2, 2] = Math.Cos(Rx);

                _rotateYMatrix[0, 0] = Math.Cos(Ry);
                _rotateYMatrix[2, 0] = Math.Sin(Ry);
                _rotateYMatrix[0, 2] = -Math.Sin(Ry);
                _rotateYMatrix[2, 2] = Math.Cos(Ry);

                _rotateZMatrix[0, 0] = Math.Cos(Rz);
                _rotateZMatrix[1, 0] = Math.Sin(Rz);
                _rotateZMatrix[0, 1] = -Math.Sin(Rz);
                _rotateZMatrix[1, 1] = Math.Cos(Rz);

                _scaleMatrix[0, 0] = Sx;
                _scaleMatrix[1, 1] = Sy;
                _scaleMatrix[2, 2] = Sz;

                var matrix =
                    _translateMatrix *
                    _rotateXMatrix * _rotateYMatrix * _rotateZMatrix *
                    _scaleMatrix;

                return matrix;
            }
        }

        #endregion

        #region Fields

        private readonly Matrix<double> _translateMatrix =
            DenseMatrix.CreateIdentity(4);

        private readonly Matrix<double> _rotateXMatrix =
            DenseMatrix.CreateIdentity(4);

        private readonly Matrix<double> _rotateYMatrix =
            DenseMatrix.CreateIdentity(4);

        private readonly Matrix<double> _rotateZMatrix =
            DenseMatrix.CreateIdentity(4);

        private readonly Matrix<double> _scaleMatrix =
            DenseMatrix.CreateIdentity(4);

        #endregion
    }
}
