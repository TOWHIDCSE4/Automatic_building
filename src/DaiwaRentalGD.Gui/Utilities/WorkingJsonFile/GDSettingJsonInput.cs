using DaiwaRentalGD.Gui.Utilities.WorkingJsonFile.JsonModel;
using DaiwaRentalGD.Gui.ViewModels.Model;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeA;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeB;
using DaiwaRentalGD.Gui.ViewModels.Model.BuildingDesign.TypeC;
using DaiwaRentalGD.Gui.ViewModels.Model.SiteDesign;
using DaiwaRentalGD.Gui.ViewModels.Optimization;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;
using DaiwaRentalGD.Optimization.Problems;
using O3.Commons.DataSpecs;
using O3.Foundation.UI.ViewModels;
using System.Collections.Generic;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile
{
    public class GDSettingJsonInput : GDSettingJson<Dictionary<string, InputJson>>
    {
        private OptimizationMainViewModel _optimizationViewModel;

        private GDModelSceneMainViewModel _modelSceneMainViewModel;

        private InputJson _inputJson;

        public GDSettingJsonInput(string jsonPath, OptimizationMainViewModel optimizationViewModel) : base(jsonPath)
        {
            _optimizationViewModel = optimizationViewModel;
        }

        public GDSettingJsonInput(string jsonPath, GDModelSceneMainViewModel modelSceneMainViewModel) : base(jsonPath)
        {
            _modelSceneMainViewModel = modelSceneMainViewModel;
        }

        public override void Process()
        {
            var jsonObj = ReadJsonFile();
            _inputJson = jsonObj.GetValueOrDefault(KEY_INPUT_JSON);
   
            if (_inputJson != null)
            {
                // Setting for Optimization.
                if (_optimizationViewModel != null)
                {
                    var problemViewModel = _optimizationViewModel.GDModelProblemViewModel;
                    SetProblemInput(problemViewModel);
                    SetProblemOutput(problemViewModel);
                }

                // Setting for Main model.
                if (_modelSceneMainViewModel != null)
                {
                    var modelSceneInput = _modelSceneMainViewModel.GDModelSceneInputsViewModel;
                    SetModelSceneInput(modelSceneInput);
                    var typeViewModel = modelSceneInput.BuildingInputsViewModel.BuildingDesignerViewModel;
                    if (typeViewModel != null)
                    {
                        SetRotationSnapping(typeViewModel);
                        SetRoofTypeDropdown(typeViewModel);
                    }
                }
            }
        }
        private void SetRoofTypeDropdown(BuildingDesignerViewModelBase typeViewModel)
        {
            if (typeViewModel is TypeABuildingDesignerViewModel)
            {
                var typeAUnitVM = (TypeAUnitArrangerViewModel)((TypeABuildingDesignerViewModel)typeViewModel).UnitArrangerViewModel;
                if (typeAUnitVM != null)
                {
                    TypeAUnitRoofType roofType = (TypeAUnitRoofType)_inputJson.RoofType;
                    typeAUnitVM.RoofType = roofType;
                }

            }
            if (typeViewModel is TypeBBuildingDesignerViewModel)
            {
                var typeBUnitVM = (TypeBUnitArrangerViewModel)((TypeBBuildingDesignerViewModel)typeViewModel).UnitArrangerViewModel;
                if (typeBUnitVM != null)
                {
                    TypeBUnitRoofType roofType = (TypeBUnitRoofType)_inputJson.RoofType;
                    typeBUnitVM.RoofType = roofType;
                }
            }
            if (typeViewModel is TypeCBuildingDesignerViewModel)
            {
                var typeCUnitVM = (TypeCUnitArrangerViewModel)((TypeCBuildingDesignerViewModel)typeViewModel).UnitArrangerViewModel;
                if (typeCUnitVM != null)
                {
                    TypeCUnitRoofType roofType = (TypeCUnitRoofType)_inputJson.RoofType;
                    typeCUnitVM.RoofType = roofType;
                }
            }
        }
        private void SetRotationSnapping(BuildingDesignerViewModelBase typeViewModel)
        {
            if (_inputJson.EnableBuildingRotationSnapping == true)
                typeViewModel.BuildingDesigner.BuildingPlacementComponent.BuildingOrientationMode = BuildingOrientationMode.Aligned;
            else
                typeViewModel.BuildingDesigner.BuildingPlacementComponent.BuildingOrientationMode = BuildingOrientationMode.Free;
        }

        private void SetNumOfUnitsPerFloor(InputSpecViewModel inputSpecViewModel)
        {
            var dataSpec = ((IntegerDataSpec)inputSpecViewModel.InputSpec.DataSpec);
            dataSpec.Min = _inputJson.NumberOfUnitsPerFloorMin;
            dataSpec.Max = _inputJson.NumberOfUnitsPerFloorMax;
        }
        private void SetNumOfFloor(InputSpecViewModel inputSpecViewModel)
        {
            var dataSpec = ((IntegerDataSpec)inputSpecViewModel.InputSpec.DataSpec);
            dataSpec.Min = _inputJson.NumberOfFloorsMin;
            dataSpec.Max = _inputJson.NumberOfFloorsMax;
        }
        private void SetUnitType(InputSpecViewModel inputSpecViewModel, GDModelProblemViewModel problemVM)
        {
            bool typeA = _inputJson.UseStaircaseTypeA;
            bool typeB = _inputJson.UseSideCorridorTypeB;
            bool typeC = _inputJson.UseNagayaTypeC;
            List<string> glstTypeForNumber = new List<string>();
            IntegerDataSpec dataSpec = ((IntegerDataSpec)inputSpecViewModel.InputSpec.DataSpec);
            if (typeA && !typeB && !typeC)
            {
                dataSpec.Min = 0;
                dataSpec.Max = 0;
                glstTypeForNumber.Add("A");
            }
            if (!typeA && typeB && !typeC)
            {
                dataSpec.Min = 0;
                dataSpec.Max = 0;
                glstTypeForNumber.Add("B");
            }
            if (typeA && typeB && !typeC)
            {
                dataSpec.Min = 0;
                dataSpec.Max = 1;
                glstTypeForNumber.Add("A");
                glstTypeForNumber.Add("B");
            }
            if (!typeA && !typeB && typeC)
            {
                dataSpec.Min = 0;
                dataSpec.Max = 0;
                glstTypeForNumber.Add("C");
            }
            if (typeA && !typeB && typeC)
            {
                dataSpec.Min = 0;
                dataSpec.Max = 1;
                glstTypeForNumber.Add("A");
                glstTypeForNumber.Add("C");
            }
            if (!typeA && typeB && typeC)
            {
                dataSpec.Min = 0;
                dataSpec.Max = 1;
                glstTypeForNumber.Add("B");
                glstTypeForNumber.Add("C");
            }
            if (typeA && typeB && typeC)
            {
                dataSpec.Min = 0;
                dataSpec.Max = 2;
                glstTypeForNumber.Add("A");
                glstTypeForNumber.Add("B");
                glstTypeForNumber.Add("C");
            }
            problemVM.GDModelProblem.BuildingDesignerProblemModule.RoofType = _inputJson.RoofType;
            problemVM.GDModelProblem.BuildingDesignerProblemModule.GlstTypeForNumber = glstTypeForNumber;
        }
        private void SetParkingAtRoadsideOrDriveway(InputSpecViewModel inputSpecViewModel)
        {
            int parkingMin = 0;
            int parkingMax = 0;
            var dataSpec = ((IntegerDataSpec)inputSpecViewModel.InputSpec.DataSpec);

            if (!_inputJson.EnableRoadsideCarParking && _inputJson.EnableDriveway)
            {
                parkingMin = 1;
                parkingMax = 1;
            }

            if (_inputJson.EnableRoadsideCarParking && _inputJson.EnableDriveway)
            {
                parkingMin = 0;
                parkingMax = 1;
            }

            dataSpec.Min = parkingMin;
            dataSpec.Max = parkingMax;
        }
        private void SetBuildingTrueNorthAngle(InputSpecViewModel inputSpecViewModel)
        {
            var dataSpec = ((RealDataSpec)inputSpecViewModel.InputSpec.DataSpec);
            dataSpec.Min = BuildingPlacementComponent.ConvertDegreesToRadians(_inputJson.BuildingRotationFromNorthMinDegree);
            dataSpec.Max = BuildingPlacementComponent.ConvertDegreesToRadians(_inputJson.BuildingRotationFromNorthMaxDegree);
        }

        private void SetFloorAreaRatio(OutputSpecViewModel outputSpecViewModel)
        {
            outputSpecViewModel.ConstraintViewModel.Min = _inputJson.FARMin * 100;
            outputSpecViewModel.ConstraintViewModel.Max = _inputJson.FARMax * 100;
        }

        private void SetBuildingCoverageRatio(OutputSpecViewModel outputSpecViewModel)
        {
            outputSpecViewModel.ConstraintViewModel.Min = _inputJson.BCRMin * 100;
            outputSpecViewModel.ConstraintViewModel.Max = _inputJson.BCRMax * 100;
        }

        private void SetProblemOutput(GDModelProblemViewModel problemViewModel)
        {
            foreach (OutputSpecViewModel outputSpecViewModel in problemViewModel.OutputSpecViewModels)
            {
                switch (outputSpecViewModel.Name)
                {
                    case LandUseEvaluatorProblemModule.FloorAreaRatioOutputName:
                        SetFloorAreaRatio(outputSpecViewModel);
                        break;
                    case LandUseEvaluatorProblemModule.BuildingCoverageRatioOutputName:
                        SetBuildingCoverageRatio(outputSpecViewModel);
                        break;
                }
            }
        }

        private void SetProblemInput(GDModelProblemViewModel problemViewModel)
        {
            foreach (InputSpecViewModel inputSpectViewModel in problemViewModel.InputSpecViewModels)
            {
                switch (inputSpectViewModel.Name)
                {
                    case BuildingDesignerProblemModule.UnitTypeInputName:
                        SetUnitType(inputSpectViewModel, problemViewModel);
                        break;
                    case BuildingDesignerProblemModule.NumOfUnitsPerFloorInputName:
                        SetNumOfUnitsPerFloor(inputSpectViewModel);
                        break;
                    case BuildingDesignerProblemModule.NumOfFloorInputName:
                        SetNumOfFloor(inputSpectViewModel);
                        break;
                    case ParkingLotDesignerProblemModule.ParkingAtRoadsideOrDrivewayInputName:
                        SetParkingAtRoadsideOrDriveway(inputSpectViewModel);
                        break;
                    case BuildingDesignerProblemModule.BuildingTNAngleInputName:
                        SetBuildingTrueNorthAngle(inputSpectViewModel);
                        break;


                }
            }
        }
        private void SetSiteInput(SiteInputsViewModel siteInputsViewModel)
        {
            var sampleSite = siteInputsViewModel.SiteDesignerViewModel as SampleSiteDesignerViewModel;
            if (sampleSite == null)
                return;
            sampleSite.NorthSlantPlanesStartHeight = _inputJson.NorthSlantPlanesStartHeight;
            sampleSite.NorthSlantPlanesStartSlope = _inputJson.NorthSlantPlanesStartSlope;
            sampleSite.RoadSlantPlanesStartHeight = _inputJson.RoadSlantPlanesStartHeight;
            sampleSite.RoadSlantPlanesStartSlope = _inputJson.RoadSlantPlanesStartSlope;
            sampleSite.SiteSlantPlanesStartHeight = _inputJson.SiteSlantPlanesStartHeight;
            sampleSite.SiteSlantPlanesStartSlope = _inputJson.SiteSlantPlanesStartSlope;
            sampleSite.AbsoluteHeightPlane = _inputJson.AbsoluteHeightPlaneHeight;
            // TODO: N/A _inputJson.AbsoluteHeightplaneHeight
        }

        private void SetModelSceneInput(GDModelSceneInputsViewModel _modelSceneInputsViewModel)
        {
            SetSiteInput(_modelSceneInputsViewModel.SiteInputsViewModel);
        }

        public const string KEY_INPUT_JSON = "InputParameters";
    }
}
