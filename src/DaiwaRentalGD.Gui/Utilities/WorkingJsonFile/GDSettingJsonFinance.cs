using DaiwaRentalGD.Gui.ViewModels.Model;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile
{
    public class GDSettingJsonFinance
    {
        public GDSettingJsonFinance(string jsonPath, GDModelSceneMainViewModel modelSceneMainViewModel)
        {
            _modelSceneMainViewModel = modelSceneMainViewModel;
            _jsonPath = jsonPath;
        }

        public void Process()
        {
            var financeDataJsonComponent = _modelSceneMainViewModel.GDModelScene.FinanceEvaluator.FinanceDataJsonComponent;
            financeDataJsonComponent.JsonFilePath = _jsonPath;
            financeDataJsonComponent.Load();
          

        }
        private GDModelSceneMainViewModel _modelSceneMainViewModel;
        private string _jsonPath = "Input/Finance.json";

        public string JsonPath { get => _jsonPath; set => _jsonPath = value; }
    }
}
