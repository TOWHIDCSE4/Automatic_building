using System;
using System.Globalization;
using System.Windows;

namespace DaiwaRentalGD.Gui.ViewModels
{
    /// <summary>
    /// Service for view models to work with views that are of
    /// <see cref="System.Windows.Window"/> subtypes.
    /// </summary>
    public class ViewModelWindowService : IViewModelWindowService
    {
        #region Constructors

        public ViewModelWindowService() : this(null)
        { }

        public ViewModelWindowService(Window currentWindow)
        {
            CurrentWindow = currentWindow;
        }

        #endregion

        #region Methods

        public static Type GetViewType(Type viewModelType)
        {
            var viewModelName = viewModelType.FullName;
            var viewName = viewModelName.Replace("ViewModel", "View");

            var uiAssemblyName = viewModelType.Assembly.FullName;

            var viewQualifiedName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}, {1}",
                viewName, uiAssemblyName
            );

            var viewType = Type.GetType(viewQualifiedName);

            return viewType;
        }

        private Window CreateWindow(ViewModelBase viewModel)
        {
            var viewType = GetViewType(viewModel.GetType());

            if (viewType == null)
            {
                throw new ArgumentException(
                    "Unsupported view model type",
                    nameof(viewModel)
                );
            }

            if (!viewType.IsSubclassOf(typeof(Window)))
            {
                throw new ArgumentException(
                    $"{nameof(viewType)} is not a subclass of " +
                    $"{nameof(Window)}",
                    nameof(viewType)
                );
            }

            var view = Activator.CreateInstance(viewType);

            var window = view as Window;

            window.DataContext = viewModel;

            return window;
        }

        public void ShowWindow(ViewModelBase viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var window = CreateWindow(viewModel);

            window.Show();
        }

        public void ShowChildWindow(ViewModelBase viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var window = CreateWindow(viewModel);

            window.Owner = CurrentWindow;

            window.Show();
        }

        #endregion

        #region Properties

        public Window CurrentWindow { get; }

        #endregion
    }
}
