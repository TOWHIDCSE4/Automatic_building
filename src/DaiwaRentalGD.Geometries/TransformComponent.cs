using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// A <see cref="Component"/> that describes the transform of
    /// the <see cref="SceneObject"/> that it is attached to.
    /// </summary>
    [Serializable]
    public class TransformComponent : Component
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="TransformComponent"/>.
        /// <see cref="Transform"/> is initialized to an identity transform.
        /// </summary>
        public TransformComponent() : base()
        { }

        /// <inheritdoc/>
        protected TransformComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _transform = reader.GetValue<TrsTransform3D>(nameof(Transform));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the transform of the <see cref="SceneObject"/> that
        /// this <see cref="TransformComponent"/> is attached to
        /// in the world space.
        /// </summary>
        /// 
        /// <returns>
        /// The transform of the <see cref="SceneObject"/> that
        /// this <see cref="TransformComponent"/> is attached to
        /// in the world space.
        /// </returns>
        /// 
        /// <remarks>
        /// <para>
        /// If the <see cref="SceneObject.Parent"/> of
        /// the <see cref="SceneObject"/> that
        /// this <see cref="TransformComponent"/> is attached to is
        /// <see langword="null"/>, its world transform is
        /// <see cref="Transform"/>.
        /// </para>
        /// <para>
        /// If this <see cref="TransformComponent"/> is not attached to
        /// a <see cref="SceneObject"/>, the world transform returned is
        /// <see cref="Transform"/>.
        /// </para>
        /// <para>
        /// If an ancestor of the <see cref="SceneObject"/> that
        /// this <see cref="TransformComponent"/> is attached to
        /// does not have a <see cref="TransformComponent"/>,
        /// that ancestor is considered to have a local identity transform.
        /// </para>
        /// </remarks>
        public ITransform3D GetWorldTransform()
        {
            var worldTransform = new CompositeTransform3D();

            worldTransform.Add(Transform);

            SceneObject currentSceneObject = SceneObject?.Parent;

            while (currentSceneObject != null)
            {
                var currentTransform =
                    currentSceneObject.GetComponent<TransformComponent>()
                    ?.Transform;

                if (currentTransform != null)
                {
                    worldTransform.Add(currentTransform);
                }

                currentSceneObject = currentSceneObject.Parent;
            }

            return worldTransform;
        }

        /// <inheritdoc/>
        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Transform), _transform);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Local transform of the <see cref="SceneObject"/> that
        /// this <see cref="TransformComponent"/> is attached to, i.e.
        /// relative to its <see cref="SceneObject.Parent"/>.
        /// If <see cref="SceneObject.Parent"/> is <see langword="null"/>,
        /// <see cref="Transform"/> is also the world transform of
        /// the <see cref="SceneObject"/>.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public TrsTransform3D Transform
        {
            get => _transform;
            set
            {
                _transform = value ??
                    throw new ArgumentNullException(nameof(value));
            }
        }

        #endregion

        #region Member variables

        private TrsTransform3D _transform = new TrsTransform3D();

        #endregion
    }
}
