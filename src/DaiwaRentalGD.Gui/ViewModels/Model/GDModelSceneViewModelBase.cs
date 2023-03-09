using System;
using DaiwaRentalGD.Model;

namespace DaiwaRentalGD.Gui.ViewModels.Model
{
    /// <summary>
    /// Base class for view models based on
    /// a <see cref="DaiwaRentalGD.Model.GDModelScene"/> instance.
    /// </summary>
    public class GDModelSceneViewModelBase : ViewModelBase
    {
        #region Construtctors

        public GDModelSceneViewModelBase(GDModelScene gdms) : base()
        {
            GDModelScene = gdms ??
                throw new ArgumentNullException(nameof(gdms));

            GDModelScene.Updated += GDModelSceneUpdatedEventHandler;
        }

        #endregion

        #region Methods

        protected virtual void GDModelSceneUpdatedEventHandler(
            object sender, EventArgs e
        )
        {
            NotifyAllGDModelScenePropertiesChanged();
        }

        protected virtual void NotifyAllGDModelScenePropertiesChanged()
        { }

        public void UpdateGDModelSceneImmediately()
        {
            GDModelScene?.ExecuteUpdate();
        }

        public void UpdateGDModelScene()
        {
            if (!IsActivated)
            {
                return;
            }

            if (DoesUpdateImmediately)
            {
                UpdateGDModelSceneImmediately();
            }
        }

        #endregion

        #region Properties

        public GDModelScene GDModelScene { get; }

        public virtual bool IsActivated
        {
            get => _isActivated;
            set
            {
                if (_isActivated == value)
                {
                    return;
                }

                _isActivated = value;

                if (_isActivated)
                {
                    GDModelScene.Updated += GDModelSceneUpdatedEventHandler;
                }
                else
                {
                    GDModelScene.Updated -= GDModelSceneUpdatedEventHandler;
                }

                NotifyPropertyChanged();
            }
        }

        public bool DoesUpdateImmediately
        {
            get => _doesUpdateImmediately;
            set
            {
                _doesUpdateImmediately = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private bool _isActivated = DefaultIsActivated;
        private bool _doesUpdateImmediately = DefaultDoesUpdateImmediately;

        #endregion

        #region Constants

        public const bool DefaultIsActivated = true;
        public const bool DefaultDoesUpdateImmediately = true;

        #endregion
    }
}
