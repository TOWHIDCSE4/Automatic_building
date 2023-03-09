using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Core;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// A composite 3D transform consisting of a list of 3D transforms.
    /// </summary>
    [Serializable]
    public class CompositeTransform3D :
        ITransform3D, IEquatable<CompositeTransform3D>, ISerializable
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="CompositeTransform3D"/>
        /// with an empty list of transforms,
        /// effectively an identity transform.
        /// </summary>
        public CompositeTransform3D()
        {
            Transforms = _transforms.AsReadOnly();
        }

        /// <summary>
        /// Creates an instance of <see cref="CompositeTransform3D"/>
        /// with a given list of transforms.
        /// </summary>
        /// 
        /// <param name="transforms">
        /// The list of transforms to be added to
        /// the created <see cref="CompositeTransform3D"/>.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="transforms"/> is <see langword="null"/>.
        /// </exception>
        public CompositeTransform3D(IEnumerable<ITransform3D> transforms) :
            this()
        {
            if (transforms == null)
            {
                throw new ArgumentNullException(nameof(transforms));
            }

            foreach (ITransform3D transform in transforms)
            {
                Add(transform);
            }
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// 
        /// <param name="info">
        /// Contains data of
        /// a serialized <see cref="CompositeTransform3D"/> instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected CompositeTransform3D(
            SerializationInfo info, StreamingContext context
        ) : this()
        {
            _transforms.AddRange(
                info.GetValue<List<ITransform3D>>(nameof(Transforms))
            );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a transform to the end of <see cref="Transforms"/>.
        /// </summary>
        /// 
        /// <param name="transform">
        /// The transform to be added.
        /// </param>
        /// 
        /// <remarks>
        /// Please refer to <see cref="Insert"/> for possible exceptions.
        /// </remarks>
        public void Add(ITransform3D transform)
        {
            Insert(_transforms.Count, transform);
        }

        /// <summary>
        /// Inserts a transform into <see cref="Transforms"/> at
        /// a specified index.
        /// </summary>
        /// 
        /// <param name="index">
        /// The index in <see cref="Transforms"/> at which to insert
        /// <paramref name="transform"/>.
        /// </param>
        /// <param name="transform">
        /// The transform to insert into <see cref="Transforms"/>.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="transform"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="transform"/> is
        /// this <see cref="CompositeTransform3D"/> since there will be
        /// an infinite loop of transforms otherwise.
        /// </exception>
        public void Insert(int index, ITransform3D transform)
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            if (ReferenceEquals(transform, this))
            {
                throw new ArgumentException(
                    "Cannot add a composite transform to itself",
                    nameof(transform)
                );
            }

            _transforms.Insert(index, transform);
        }

        /// <summary>
        /// Removes the transform at a specified index from
        /// <see cref="Transforms"/>.
        /// </summary>
        /// 
        /// <param name="index">
        /// The index of the transform to be removed.
        /// </param>
        /// 
        /// <returns>
        /// The removed transform.
        /// </returns>
        /// 
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown if <paramref name="index"/> is out of range.
        /// </exception>
        public ITransform3D Remove(int index)
        {
            ITransform3D removedTransform = _transforms[index];

            _transforms.RemoveAt(index);

            return removedTransform;
        }

        /// <summary>
        /// Transforms a 3D vector that represents a position.
        /// This is effectively done by applying the transforms in
        /// <see cref="Transforms"/> to the given position in sequence.
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

            Vector<double> transformedPosition3D = position3D.Clone();

            foreach (ITransform3D transform in Transforms)
            {
                transformedPosition3D =
                    transform.Transform(transformedPosition3D);
            }

            return transformedPosition3D;
        }

        /// <summary>
        /// Transforms a 3D vector that represents a direction.
        /// This is effectively done by applying the transforms in
        /// <see cref="Transforms"/> to the given direction in sequence.
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

            Vector<double> transformedDir3D = dir3D.Clone();

            foreach (ITransform3D transform in Transforms)
            {
                transformedDir3D = transform.TransformDir(transformedDir3D);
            }

            return transformedDir3D;
        }

        /// <summary>
        /// Determines whether this <see cref="CompositeTransform3D"/> and
        /// a given object have the same value.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The object to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is
        /// a <see cref="CompositeTransform3D"/> and have the same
        /// list of transforms as this <see cref="CompositeTransform3D"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj) =>
            Equals(obj as CompositeTransform3D);

        /// <summary>
        /// Determines whether this <see cref="CompositeTransform3D"/> and
        /// another have the same value.
        /// </summary>
        /// 
        /// <param name="other">
        /// The other <see cref="CompositeTransform3D"/> to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="other"/> has the same
        /// list of transforms as this <see cref="CompositeTransform3D"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool Equals(CompositeTransform3D other)
        {
            return ReferenceEquals(this, other) || (
                other != null &&
                _transforms.SequenceEqual(other._transforms)
            );
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1755184145;

            foreach (ITransform3D transform in Transforms)
            {
                hashCode = hashCode * -1521134295 + transform.GetHashCode();
            }

            return hashCode;
        }

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(Transforms), _transforms);
        }

        /// <summary>
        /// Determines whether two <see cref="CompositeTransform3D"/>s
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="CompositeTransform3D"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="CompositeTransform3D"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have the same list of transforms.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(
            CompositeTransform3D left, CompositeTransform3D right
        ) => EqualityComparer<CompositeTransform3D>.Default
            .Equals(left, right);

        /// <summary>
        /// Determines whether two <see cref="CompositeTransform3D"/>s
        /// have different values.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="CompositeTransform3D"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="CompositeTransform3D"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have different lists of transforms.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(
            CompositeTransform3D left, CompositeTransform3D right
        ) => !(left == right);

        #endregion

        #region Properties

        /// <summary>
        /// The list of transforms in this <see cref="CompositeTransform3D"/>.
        /// </summary>
        public IReadOnlyList<ITransform3D> Transforms { get; }

        /// <summary>
        /// The homogeneous matrix of this <see cref="CompositeTransform3D"/>.
        /// </summary>
        public Matrix<double> Matrix
        {
            get
            {
                Matrix<double> matrix = DenseMatrix.CreateIdentity(4);

                foreach (ITransform3D transform in Transforms)
                {
                    matrix = transform.Matrix * matrix;
                }

                return matrix;
            }
        }

        #endregion

        #region Fields

        private readonly List<ITransform3D> _transforms =
            new List<ITransform3D>();

        #endregion
    }
}
