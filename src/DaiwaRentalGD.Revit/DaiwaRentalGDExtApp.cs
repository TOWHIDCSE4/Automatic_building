using Autodesk.Revit.UI;

namespace DaiwaRentalGD.Revit
{
    public class DaiwaRentalGDExtApp : IExternalApplication
    {
        #region Constructors

        public DaiwaRentalGDExtApp()
        { }

        #endregion

        #region Methods

        public Result OnStartup(UIControlledApplication application)
        {
            InitializePanel(application);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private void InitializePanel(UIControlledApplication application)
        {
            var panel = application.CreateRibbonPanel(PanelName);

            AddConvertAndSaveButton(panel);
        }

        private void AddConvertAndSaveButton(RibbonPanel panel)
        {
            string assemblyPath =
                typeof(ConvertAndSaveExtCmd).Assembly.Location;

            var buttonData = new PushButtonData(
                ConvertAndSaveButtonName,
                ConvertAndSaveButtonText,
                assemblyPath,
                typeof(ConvertAndSaveExtCmd).FullName
            );

            var button = panel.AddItem(buttonData) as PushButton;

            button.ToolTip = ConvertAndSaveButtonToolTip;

        }

        #endregion

        #region Constants

        public const string PanelName = "Daiwa Rental GD";

        public const string ConvertAndSaveButtonName =
            nameof(ConvertAndSaveExtCmd);

        public const string ConvertAndSaveButtonText =
            "Convert and Save";

        public const string ConvertAndSaveButtonToolTip =
            "Convert and save current Revit unit catalog entry into " +
            "format that the GD model works with";

        #endregion
    }
}
