using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.SiteDesign
{
    /// <summary>
    /// A scene object that contains the components for designing
    /// various aspects of a site.
    /// </summary>
    [Serializable]
    public class SiteDesigner : SceneObject
    {
        #region Constructors

        public SiteDesigner() : base()
        {
            InitializeComponents();
        }

        protected SiteDesigner(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _siteCreatorComponent = reader.GetValue<SiteCreatorComponent>(
                nameof(SiteCreatorComponent)
            );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(SiteCreatorComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(SiteCreatorComponent), _siteCreatorComponent
            );
        }

        #endregion

        #region Properties

        public SiteCreatorComponent SiteCreatorComponent
        {
            get => _siteCreatorComponent;
            set
            {
                ReplaceComponent(_siteCreatorComponent, value);
                _siteCreatorComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private SiteCreatorComponent _siteCreatorComponent =
            new SiteCreatorComponent();

        #endregion
    }
}
