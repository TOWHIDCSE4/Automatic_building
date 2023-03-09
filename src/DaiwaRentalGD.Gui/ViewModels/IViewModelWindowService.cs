namespace DaiwaRentalGD.Gui.ViewModels
{
    /// <summary>
    /// Interface of service for view models to work with windows.
    /// </summary>
    public interface IViewModelWindowService
    {
        #region Methods

        void ShowWindow(ViewModelBase viewModel);

        void ShowChildWindow(ViewModelBase viewModel);

        #endregion
    }
}
