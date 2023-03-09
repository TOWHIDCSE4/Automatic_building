using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Represents a 3D bounding box.
    /// </summary>
    [Serializable]
    public class BBox : IBBox, IEquatable<BBox>, ISerializable
    {
        #region Constructors

        /// <summary>
        /// Creates an empty <see cref="BBox"/> with a size of 0
        /// in each dimension.
        /// </summary>
        public BBox()
        { }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// 
        /// <param name="info">
        /// Contains data of a serialized <see cref="BBox"/>
        /// instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected BBox(SerializationInfo info, StreamingContext context)
        {
            _minX = info.GetDouble(nameof(MinX));
            _maxX = info.GetDouble(nameof(MaxX));

            _minY = info.GetDouble(nameof(MinY));
            _maxY = info.GetDouble(nameof(MaxY));

            _minZ = info.GetDouble(nameof(MinZ));
            _maxZ = info.GetDouble(nameof(MaxZ));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a <see cref="BBox"/> with lower bounds and upper bounds
        /// in each dimension.
        /// </summary>
        /// 
        /// <param name="minX">Lower bound in X Dimension.</param>
        /// <param name="minY">Lower bound in Y Dimension.</param>
        /// <param name="minZ">Lower bound in Z Dimension.</param>
        /// <param name="maxX">Upper bound in X Dimension.</param>
        /// <param name="maxY">Upper bound in Y Dimension.</param>
        /// <param name="maxZ">Upper bound in Z Dimension.</param>
        /// 
        /// <returns>
        /// A <see cref="BBox"/> with given bounds.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown if
        /// <paramref name="maxX"/> is smaller than <paramref name="minX"/> or
        /// <paramref name="maxY"/> is smaller than <paramref name="minY"/> or
        /// <paramref name="maxZ"/> is smaller than <paramref name="minZ"/>.
        /// </exception>
        public static BBox FromMinAndMax(
            double minX, double minY, double minZ,
            double maxX, double maxY, double maxZ
        )
        {
            BBox bbox = new BBox();

            bbox.SetMinXMaxX(minX, maxX);
            bbox.SetMinYMaxY(minY, maxY);
            bbox.SetMinZMaxZ(minZ, maxZ);

            return bbox;
        }

        /// <summary>
        /// Creates a <see cref="BBox"/> with lower bounds and sizes
        /// in each dimension.
        /// </summary>
        /// <param name="minX">Lower bound in X Dimension.</param>
        /// <param name="minY">Lower bound in Y Dimension.</param>
        /// <param name="minZ">Lower bound in Z Dimension.</param>
        /// <param name="sizeX">Size in X Dimension.</param>
        /// <param name="sizeY">Size in Y Dimension.</param>
        /// <param name="sizeZ">Size in Z Dimension.</param>
        /// 
        /// <returns>
        /// A <see cref="BBox"/> with given lower bounds and sizes.
        /// </returns>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown if
        /// <paramref name="sizeX"/> or
        /// <paramref name="sizeY"/> or
        /// <paramref name="sizeZ"/> is negative.
        /// </exception>
        public static BBox FromMinAndSize(
            double minX, double minY, double minZ,
            double sizeX, double sizeY, double sizeZ
        )
        {
            BBox bbox = new BBox();

            bbox.SetMinXSizeX(minX, sizeX);
            bbox.SetMinYSizeY(minY, sizeY);
            bbox.SetMinZSizeZ(minZ, sizeZ);

            return bbox;
        }

        /// <summary>
        /// Gets the bounding box of this <see cref="BBox"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The bounding box of this <see cref="BBox"/>, which is itself.
        /// </returns>
        public BBox GetBBox()
        {
            return this;
        }

        /// <summary>
        /// Sets the bounds in X Dimension.
        /// </summary>
        /// 
        /// <param name="minX">Lower bound in X Dimension.</param>
        /// <param name="maxX">Upper bound in X Dimension.</param>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="maxX"/> is smaller than
        /// <paramref name="minX"/>.
        /// </exception>
        public void SetMinXMaxX(double minX, double maxX)
        {
            if (maxX < minX)
            {
                throw new ArgumentException(
                    $"{nameof(maxX)} cannot be smaller than {nameof(minX)}",
                    nameof(maxX)
                );
            }

            _minX = minX;
            _maxX = maxX;
        }

        /// <summary>
        /// Sets the lower bound and size in X Dimension.
        /// </summary>
        /// 
        /// <param name="minX">Lower bound in X Dimension.</param>
        /// <param name="sizeX">Size in X Dimension.</param>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="sizeX"/> is negative.
        /// </exception>
        public void SetMinXSizeX(double minX, double sizeX)
        {
            if (sizeX < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sizeX),
                    $"{nameof(sizeX)} cannot be negative"
                );
            }

            _minX = minX;
            _maxX = _minX + sizeX;
        }

        /// <summary>
        /// Sets the bounds in Y Dimension.
        /// </summary>
        /// 
        /// <param name="minY">Lower bound in Y Dimension.</param>
        /// <param name="maxY">Upper bound in Y Dimension.</param>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="maxY"/> is smaller than
        /// <paramref name="minY"/>.
        /// </exception>
        public void SetMinYMaxY(double minY, double maxY)
        {
            if (maxY < minY)
            {
                throw new ArgumentException(
                    $"{nameof(maxY)} cannot be smaller than {nameof(minY)}",
                    nameof(maxY)
                );
            }

            _minY = minY;
            _maxY = maxY;
        }

        /// <summary>
        /// Sets the lower bound and size in Y Dimension.
        /// </summary>
        /// 
        /// <param name="minY">Lower bound in Y Dimension.</param>
        /// <param name="sizeY">Size in Y Dimension.</param>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="sizeY"/> is negative.
        /// </exception>
        public void SetMinYSizeY(double minY, double sizeY)
        {
            if (sizeY < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sizeY),
                    $"{nameof(sizeY)} cannot be negative"
                );
            }

            _minY = minY;
            _maxY = _minY + sizeY;
        }

        /// <summary>
        /// Sets the bounds in Z Dimension.
        /// </summary>
        /// 
        /// <param name="minZ">Lower bound in Z Dimension.</param>
        /// <param name="maxZ">Upper bound in Z Dimension.</param>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="maxZ"/> is smaller than
        /// <paramref name="minZ"/>.
        /// </exception>
        public void SetMinZMaxZ(double minZ, double maxZ)
        {
            if (maxZ < minZ)
            {
                throw new ArgumentException(
                    $"{nameof(maxZ)} cannot be smaller than {nameof(minZ)}",
                    nameof(maxZ)
                );
            }

            _minZ = minZ;
            _maxZ = maxZ;
        }

        /// <summary>
        /// Sets the lower bound and size in Z Dimension.
        /// </summary>
        /// 
        /// <param name="minZ">Lower bound in Z Dimension.</param>
        /// <param name="sizeZ">Size in Z Dimension.</param>
        /// 
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="sizeZ"/> is negative.
        /// </exception>
        public void SetMinZSizeZ(double minZ, double sizeZ)
        {
            if (sizeZ < 0.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sizeZ),
                    $"{nameof(sizeZ)} cannot be negative"
                );
            }

            _minZ = minZ;
            _maxZ = _minZ + sizeZ;
        }

        /// <summary>
        /// Gets the <see cref="BBox"/> of a collection of objects that have
        /// their own <see cref="BBox"/>.
        /// </summary>
        /// 
        /// <param name="ibboxes">
        /// The collection of objects that have their own <see cref="BBox"/>.
        /// </param>
        /// 
        /// <returns>
        /// The <see cref="BBox"/> of the given collection of objects.
        /// If <paramref name="ibboxes"/> is empty,
        /// an empty <see cref="BBox"/> is returned.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="ibboxes"/> is <see langword="null"/>.
        /// </exception>
        public static BBox GetBBox(IEnumerable<IBBox> ibboxes)
        {
            if (ibboxes == null)
            {
                throw new ArgumentNullException(nameof(ibboxes));
            }

            if (!ibboxes.Any())
            {
                return new BBox();
            }

            double minX = Enumerable.Min(ibboxes, (b) => b.GetBBox().MinX);
            double minY = Enumerable.Min(ibboxes, (b) => b.GetBBox().MinY);
            double minZ = Enumerable.Min(ibboxes, (b) => b.GetBBox().MinZ);

            double maxX = Enumerable.Max(ibboxes, (b) => b.GetBBox().MaxX);
            double maxY = Enumerable.Max(ibboxes, (b) => b.GetBBox().MaxY);
            double maxZ = Enumerable.Max(ibboxes, (b) => b.GetBBox().MaxZ);

            BBox bbox =
                BBox.FromMinAndMax(minX, minY, minZ, maxX, maxY, maxZ);

            return bbox;
        }

        /// <summary>
        /// Gets the center point of the bounding box.
        /// </summary>
        /// 
        /// <returns>
        /// A point located at the center of the bounding box.
        /// </returns>
        public Point GetCenter()
        {
            return new Point(
                (MinX + MaxX) / 2.0,
                (MinY + MaxY) / 2.0,
                (MinZ + MaxZ) / 2.0
            );
        }

        /// <summary>
        /// Checks if two <see cref="BBox"/>s overlap or are tangential
        /// at any part.
        /// </summary>
        /// 
        /// <param name="bbox0">
        /// The first <see cref="BBox"/> to be checked.
        /// </param>
        /// <param name="bbox1">
        /// The second <see cref="BBox"/> to be checked.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if the two <see cref="BBox"/> overlap
        /// or are tangential.
        /// Otherwise <see langword="false"/>
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="bbox0"/> or <paramref name="bbox1"/>
        /// is <see langword="null"/>.
        /// </exception>
        public static bool DoesOverlap(BBox bbox0, BBox bbox1)
        {
            if (bbox0 == null)
            {
                throw new ArgumentNullException(nameof(bbox0));
            }

            if (bbox1 == null)
            {
                throw new ArgumentNullException(nameof(bbox1));
            }

            var center0 = bbox0.GetCenter();
            var center1 = bbox1.GetCenter();

            return (
                Math.Abs(center0.X - center1.X) <=
                (bbox0.SizeX + bbox1.SizeX) / 2.0
            ) && (
                Math.Abs(center0.Y - center1.Y) <=
                (bbox0.SizeY + bbox1.SizeY) / 2.0
            ) && (
                Math.Abs(center0.Z - center1.Z) <=
                (bbox0.SizeZ + bbox1.SizeZ) / 2.0
            );
        }

        /// <summary>
        /// Checks if two <see cref="BBox"/>s overlap at any part.
        /// Tangential <see cref="BBox"/>s are not considered to overlap.
        /// </summary>
        /// 
        /// <param name="bbox0">
        /// The first <see cref="BBox"/> to be checked.
        /// </param>
        /// <param name="bbox1">
        /// The second <see cref="BBox"/> to be checked.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if the two <see cref="BBox"/> overlap.
        /// Otherwise <see langword="false"/>
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="bbox0"/> or <paramref name="bbox1"/>
        /// is <see langword="null"/>.
        /// </exception>
        public static bool DoesOverlapInterior(BBox bbox0, BBox bbox1)
        {
            if (bbox0 == null)
            {
                throw new ArgumentNullException(nameof(bbox0));
            }

            if (bbox1 == null)
            {
                throw new ArgumentNullException(nameof(bbox1));
            }

            var center0 = bbox0.GetCenter();
            var center1 = bbox1.GetCenter();

            return (
                Math.Abs(center0.X - center1.X) <
                (bbox0.SizeX + bbox1.SizeX) / 2.0
            ) && (
                Math.Abs(center0.Y - center1.Y) <
                (bbox0.SizeY + bbox1.SizeY) / 2.0
            ) && (
                Math.Abs(center0.Z - center1.Z) <
                (bbox0.SizeZ + bbox1.SizeZ) / 2.0
            );
        }

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            info.AddValue(nameof(MinX), _minX);
            info.AddValue(nameof(MaxX), _maxX);

            info.AddValue(nameof(MinY), _minY);
            info.AddValue(nameof(MaxY), _maxY);

            info.AddValue(nameof(MinZ), _minZ);
            info.AddValue(nameof(MaxZ), _maxZ);
        }

        /// <summary>
        /// Determines whether this <see cref="BBox"/> and a specified object
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The object to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is
        /// a <see cref="BBox"/> and has the same bounds as this
        /// <see cref="BBox"/>. Otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as BBox);

        /// <summary>
        /// Determines whether this <see cref="BBox"/> and another
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="other">
        /// The other <see cref="BBox"/> to be compared with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="other"/> has
        /// the same bounds as this <see cref="BBox"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(BBox other)
        {
            return ReferenceEquals(this, other) || (
                other != null &&
                _minX == other._minX &&
                _minY == other._minY &&
                _minZ == other._minZ &&
                _maxX == other._maxX &&
                _maxY == other._maxY &&
                _maxZ == other._maxZ
            );
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -1838259446;

            hashCode = hashCode * -1521134295 + _minX.GetHashCode();
            hashCode = hashCode * -1521134295 + _minY.GetHashCode();
            hashCode = hashCode * -1521134295 + _minZ.GetHashCode();
            hashCode = hashCode * -1521134295 + _maxX.GetHashCode();
            hashCode = hashCode * -1521134295 + _maxY.GetHashCode();
            hashCode = hashCode * -1521134295 + _maxZ.GetHashCode();

            return hashCode;
        }

        /// <summary>
        /// Determines whether two <see cref="BBox"/> objects
        /// have the same value.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="BBox"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="BBox"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have the same bounds.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator ==(BBox left, BBox right) =>
            EqualityComparer<BBox>.Default.Equals(left, right);

        /// <summary>
        /// Determines whether two <see cref="BBox"/> objects
        /// have different values.
        /// </summary>
        /// 
        /// <param name="left">
        /// The first <see cref="BBox"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="BBox"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and
        /// <paramref name="right"/> have the different bounds.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool operator !=(BBox left, BBox right) =>
            !(left == right);

        #endregion

        #region Properties

        /// <summary>
        /// Lower bound in X Dimension.
        /// </summary>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="value"/> is greater than
        /// <see cref="MaxX"/>.
        /// </exception>
        public double MinX
        {
            get => _minX;
            set
            {
                if (value > MaxX)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MinX)} cannot be " +
                        $"greater than {nameof(MaxX)}"
                    );
                }

                _minX = value;
            }
        }

        /// <summary>
        /// Lower bound in Y Dimension.
        /// </summary>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="value"/> is greater than
        /// <see cref="MaxY"/>.
        /// </exception>
        public double MinY
        {
            get => _minY;
            set
            {
                if (value > MaxY)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MinY)} cannot be " +
                        $"greater than {nameof(MaxY)}"
                    );
                }

                _minY = value;
            }
        }

        /// <summary>
        /// Lower bound in Z Dimension.
        /// </summary>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="value"/> is greater than
        /// <see cref="MaxZ"/>.
        /// </exception>
        public double MinZ
        {
            get => _minZ;
            set
            {
                if (value > MaxZ)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MinZ)} cannot be " +
                        $"greater than {nameof(MaxZ)}"
                    );
                }

                _minZ = value;
            }
        }

        /// <summary>
        /// Upper bound in X Dimension.
        /// </summary>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="value"/> is smaller than
        /// <see cref="MinX"/>.
        /// </exception>
        public double MaxX
        {
            get => _maxX;
            set
            {
                if (value < MinX)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxX)} cannot be " +
                        $"smaller than {nameof(MinX)}"
                    );
                }

                _maxX = value;
            }
        }

        /// <summary>
        /// Upper bound in Y Dimension.
        /// </summary>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="value"/> is smaller than
        /// <see cref="MinY"/>.
        /// </exception>
        public double MaxY
        {
            get => _maxY;
            set
            {
                if (value < MinY)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxY)} cannot be " +
                        $"smaller than {nameof(MinY)}"
                    );
                }

                _maxY = value;
            }
        }

        /// <summary>
        /// Upper bound in Z Dimension.
        /// </summary>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="value"/> is smaller than
        /// <see cref="MinZ"/>.
        /// </exception>
        public double MaxZ
        {
            get => _maxZ;
            set
            {
                if (value < MinZ)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(MaxZ)} cannot be " +
                        $"smaller than {nameof(MinZ)}"
                    );
                }

                _maxZ = value;
            }
        }

        /// <summary>
        /// Size in X Dimension.
        /// </summary>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="value"/> is negative.
        /// </exception>
        public double SizeX
        {
            get => MaxX - MinX;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SizeX)} cannot be negative"
                    );
                }

                _maxX = MinX + value;
            }
        }

        /// <summary>
        /// Size in Y Dimension.
        /// </summary>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="value"/> is negative.
        /// </exception>
        public double SizeY
        {
            get => MaxY - MinY;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SizeY)} cannot be negative"
                    );
                }

                _maxY = MinY + value;
            }
        }

        /// <summary>
        /// Size in Z Dimension.
        /// </summary>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="value"/> is negative.
        /// </exception>
        public double SizeZ
        {
            get => MaxZ - MinZ;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(SizeZ)} cannot be negative"
                    );
                }

                _maxZ = MinZ + value;
            }
        }

        #endregion

        #region Fields

        private double _minX;
        private double _minY;
        private double _minZ;

        private double _maxX;
        private double _maxY;
        private double _maxZ;

        #endregion
    }
}
