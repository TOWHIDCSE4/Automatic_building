using System.IO;
using System.Windows;
using System.Windows.Input;
using DaiwaRentalGD.Gui.Utilities.Configs;
using DaiwaRentalGD.Gui.Utilities.WorkingJsonFile;
using DaiwaRentalGD.Gui.ViewModels;
using DaiwaRentalGD.Gui.ViewModels.Model;
using DaiwaRentalGD.Gui.ViewModels.Model.SiteDesign;
using DaiwaRentalGD.Model.Common;
using DaiwaRentalGD.Model.Samples;

namespace DaiwaRentalGD.Gui.Views.Model
{
    /// <summary>
    /// Interaction logic for GDModelSceneMainView.xaml
    /// </summary>
    public partial class GDModelSceneMainView : Window
    {
        #region Constructors

        public GDModelSceneMainView()
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
            if (GDModelSceneMainViewModel != null)
            {
                GDModelSceneMainViewModel.WindowService =
                    new ViewModelWindowService(this);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (Content as FrameworkElement)?.Focus();
        }       

        #endregion

        #region Properties

        public GDModelSceneMainViewModel GDModelSceneMainViewModel
        {
            get => DataContext as GDModelSceneMainViewModel;
            set => DataContext = value;
        }

        #endregion
    }
}
