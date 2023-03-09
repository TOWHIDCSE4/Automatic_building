using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign;
using DaiwaRentalGD.Gui.Visualization3D;

namespace DaiwaRentalGD.Gui.Views.Model.BuildingDesign
{
    /// <summary>
    /// Interaction logic for UnitCatalogEntryViewportView.xaml
    /// </summary>
    public partial class UnitCatalogEntryViewportView : UserControl
    {
        #region Constructors

        public UnitCatalogEntryViewportView()
        {
            InitializeComponent();

            InitializeCamera();
        }

        #endregion

        #region Methods

        private void InitializeCamera()
        {
            CameraController.MinCameraWidth = 10.0f;
            CameraController.MaxCameraWidth = 50.0f;
            CameraController.Camera.Width = 25.0;

            ModelViewport3D.Camera = CameraController.Camera;
            ModelViewport3D.Children.Add(_modelVisual3D);
        }

        private void UserControl_DataContextChanged(
            object sender, DependencyPropertyChangedEventArgs e
        )
        {
            var viewModel = DataContext as UnitCatalogEntryViewportViewModel;

            _modelVisual3D.Content = viewModel?.Model3D;
        }

        private void ModelViewport3D_MouseDown(
            object sender, MouseButtonEventArgs e
        )
        {
            CameraController.IInputElement_MouseDown(sender, e);
        }

        private void ModelViewport3D_MouseMove(
            object sender, MouseEventArgs e
        )
        {
            CameraController.IInputElement_MouseMove(sender, e);
        }

        private void ModelViewport3D_MouseUp(
            object sender, MouseButtonEventArgs e
        )
        {
            CameraController.IInputElement_MouseUp(sender, e);
        }

        private void ModelViewport3D_MouseWheel(
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
