using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Physics
{
    /// <summary>
    /// A component specifying a rigid body in a 2D space.
    /// </summary>
    [Serializable]
    public class RigidBody2DComponent : Component
    {
        #region Constructors

        /// <summary>
        /// Creates and instance of <see cref="RigidBody2DComponent"/>.
        /// </summary>
        public RigidBody2DComponent() : base()
        { }

        /// <inheritdoc/>
        protected RigidBody2DComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _isSpace = reader.GetValue<bool>(nameof(IsSpace));
            _isStatic = reader.GetValue<bool>(nameof(IsStatic));
            _mass = reader.GetValue<double>(nameof(Mass));
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(IsSpace), _isSpace);
            writer.AddValue(nameof(IsStatic), _isStatic);
            writer.AddValue(nameof(Mass), _mass);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The <see cref="CollisionBody2DComponent"/> that is attached to
        /// the same <see cref="SceneObject"/> as
        /// this <see cref="RigidBody2DComponent"/> does.
        /// </summary>
        public CollisionBody2DComponent CollisionBody2DComponent =>
            SceneObject?.GetComponent<CollisionBody2DComponent>();

        /// <summary>
        /// The <see cref="TransformComponent"/> that is attached to
        /// the same <see cref="SceneObject"/> as
        /// this <see cref="RigidBody2DComponent"/> does.
        /// </summary>
        public TransformComponent TransformComponent =>
            SceneObject?.GetComponent<TransformComponent>();

        /// <summary>
        /// Indicates whether this <see cref="RigidBody2DComponent"/>
        /// has a valid setup in order to work.
        /// This <see cref="RigidBody2DComponent"/> is valid only if
        /// a <see cref="CollisionBody2DComponent"/> and
        /// a <see cref="TransformComponent"/> are attached to
        /// the same <see cref="SceneObject"/> as
        /// this <see cref="RigidBody2DComponent"/> does.
        /// </summary>
        public bool IsValid =>
            CollisionBody2DComponent != null &&
            TransformComponent != null;

        /// <summary>
        /// Whether this <see cref="RigidBody2DComponent"/> is
        /// a space. If not, it is a solid.
        /// </summary>
        /// 
        /// <remarks>
        /// During collision response, for two overlapping rigid bodies:
        /// 
        /// <list type="bullet">
        /// <item>
        ///     <description>
        ///     If both are solids, then they will be moved apart
        ///     </description>
        /// </item>
        /// <item>
        ///     <description>
        ///     If both are spaces, then they will not be moved since
        ///     spaces can overlap
        ///     </description>
        /// </item>
        /// <item>
        ///     <description>
        ///     If one is a solid and the other is a space,
        ///     the solid will be moved so that it is contained (as much
        ///     as possible) by the space with no edge intersection
        ///     </description>
        /// </item>
        /// </list>
        /// </remarks>
        public bool IsSpace
        {
            get => _isSpace;
            set
            {
                _isSpace = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsActualStatic));
                NotifyPropertyChanged(nameof(EffectiveMass));
            }
        }

        /// <summary>
        /// Whether this rigid body is static.
        /// </summary>
        public bool IsStatic
        {
            get => _isStatic;
            set
            {
                _isStatic = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsActualStatic));
                NotifyPropertyChanged(nameof(EffectiveMass));
            }
        }

        /// <summary>
        /// Whether this rigid body is actually static.
        /// A space rigid body is always actually static.
        /// A solid rigid body is actually static when
        /// <see cref="IsStatic"/> is <see langword="true"/>.
        /// If <see cref="IsActualStatic"/> is <see langword="true"/>,
        /// then the rigid body will not be moved during collision response,
        /// and has an <see cref="EffectiveMass"/> of infinity.
        /// </summary>
        public bool IsActualStatic => IsStatic || IsSpace;

        /// <summary>
        /// The mass of this <see cref="RigidBody2DComponent"/>.
        /// </summary>
        public double Mass
        {
            get => _mass;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(Mass)} cannot be negative"
                    );
                }

                _mass = value;
                NotifyPropertyChanged();

                if (!IsActualStatic)
                {
                    NotifyPropertyChanged(nameof(EffectiveMass));
                }
            }
        }

        /// <summary>
        /// The effective mass of this rigid body.
        /// If <see cref="IsActualStatic"/> is <see langword="true"/>,
        /// then this rigid body cannot be moved physically
        /// and has an effective mass of infinity.
        /// Otherwise its effective mass is <see cref="Mass"/>.
        /// </summary>
        public double EffectiveMass =>
            IsActualStatic ? double.PositiveInfinity : Mass;

        #endregion

        #region Member variables

        private bool _isSpace = DefaultIsSpace;
        private bool _isStatic = DefaultIsStatic;
        private double _mass = DefaultMass;

        #endregion

        #region Constants

        /// <summary>
        /// Default value for <see cref="IsSpace"/>.
        /// </summary>
        public const bool DefaultIsSpace = false;

        /// <summary>
        /// Default value for <see cref="IsStatic"/>.
        /// </summary>
        public const bool DefaultIsStatic = false;

        /// <summary>
        /// Default value for <see cref="Mass"/>.
        /// </summary>
        public const double DefaultMass = 1.0;

        #endregion
    }
}
