using DaiwaRentalGD.Gui.Utilities.WorkingJsonFile.JsonModel;
using DaiwaRentalGD.Gui.ViewModels.Model;
using DaiwaRentalGD.Model.BuildingDesign;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile
{
    public class GDOutputJsonLayout : GDOutputJson<Dictionary<string, OutputLayout>>
    {
        private GDModelSceneMainViewModel _modelSceneMainViewModel;

        public GDOutputJsonLayout(GDModelSceneMainViewModel modelSceneMainViewModel)
        {
            _modelSceneMainViewModel = modelSceneMainViewModel;
        }
        public override void GetData()
        {
            var outputLayout = new OutputLayout();
            var dataViewModel = _modelSceneMainViewModel.GDModelSceneDataViewModel;
            var inputsViewModel = _modelSceneMainViewModel.GDModelSceneInputsViewModel;

            outputLayout.BCR = dataViewModel.LandUseDataViewModel.BuildingCoverageRatio;
            outputLayout.FAR = dataViewModel.LandUseDataViewModel.FloorAreaRatio ?? 0;
            outputLayout.ROIPerYear = dataViewModel.FinanceDataViewModel.GrossRorPerYear ?? 0;
            outputLayout.NumberOfCarParkingSpots = dataViewModel.ParkingLotDataViewModel.NumOfCarParkingSpaces ?? 0;
            outputLayout.ClearNorthSlantPlanes = (bool)dataViewModel.SlantPlanesDataViewModel.IsNorthSlantPlanesValid;
            outputLayout.ClearAdjacentSiteSlantPlanes = (bool)dataViewModel.SlantPlanesDataViewModel.IsAdjacentSiteSlantPlanesValid;
            outputLayout.ClearRoadSlantPlanes = (bool)dataViewModel.SlantPlanesDataViewModel.IsRoadSlantPlanesValid;
            outputLayout.ClearAbsoluteHeightPlanes = (bool)dataViewModel.SlantPlanesDataViewModel.IsAbsoluteHeightPlanesValid;
            outputLayout.NumberOfFloors = inputsViewModel.BuildingInputsViewModel.BuildingDesignerViewModel.BuildingDesigner.UnitArrangerComponent.NumOfFloors;
            outputLayout.NumberOfUnitsPerFloor = inputsViewModel.BuildingInputsViewModel.BuildingDesignerViewModel.BuildingDesigner.UnitArrangerComponent.NumOfUnitsPerFloor;
            outputLayout.NumberOfUnits = (inputsViewModel.BuildingInputsViewModel.BuildingDesignerViewModel.BuildingDesigner.UnitArrangerComponent.NumOfFloors) * (inputsViewModel.BuildingInputsViewModel.BuildingDesignerViewModel.BuildingDesigner.UnitArrangerComponent.NumOfUnitsPerFloor);
            outputLayout.Cost = (dataViewModel.FinanceDataViewModel.BuildingCostYen ?? 0) + (dataViewModel.FinanceDataViewModel.ParkingLotCostYen ?? 0);
            outputLayout.ProfitPerYear = (dataViewModel.FinanceDataViewModel.BuildingRevenueYenPerYear ?? 0) + (dataViewModel.FinanceDataViewModel.ParkingLotRevenueYenPerYear ?? 0);
            outputLayout.NumberOfBicycleSlots = dataViewModel.ParkingLotDataViewModel.NumOfBicycleParkingSpaces ?? 0;
            outputLayout.NumberOfMissingBicycleSlots = setNumberOfMissingBicycleSlots(dataViewModel.ParkingLotDataViewModel.NumOfBicycleParkingSpaces ?? default, dataViewModel.ParkingLotDataViewModel.MinNumOfBicycleParkingSpaces ?? default);
            outputLayout.NumberOfMissingCarParkingSpots = setNumberOfMissingCarParkingSpots(dataViewModel.ParkingLotDataViewModel.NumOfCarParkingSpaces ?? default, dataViewModel.ParkingLotDataViewModel.MinNumOfCarParkingSpaces ?? default);
            SetUnitStatsAndUnitTypes(outputLayout);
            var ResDict = new Dictionary<string, OutputLayout>();
            ResDict.Add(KEY_OUTPUT_ID, outputLayout);
            Data = ResDict;
        }
        private int setNumberOfMissingCarParkingSpots(int NumOfCarParkingSpaces, double MinNumOfCarParkingSpaces)
        {
            int NumberOfMissingCarParkingSpots = (int)Math.Ceiling(MinNumOfCarParkingSpaces) - NumOfCarParkingSpaces;
            return NumberOfMissingCarParkingSpots < 0 ? 0 : NumberOfMissingCarParkingSpots;
        }
        private double setNumberOfMissingBicycleSlots(int NumOfBicycleParkingSpaces, double MinNumOfBicycleParkingSpaces)
        {
            int NumberOfMissingBicycleSlots = (int)Math.Ceiling(MinNumOfBicycleParkingSpaces) - NumOfBicycleParkingSpaces;
            return NumberOfMissingBicycleSlots < 0 ? 0 : NumberOfMissingBicycleSlots;

        }

        private void SetUnitStatsAndUnitTypes(OutputLayout outputLayout)
        {
            #region unitTypes
            var bc = _modelSceneMainViewModel.GDModelScene.Building.BuildingComponent;
            List<List<string>> unitTypes = new List<List<string>>();
            for (int floor = 0; floor < bc.NumOfFloors; ++floor)
            {
                List<string> floorUnitType = new List<string>();
                for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
                {
                    Unit unit = bc.GetUnit(floor, stack);
                    if (unit == null)
                        continue;
                    if (!(unit.UnitComponent is CatalogUnitComponent cuc))
                        continue;
                    floorUnitType.Add(string.Format("{0} ({1})", cuc.EntryName.FullName, cuc.PlanName));
                }
                unitTypes.Add(floorUnitType);
            }
            outputLayout.UnitTypes = unitTypes;
            #endregion
            #region uitStats
            var uitStats = from unitName in unitTypes.SelectMany(unit => unit)
                           group unitName by unitName into NewUnitALL
                           select new UnitStat
                           {
                               Unit_Type = NewUnitALL.Key,
                               NumberOfUnitsOfType = NewUnitALL.Count()
                           };
            outputLayout.UnitStats = uitStats.ToList();
            #endregion
        }

        public const string KEY_OUTPUT_ID = "Output_Detail";
    }
}
