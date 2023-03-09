using System;

namespace DaiwaRentalGD.Scene
{
    /// <summary>
    /// Event arguments for event <see cref="SceneObject.Removed"/>.
    /// </summary>
    public class SceneObjectRemovedEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SceneObjectRemovedEventArgs"/>.
        /// </summary>
        /// 
        /// <param name="scene">
        /// The <see cref="Scene"/> where <see cref="SceneObject.Added"/>
        /// event occurs.
        /// </param>
        /// 
        /// <remarks>
        /// Please see
        /// <see cref="SceneObjectRemovedEventArgs(Scene, SceneObject)"/>
        /// for possible exceptions to be thrown.
        /// </remarks>
        public SceneObjectRemovedEventArgs(Scene scene) : this(scene, null)
        { }

        /// <summary>
        /// Creates an instance of <see cref="SceneObjectRemovedEventArgs"/>.
        /// </summary>
        /// 
        /// <param name="scene">
        /// The <see cref="Scene"/> where <see cref="SceneObject.Removed"/>
        /// event occurs.
        /// </param>
        /// <param name="replacingSceneObject">
        /// The <see cref="SceneObject"/> that replaced
        /// the removed <see cref="SceneObject"/>
        /// if <see cref="SceneObject.Removed"/> event occurs as a result of
        /// <see cref="Scene.ReplaceSceneObject"/>.
        /// Pass <see langword="null"/> if not.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="scene"/> is <see langword="null"/>.
        /// </exception>
        public SceneObjectRemovedEventArgs(
            Scene scene, SceneObject replacingSceneObject
        ) : base()
        {
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));

            ReplacingSceneObject = replacingSceneObject;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The <see cref="Scene"/> where <see cref="SceneObject.Removed"/>
        /// event occurs.
        /// </summary>
        public Scene Scene { get; }

        /// <summary>
        /// The <see cref="SceneObject"/> that replaced
        /// the removed <see cref="SceneObject"/>
        /// if <see cref="SceneObject.Removed"/> event occurs as a result of
        /// <see cref="Scene.ReplaceSceneObject"/>.
        /// Returns <see langword="null"/> if not.
        /// </summary>
        public SceneObject ReplacingSceneObject { get; }

        #endregion
    }
}
