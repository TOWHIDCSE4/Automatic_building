using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Scene
{
    /// <summary>
    /// Represents a component that can be attached to
    /// a <see cref="DaiwaRentalGD.Scene.SceneObject"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// Please refer to <i>Software Architecture Specification</i> for more
    /// information on component-based architecture and scene graphs.
    /// </remarks>
    [Serializable]
    public class Component : IWorkspaceItem, INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="Component"/>.
        /// </summary>
        public Component()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info">
        /// Contains data of a serialized <see cref="Component"/> instance.
        /// </param>
        /// <param name="context">
        /// Context information for deserialization.
        /// </param>
        protected Component(SerializationInfo info, StreamingContext context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            _name = reader.GetValue<string>(nameof(Name));

            SceneObject = reader.GetValue<SceneObject>(nameof(SceneObject));

            _isEnabled = reader.GetValue<bool>(nameof(IsEnabled));
            _isExecutingUpdate =
                reader.GetValue<bool>(nameof(IsExecutingUpdate));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the update logic defined in <see cref="Update"/>,
        /// changes the state of the <see cref="Component"/> accordingly
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
        /// The update logic of the <see cref="Component"/>.
        /// Should be overridden by subclasses to include
        /// actual functionality.
        /// </summary>
        protected virtual void Update()
        { }

        /// <summary>
        /// Logic to be run after <see cref="Update"/> is called,
        /// such as raising update-related events.
        /// </summary>
        protected internal virtual void OnUpdated()
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Logic to be run after the <see cref="Component"/> is added
        /// to <see cref="SceneObject"/>.
        /// </summary>
        protected internal virtual void OnAdded()
        { }

        /// <summary>
        /// Logic to be run after the <see cref="Component"/> is removed
        /// from <see cref="SceneObject"/>.
        /// </summary>
        protected internal virtual void OnRemoved()
        { }

        /// <summary>
        /// Logic to be run after <see cref="SceneObject"/> is parented to
        /// another <see cref="DaiwaRentalGD.Scene.SceneObject"/>.
        /// </summary>
        protected internal virtual void OnSceneObjectParented()
        { }

        /// <summary>
        /// Logic to be run after <see cref="SceneObject"/> is unparented
        /// from another <see cref="DaiwaRentalGD.Scene.SceneObject"/>.
        /// </summary>
        protected internal virtual void OnSceneObjectUnparented()
        { }

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
            new[] { SceneObject };

        /// <inheritdoc/>
        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Name), _name);

            writer.AddValue(nameof(SceneObject), SceneObject);

            writer.AddValue(nameof(IsEnabled), _isEnabled);
            writer.AddValue(nameof(IsExecutingUpdate), _isExecutingUpdate);
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public WorkspaceItemInfo ItemInfo { get; }

        /// <summary>
        /// Name of this <see cref="Component"/>.
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
        /// The <see cref="DaiwaRentalGD.Scene.SceneObject"/>
        /// that this <see cref="Component"/> is attached to.
        /// </summary>
        public SceneObject SceneObject
        {
            get;
            internal set;
        }

        /// <summary>
        /// Whether the <see cref="Component"/> is enabled.
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
        /// Whether the <see cref="Component"/> is currently executing
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
