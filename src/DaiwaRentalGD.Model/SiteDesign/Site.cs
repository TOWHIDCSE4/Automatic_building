using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Physics;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.SiteDesign
{
    /// <summary>
    /// A scene object representing a site.
    /// </summary>
    [Serializable]
    public class Site : SceneObject
    {
        #region Constructors

        public Site() : base()
        {
            InitializeComponents();
        }

        protected Site(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _transformComponent = reader.GetValue<TransformComponent>(
                nameof(TransformComponent)
            );

            _collisionBody2DComponent =
                reader.GetValue<CollisionBody2DComponent>(
                    nameof(CollisionBody2DComponent)
                );

            _siteComponent =
                reader.GetValue<SiteComponent>(nameof(SiteComponent));
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(TransformComponent);
            AddComponent(CollisionBody2DComponent);
            AddComponent(SiteComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(TransformComponent), _transformComponent);
            writer.AddValue(
                nameof(CollisionBody2DComponent), _collisionBody2DComponent
            );
            writer.AddValue(nameof(SiteComponent), _siteComponent);
        }

        #endregion

        #region Properties

        public TransformComponent TransformComponent
        {
            get => _transformComponent;
            set
            {
                ReplaceComponent(_transformComponent, value);
                _transformComponent = value;

                NotifyPropertyChanged();
            }
        }

        public CollisionBody2DComponent CollisionBody2DComponent
        {
            get => _collisionBody2DComponent;
            set
            {
                ReplaceComponent(_collisionBody2DComponent, value);
                _collisionBody2DComponent = value;

                NotifyPropertyChanged();
            }
        }

        public SiteComponent SiteComponent
        {
            get => _siteComponent;
            set
            {
                ReplaceComponent(_siteComponent, value);
                _siteComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private TransformComponent _transformComponent =
            new TransformComponent();

        private CollisionBody2DComponent _collisionBody2DComponent =
            new CollisionBody2DComponent();

        private SiteComponent _siteComponent =
            new SiteComponent();

        #endregion
    }
}
