using System;

namespace DaiwaRentalGD.Scene
{
    /// <summary>
    /// Event arguments for event <see cref="SceneObject.Added"/>.
    /// </summary>
    public class SceneObjectAddedEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SceneObjectAddedEventArgs"/>.
        /// </summary>
        /// 
        /// <param name="scene">
        /// The <see cref="Scene"/> where <see cref="SceneObject.Added"/>
        /// event occurs.
        /// </param>
        /// 
        /// <remarks>
        /// Please see
        /// <see cref="SceneObjectAddedEventArgs(Scene, SceneObject)"/>
        /// for possible exceptions to be thrown.
        /// </remarks>
        public SceneObjectAddedEventArgs(Scene scene) : this(scene, null)
        { }

        /// <summary>
        /// Creates an instance of <see cref="SceneObjectAddedEventArgs"/>.
        /// </summary>
        /// 
        /// <param name="scene">
        /// The <see cref="Scene"/> where <see cref="SceneObject.Added"/>
        /// event occurs.
        /// </param>
        /// <param name="replacedSceneObject">
        /// The <see cref="SceneObject"/> that is replaced by
        /// the added <see cref="SceneObject"/>
        /// if <see cref="SceneObject.Added"/> event occurs as a result of
        /// <see cref="Scene.ReplaceSceneObject"/>.
        /// Pass <see langword="null"/> if not.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="scene"/> is <see langword="null"/>.
        /// </exception>
        public SceneObjectAddedEventArgs(
            Scene scene, SceneObject replacedSceneObject
        ) : base()
        {
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));

            ReplacedSceneObject = replacedSceneObject;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The <see cref="Scene"/> where <see cref="SceneObject.Added"/>
        /// event occurs.
        /// </summary>
        public Scene Scene { get; }

        /// <summary>
        /// The <see cref="SceneObject"/> that is replaced by
        /// the added <see cref="SceneObject"/>
        /// if <see cref="SceneObject.Added"/> event occurs as a result of
        /// <see cref="Scene.ReplaceSceneObject"/>.
        /// Returns <see langword="null"/> if not.
        /// </summary>
        public SceneObject ReplacedSceneObject { get; }

        #endregion
    }
}
