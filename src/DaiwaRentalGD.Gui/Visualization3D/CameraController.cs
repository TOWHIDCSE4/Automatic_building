using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Provides mouse control for a <see cref="OrthographicCamera"/>.
    /// </summary>
    public class CameraController : INotifyPropertyChanged
    {
        #region Constructors

        public CameraController()
        {
            UpdateCameraTransform();
        }

        #endregion

        #region Methods

        public void IInputElement_MouseDown(
            object sender, MouseButtonEventArgs e
        )
        {
            var inputElement = sender as IInputElement;

            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragRotateStarted = true;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                _isDragPanStarted = true;
            }

            _mousePosition = e.GetPosition(inputElement);

            inputElement.CaptureMouse();
        }

        public void IInputElement_MouseMove(
            object sender, MouseEventArgs e
        )
        {
            var inputElement = sender as IInputElement;

            if (_isDragRotateStarted)
            {
                MouseMoveRotate(inputElement, e);
            }
            else if (_isDragPanStarted)
            {
                MouseMovePan(inputElement, e);
            }
        }

        private void MouseMoveRotate(
            IInputElement inputElement, MouseEventArgs e
        )
        {
            var currentMousePosition = e.GetPosition(inputElement);

            var mousePositionDelta = currentMousePosition - _mousePosition;

            _mousePosition = currentMousePosition;

            HorizontalAngle += mousePositionDelta.X * RotateRate;
            HorizontalAngle %= Math.PI * 2.0;

            VerticalAngle += mousePositionDelta.Y * RotateRate;
            VerticalAngle = Math.Max(
                MinVerticalAngle,
                Math.Min(MaxVerticalAngle, VerticalAngle)
            );

            UpdateCameraTransform();
        }

        private void MouseMovePan(
            IInputElement inputElement, MouseEventArgs e
        )
        {
            var offsetDirX = Vector3D.CrossProduct(
                Camera.LookDirection, Camera.UpDirection
            );
            offsetDirX.Normalize();

            var offsetDirY = Vector3D.CrossProduct(
                offsetDirX, Camera.LookDirection
            );
            offsetDirY.Normalize();

            var currentMousePosition = e.GetPosition(inputElement);

            var mousePositionDelta = currentMousePosition - _mousePosition;

            _mousePosition = currentMousePosition;

            var offsetX = offsetDirX * -mousePositionDelta.X * PanRate;
            var offsetY = offsetDirY * mousePositionDelta.Y * PanRate;

            LookTarget += offsetX;
            LookTarget += offsetY;

            UpdateCameraTransform();
        }

        public void IInputElement_MouseUp(
            object sender, MouseButtonEventArgs e
        )
        {
            var inputElement = sender as IInputElement;

            _isDragRotateStarted = false;
            _isDragPanStarted = false;

            inputElement.ReleaseMouseCapture();
        }

        public void IInputElement_MouseWheel(
            object sender, MouseWheelEventArgs e
        )
        {
            double zoomDelta = e.Delta > 0 ? -ZoomRate : ZoomRate;

            double cameraWidth = Camera.Width + zoomDelta;

            cameraWidth = Math.Max(
                MinCameraWidth,
                Math.Min(MaxCameraWidth, cameraWidth)
            );

            Camera.Width = cameraWidth;
        }

        public void UpdateCameraTransform()
        {
            var horizontalRadius = OrbitRadius * Math.Cos(VerticalAngle);

            var horizontalOffset = new Vector3D(
                -horizontalRadius * Math.Cos(HorizontalAngle),
                -horizontalRadius * Math.Sin(HorizontalAngle),
                0.0
            );

            var verticalOffset = new Vector3D(
                0.0, 0.0, OrbitRadius * Math.Sin(VerticalAngle)
            );

            var cameraPosition =
                LookTarget + horizontalOffset + verticalOffset;

            Camera.Position = new Point3D(
                cameraPosition.X, cameraPosition.Y, cameraPosition.Z
            );

            var lookDir = LookTarget - cameraPosition;
            lookDir.Normalize();

            Camera.LookDirection = lookDir;
        }

        protected void NotifyPropertyChanged(
            [CallerMemberName] string propertyName = ""
        )
        {
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(propertyName)
            );
        }

        #endregion

        #region Properties

        public OrthographicCamera Camera { get; } = new OrthographicCamera
        {
            NearPlaneDistance = DefaultNearPlane,
            FarPlaneDistance = DefaultFarPlane,
            UpDirection = new Vector3D(0.0, 0.0, 1.0),
            Width = DefaultCameraWidth
        };

        public Vector3D LookTarget
        {
            get => _lookTarget;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _lookTarget = value;
                NotifyPropertyChanged();
            }
        }

        public double OrbitRadius
        {
            get => _orbitRadius;
            set
            {
                _orbitRadius = value;
                NotifyPropertyChanged();
            }
        }

        public double MinVerticalAngle
        {
            get => _minVerticalAngle;
            set
            {
                _minVerticalAngle = value;
                NotifyPropertyChanged();
            }
        }

        public double MaxVerticalAngle
        {
            get => _maxVerticalAngle;
            set
            {
                _maxVerticalAngle = value;
                NotifyPropertyChanged();
            }
        }

        public double VerticalAngle
        {
            get => _verticalAngle;
            set
            {
                _verticalAngle = value;
                NotifyPropertyChanged();
            }
        }

        public double HorizontalAngle
        {
            get => _horizontalAngle;
            set
            {
                _horizontalAngle = value;
                NotifyPropertyChanged();
            }
        }

        public double RotateRate
        {
            get => _rotateRate;
            set
            {
                _rotateRate = value;
                NotifyPropertyChanged();
            }
        }

        public double PanRate
        {
            get => _panRate;
            set
            {
                _panRate = value;
                NotifyPropertyChanged();
            }
        }

        public double ZoomRate
        {
            get => _zoomRate;
            set
            {
                _zoomRate = value;
                NotifyPropertyChanged();
            }
        }

        public double MinCameraWidth
        {
            get => _minCameraWidth;
            set
            {
                _minCameraWidth = value;
                NotifyPropertyChanged();
            }
        }

        public double MaxCameraWidth
        {
            get => _maxCameraWidth;
            set
            {
                _maxCameraWidth = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private Vector3D _lookTarget = new Vector3D(20,20,0);

        private double _orbitRadius = DefaultOrbitRadius;

        private double _minVerticalAngle = DefaultMinVerticalAngle;
        private double _maxVerticalAngle = DefaultMaxVerticalAngle;
        private double _verticalAngle = DefaultVerticalAngle;
        private double _horizontalAngle = DefaultHorizontalAngle;

        private double _minCameraWidth = DefaultMinCameraWidth;
        private double _maxCameraWidth = DefaultMaxCameraWidth;

        private double _rotateRate = DefaultRotateRate;
        private double _panRate = DefaultPanRate;
        private double _zoomRate = DefaultZoomRate;

        private Point _mousePosition;
        private bool _isDragRotateStarted = false;
        private bool _isDragPanStarted = false;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constants

        public const double DefaultOrbitRadius = 50.0;

        public const double DefaultMinVerticalAngle = Math.PI / 180.0 * 0.0;
        public const double DefaultMaxVerticalAngle =
            Math.PI / 180.0 * 90.0 - 1e-4;
        public const double DefaultVerticalAngle = DefaultMaxVerticalAngle; // Math.PI / 180.0 * 60.0;

        public const double DefaultHorizontalAngle = Math.PI / 180.0 * 90.0; // Math.PI / 180.0 * 45.0;

        public const double DefaultRotateRate = 0.01;
        public const double DefaultPanRate = 0.05;
        public const double DefaultZoomRate = 3.0;

        public const double DefaultMinCameraWidth = 30.0;
        public const double DefaultMaxCameraWidth = 500.0;
        public const double DefaultCameraWidth = 120.0; //75.0;

        public const double DefaultNearPlane = 0.1;
        public const double DefaultFarPlane = 1000.0;

        #endregion
    }
}
