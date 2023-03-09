using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DaiwaRentalGD.Gui.Visualization3D
{
    /// <summary>
    /// Builds <see cref="System.Windows.Media.Media3D.Model3D"/> with
    /// <see cref="System.Windows.Media.Media3D.Light"/>s for lighting up
    /// the scene.
    /// </summary>
    public class LightsModel3DBuilder
    {
        #region Constructors

        public LightsModel3DBuilder()
        { }

        #endregion

        #region Methods

        public Model3D CreateModel()
        {
            Light ambientLight = new AmbientLight
            {
                Color = AmbientLightColor
            };

            Light directionalLight = new DirectionalLight
            {
                Color = DirectionalLightColor,
                Direction = DirectionalLightDirection
            };

            var lightsModel = new Model3DGroup
            {
                Children =
                {
                    ambientLight,
                    directionalLight
                }
            };

            return lightsModel;
        }

        #endregion

        #region Properties

        public Color AmbientLightColor { get; set; } =
            DefaultAmbientLightColor;

        public Color DirectionalLightColor { get; set; } =
            DefaultDirectionalLightColor;

        public Vector3D DirectionalLightDirection { get; set; } =
            DefaultDirectionalLightDirection;

        #endregion

        #region Constants

        public static readonly Color DefaultAmbientLightColor =
            Color.FromRgb(90, 90, 90);

        public static readonly Color DefaultDirectionalLightColor =
            Color.FromRgb(255, 255, 255);

        public static readonly Vector3D DefaultDirectionalLightDirection =
            new Vector3D(1.0, 0.5, -2.0);

        #endregion
    }
}
