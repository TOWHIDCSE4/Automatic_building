using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.SiteDesign
{
    /// <summary>
    /// A component that creates a site.
    /// </summary>
    [Serializable]
    public class SiteCreatorComponent : Component
    {
        #region Constructors

        public SiteCreatorComponent() : base()
        { }

        protected SiteCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _site = reader.GetValue<Site>(nameof(Site));
        }

        #endregion

        #region Methods

        public virtual void UpdateSite(Site site)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            site.SiteComponent.ClearSite();
        }

        protected override void Update()
        {
            if (Site == null) { return; }

            UpdateSite(Site);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Site), _site);
        }

        #endregion

        #region Properties

        public Site Site
        {
            get => _site;
            set
            {
                _site = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private Site _site;

        #endregion
    }
}
