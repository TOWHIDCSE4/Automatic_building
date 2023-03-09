using DaiwaRentalGD.Gui.Utilities.WorkingJsonFile.JsonModel;
using DaiwaRentalGD.Gui.ViewModels.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile
{
    public class GDOutputSummaryJson : GDOutputJson<Dictionary<string, List<OutputSummary>>>
    {
        private GDModelSceneMainViewModel _modelSceneMainViewModel;

        public GDOutputSummaryJson()
        {
            var resDict = new Dictionary<string, List<OutputSummary>>();
            resDict.Add(KEY_Output_summary, new List<OutputSummary>());
            Data = resDict;
        }
        public void GetData(GDModelSceneMainViewModel modelSceneMainViewModel, string solutionName)
        {
            _modelSceneMainViewModel = modelSceneMainViewModel;
            var outputSummary = new OutputSummary();
            var dataViewModel = _modelSceneMainViewModel.GDModelSceneDataViewModel;
            var inputsViewModel = _modelSceneMainViewModel.GDModelSceneInputsViewModel;
            outputSummary.ID = solutionName;
            outputSummary.BCR = dataViewModel.LandUseDataViewModel.BuildingCoverageRatio;
            outputSummary.FAR = dataViewModel.LandUseDataViewModel.FloorAreaRatio ?? default;
            outputSummary.Cost = dataViewModel.FinanceDataViewModel.BuildingCostYen ?? default;
            outputSummary.ProfitPerYear = dataViewModel.FinanceDataViewModel.BuildingRevenueYenPerYear ?? default;
            outputSummary.ROIPerYear = dataViewModel.FinanceDataViewModel.GrossRorPerYear ?? default;
            outputSummary.NumofCarParkingSpots = dataViewModel.ParkingLotDataViewModel.NumOfCarParkingSpaces ?? default;
            outputSummary.NumofBicycleParkingSlots = dataViewModel.ParkingLotDataViewModel.NumOfBicycleParkingSpaces ?? default;
            outputSummary.NumberofFloors = inputsViewModel.BuildingInputsViewModel.BuildingDesignerViewModel.BuildingDesigner.UnitArrangerComponent.NumOfFloors;
            outputSummary.UnitType = inputsViewModel.BuildingInputsViewModel.BuildingDesignerViewModel.UnitTypeName;


            var glstSummary = Data.GetValueOrDefault(KEY_Output_summary);
            glstSummary.Add(outputSummary);
            var resDict = new Dictionary<string, List<OutputSummary>>();
            resDict.Add(KEY_Output_summary, glstSummary);
            Data = resDict;
        }

        public override void GetData()
        {
            throw new NotImplementedException();
        }
        public const string KEY_Output_summary = "Output_Summary";
    }
}
