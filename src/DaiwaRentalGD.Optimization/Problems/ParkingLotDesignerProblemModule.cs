using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model;
using O3.Commons.DataSpecs;
using O3.Foundation;
using Workspaces.Core;

namespace DaiwaRentalGD.Optimization.Problems
{
    /// <summary>
    /// Problem module that corresponds to
    /// <see cref="DaiwaRentalGD.Model.GDModelScene.ParkingLotDesigner"/>.
    /// </summary>
    [Serializable]
    public class ParkingLotDesignerProblemModule : GDModelProblemModule
    {
        #region Constructors

        public ParkingLotDesignerProblemModule(GDModelProblem problem) :
            base(problem)
        {
            WalkwayRoadEdgeIndexIndexInputSpecs =
                _walkwayRoadEdgeIndexIndexInputSpecs.AsReadOnly();

            WalkwayRoadEdgeSafeParamInputSpecs =
                _walkwayRoadEdgeSafeParamInputSpecs.AsReadOnly();

            DrivewayRoadEdgeIndexIndexInputSpecs =
                _drivewayRoadEdgeIndexIndexInputSpecs.AsReadOnly();

            DrivewayRoadEdgeSafeParamInputSpecs =
                _drivewayRoadEdgeSafeParamInputSpecs.AsReadOnly();
        }

        protected ParkingLotDesignerProblemModule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            WalkwayRoadEdgeIndexIndexInputSpecs =
                _walkwayRoadEdgeIndexIndexInputSpecs.AsReadOnly();

            WalkwayRoadEdgeSafeParamInputSpecs =
                _walkwayRoadEdgeSafeParamInputSpecs.AsReadOnly();

            var reader = new WorkspaceItemReader(this, info, context);

            NumOfWalkwayEntrancesInputSpec = reader.GetValue<InputSpec>(
                nameof(NumOfWalkwayEntrancesInputSpec)
            );

            _walkwayRoadEdgeIndexIndexInputSpecs.AddRange(
                reader.GetValues<InputSpec>(
                    nameof(WalkwayRoadEdgeIndexIndexInputSpecs)
                )
            );

            _walkwayRoadEdgeSafeParamInputSpecs.AddRange(
                reader.GetValues<InputSpec>(
                    nameof(WalkwayRoadEdgeSafeParamInputSpecs)
                )
            );
            //
            DrivewayRoadEdgeIndexIndexInputSpecs =
                _drivewayRoadEdgeIndexIndexInputSpecs.AsReadOnly();

            DrivewayRoadEdgeSafeParamInputSpecs =
                _drivewayRoadEdgeSafeParamInputSpecs.AsReadOnly();

            NumOfDrivewayEntrancesInputSpec = reader.GetValue<InputSpec>(
                nameof(NumOfDrivewayEntrancesInputSpec)
            );

            _drivewayRoadEdgeIndexIndexInputSpecs.AddRange(
                reader.GetValues<InputSpec>(
                    nameof(DrivewayRoadEdgeIndexIndexInputSpecs)
                )
            );

            _drivewayRoadEdgeSafeParamInputSpecs.AddRange(
                reader.GetValues<InputSpec>(
                    nameof(DrivewayRoadEdgeSafeParamInputSpecs)
                )
            );
            //

            OverlapWithDrivewaysInputSpec = reader.GetValue<InputSpec>(
                nameof(OverlapWithDrivewaysInputSpec)
            );

            AllowDrivewayTurningInputSpec = reader.GetValue<InputSpec>(
                nameof(AllowDrivewayTurningInputSpec)
            );

            ParkingAtRoadsideOrDrivewayInputSpec = reader.GetValue<InputSpec>(
                nameof(ParkingAtRoadsideOrDrivewayInputSpec)
            );
        }

        #endregion

        #region Methods

        #region UpdateInputSpecs()

        public override void UpdateInputSpecs()
        {
            UpdateInputSpecsFromWalkwayDesignerComponent();
        }

        private void UpdateInputSpecsFromWalkwayDesignerComponent()
        {
            _walkwayRoadEdgeIndexIndexInputSpecs.Clear();
            _walkwayRoadEdgeSafeParamInputSpecs.Clear();

            var gdms = Problem.GDModelScene;

            var wdc = gdms.ParkingLotDesigner.WalkwayDesignerComponent;

            NumOfWalkwayEntrancesInputSpec.Constant =
                wdc.NumOfWalkwayEntrances;

            Problem.AddInputSpec(NumOfWalkwayEntrancesInputSpec);

            int numOfRoadEdges =
                gdms.Site.SiteComponent.RoadEdgeIndices.Count;

            for (int walkwayIndex = 0;
                walkwayIndex < DefaultMaxNumOfWalkwayEntrances;
                ++walkwayIndex)
            {
                var wreiiInputSpec = new InputSpec
                {
                    Name = GetWalkwayRoadEdgeIndexIndexInputName(
                        walkwayIndex
                    ),
                    DataSpec = new IntegerDataSpec
                    {
                        Min = 0,
                        Max = numOfRoadEdges - 1
                    }
                };

                _walkwayRoadEdgeIndexIndexInputSpecs.Add(wreiiInputSpec);

                Problem.AddInputSpec(wreiiInputSpec);

                var wrespInputSpec = new InputSpec
                {
                    Name = GetWalkwayRoadEdgeSafeParamInputName(
                        walkwayIndex
                    ),
                    DataSpec = new RealDataSpec
                    {
                        Min = 0.0,
                        Max = 1.0
                    }
                };

                _walkwayRoadEdgeSafeParamInputSpecs.Add(wrespInputSpec);

                Problem.AddInputSpec(wrespInputSpec);
            }

            for (int walkwayIndex = 0;
                walkwayIndex < wdc.NumOfWalkwayEntrances;
                ++walkwayIndex)
            {
                var wec =
                    wdc.WalkwayEntrances[walkwayIndex]
                    .WalkwayEntranceComponent;

                var wreiiInputSpec =
                    WalkwayRoadEdgeIndexIndexInputSpecs[walkwayIndex];

                wreiiInputSpec.Constant = wec.RoadEdgeIndexIndex;

                var wrespInputSpec =
                    WalkwayRoadEdgeSafeParamInputSpecs[walkwayIndex];

                wrespInputSpec.Constant = wec.RoadEdgeSafeParam;
            }

            //
            _drivewayRoadEdgeIndexIndexInputSpecs.Clear();
            _drivewayRoadEdgeSafeParamInputSpecs.Clear();

            var ddc = gdms.ParkingLotDesigner.DrivewayDesignerComponent;

            NumOfDrivewayEntrancesInputSpec.Constant =
                ddc.NumOfDrivewayEntrances;

            Problem.AddInputSpec(NumOfDrivewayEntrancesInputSpec);

            for (int drivewayIndex = 0;
                drivewayIndex < DefaultMaxNumOfDrivewayEntrances;
                ++drivewayIndex)
            {
                var dreiiInputSpec = new InputSpec
                {
                    Name = GetDrivewayRoadEdgeIndexIndexInputName(
                        drivewayIndex
                    ),
                    DataSpec = new IntegerDataSpec
                    {
                        Min = 0,
                        Max = numOfRoadEdges - 1
                    }
                };

                _drivewayRoadEdgeIndexIndexInputSpecs.Add(dreiiInputSpec);

                Problem.AddInputSpec(dreiiInputSpec);

                var drespInputSpec = new InputSpec
                {
                    Name = GetDrivewayRoadEdgeSafeParamInputName(
                        drivewayIndex
                    ),
                    DataSpec = new RealDataSpec
                    {
                        Min = 0.0,
                        Max = 1.0
                    }
                };

                _drivewayRoadEdgeSafeParamInputSpecs.Add(drespInputSpec);

                Problem.AddInputSpec(drespInputSpec);
            }

            for (int drivewayIndex = 0;
                drivewayIndex < ddc.NumOfDrivewayEntrances;
                ++drivewayIndex)
            {
                var dec =
                    ddc.DrivewayEntrances[drivewayIndex]
                    .DrivewayEntranceComponent;

                var dreiiInputSpec =
                    DrivewayRoadEdgeIndexIndexInputSpecs[drivewayIndex];

                dreiiInputSpec.Constant = dec.RoadEdgeIndexIndex;

                var drespInputSpec =
                    DrivewayRoadEdgeSafeParamInputSpecs[drivewayIndex];

                drespInputSpec.Constant = dec.RoadEdgeSafeParam;
            }

            //
            OverlapWithDrivewaysInputSpec.Constant =
                wdc.OverlapWithDriveways ? 1 : 0;
            Problem.AddInputSpec(OverlapWithDrivewaysInputSpec);

            AllowDrivewayTurningInputSpec.Constant =
                ddc.AllowDrivewayTurning ? 1 : 0;
            Problem.AddInputSpec(AllowDrivewayTurningInputSpec);

            Problem.AddInputSpec(ParkingAtRoadsideOrDrivewayInputSpec);
        }

        #endregion

        #region SetFromSolutionInputs()

        public override void SetFromSolutionInputs(
            GDModelScene scene, Solution solution
        )
        {
            SetFromSolutionInputsToWalkwayDesignerComponent(
                scene, solution
            );
        }

        private void SetFromSolutionInputsToWalkwayDesignerComponent(
            GDModelScene scene, Solution solution
        )
        {
            var wdc = scene.ParkingLotDesigner.WalkwayDesignerComponent;
            wdc.ParkingLot.ParkingLotComponent.ParkingLotDesigner = scene.ParkingLotDesigner;

            int numOfWalkwayEntrances =
                solution.GetInput<int>(NumOfWalkwayEntrancesInputSpec);

            wdc.NumOfWalkwayEntrances = numOfWalkwayEntrances;

            for (int walkwayIndex = 0; walkwayIndex < numOfWalkwayEntrances;
                ++walkwayIndex)
            {
                var wec =
                    wdc.WalkwayEntrances[walkwayIndex]
                    .WalkwayEntranceComponent;

                var wreiiInputSpec =
                    WalkwayRoadEdgeIndexIndexInputSpecs[walkwayIndex];
                int wreii = solution.GetInput<int>(wreiiInputSpec);

                wec.RoadEdgeIndexIndex = wreii;

                var wrespInputSpec =
                    WalkwayRoadEdgeSafeParamInputSpecs[walkwayIndex];
                double wresp = solution.GetInput<double>(wrespInputSpec);

                wec.RoadEdgeSafeParam = wresp;
            }

            var ddc = scene.ParkingLotDesigner.DrivewayDesignerComponent;

            int numOfDrivewayEntrances =
                solution.GetInput<int>(NumOfDrivewayEntrancesInputSpec);

            ddc.NumOfDrivewayEntrances = numOfDrivewayEntrances;

            for (int drivewayIndex = 0; drivewayIndex < numOfDrivewayEntrances;
                ++drivewayIndex)
            {
                var dec =
                    ddc.DrivewayEntrances[drivewayIndex]
                    .DrivewayEntranceComponent;

                var dreiiInputSpec =
                    DrivewayRoadEdgeIndexIndexInputSpecs[drivewayIndex];
                int dreii = solution.GetInput<int>(dreiiInputSpec);

                dec.RoadEdgeIndexIndex = dreii;

                var drespInputSpec =
                    DrivewayRoadEdgeSafeParamInputSpecs[drivewayIndex];
                double dresp = solution.GetInput<double>(drespInputSpec);

                dec.RoadEdgeSafeParam = dresp;
            }

            int overlapWithDriveways =
                solution.GetInput<int>(OverlapWithDrivewaysInputSpec);

            wdc.OverlapWithDriveways = overlapWithDriveways > 0 ? true : false;
            wdc.ParkingLot.ParkingLotComponent.ParkingLotDesigner.setDesignOrder_Overlap(wdc.OverlapWithDriveways);

            int allowDrivewayTurning =
                solution.GetInput<int>(AllowDrivewayTurningInputSpec);

            ddc.AllowDrivewayTurning = allowDrivewayTurning > 0 ? true : false;
            ddc.DrivewayShapeGrammar.setShapeGrammerRule_AllowDrivewayTuring(ddc.AllowDrivewayTurning);


            var rcpadc = scene.ParkingLotDesigner.RoadsideCarParkingAreaDesignerComponent;

            int parkingAtRoadsideOrDriveway =
                solution.GetInput<int>(ParkingAtRoadsideOrDrivewayInputSpec);

            var gdms = Problem.GDModelScene;
            int numOfRoadEdges = gdms.Site.SiteComponent.RoadEdgeIndices.Count;
            for (int rei = 0; rei < numOfRoadEdges; rei++) {
                rcpadc.RoadsideCarParkingAreaParamsList[rei].IsEnabled = parkingAtRoadsideOrDriveway > 0 ? true : false;
            }
        }

        #endregion

        #region SetToSolutionOutputs()

        public override void SetToSolutionOutputs(
            GDModelScene scene, Solution solution
        )
        {
            var plrc =
                scene.ParkingLotDesigner.ParkingLotRequirementsComponent;

            if (!plrc.IsBuildingAccessibleViaWalkway)
            {
                solution.Fail();
            }

            if (plrc.CarParkingSpaceFulfillment < 1.0)
            {
                solution.Fail();
            }
        }

        #endregion

        public string GetWalkwayRoadEdgeIndexIndexInputName(int walkwayIndex)
        {
            var inputName = string.Format(
                RoadEdgeIndexIndexInputNameFormat,
                walkwayIndex
            );

            return inputName;
        }

        public int GetWalkwayRoadEdgeIndexIndexInputIndex(int walkwayIndex)
        {
            var inputSpec = WalkwayRoadEdgeIndexIndexInputSpecs[walkwayIndex];

            var inputIndex = Problem.InputSpecs.IndexOf(inputSpec);

            return inputIndex;
        }

        public string GetWalkwayRoadEdgeSafeParamInputName(int walkwayIndex)
        {
            var inputName = string.Format(
                RoadEdgeSafeParamInputNameFormat,
                walkwayIndex
            );

            return inputName;
        }

        public int GetWalkwayRoadEdgeSafeParamInputIndex(int walkwayIndex)
        {
            var inputSpec = WalkwayRoadEdgeSafeParamInputSpecs[walkwayIndex];

            var inputIndex = Problem.InputSpecs.IndexOf(inputSpec);

            return inputIndex;
        }

        public string GetDrivewayRoadEdgeIndexIndexInputName(int drivewayIndex)
        {
            var inputName = string.Format(
                DrivewayRoadEdgeIndexIndexInputNameFormat,
                drivewayIndex
            );

            return inputName;
        }

        public int GetDrivewayRoadEdgeIndexIndexInputIndex(int drivewayIndex)
        {
            var inputSpec = DrivewayRoadEdgeIndexIndexInputSpecs[drivewayIndex];

            var inputIndex = Problem.InputSpecs.IndexOf(inputSpec);

            return inputIndex;
        }

        public string GetDrivewayRoadEdgeSafeParamInputName(int drivewayIndex)
        {
            var inputName = string.Format(
                DrivewayRoadEdgeSafeParamInputNameFormat,
                drivewayIndex
            );

            return inputName;
        }

        public int GetDrivewayRoadEdgeSafeParamInputIndex(int drivewayIndex)
        {
            var inputSpec = DrivewayRoadEdgeSafeParamInputSpecs[drivewayIndex];

            var inputIndex = Problem.InputSpecs.IndexOf(inputSpec);

            return inputIndex;
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(NumOfWalkwayEntrancesInputSpec)
            .Append(OverlapWithDrivewaysInputSpec)
            .Concat(WalkwayRoadEdgeIndexIndexInputSpecs)
            .Concat(WalkwayRoadEdgeSafeParamInputSpecs)
            .Append(NumOfDrivewayEntrancesInputSpec)
            .Concat(DrivewayRoadEdgeIndexIndexInputSpecs)
            .Concat(DrivewayRoadEdgeSafeParamInputSpecs)
            .Append(AllowDrivewayTurningInputSpec)
            .Append(ParkingAtRoadsideOrDrivewayInputSpec);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(NumOfWalkwayEntrancesInputSpec),
                NumOfWalkwayEntrancesInputSpec
            );

            writer.AddValues(
                nameof(WalkwayRoadEdgeIndexIndexInputSpecs),
                WalkwayRoadEdgeIndexIndexInputSpecs
            );

            writer.AddValues(
                nameof(WalkwayRoadEdgeSafeParamInputSpecs),
                WalkwayRoadEdgeSafeParamInputSpecs
            );

            writer.AddValue(
                nameof(NumOfDrivewayEntrancesInputSpec),
                NumOfDrivewayEntrancesInputSpec
            );

            writer.AddValues(
                nameof(DrivewayRoadEdgeIndexIndexInputSpecs),
                DrivewayRoadEdgeIndexIndexInputSpecs
            );

            writer.AddValues(
                nameof(DrivewayRoadEdgeSafeParamInputSpecs),
                DrivewayRoadEdgeSafeParamInputSpecs
            );

            writer.AddValue(
                nameof(OverlapWithDrivewaysInputSpec),
                OverlapWithDrivewaysInputSpec
            );

            writer.AddValue(
                nameof(AllowDrivewayTurningInputSpec),
                AllowDrivewayTurningInputSpec
            );

            writer.AddValue(
                nameof(ParkingAtRoadsideOrDrivewayInputSpec),
                ParkingAtRoadsideOrDrivewayInputSpec
            );
        }
        

        #endregion

        #region Properties

        public InputSpec NumOfWalkwayEntrancesInputSpec { get; } =
            new InputSpec
            {
                Name = NumOfWalkwayEntrancesInputName,
                DataSpec = new IntegerDataSpec
                {
                    Min = 1,
                    Max = DefaultMaxNumOfWalkwayEntrances
                }
            };

        public int NumOfWalkwayEntrancesInputIndex =>
            Problem.InputSpecs.IndexOf(NumOfWalkwayEntrancesInputSpec);

        public IReadOnlyList<InputSpec> WalkwayRoadEdgeIndexIndexInputSpecs
        { get; }

        public IReadOnlyList<InputSpec> WalkwayRoadEdgeSafeParamInputSpecs
        { get; }

        public InputSpec NumOfDrivewayEntrancesInputSpec { get; } =
            new InputSpec
            {
                Name = NumOfDrivewayEntrancesInputName,
                DataSpec = new IntegerDataSpec
                {
                    Min = 1,
                    Max = DefaultMaxNumOfDrivewayEntrances
                }
            };

        public int NumOfDrivewayEntrancesInputIndex =>
            Problem.InputSpecs.IndexOf(NumOfDrivewayEntrancesInputSpec);

        public IReadOnlyList<InputSpec> DrivewayRoadEdgeIndexIndexInputSpecs
        { get; }

        public IReadOnlyList<InputSpec> DrivewayRoadEdgeSafeParamInputSpecs
        { get; }

        public InputSpec OverlapWithDrivewaysInputSpec { get; } =
            new InputSpec
            {
                Name = OverlapWithDrivewaysInputName,
                DataSpec = new IntegerDataSpec
                {
                    Min = 0,
                    Max = 1
                }
            };

        public InputSpec AllowDrivewayTurningInputSpec { get; } =
            new InputSpec
            {
                Name = AllowDrivewayTurningInputName,
                DataSpec = new IntegerDataSpec
                {
                    Min = 0,
                    Max = 1
                }
            };

        public InputSpec ParkingAtRoadsideOrDrivewayInputSpec { get; } =
            new InputSpec
            {
                Name = ParkingAtRoadsideOrDrivewayInputName,
                DataSpec = new IntegerDataSpec
                {
                    Min = 0,
                    Max = 1
                }
            };
        #endregion

        #region Member variables

        private readonly List<InputSpec> _walkwayRoadEdgeIndexIndexInputSpecs
            = new List<InputSpec>();

        private readonly List<InputSpec> _walkwayRoadEdgeSafeParamInputSpecs
            = new List<InputSpec>();

        private readonly List<InputSpec> _drivewayRoadEdgeIndexIndexInputSpecs
            = new List<InputSpec>();

        private readonly List<InputSpec> _drivewayRoadEdgeSafeParamInputSpecs
            = new List<InputSpec>();

        #endregion

        #region Constants

        public const string NumOfWalkwayEntrancesInputName =
            "Num of Walkway Entrances";

        private const string RoadEdgeIndexIndexInputNameFormat =
            "Walkway Road Edge {0}";

        private const string RoadEdgeSafeParamInputNameFormat =
            "Walkway Road Edge Param {0}";

        public const string NumOfDrivewayEntrancesInputName =
            "Num of Drivekway Entrances";

        private const string DrivewayRoadEdgeIndexIndexInputNameFormat =
            "Driveway Road Edge {0}";

        private const string DrivewayRoadEdgeSafeParamInputNameFormat =
            "Driveway Road Edge Param {0}";

        private const string OverlapWithDrivewaysInputName =
            "Walkway Overlap with Driveways";

        private const string AllowDrivewayTurningInputName =
            "Allow Driveway Turning";

        public const string ParkingAtRoadsideOrDrivewayInputName =
            "Parking at Roadside or Driveway";

        public const int DefaultMaxNumOfWalkwayEntrances = 1;

        public const int DefaultMaxNumOfDrivewayEntrances = 1;

        public const string BuildingNotAccessibleMessage =
            "Building not accessible via walkway";

        public const string CarParkingSpacesInsufficientMessage =
            "Insufficient car parking spaces";

        #endregion
    }
}
