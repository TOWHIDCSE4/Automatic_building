using System;
using System.Windows.Media.Media3D;
using DaiwaRentalGD.Gui.Visualization3D;
using DaiwaRentalGD.Gui.VisualizationSVG;
using DaiwaRentalGD.Model;
using Svg;

namespace DaiwaRentalGD.Gui.ViewModels.Model
{
    /// <summary>
    /// View model for visualizing
    /// <see cref="DaiwaRentalGD.Model.GDModelScene"/> in a viewport.
    /// </summary>
    public class GDModelSceneViewportViewModel : GDModelSceneViewModelBase
    {
        #region Constructors

        public GDModelSceneViewportViewModel(GDModelScene gdms) : base(gdms)
        { }

        #endregion

        #region Methods

        public Model3D CreateModel()
        {
            return GDModelSceneModel3DBuilder.CreateModel(GDModelScene);
        }
        public SvgDocument CreateSVG()
        {
            return GDModelSceneModelSVGBuilder.CreateSVG(GDModelScene);
        }

        protected override void GDModelSceneUpdatedEventHandler(
            object sender, EventArgs e
        )
        {
            GDModelSceneUpdated?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Propertites

        public GDModelSceneModel3DBuilder GDModelSceneModel3DBuilder
        { get; } = new GDModelSceneModel3DBuilder();

        public GDModelSceneModelSVGBuilder GDModelSceneModelSVGBuilder
        { get; } = new GDModelSceneModelSVGBuilder();

        #endregion

        #region Events

        public event EventHandler<EventArgs> GDModelSceneUpdated;

        #endregion
    }
}
