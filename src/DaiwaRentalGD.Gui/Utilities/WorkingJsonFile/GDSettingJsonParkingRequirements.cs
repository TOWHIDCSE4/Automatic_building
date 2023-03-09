using DaiwaRentalGD.Gui.ViewModels.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile
{
    public class GDSettingJsonParkingRequirements
    {
        public GDSettingJsonParkingRequirements(string jsonPath, GDModelSceneMainViewModel modelSceneMainViewModel)
        {
            _modelSceneMainViewModel = modelSceneMainViewModel;
            _jsonPath = jsonPath;
        }

        public void Process()
        {
            var parkingRequirementsJsonComponent = _modelSceneMainViewModel.GDModelScene.ParkingLotDesigner.ParkingRequirementsJsonComponent;
            parkingRequirementsJsonComponent.JsonFilePath = _jsonPath;
            parkingRequirementsJsonComponent.Load();
        }
        private GDModelSceneMainViewModel _modelSceneMainViewModel;
        private string _jsonPath = "Input/ParkingRequirements.json";

        public string JsonPath { get => _jsonPath; set => _jsonPath = value; }
    }
}
