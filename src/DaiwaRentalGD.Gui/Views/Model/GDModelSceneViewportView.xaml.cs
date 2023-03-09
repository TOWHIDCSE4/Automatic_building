using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Gui.ViewModels.Model;
using DaiwaRentalGD.Gui.Visualization3D;

namespace DaiwaRentalGD.Gui.Views.Model
{
    /// <summary>
    /// Interaction logic for GDModelSceneViewportView.xaml
    /// </summary>
    public partial class GDModelSceneViewportView : UserControl
    {
        #region Constructors

        public GDModelSceneViewportView()
        {
            InitializeComponent();

            ModelSceneViewport3D.Camera = CameraController.Camera;
            ModelSceneViewport3D.Children.Add(_modelVisual3D);
        }

        #endregion

        #region Methods

        private void UserControl_DataContextChanged(
            object sender, DependencyPropertyChangedEventArgs e
        )
        {
            if (e.OldValue is GDModelSceneViewportViewModel oldViewModel)
            {
                oldViewModel.GDModelSceneUpdated -=
                    ViewModel_GDModelSceneUpdated;
            }

            if (DataContext is GDModelSceneViewportViewModel viewModel)
            {
                viewModel.GDModelSceneUpdated +=
                    ViewModel_GDModelSceneUpdated;

                _modelVisual3D.Content = viewModel.CreateModel();
            }
        }

        private void ViewModel_GDModelSceneUpdated(
            object sender, System.EventArgs e
        )
        {
            var viewModel = sender as GDModelSceneViewportViewModel;

            _modelVisual3D.Content = viewModel.CreateModel();
        }

        private void ModelSceneViewport3D_MouseDown(
            object sender, MouseButtonEventArgs e
        )
        {
            CameraController.IInputElement_MouseDown(sender, e);
        }

        private void ModelSceneViewport3D_MouseMove(
            object sender, MouseEventArgs e
        )
        {
            CameraController.IInputElement_MouseMove(sender, e);
        }

        private void ModelSceneViewport3D_MouseUp(
            object sender, MouseButtonEventArgs e
        )
        {
            CameraController.IInputElement_MouseUp(sender, e);
        }

        private void ModelSceneViewport3D_MouseWheel(
            object sender, MouseWheelEventArgs e
        )
        {
            CameraController.IInputElement_MouseWheel(sender, e);
        }

        #endregion

        #region Properties

        public CameraController CameraController { get; } =
            new CameraController();

        #endregion

        #region Member variables

        private readonly ModelVisual3D _modelVisual3D =
             new ModelVisual3D();

        #endregion
    }
}
