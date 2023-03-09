using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.BuildingDesign;
using DaiwaRentalGD.Model.BuildingDesign.TypeA;
using DaiwaRentalGD.Model.BuildingDesign.TypeB;
using DaiwaRentalGD.Model.BuildingDesign.TypeC;
using O3.Commons.DataSpecs;
using O3.Foundation;
using Workspaces.Core;

namespace DaiwaRentalGD.Optimization.Problems
{
    /// <summary>
    /// Problem module that corresponds to
    /// <see cref="DaiwaRentalGD.Model.GDModelScene.BuildingDesigner"/>.
    /// </summary>
    [Serializable]
    public class BuildingDesignerProblemModule : GDModelProblemModule
    {

        #region Constructors

        public BuildingDesignerProblemModule(GDModelProblem problem) :
            base(problem)
        {
            NormalizedEntryIndexInputSpecs =
                _normalizedEntryIndexInputSpecs.AsReadOnly();

            InitializeNormalizedEntryIndexInputSpecs();
        }

        protected BuildingDesignerProblemModule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            NormalizedEntryIndexInputSpecs =
                _normalizedEntryIndexInputSpecs.AsReadOnly();

            var reader = new WorkspaceItemReader(this, info, context);
            UnitTypeInputSpec = reader.GetValue<InputSpec>( nameof(UnitTypeInputSpec));
            EntraceTypeInputSpec = reader.GetValue<InputSpec>(nameof(EntraceTypeInputSpec));
            BuildingXInputSpec =
                reader.GetValue<InputSpec>(nameof(BuildingXInputSpec));

            BuildingYInputSpec =
                reader.GetValue<InputSpec>(nameof(BuildingYInputSpec));

            BuildingTNAngleInputSpec =
                reader.GetValue<InputSpec>(nameof(BuildingTNAngleInputSpec));

            NumOfUnitsPerFloorInputSpec = reader.GetValue<InputSpec>(
                nameof(NumOfUnitsPerFloorInputSpec)
            );
            NumOfFloorInputSpec = reader.GetValue<InputSpec>(
            nameof(NumOfFloorInputSpec));


            _normalizedEntryIndexInputSpecs.AddRange(
                reader.GetValues<InputSpec>(
                    nameof(NormalizedEntryIndexInputSpecs)
                )
            );
        }

        #endregion

        #region Methods

        private void InitializeNormalizedEntryIndexInputSpecs()
        {
            for (int stack = 0; stack < DefaultMaxNumOfUnitsPerFloor; ++stack)
            {
                var inputSpec = new InputSpec
                {
                    Name = GetNormalizedEntryIndexInputName(stack),
                    DataSpec = new RealDataSpec
                    {
                        Min = 0.0,
                        Max = 1.0
                    }
                };

                _normalizedEntryIndexInputSpecs.Add(inputSpec);
            }
        }

        #region UpdateInputSpecs()

        public override void UpdateInputSpecs()
        {
            UpdateInputSpecsFromBuildingPlacementComponent();
            UpdateInputSpecsFromUnitArrangerComponent();
        }

        private void UpdateInputSpecsFromBuildingPlacementComponent()
        {
            var bpc =
                Problem.GDModelScene
                .BuildingDesigner.BuildingPlacementComponent;

            var buildingXDS = BuildingXInputSpec.DataSpec as RealDataSpec;
            buildingXDS.SetDomain(bpc.BuildingMinX, bpc.BuildingMaxX);
            BuildingXInputSpec.Constant = bpc.BuildingX;
            Problem.AddInputSpec(BuildingXInputSpec);

            var buildingYDS = BuildingYInputSpec.DataSpec as RealDataSpec;
            buildingYDS.SetDomain(bpc.BuildingMinY, bpc.BuildingMaxY);
            BuildingYInputSpec.Constant = bpc.BuildingY;
            Problem.AddInputSpec(BuildingYInputSpec);

            BuildingTNAngleInputSpec.Constant = bpc.BuildingTNAngle;
            Problem.AddInputSpec(BuildingTNAngleInputSpec);
        }

        private void UpdateInputSpecsFromUnitArrangerComponent()
        {
            var uac =
                Problem.GDModelScene
                .BuildingDesigner.UnitArrangerComponent;
            UnitTypeInputSpec.Constant = uac.UnitTypeForNumber;
            Problem.AddInputSpec(UnitTypeInputSpec);

            EntraceTypeInputSpec.Constant = uac.EntraceTypeForNumber;
            Problem.AddInputSpec(EntraceTypeInputSpec);

            NumOfUnitsPerFloorInputSpec.Constant = uac.NumOfUnitsPerFloor;
            Problem.AddInputSpec(NumOfUnitsPerFloorInputSpec);

            NumOfFloorInputSpec.Constant = uac.NumOfFloors;
            Problem.AddInputSpec(NumOfFloorInputSpec);



            foreach (var entryInputSpec in NormalizedEntryIndexInputSpecs)
            {
                Problem.AddInputSpec(entryInputSpec);
            }

            //only Num Of Units Per Floor
            for (int stack = 0; stack < uac.NumOfUnitsPerFloor; ++stack)
            {
                var inputSpec = NormalizedEntryIndexInputSpecs[stack];
                inputSpec.Constant = uac.NormalizedEntryIndices[stack];
            }           
        }

        #endregion

        #region SetFromSolutionInputs()

        public override void SetFromSolutionInputs(
            GDModelScene scene, Solution solution
        )
        {
            SetFromSolutionInputsToBuildingPlacementComponent(
                scene, solution
            );
            SetFromSolutionInputsToUnitArrangerComponent(
                scene, solution
            );
        }

        private void SetFromSolutionInputsToBuildingPlacementComponent(
            GDModelScene scene, Solution solution
        )
        {
            var bpc = scene.BuildingDesigner.BuildingPlacementComponent;

            double buildingX = solution.GetInput<double>(BuildingXInputSpec);
            bpc.BuildingX = buildingX;

            double buildingY = solution.GetInput<double>(BuildingYInputSpec);
            bpc.BuildingY = buildingY;

            double buildingTNAngle =
                solution.GetInput<double>(BuildingTNAngleInputSpec);
            bpc.BuildingTNAngle = buildingTNAngle;

        }

        private void SetFromSolutionInputsToUnitArrangerComponent(
            GDModelScene scene, Solution solution
        )
        {
            
            var uac = scene.BuildingDesigner.UnitArrangerComponent;

            int unitTypeForNumber =  solution.GetInput<int>(UnitTypeInputSpec);
            int entraceTypeForNumber = solution.GetInput<int>(EntraceTypeInputSpec);

            uac.UnitTypeForNumber = unitTypeForNumber;
            uac.EntraceTypeForNumber = entraceTypeForNumber;
       
            if (_glstTypeForNumber.Count > unitTypeForNumber)
            {
                switch (_glstTypeForNumber[unitTypeForNumber])
                {
                    case "A":
                        scene.BuildingDesigner = new TypeABuildingDesigner();
                        var roofTypeA = (TypeAUnitRoofType)_roofType;
                        switch (roofTypeA)
                        {
                            case TypeAUnitRoofType.Gable:
                                scene.BuildingDesigner.RoofCreatorComponent = new GableRoofCreatorComponent();
                                break;
                            case TypeAUnitRoofType.Flat:
                                scene.BuildingDesigner.RoofCreatorComponent = new FlatRoofCreatorComponent();
                                break;
                        }
                        var typeAarrangerComponent = scene.BuildingDesigner.UnitArrangerComponent as TypeAUnitArrangerComponent;
                        typeAarrangerComponent.EntranceType = (TypeAUnitEntranceType)entraceTypeForNumber;
                        break;
                    case "B":
                        scene.BuildingDesigner = new TypeBBuildingDesigner();
                        var roofTypeB = (TypeBUnitRoofType)_roofType;
                        switch (roofTypeB)
                        {
                            case TypeBUnitRoofType.Gable:
                                scene.BuildingDesigner.RoofCreatorComponent = new GableRoofCreatorComponent();
                                break;
                            case TypeBUnitRoofType.Flat:
                                scene.BuildingDesigner.RoofCreatorComponent = new FlatRoofCreatorComponent();
                                break;
                        }
                        break;
                    case "C":
                        scene.BuildingDesigner = new TypeCBuildingDesigner();
                        var roofTypeC = (TypeCUnitRoofType)_roofType;
                        switch (roofTypeC)
                        {
                            case TypeCUnitRoofType.Gable:
                                scene.BuildingDesigner.RoofCreatorComponent = new GableRoofCreatorComponent();
                                break;
                            case TypeCUnitRoofType.Flat:
                                scene.BuildingDesigner.RoofCreatorComponent = new FlatRoofCreatorComponent();
                                break;
                        }
                        var typeCarrangerComponent = scene.BuildingDesigner.UnitArrangerComponent as TypeCUnitArrangerComponent;
                        typeCarrangerComponent.EntranceType = (TypeCUnitEntranceType)entraceTypeForNumber;
                        break;
                }
                uac = scene.BuildingDesigner.UnitArrangerComponent;
            }
            int numOfUnitsPerFloor = solution.GetInput<int>(NumOfUnitsPerFloorInputSpec);
            uac.NumOfUnitsPerFloor = numOfUnitsPerFloor;
            for (int stack = 0; stack < numOfUnitsPerFloor; ++stack)
            {
                double normalizedEntryIndex = solution.GetInput<double>(
                    NormalizedEntryIndexInputSpecs[stack]
                );
                uac.SetNormalizedEntryIndex(stack, normalizedEntryIndex);
            }

            int numOfFloor = solution.GetInput<int>(NumOfFloorInputSpec);
            if (scene.BuildingDesigner is TypeCBuildingDesigner && numOfFloor > 2)
                numOfFloor = 2;
            uac.NumOfFloors = numOfFloor;

        }

        #endregion

        public override void SetToSolutionOutputs(
            GDModelScene scene, Solution solution
        )
        {
            var tc = scene.Building.TransformComponent;

            // TODO Setting inputs in this method is a temporary way
            // of recording the actual buliding position

            solution.SetInput(BuildingXInputSpec, tc.Transform.Tx);
            solution.SetInput(BuildingYInputSpec, tc.Transform.Ty);
        }

        public string GetNormalizedEntryIndexInputName(int stack)
        {
            var inputName = string.Format(
                NormalizedEntryIndexInputNameFormat, stack
            );

            return inputName;
        }

        public int GetNormalizedEntryIndexInputIndex(int stack)
        {
            var inputSpec = NormalizedEntryIndexInputSpecs[stack];

            var inputIndex =
                Problem?.InputSpecs.IndexOf(inputSpec) ??
                InvalidInputIndex;

            return inputIndex;
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(UnitTypeInputSpec)
             .Append(EntraceTypeInputSpec)
            .Append(BuildingXInputSpec)
            .Append(BuildingYInputSpec)
            .Append(BuildingTNAngleInputSpec)
            .Append(NumOfUnitsPerFloorInputSpec)
            .Append(NumOfFloorInputSpec)
            .Concat(NormalizedEntryIndexInputSpecs);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);
            writer.AddValue(
            nameof(UnitTypeInputSpec),
            UnitTypeInputSpec
            );
            writer.AddValue(
          nameof(EntraceTypeInputSpec),
          EntraceTypeInputSpec
          );
            writer.AddValue(nameof(BuildingXInputSpec), BuildingXInputSpec);
            writer.AddValue(nameof(BuildingYInputSpec), BuildingYInputSpec);
            writer.AddValue(
                nameof(BuildingTNAngleInputSpec), BuildingTNAngleInputSpec
            );
            writer.AddValue(
                nameof(NumOfUnitsPerFloorInputSpec),
                NumOfUnitsPerFloorInputSpec
            );
            writer.AddValue(
            nameof(NumOfFloorInputSpec),
            NumOfFloorInputSpec
       );

            writer.AddValues(
                nameof(NormalizedEntryIndexInputSpecs),
                _normalizedEntryIndexInputSpecs
            );
        }

        #endregion

        #region Properties

        public InputSpec BuildingXInputSpec { get; } =
            new InputSpec
            {
                Name = BuildingXInputName,
                DataSpec = new RealDataSpec()
            };

        public int BuildingXInputIndex =>
            Problem?.InputSpecs.IndexOf(BuildingXInputSpec) ??
            InvalidInputIndex;

        public InputSpec BuildingYInputSpec { get; } =
            new InputSpec
            {
                Name = BuildingYInputName,
                DataSpec = new RealDataSpec()
            };

        public int BuildingYInputIndex =>
            Problem?.InputSpecs.IndexOf(BuildingYInputSpec) ??
            InvalidInputIndex;

        public InputSpec BuildingTNAngleInputSpec { get; } =
            new InputSpec
            {
                Name = BuildingTNAngleInputName,
                DataSpec = new RealDataSpec
                {
                    Min = -Math.PI / 2.0,
                    Max = Math.PI / 2.0
                }
            };

        public int BuildingTNAngleInputIndex =>
            Problem?.InputSpecs.IndexOf(BuildingTNAngleInputSpec) ??
            InvalidInputIndex;

        public InputSpec NumOfUnitsPerFloorInputSpec { get; } =
            new InputSpec
            {
                Name = NumOfUnitsPerFloorInputName,
                DataSpec = new IntegerDataSpec
                {
                    Min = 2,
                    Max = DefaultMaxNumOfUnitsPerFloor
                }
            };

        public int NumOfUnitsPerFloorInputIndex =>
            Problem?.InputSpecs.IndexOf(NumOfUnitsPerFloorInputSpec) ??
            InvalidInputIndex;

        public InputSpec NumOfFloorInputSpec { get; } =
          new InputSpec
          {
              Name = NumOfFloorInputName,
              DataSpec = new IntegerDataSpec
              {
                  Min = 0,
                  Max = DefaultMaxNumOfFloor
              }
          };

        public int NumOfFloorInputIndex =>
            Problem?.InputSpecs.IndexOf(NumOfFloorInputSpec) ??
            InvalidInputIndex;
        public int UnitTypeInputIndex =>
            Problem?.InputSpecs.IndexOf(UnitTypeInputSpec) ??
            InvalidInputIndex;
        public int EntraceTypeInputIndex =>
          Problem?.InputSpecs.IndexOf(EntraceTypeInputSpec) ??
          InvalidInputIndex;

        public InputSpec UnitTypeInputSpec { get; } =
            new InputSpec
            {
                Name = UnitTypeInputName,
                DataSpec = new IntegerDataSpec
                {
                    Min = 1,
                    Max = 2
                }
            };

        public InputSpec EntraceTypeInputSpec { get; } =
          new InputSpec
          {
              Name = EntraceTypeInputName,
              DataSpec = new IntegerDataSpec
              {
                  Min = 0,
                  Max = 1
              }
          };
        public IReadOnlyList<InputSpec> NormalizedEntryIndexInputSpecs
        { get; }
        public List<string> GlstTypeForNumber { get => _glstTypeForNumber; set => _glstTypeForNumber = value; }
        public byte RoofType { get => _roofType; set => _roofType = value; }

        #endregion

        #region Member variables
        private List<string> _glstTypeForNumber = new List<string>();
        private byte _roofType;
        private readonly List<InputSpec> _normalizedEntryIndexInputSpecs =
            new List<InputSpec>();

        #endregion

        #region Constants

        public const string BuildingXInputName =
            "Building X";

        public const string BuildingYInputName =
            "Building Y";

        public const string BuildingTNAngleInputName =
            "Building True North Angle";

        public const string NumOfUnitsPerFloorInputName =
            "Num of Units per Floor";
        public const string NumOfFloorInputName =
           "Num of Floor";
        public const string UnitTypeInputName =
           "Unit Type";
        public const string EntraceTypeInputName =
        "Entrace Type";
        private const string NormalizedEntryIndexInputNameFormat =
            "Normalized Entry Index {0}";

        public const int DefaultMaxNumOfUnitsPerFloor = 6;
        public const int DefaultMaxNumOfFloor = 6;

        #endregion
    }
}
