using System.Windows;
using System.Windows.Input;
using DaiwaRentalGD.Gui.ViewModels;
using DaiwaRentalGD.Gui.ViewModels.Optimization;

namespace DaiwaRentalGD.Gui.Views.Optimization
{
    /// <summary>
    /// Interaction logic for OptimizationMainView.xaml
    /// </summary>
    public partial class OptimizationMainView : Window
    {
        #region Constructors

        public OptimizationMainView()
        {
            InitializeComponent();

            if (Content is FrameworkElement fe)
            {
                fe.Focusable = true;
            }
        }

        #endregion

        #region Methods

        private void Window_DataContextChanged(
            object sender, DependencyPropertyChangedEventArgs e
        )
        {
            if (OptimizationMainViewModel != null)
            {
                OptimizationMainViewModel.WindowService =
                    new ViewModelWindowService(this);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (Content as UIElement)?.Focus();
        }

        #endregion

        #region Properties

        public OptimizationMainViewModel OptimizationMainViewModel
        {
            get => DataContext as OptimizationMainViewModel;
            set => DataContext = value;
        }

        #endregion
    }
}
