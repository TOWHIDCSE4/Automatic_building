using System.Collections.Generic;
using System.Linq;
using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model.SiteDesign
{
    /// <summary>
    /// View model for inputs related to site creation.
    /// </summary>
    public class SiteInputsViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public SiteInputsViewModel(GDModelScene gdms) : base(gdms)
        {
            AllSiteDesignerViewModels = new List<SiteDesignerViewModelBase>
            {
                new SampleSiteDesignerViewModel(gdms),
                new GisSiteDesignerViewModel(gdms)
            }.AsReadOnly();

            SiteDesignerViewModel =
                AllSiteDesignerViewModels.FirstOrDefault();
        }

        #endregion

        #region Methods

        #endregion

        #region Proeprties

        public override bool IsActivated
        {
            get => base.IsActivated;
            set
            {
                base.IsActivated = value;

                if (SiteDesignerViewModel != null)
                {
                    SiteDesignerViewModel.IsActivated = value;
                }
            }
        }

        public IReadOnlyList<SiteDesignerViewModelBase>
            AllSiteDesignerViewModels
        { get; }

        public SiteDesignerViewModelBase SiteDesignerViewModel
        {
            get => _siteDesignerViewModel;
            set
            {
                if (_siteDesignerViewModel != null)
                {
                    _siteDesignerViewModel.IsActivated = false;
                }

                _siteDesignerViewModel = value;

                if (_siteDesignerViewModel != null)
                {
                    _siteDesignerViewModel.IsActivated = true;
                }

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private SiteDesignerViewModelBase _siteDesignerViewModel;

        #endregion
    }
}
