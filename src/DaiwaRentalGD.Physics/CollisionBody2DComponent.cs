using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Physics
{
    /// <summary>
    /// A component representing a 2D collision body specified by
    /// <see cref="Polygon"/>s.
    /// </summary>
    [Serializable]
    public class CollisionBody2DComponent : Component
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="CollisionBody2DComponent"/>
        /// with an empty <see cref="CollisionPolygons"/>.
        /// </summary>
        public CollisionBody2DComponent() : base()
        {
            CollisionPolygons = _collisionPolygons.AsReadOnly();
        }

        /// <summary>
        /// Creates an instance of <see cref="CollisionBody2DComponent"/>
        /// with given collision polygons.
        /// </summary>
        /// 
        /// <param name="collisionPolygons">
        /// Collision polygons to be added to <see cref="CollisionPolygons"/>.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="collisionPolygons"/> is
        /// <see langword="null"/>.
        /// </exception>
        public CollisionBody2DComponent(
            IEnumerable<Polygon> collisionPolygons
        ) : this()
        {
            if (collisionPolygons == null)
            {
                throw new ArgumentNullException(nameof(collisionPolygons));
            }

            foreach (var collisionPolygon in collisionPolygons)
            {
                AddCollisionPolygon(collisionPolygon);
            }
        }

        /// <inheritdoc/>
        protected CollisionBody2DComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            CollisionPolygons = _collisionPolygons.AsReadOnly();

            var reader = new WorkspaceItemReader(this, info, context);

            _collisionPolygons.AddRange(
                reader.GetValues<Polygon>(nameof(CollisionPolygons))
            );

            _epsilon = reader.GetValue<double>(nameof(Epsilon));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a <see cref="Polygon"/> specifying part of the shape
        /// of this <see cref="CollisionBody2DComponent"/>.
        /// </summary>
        /// 
        /// <param name="collisionPolygon">
        /// A <see cref="Polygon"/> specifying part of the shape of
        /// this <see cref="CollisionBody2DComponent"/>.
        /// </param>
        /// 
        /// <remarks>
        /// Please see <see cref="InsertCollisionPolygon"/> for
        /// possible exceptions.
        /// </remarks>
        public void AddCollisionPolygon(Polygon collisionPolygon)
        {
            InsertCollisionPolygon(
                _collisionPolygons.Count,
                collisionPolygon
            );
        }

        /// <summary>
        /// Inserts a <see cref="Polygon"/> specifying part of the shape
        /// of this <see cref="CollisionBody2DComponent"/> at a given index
        /// in <see cref="CollisionPolygons"/>.
        /// </summary>
        /// 
        /// <param name="collisionPolygonIndex">
        /// The index in <see cref="CollisionPolygons"/> at which to insert
        /// <paramref name="collisionPolygon"/>.
        /// </param>
        /// <param name="collisionPolygon">
        /// The <see cref="Polygon"/> specifying part of the the shape of
        /// this <see cref="CollisionBody2DComponent"/>.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="collisionPolygon"/> is
        /// <see langword="null"/>.
        /// </exception>
        public void InsertCollisionPolygon(
            int collisionPolygonIndex, Polygon collisionPolygon
        )
        {
            _collisionPolygons.Insert(
                collisionPolygonIndex,
                collisionPolygon ??
                throw new ArgumentNullException(nameof(collisionPolygon))
            );
        }

        /// <summary>
        /// Removes a <see cref="Polygon"/> specifying part of the shape of
        /// this <see cref="CollisionBody2DComponent"/>.
        /// </summary>
        /// 
        /// <param name="collisionPolygon">
        /// The <see cref="Polygon"/> specifying part of the shape of this
        /// <see cref="CollisionBody2DComponent"/> to be removed.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="collisionPolygon"/> is
        /// found and removed from <see cref="CollisionPolygons"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public bool RemoveCollisionPolygon(Polygon collisionPolygon)
        {
            return _collisionPolygons.Remove(collisionPolygon);
        }

        /// <summary>
        /// Removes all <see cref="Polygon"/>s specifying the shape of
        /// this <see cref="CollisionBody2DComponent"/>,
        /// i.e. clears <see cref="CollisionPolygons"/>.
        /// </summary>
        public void ClearCollisionPolygons()
        {
            _collisionPolygons.Clear();
        }

        /// <summary>
        /// Gets the local transform of this
        /// <see cref="CollisionBody2DComponent"/>, i.e.
        /// transform relative to the <see cref="SceneObject.Parent"/> of
        /// the <see cref="SceneObject"/> it is attached to.
        /// </summary>
        /// 
        /// <returns>
        /// The local transform of
        /// this <see cref="CollisionBody2DComponent"/>.
        /// If this <see cref="CollisionBody2DComponent"/> is not attached to
        /// a <see cref="SceneObject"/> or if the <see cref="SceneObject"/>
        /// has no <see cref="TransformComponent"/>, then
        /// an identity transform is returned.
        /// </returns>
        public ITransform3D GetTransform()
        {
            return
                SceneObject
                ?.GetComponent<TransformComponent>()
                ?.Transform ??
                new TrsTransform3D();
        }

        /// <summary>
        /// Gets the world transform of this
        /// <see cref="CollisionBody2DComponent"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The world transform of
        /// this <see cref="CollisionBody2DComponent"/>.
        /// If this <see cref="CollisionBody2DComponent"/> is not attached to
        /// a <see cref="SceneObject"/> or if the <see cref="SceneObject"/>
        /// has no <see cref="TransformComponent"/>, then
        /// an identity transform is returned.
        /// </returns>
        public ITransform3D GetWorldTransform()
        {
            return
                SceneObject
                ?.GetComponent<TransformComponent>()
                ?.GetWorldTransform() ??
                new TrsTransform3D();
        }

        /// <summary>
        /// Gets the list of <see cref="Polygon"/>s specifying the shape of
        /// this <see cref="CollisionBody2DComponent"/> locally transformed,
        /// i.e. <see cref="CollisionPolygons"/> transformed by
        /// the transform returned by <see cref="GetTransform"/>.
        /// This <see cref="CollisionBody2DComponent"/> is not modified.
        /// </summary>
        /// 
        /// <returns>
        /// Locally transformed <see cref="CollisionPolygons"/>.
        /// </returns>
        public IReadOnlyList<Polygon> GetTransformedCollisionPolygons()
        {
            var transform = GetTransform();

            var transformedCollisionPolygons =
                CollisionPolygons
                .Select(
                    collisionPolygon =>
                    {
                        var tcp = new Polygon(collisionPolygon);
                        tcp.Transform(transform);

                        return tcp;
                    }
                ).ToList();

            return transformedCollisionPolygons;
        }

        /// <summary>
        /// Gets the list of <see cref="Polygon"/>s specifying the shape of
        /// this <see cref="CollisionBody2DComponent"/> transformed to
        /// the world space, i.e. <see cref="CollisionPolygons"/>
        /// transformed by the transform returned by
        /// <see cref="GetWorldTransform"/>.
        /// This <see cref="CollisionBody2DComponent"/> is not modified.
        /// </summary>
        /// 
        /// <returns>
        /// A list of <see cref="Polygon"/>s from
        /// <see cref="CollisionPolygons"/> transformed into the world space.
        /// </returns>
        public IReadOnlyList<Polygon> GetWorldCollisionPolygons()
        {
            var worldTransform = GetWorldTransform();

            var worldCollisionPolygons =
                CollisionPolygons
                .Select(
                    collisionPolygon =>
                    {
                        var wcp = new Polygon(collisionPolygon);
                        wcp.Transform(worldTransform);

                        return wcp;
                    }
                ).ToList();

            return worldCollisionPolygons;
        }

        /// <summary>
        /// Checks if a given <see cref="SceneObject"/> is
        /// contained in one of the <see cref="CollisionPolygons"/> in
        /// this <see cref="CollisionBody2DComponent"/>.
        /// </summary>
        /// 
        /// <param name="sceneObject">
        /// The <see cref="SceneObject"/> to check whether
        /// one of the <see cref="CollisionPolygons"/> in
        /// this <see cref="CollisionBody2DComponent"/> contains.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if
        /// <paramref name="sceneObject"/> has a
        /// <see cref="CollisionBody2DComponent"/> and is contained in
        /// one of the <see cref="CollisionPolygons"/> in
        /// this <see cref="CollisionBody2DComponent"/>, or if
        /// <paramref name="sceneObject"/> does not have
        /// a <see cref="CollisionBody2DComponent"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="sceneObject"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// 
        /// <remarks>
        /// Please note that <paramref name="sceneObject"/> must be contained
        /// by at least one of the <see cref="CollisionPolygons"/> in
        /// this <see cref="CollisionBody2DComponent"/> to be considered
        /// being contained. No polygon boolean operation is performed
        /// on <see cref="CollisionPolygons"/> before containment check
        /// out of simplicity from project requirements.
        /// </remarks>
        public bool DoesContain(SceneObject sceneObject)
        {
            if (sceneObject == null)
            {
                throw new ArgumentNullException(nameof(sceneObject));
            }

            return DoesContain(new[] { sceneObject })[0];
        }

        /// <summary>
        /// Checks if this <see cref="CollisionBody2DComponent"/>
        /// contains a list of <see cref="SceneObject"/> individually.
        /// </summary>
        /// 
        /// <param name="sceneObjects">
        /// The list of <see cref="SceneObject"/>s to check whether
        /// this <see cref="CollisionBody2DComponent"/> contains.
        /// </param>
        /// 
        /// <returns>
        /// A list of <see cref="bool"/> values, each of which is
        /// <see langword="true"/> if
        /// this <see cref="CollisionBody2DComponent"/> contains
        /// the corresponding <see cref="SceneObject"/> at the same index
        /// in <paramref name="sceneObjects"/>
        /// and <see langword="false"/> otherwise.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="sceneObjects"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// 
        /// <remarks>
        /// Please see the remarks of <see cref="DoesContain(SceneObject)"/>
        /// for a definition of containment.
        /// </remarks>
        public IReadOnlyList<bool> DoesContain(
            IReadOnlyList<SceneObject> sceneObjects
        )
        {
            if (sceneObjects == null)
            {
                throw new ArgumentNullException(nameof(sceneObjects));
            }

            List<bool> doesContainSos = new List<bool>();

            var thisWcps = GetWorldCollisionPolygons();

            foreach (var so in sceneObjects)
            {
                var wcps =
                    so?.GetComponent<CollisionBody2DComponent>()
                    ?.GetWorldCollisionPolygons();

                if (wcps == null)
                {
                    doesContainSos.Add(true);
                }
                else
                {
                    bool doesContainSo = true;

                    foreach (var wcp in wcps)
                    {
                        bool doesContainWcp = false;

                        foreach (var thisWcp in thisWcps)
                        {
                            if (thisWcp.DoesContain2D(wcp, Epsilon))
                            {
                                doesContainWcp = true;
                                break;
                            }
                        }

                        if (!doesContainWcp)
                        {
                            doesContainSo = false;
                            break;
                        }
                    }

                    doesContainSos.Add(doesContainSo);
                }
            }

            return doesContainSos;
        }

        /// <summary>
        /// Checks if this <see cref="CollisionBody2DComponent"/>
        /// overlaps with a given <see cref="SceneObject"/>.
        /// </summary>
        /// 
        /// <param name="sceneObject">
        /// The <see cref="SceneObject"/> to check whether
        /// this <see cref="CollisionBody2DComponent"/> overlaps with.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if
        /// <paramref name="sceneObject"/> has a
        /// <see cref="CollisionBody2DComponent"/> and
        /// this <see cref="CollisionBody2DComponent"/> overlaps with
        /// <paramref name="sceneObject"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="sceneObject"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// 
        /// <remarks>
        /// Two <see cref="CollisionBody2DComponent"/>s overlap if any pair
        /// of <see cref="Polygon"/>s from their
        /// <see cref="CollisionPolygons"/> overlap.
        /// </remarks>
        public bool DoesOverlap(SceneObject sceneObject)
        {
            if (sceneObject == null)
            {
                throw new ArgumentNullException(nameof(sceneObject));
            }

            return DoesOverlapAny(new[] { sceneObject });
        }

        /// <summary>
        /// Checks if this <see cref="CollisionBody2DComponent"/>
        /// overlaps with a list of <see cref="SceneObject"/> individually.
        /// </summary>
        /// 
        /// <param name="sceneObjects">
        /// The list of <see cref="SceneObject"/>s to check whether
        /// this <see cref="CollisionBody2DComponent"/> overlaps with.
        /// </param>
        /// 
        /// <returns>
        /// A list of <see cref="bool"/> values, each of which is
        /// <see langword="true"/> if
        /// this <see cref="CollisionBody2DComponent"/> overlaps with
        /// the corresponding <see cref="SceneObject"/> at the same index
        /// in <paramref name="sceneObjects"/>
        /// and <see langword="false"/> otherwise.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="sceneObjects"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// 
        /// <remarks>
        /// Please see the remarks of <see cref="DoesOverlap(SceneObject)"/>
        /// for a definition of overlap.
        /// </remarks>
        public IReadOnlyList<bool> DoesOverlap(
            IReadOnlyList<SceneObject> sceneObjects
        )
        {
            if (sceneObjects == null)
            {
                throw new ArgumentNullException(nameof(sceneObjects));
            }

            List<bool> doesOverlapSos = new List<bool>();

            var thisWcps = GetWorldCollisionPolygons();

            foreach (var so in sceneObjects)
            {
                var cbc = so?.GetComponent<CollisionBody2DComponent>();

                if (cbc == null)
                {
                    doesOverlapSos.Add(false);
                }
                else if (cbc == this)
                {
                    // No overlap with self
                    doesOverlapSos.Add(false);
                }
                else
                {
                    var wcps = cbc.GetWorldCollisionPolygons();

                    bool doesOverlapSo = false;

                    foreach (var wcp in wcps)
                    {
                        bool doesOverlapWcp = false;

                        foreach (var thisWcp in thisWcps)
                        {
                            if (thisWcp.DoesOverlap2D(wcp, Epsilon))
                            {
                                doesOverlapWcp = true;
                                break;
                            }
                        }

                        if (doesOverlapWcp)
                        {
                            doesOverlapSo = true;
                            break;
                        }
                    }

                    doesOverlapSos.Add(doesOverlapSo);
                }
            }

            return doesOverlapSos;
        }

        /// <summary>
        /// Checks if this <see cref="CollisionBody2DComponent"/>
        /// overlaps with any <see cref="SceneObject"/> from a given list.
        /// </summary>
        /// 
        /// <param name="sceneObjects">
        /// The list of <see cref="SceneObject"/>s to check whether
        /// this <see cref="CollisionBody2DComponent"/> overlaps with any.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if
        /// this <see cref="CollisionBody2DComponent"/> overlaps with
        /// any <see cref="SceneObject"/> in <paramref name="sceneObjects"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="sceneObjects"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// 
        /// <remarks>
        /// Please see the remarks of <see cref="DoesOverlap(SceneObject)"/>
        /// for a definition of overlap.
        /// </remarks>
        public bool DoesOverlapAny(IReadOnlyList<SceneObject> sceneObjects)
        {
            if (sceneObjects == null)
            {
                throw new ArgumentNullException(nameof(sceneObjects));
            }

            var thisWcps = GetWorldCollisionPolygons();

            foreach (var so in sceneObjects)
            {
                var cbc = so?.GetComponent<CollisionBody2DComponent>();

                if (cbc == null)
                {
                    continue;
                }
                else if (cbc == this)
                {
                    // No overlap with self
                    continue;
                }
                else
                {
                    var wcps = cbc.GetWorldCollisionPolygons();

                    foreach (var wcp in wcps)
                    {
                        foreach (var thisWcp in thisWcps)
                        {
                            if (thisWcp.DoesOverlap2D(wcp, Epsilon))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValues(nameof(CollisionPolygons), _collisionPolygons);
            writer.AddValue(nameof(Epsilon), _epsilon);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The list of <see cref="Polygon"/>s specifying the shape of
        /// this <see cref="CollisionBody2DComponent"/>.
        /// </summary>
        public IReadOnlyList<Polygon> CollisionPolygons { get; }

        /// <summary>
        /// Tolerance used to deal with precision errors in calculation
        /// such as <see cref="DoesContain"/>, <see cref="DoesOverlap"/> and
        /// <see cref="DoesOverlapAny"/>.
        /// </summary>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="value"/> is negative.
        /// </exception>
        public double Epsilon
        {
            get => _epsilon;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(Epsilon)} cannot be negative"
                    );
                }

                _epsilon = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Fields

        private readonly List<Polygon> _collisionPolygons =
            new List<Polygon>();

        private double _epsilon = DefaultEpsilon;

        #endregion

        #region Constants

        /// <summary>
        /// Default value for <see cref="Epsilon"/>.
        /// </summary>
        public const double DefaultEpsilon = Polygon.DefaultEpsilon;

        #endregion
    }
}
