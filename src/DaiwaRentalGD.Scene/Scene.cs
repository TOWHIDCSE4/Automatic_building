using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Scene
{
    /// <summary>
    /// Represents a scene containing a collection of
    /// <see cref="SceneObject"/>s that will work together as a system
    /// by referencing each other and updating in order.
    /// </summary>
    [Serializable]
    public class Scene : IWorkspaceItem, INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="Scene"/>.
        /// </summary>
        public Scene()
        {
            ItemInfo = new WorkspaceItemInfo();

            SceneObjects = _sceneObjects.AsReadOnly();
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info">
        /// Contains data of a serialized <see cref="Scene"/> instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected Scene(SerializationInfo info, StreamingContext context)
        {
            SceneObjects = _sceneObjects.AsReadOnly();

            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            _name = reader.GetValue<string>(nameof(Name));

            _sceneObjects.AddRange(
                reader.GetValues<SceneObject>(nameof(SceneObjects))
            );

            _isEnabled = reader.GetValue<bool>(nameof(IsEnabled));

            _isExecutingUpdate =
                reader.GetValue<bool>(nameof(IsExecutingUpdate));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a <see cref="SceneObject"/> to this <see cref="Scene"/>
        /// at the end of <see cref="SceneObjects"/>.
        /// </summary>
        /// 
        /// <param name="sceneObject">
        /// The <see cref="SceneObject"/> to be added.
        /// </param>
        /// 
        /// <remarks>
        /// Please refer to <see cref="InsertSceneObject"/> for
        /// possible exceptions to be thrown.
        /// </remarks>
        public void AddSceneObject(SceneObject sceneObject)
        {
            InsertSceneObject(_sceneObjects.Count, sceneObject);
        }

        /// <summary>
        /// Adds a <see cref="SceneObject"/> to this <see cref="Scene"/>
        /// at a specified index in <see cref="SceneObjects"/>.
        /// </summary>
        /// 
        /// <param name="sceneObjectIndex">
        /// The index in <see cref="SceneObjects"/> at which
        /// <paramref name="sceneObject"/> will be added.
        /// </param>
        /// <param name="sceneObject">
        /// The <see cref="SceneObject"/> to be added.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="sceneObject"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="sceneObject"/> is already in
        /// a <see cref="Scene"/>.
        /// </exception>
        public void InsertSceneObject(
            int sceneObjectIndex, SceneObject sceneObject
        )
        {
            // Validate parameters

            if (sceneObject == null)
            {
                throw new ArgumentNullException(nameof(sceneObject));
            }

            if (sceneObject.Scene != null)
            {
                throw new ArgumentException(
                    $"{nameof(sceneObject)} is already in a scene",
                    nameof(sceneObject)
                );
            }

            // Insert scene object

            _sceneObjects.Insert(sceneObjectIndex, sceneObject);

            // Add descendants

            foreach (var descendant in sceneObject.Descendants)
            {
                _sceneObjects.Add(descendant);
            }

            // Update scene

            sceneObject.Scene = this;

            // Invoke event

            sceneObject.OnAdded(new SceneObjectAddedEventArgs(this));

            foreach (var descendant in sceneObject.Descendants)
            {
                descendant.OnAdded(new SceneObjectAddedEventArgs(this));
            }
        }

        /// <summary>
        /// Replace a <see cref="SceneObject"/> in this <see cref="Scene"/>
        /// with one not in this <see cref="Scene"/>.
        /// </summary>
        /// 
        /// <param name="oldSceneObject">
        /// The <see cref="SceneObject"/> in this <see cref="Scene"/> to be
        /// replaced by <paramref name="newSceneObject"/>.
        /// </param>
        /// <param name="newSceneObject">
        /// The <see cref="SceneObject"/> to be added to
        /// <see cref="Scene"/> and replace <paramref name="oldSceneObject"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="oldSceneObject"/>
        /// is found in this <see cref="Scene"/> and successfully replaced by
        /// <paramref name="newSceneObject"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="oldSceneObject"/> or
        /// <paramref name="newSceneObject"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if:
        /// <list type="bullet">
        /// <item>
        ///     <description>
        ///     <paramref name="newSceneObject"/> is already in
        ///     a <see cref="Scene"/>
        ///     </description>
        /// </item>
        /// <item>
        ///     <description>
        ///     <paramref name="newSceneObject"/> has any child
        ///     <see cref="SceneObject"/>
        ///     </description>
        /// </item>
        /// </list>
        /// </exception>
        public bool ReplaceSceneObject(
            SceneObject oldSceneObject, SceneObject newSceneObject
        )
        {
            // Validate parameters

            if (oldSceneObject == null)
            {
                throw new ArgumentNullException(nameof(oldSceneObject));
            }

            if (newSceneObject == null)
            {
                throw new ArgumentNullException(nameof(newSceneObject));
            }

            if (newSceneObject.Scene != null)
            {
                throw new ArgumentException(
                    $"{nameof(newSceneObject)} is already in a scene",
                    nameof(newSceneObject)
                );
            }

            if (newSceneObject.Children.Any())
            {
                throw new ArgumentException(
                    $"{nameof(newSceneObject)} cannot have children",
                    nameof(newSceneObject)
                );
            }

            int oldSceneObjectIndex = _sceneObjects.IndexOf(oldSceneObject);

            if (oldSceneObjectIndex == -1)
            {
                return false;
            }

            // Keep parent and children info

            SceneObject oldParent = oldSceneObject.Parent;

            int oldChildIndex =
                oldParent?.GetChildIndex(oldSceneObject) ?? -1;

            var oldChildren = oldSceneObject.Children.ToList();

            // Disconnect and remove old scene object

            oldParent?.RemoveChild(oldSceneObject);

            foreach (var oldChild in oldChildren)
            {
                oldSceneObject.RemoveChild(oldChild);
            }

            _sceneObjects.Remove(oldSceneObject);
            oldSceneObject.Scene = null;

            // Add and connect new scene object

            _sceneObjects.Insert(oldSceneObjectIndex, newSceneObject);
            newSceneObject.Scene = this;

            oldParent?.InsertChild(oldChildIndex, newSceneObject);

            foreach (var oldChild in oldChildren)
            {
                newSceneObject.AddChild(oldChild);
            }

            oldSceneObject.OnRemoved(
                new SceneObjectRemovedEventArgs(this, newSceneObject)
            );

            newSceneObject.OnAdded(
                new SceneObjectAddedEventArgs(this, oldSceneObject)
            );

            return true;
        }

        /// <summary>
        /// Removes a <see cref="SceneObject"/> from this <see cref="Scene"/>.
        /// </summary>
        /// 
        /// <param name="sceneObject">
        /// The <see cref="SceneObject"/> to be removed.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="sceneObject"/>
        /// is found in this <see cref="Scene"/> and successfully removed.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public bool RemoveSceneObject(SceneObject sceneObject)
        {
            // Remove scene object

            bool isRemoved = _sceneObjects.Remove(sceneObject);

            if (!isRemoved)
            {
                return isRemoved;
            }

            // Unparent

            if (sceneObject.Parent != null)
            {
                sceneObject.Parent.RemoveChild(sceneObject);
            }

            // Remove descendants

            foreach (SceneObject descendant in sceneObject.Descendants)
            {
                _sceneObjects.Remove(descendant);
            }

            // Update scene

            sceneObject.Scene = null;

            // Invoke events

            sceneObject.OnRemoved(new SceneObjectRemovedEventArgs(this));

            foreach (var descendant in sceneObject.Descendants)
            {
                descendant.OnRemoved(new SceneObjectRemovedEventArgs(this));
            }

            return true;
        }

        /// <summary>
        /// Gets the index of a <see cref="SceneObject"/> in
        /// <see cref="SceneObjects"/>.
        /// </summary>
        /// 
        /// <param name="sceneObject">
        /// The <see cref="SceneObject"/> to be looked up.
        /// </param>
        /// 
        /// <returns>
        /// Index of <paramref name="sceneObject"/> in
        /// <see cref="SceneObjects"/> if found.
        /// Otherwise <c>-1</c>.
        /// </returns>
        public int GetSceneObjectIndex(SceneObject sceneObject)
        {
            return _sceneObjects.IndexOf(sceneObject);
        }

        /// <summary>
        /// Executes the update logic defined in <see cref="Update"/>,
        /// changes the state of the <see cref="Scene"/> accordingly
        /// and raises update-related events.
        /// </summary>
        /// 
        /// <remarks>
        /// This method is a no-op if the component is disabled
        /// or is currently executing the update logic.
        /// </remarks>
        public void ExecuteUpdate()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (IsExecutingUpdate)
            {
                return;
            }

            IsExecutingUpdate = true;

            Update();

            OnUpdated();

            IsExecutingUpdate = false;
        }

        /// <summary>
        /// The update logic of the <see cref="Scene"/>.
        /// Executes the logic of every <see cref="SceneObject"/>
        /// in the order they are in <see cref="SceneObjects"/>.
        /// Can be overridden to include additional logic.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Typically, a <see cref="Scene"/> is mainly a container for
        /// a collection of <see cref="SceneObject"/>s which work together
        /// as a system.
        /// It is possible to override
        /// <see cref="Update"/> to perform work other than calling
        /// <see cref="SceneObject.ExecuteUpdate"/> on
        /// all <see cref="SceneObjects"/> though.
        /// </para>
        /// <para>
        /// Please make sure that if a <see cref="SceneObject"/> depends
        /// on another in order to update, the latter should be updated before
        /// the former, thus should appear before the former in
        /// <see cref="SceneObjects"/>.
        /// </para>
        /// </remarks>
        protected virtual void Update()
        {
            foreach (SceneObject sceneObject in SceneObjects.ToList())
            {
                sceneObject.ExecuteUpdate();
            }
        }

        /// <summary>
        /// Logic to be run after <see cref="Update"/> is called,
        /// such as raising <see cref="Updated"/> event.
        /// </summary>
        protected internal virtual void OnUpdated()
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> event
        /// when the value of a property is changed.
        /// </summary>
        /// 
        /// <param name="propertyName">
        /// Name of the property whose value is changed.
        /// </param>
        protected void NotifyPropertyChanged(
            [CallerMemberName] string propertyName = ""
        )
        {
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(propertyName)
            );
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>().Concat(_sceneObjects);

        /// <inheritdoc/>
        public virtual  void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Name), _name);

            writer.AddValues(nameof(SceneObjects), _sceneObjects);

            writer.AddValue(nameof(IsEnabled), _isEnabled);
            writer.AddValue(nameof(IsExecutingUpdate), _isExecutingUpdate);
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public WorkspaceItemInfo ItemInfo { get; }

        /// <summary>
        /// Name of this <see cref="Scene"/>.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public string Name
        {
            get => _name;
            set
            {
                _name = value ??
                    throw new ArgumentNullException(nameof(value));

                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The collection of <see cref="SceneObject"/>s in
        /// this <see cref="Scene"/>.
        /// </summary>
        public IReadOnlyList<SceneObject> SceneObjects { get; }

        /// <summary>
        /// The collection of root-level <see cref="SceneObject"/>s
        /// in this scene, i.e. <see cref="SceneObject"/>s that are
        /// not parented to another.
        /// </summary>
        public IReadOnlyList<SceneObject> RootSceneObjects =>
            SceneObjects
            .Where(sceneObject => sceneObject.Parent == null)
            .ToList();

        /// <summary>
        /// Whether the <see cref="Scene"/> is enabled.
        /// <see cref="ExecuteUpdate"/> becomes a no-op
        /// when this property is <see langword="false"/>.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Whether the <see cref="Scene"/> is currently executing
        /// the update logic, i.e. in the middle of a call to
        /// <see cref="ExecuteUpdate"/>. This is to prevent
        /// directly or indirectly nested calls to <see cref="Update"/>.
        /// </summary>
        public bool IsExecutingUpdate
        {
            get => _isExecutingUpdate;
            private set
            {
                _isExecutingUpdate = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs after <see cref="Update"/> returns,
        /// i.e. at the end of <see cref="ExecuteUpdate"/>.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<EventArgs> Updated;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Fields

        private string _name = DefaultName;

        private readonly List<SceneObject> _sceneObjects =
            new List<SceneObject>();

        private bool _isEnabled = DefaultIsEnabled;

        private bool _isExecutingUpdate;

        #endregion

        #region Constants

        /// <summary>
        /// Default value of <see cref="Name"/>.
        /// </summary>
        public const string DefaultName = "";

        /// <summary>
        /// Default value of <see cref="IsEnabled"/>.
        /// </summary>
        public const bool DefaultIsEnabled = true;

        #endregion
    }
}
