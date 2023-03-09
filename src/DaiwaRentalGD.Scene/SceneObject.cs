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
    /// Represents an object in
    /// a <see cref="DaiwaRentalGD.Scene.Scene"/>.
    /// A <see cref="SceneObject"/> consists of
    /// a collection of <see cref="Component"/>s attached to it,
    /// and can have a collection of children <see cref="SceneObject"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// Please refer to <i>Software Architecture Specification</i> for more
    /// information on component-based architecture and scene graphs.
    /// </remarks>
    [Serializable]
    public class SceneObject : IWorkspaceItem, INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SceneObject"/>.
        /// </summary>
        public SceneObject()
        {
            ItemInfo = new WorkspaceItemInfo();

            Children = _children.AsReadOnly();
            Components = _components.AsReadOnly();
        }

        /// <summary>
        /// Creates an instance of <see cref="SceneObject"/>
        /// with provided <see cref="Component"/>s attached.
        /// </summary>
        /// 
        /// <param name="components">
        /// <see cref="Component"/>s to be attached upon creation of
        /// this <see cref="SceneObject"/>.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="components"/> is
        /// <see langword="null"/>.
        /// </exception>
        public SceneObject(IEnumerable<Component> components) : this()
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            foreach (Component component in components)
            {
                AddComponent(component);
            }
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info">
        /// Contains data of a serialized <see cref="SceneObject"/> instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected SceneObject(
            SerializationInfo info, StreamingContext context
        )
        {
            Children = _children.AsReadOnly();
            Components = _components.AsReadOnly();

            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            _name = reader.GetValue<string>(nameof(Name));

            _scene = reader.GetValue<Scene>(nameof(Scene));

            Parent = reader.GetValue<SceneObject>(nameof(Parent));

            _children.AddRange(
                reader.GetValues<SceneObject>(nameof(Children))
            );

            _components.AddRange(
                reader.GetValues<Component>(nameof(Components))
            );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds another <see cref="SceneObject"/> as a child of
        /// this <see cref="SceneObject"/> (parenting).
        /// <paramref name="child"/> will be added at the end of
        /// <see cref="Children"/>.
        /// This <see cref="SceneObject"/> thus becomes the parent of
        /// <paramref name="child"/>.
        /// </summary>
        /// 
        /// <param name="child">
        /// The <see cref="SceneObject"/> to be added as child.
        /// </param>
        /// 
        /// <remarks>
        /// Please refer to <see cref="InsertChild(int, SceneObject)"/>
        /// for possible exceptions to be thrown.
        /// </remarks>
        public void AddChild(SceneObject child)
        {
            InsertChild(_children.Count, child);
        }

        /// <summary>
        /// <para>
        /// Adds another <see cref="SceneObject"/> as a child of
        /// this <see cref="SceneObject"/> (parenting).
        /// <paramref name="child"/> will be inserted to
        /// <see cref="Children"/> at Index <paramref name="childIndex"/>.
        /// This <see cref="SceneObject"/> thus becomes the parent of
        /// <paramref name="child"/>.
        /// </para>
        /// <para>
        /// If this <see cref="SceneObject"/> is in
        /// a <see cref="DaiwaRentalGD.Scene.Scene"/>
        /// while <paramref name="child"/> is not in any,
        /// <paramref name="child"/> is added to <see cref="Scene"/>
        /// before being parented to this <see cref="SceneObject"/>.
        /// </para>
        /// </summary>
        /// 
        /// <param name="childIndex">
        /// Index into <see cref="Children"/> at which
        /// <paramref name="child"/> will be inserted.
        /// </param>
        /// <param name="child">
        /// The <see cref="SceneObject"/> to be added as child.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="child"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item>
        ///     <description>
        ///     <paramref name="child"/> is this <see cref="SceneObject"/>
        ///     </description>
        /// </item>
        /// <item>
        ///     <description>
        ///     <paramref name="child"/> is an ancestor of
        ///     this <see cref="SceneObject"/>
        ///     </description>
        /// </item>
        /// <item>
        ///     <description>
        ///     This <see cref="SceneObject"/> is an ancestor of
        ///     <paramref name="child"/>
        ///     </description>
        /// </item>
        /// <item>
        ///     <description>
        ///     <paramref name="child"/> is already parented
        ///     </description>
        /// </item>
        /// <item>
        ///     <description>
        ///     <paramref name="child"/> is in a different
        ///     <see cref="DaiwaRentalGD.Scene.Scene"/>
        ///     that is not <see langword="null"/>
        ///     </description>
        /// </item>
        /// </list>
        /// </exception>
        public void InsertChild(int childIndex, SceneObject child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (child == this)
            {
                throw new ArgumentException(
                    $"{nameof(child)} cannot be current scene object",
                    nameof(child)
                );
            }

            if (IsAncestorOf(child))
            {
                throw new ArgumentException(
                    $"Current scene object cannot be " +
                    $"ancestor of {nameof(child)}",
                    nameof(child)
                );
            }

            if (child.IsAncestorOf(this))
            {
                throw new ArgumentException(
                    $"{nameof(child)} cannot be " +
                    $"ancestor of current scene object",
                    nameof(child)
                );
            }

            if (child.Parent != null)
            {
                throw new ArgumentException(
                    $"{nameof(child)} is already parented",
                    nameof(child)
                );
            }

            if (child.Scene != Scene)
            {
                if (child.Scene == null)
                {
                    Scene.AddSceneObject(child);
                }
                else
                {
                    throw new ArgumentException(
                        $"{nameof(child)} is in a different scene",
                        nameof(child)
                    );
                }
            }

            _children.Insert(childIndex, child);
            child.Parent = this;

            child.OnParented();
        }

        /// <summary>
        /// Removes a <see cref="SceneObject"/> from
        /// this <see cref="SceneObject"/> as a child
        /// if the former is a child of the latter,
        /// i.e. unparenting the former from the latter.
        /// </summary>
        /// 
        /// <param name="child">
        /// The <see cref="SceneObject"/> to be removed as a child.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if
        /// <paramref name="child"/> is a child of
        /// this <see cref="SceneObject"/> and removed successfully.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public bool RemoveChild(SceneObject child)
        {
            bool isRemoved = _children.Remove(child);

            if (isRemoved)
            {
                child.Parent = null;
                child.OnUnparented();
            }

            return isRemoved;
        }

        /// <summary>
        /// Gets the index of <paramref name="child"/> in
        /// <see cref="Children"/>.
        /// </summary>
        /// 
        /// <param name="child">
        /// The child <see cref="SceneObject"/> to be looked up.
        /// </param>
        /// 
        /// <returns>
        /// Index of <paramref name="child"/> in <see cref="Children"/>.
        /// <c>-1</c> if not found.
        /// </returns>
        public int GetChildIndex(SceneObject child)
        {
            return _children.IndexOf(child);
        }

        /// <summary>
        /// Checks whether this <see cref="SceneObject"/> is
        /// an ancestor of <paramref name="sceneObject"/>,
        /// i.e. its parent, its parent's parent, etc.
        /// </summary>
        /// 
        /// <param name="sceneObject">
        /// The <see cref="SceneObject"/> to be checked for whether
        /// this <see cref="SceneObject"/> is one of its ancestors.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if this <see cref="SceneObject"/> is
        /// an ancestor of <paramref name="sceneObject"/>'s.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="sceneObject"/> is
        /// <see langword="null"/>.
        /// </exception>
        public bool IsAncestorOf(SceneObject sceneObject)
        {
            if (sceneObject == null)
            {
                throw new ArgumentNullException(nameof(sceneObject));
            }

            SceneObject current = sceneObject.Parent;

            while (current != null)
            {
                if (current == this)
                {
                    return true;
                }

                current = current.Parent;
            }

            return false;
        }

        /// <summary>
        /// Adds (attaches) a <see cref="Component"/> to
        /// this <see cref="SceneObject"/>
        /// at the end of <see cref="Components"/>.
        /// </summary>
        /// 
        /// <param name="component">
        /// The <see cref="Component"/> to be added to this
        /// <see cref="SceneObject"/>.
        /// </param>
        /// 
        /// <remarks>
        /// Please refer to <see cref="InsertComponent(int, Component)"/>
        /// for possible exceptions to be thrown.
        /// </remarks>
        public void AddComponent(Component component)
        {
            InsertComponent(Components.Count, component);
        }

        /// <summary>
        /// Adds (attaches) a <see cref="Component"/> to
        /// this <see cref="SceneObject"/>
        /// at a specified index in <see cref="Components"/>.
        /// </summary>
        /// 
        /// <param name="componentIndex">
        /// The index in <see cref="Components"/> at which
        /// <paramref name="component"/> will be added.
        /// </param>
        /// <param name="component">
        /// The <see cref="Component"/> to be added to
        /// this <see cref="SceneObject"/>.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="component"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="component"/> is already attached to
        /// a <see cref="SceneObject"/>.
        /// </exception>
        public void InsertComponent(int componentIndex, Component component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (component.SceneObject != null)
            {
                throw new ArgumentException(
                    $"{nameof(component)} is already attached to " +
                    $"a scene object",
                    nameof(component)
                );
            }

            _components.Insert(componentIndex, component);

            component.SceneObject = this;

            component.OnAdded();
        }

        /// <summary>
        /// Replaces a <see cref="Component"/> attached to
        /// this <see cref="SceneObject"/> with
        /// another <see cref="Component"/>.
        /// </summary>
        /// 
        /// <param name="oldComponent">
        /// The <see cref="Component"/> attached to
        /// this <see cref="SceneObject"/>
        /// to be replaced by <paramref name="newComponent"/>.
        /// </param>
        /// <param name="newComponent">
        /// The <see cref="Component"/> to replace
        /// <paramref name="oldComponent"/>.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="oldComponent"/> is
        /// found on this <see cref="SceneObject"/> and successfully
        /// replaced by <paramref name="newComponent"/>.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="oldComponent"/> or
        /// <paramref name="newComponent"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="newComponent"/> is already attached
        /// to a <see cref="SceneObject"/>.
        /// </exception>
        public bool ReplaceComponent(
            Component oldComponent, Component newComponent
        )
        {
            if (oldComponent == null)
            {
                throw new ArgumentNullException(nameof(oldComponent));
            }

            if (newComponent == null)
            {
                throw new ArgumentNullException(nameof(newComponent));
            }

            if (newComponent.SceneObject != null)
            {
                throw new ArgumentException(
                    $"{nameof(newComponent)} is already attached to " +
                    "a scene object",
                    nameof(newComponent)
                );
            }

            int oldComponentIndex = _components.IndexOf(oldComponent);

            if (oldComponentIndex == -1)
            {
                return false;
            }

            RemoveComponent(oldComponent);

            InsertComponent(oldComponentIndex, newComponent);

            return true;
        }

        /// <summary>
        /// Removes a <see cref="Component"/> from
        /// this <see cref="SceneObject"/>.
        /// </summary>
        /// 
        /// <param name="component">
        /// The <see cref="Component"/> to be removed.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="component"/>
        /// is found on this <see cref="SceneObject"/> and removed.
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public bool RemoveComponent(Component component)
        {
            bool isRemoved = _components.Remove(component);

            if (isRemoved)
            {
                component.SceneObject = null;
                component.OnRemoved();
            }

            return isRemoved;
        }

        /// <summary>
        /// Gets the index of a <see cref="Component"/> in
        /// <see cref="Components"/>.
        /// </summary>
        /// 
        /// <param name="component">
        /// The <see cref="Component"/> to be looked up.
        /// </param>
        /// 
        /// <returns>
        /// The index of <paramref name="component"/> in
        /// <see cref="Components"/>. -1 if not found.
        /// </returns>
        public int GetComponentIndex(Component component)
        {
            return _components.IndexOf(component);
        }

        /// <summary>
        /// Gets the first <see cref="Component"/>
        /// of type <typeparamref name="T"/>
        /// attached to this <see cref="SceneObject"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of <see cref="Component"/> to be looked up.
        /// </typeparam>
        /// 
        /// <returns>
        /// The first <see cref="Component"/> of type <typeparamref name="T"/>
        /// attached to this <see cref="SceneObject"/>, if exists.
        /// Otherwise <see langword="null"/>.
        /// </returns>
        public T GetComponent<T>() where T : Component
        {
            return _components
                .FirstOrDefault(component => component is T) as T;
        }

        /// <summary>
        /// Gets all <see cref="Component"/>s of type <typeparamref name="T"/>
        /// attached to this <see cref="SceneObject"/>
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of <see cref="Component"/> to be looked up.
        /// </typeparam>
        /// 
        /// <returns>
        /// A list of all <see cref="Component"/>s
        /// of type <typeparamref name="T"/>
        /// attached to this <see cref="SceneObject"/>.
        /// If none exists, an empty list is returned.
        /// </returns>
        public IReadOnlyList<T> GetComponents<T>() where T : Component
        {
            return _components
                .Where(component => component is T)
                .OfType<T>().ToList();
        }

        /// <summary>
        /// Executes the update logic defined in <see cref="Update"/>,
        /// changes the state of the <see cref="SceneObject"/> accordingly
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
        /// The update logic of the <see cref="SceneObject"/>.
        /// Executes the logic of every <see cref="Component"/> attached
        /// in the order they are in <see cref="Components"/>.
        /// Can be overridden to include additional logic.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Typically, a <see cref="SceneObject"/> is mainly a container for
        /// a collection of <see cref="Component"/>s which include
        /// the actual business logic. It is possible to override
        /// <see cref="Update"/> to perform work other than calling
        /// <see cref="Component.ExecuteUpdate"/> on
        /// all <see cref="Components"/> though.
        /// </para>
        /// <para>
        /// Please make sure that if a <see cref="Component"/> depends
        /// on another in order to update, the latter should be updated before
        /// the former, thus should appear before the former in
        /// <see cref="Components"/>.
        /// </para>
        /// </remarks>
        protected virtual void Update()
        {
            foreach (Component component in Components.ToList())
            {
                component.ExecuteUpdate();
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
        /// Logic to be run after this <see cref="SceneObject"/> is
        /// parented to another <see cref="SceneObject"/>.
        /// </summary>
        protected internal virtual void OnParented()
        {
            foreach (var component in Components)
            {
                component.OnSceneObjectParented();
            }
        }

        /// <summary>
        /// Logic to be run after this <see cref="SceneObject"/> is
        /// unparented from another <see cref="SceneObject"/>.
        /// </summary>
        protected internal virtual void OnUnparented()
        {
            foreach (var component in Components)
            {
                component.OnSceneObjectUnparented();
            }
        }

        /// <summary>
        /// Logic to be run when this <see cref="SceneObject"/> is added to
        /// <see cref="Scene"/>, such as raising
        /// <see cref="Added"/> event.
        /// </summary>
        /// 
        /// <param name="e">
        /// Includes data of adding this <see cref="SceneObject"/> to
        /// <see cref="Scene"/>.
        /// </param>
        protected internal virtual void OnAdded(
            SceneObjectAddedEventArgs e
        )
        {
            Added?.Invoke(this, e);
        }

        /// <summary>
        /// Logic to be run when this <see cref="SceneObject"/> is
        /// removed from <see cref="Scene"/>, such as raising
        /// <see cref="Removed"/> event.
        /// </summary>
        /// 
        /// <param name="e">
        /// Includes data of removing this <see cref="SceneObject"/> from
        /// <see cref="Scene"/>.
        /// </param>
        protected internal virtual void OnRemoved(
            SceneObjectRemovedEventArgs e
        )
        {
            Removed?.Invoke(this, e);
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
            Enumerable.Empty<IWorkspaceItem>()
            .Append(_scene)
            .Append(Parent).Concat(_children)
            .Concat(_components);

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Name), _name);

            writer.AddValue(nameof(Scene), _scene);

            writer.AddValue(nameof(Parent), Parent);
            writer.AddValues(nameof(Children), _children);

            writer.AddValues(nameof(Components), _components);

            writer.AddValue(nameof(IsEnabled), _isEnabled);
            writer.AddValue(nameof(IsExecutingUpdate), _isExecutingUpdate);
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public WorkspaceItemInfo ItemInfo { get; }

        /// <summary>
        /// Name of this <see cref="SceneObject"/>.
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
        /// The <see cref="DaiwaRentalGD.Scene.Scene"/> that
        /// this <see cref="SceneObject"/> is in.
        /// Setting the property on this <see cref="Scene"/> will also set
        /// the <see cref="Scene"/> property of all of its
        /// <see cref="Descendants"/>.
        /// </summary>
        public Scene Scene
        {
            get => _scene;
            internal set
            {
                _scene = value;

                foreach (SceneObject child in Children)
                {
                    child.Scene = value;
                }
            }
        }

        /// <summary>
        /// The parent <see cref="SceneObject"/> of
        /// this <see cref="SceneObject"/>, i.e.
        /// this <see cref="SceneObject"/> is a child of
        /// <see cref="Parent"/>.
        /// </summary>
        public SceneObject Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// The children <see cref="SceneObject"/>s of
        /// this <see cref="SceneObject"/>, i.e.
        /// the collection of <see cref="SceneObject"/>s to which
        /// this <see cref="SceneObject"/> is the <see cref="Parent"/>.
        /// </summary>
        public IReadOnlyList<SceneObject> Children { get; }

        /// <summary>
        /// The descendant <see cref="SceneObject"/>s of
        /// this <see cref="SceneObject"/>, i.e.
        /// the collection of <see cref="SceneObject"/>s to which
        /// this <see cref="SceneObject"/> is an ancestor.
        /// </summary>
        public IReadOnlyList<SceneObject> Descendants
        {
            get
            {
                List<SceneObject> descendants = new List<SceneObject>();

                Queue<SceneObject> queue = new Queue<SceneObject>(Children);

                while (queue.Count > 0)
                {
                    SceneObject current = queue.Dequeue();

                    descendants.Add(current);

                    foreach (SceneObject child in current.Children)
                    {
                        queue.Enqueue(child);
                    }
                }

                return descendants;
            }
        }

        /// <summary>
        /// The <see cref="Component"/>s that are attached to
        /// this <see cref="SceneObject"/>.
        /// </summary>
        public IReadOnlyList<Component> Components { get; }

        /// <summary>
        /// Whether the <see cref="SceneObject"/> is enabled.
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
        /// Whether the <see cref="SceneObject"/> is currently executing
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
        public event EventHandler Updated;

        /// <summary>
        /// Occurs after this <see cref="SceneObject"/> is added to
        /// <see cref="Scene"/>.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<SceneObjectAddedEventArgs> Added;

        /// <summary>
        /// Occurs after this <see cref="SceneObject"/> is removed from
        /// <see cref="Scene"/>.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<SceneObjectRemovedEventArgs> Removed;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Fields

        private string _name = DefaultName;

        private Scene _scene;

        private readonly List<SceneObject> _children =
            new List<SceneObject>();

        private readonly List<Component> _components =
            new List<Component>();

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
