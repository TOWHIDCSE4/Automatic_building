using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model;
using O3.Foundation;
using Workspaces.Core;
using Workspaces.Json;

namespace DaiwaRentalGD.Optimization.Problems
{
    /// <summary>
    /// Defines the problem of optimizing a GD model.
    /// </summary>
    [Serializable]
    public class GDModelProblem : Problem
    {
        #region Constructors

        public GDModelProblem(GDModelScene scene) : base()
        {
            GDModelScene = scene ??
                throw new ArgumentNullException(nameof(scene));

            SaveGDModelSceneJson();

            Modules = _modules.AsReadOnly();

            InitializeModules();
            UpdateInputSpecsAndOutputSpecs();
        }

        protected GDModelProblem(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            Modules = _modules.AsReadOnly();

            var reader = new WorkspaceItemReader(this, info, context);

            GDModelScene =
                reader.GetValue<GDModelScene>(nameof(GDModelScene));

            SaveGDModelSceneJson();

            _modules.AddRange(
                reader.GetValues<GDModelProblemModule>(nameof(Modules))
            );

            BuildingDesignerProblemModule =
                reader.GetValue<BuildingDesignerProblemModule>(
                    nameof(BuildingDesignerProblemModule)
                );

            ParkingLotDesignerProblemModule =
                reader.GetValue<ParkingLotDesignerProblemModule>(
                    nameof(ParkingLotDesignerProblemModule)
                );

            LandUseEvaluatorProblemModule =
                reader.GetValue<LandUseEvaluatorProblemModule>(
                    nameof(LandUseEvaluatorProblemModule)
                );

            SetbackProblemModule =
                reader.GetValue<SetbackProblemModule>(
                    nameof(SetbackProblemModule)
                );

            SlantPlanesProblemModule =
                reader.GetValue<SlantPlanesProblemModule>(
                    nameof(SlantPlanesProblemModule)
                );

            FinanceEvaluatorProblemModule =
                reader.GetValue<FinanceEvaluatorProblemModule>(
                    nameof(FinanceEvaluatorProblemModule)
                );
        }

        #endregion

        #region Methods

        private void InitializeModules()
        {
            BuildingDesignerProblemModule =
                new BuildingDesignerProblemModule(this);
            _modules.Add(BuildingDesignerProblemModule);

            ParkingLotDesignerProblemModule =
                new ParkingLotDesignerProblemModule(this);
            _modules.Add(ParkingLotDesignerProblemModule);

            LandUseEvaluatorProblemModule =
                new LandUseEvaluatorProblemModule(this);
            _modules.Add(LandUseEvaluatorProblemModule);

            SetbackProblemModule = new SetbackProblemModule(this);
            _modules.Add(SetbackProblemModule);

            SlantPlanesProblemModule = new SlantPlanesProblemModule(this);
            _modules.Add(SlantPlanesProblemModule);

            FinanceEvaluatorProblemModule =
                new FinanceEvaluatorProblemModule(this);
            _modules.Add(FinanceEvaluatorProblemModule);
        }

        new internal void AddInputSpec(InputSpec inputSpec)
        {
            base.AddInputSpec(inputSpec);
        }

        new internal void AddOutputSpec(OutputSpec outputSpec)
        {
            base.AddOutputSpec(outputSpec);
        }

        private void UpdateInputSpecsAndOutputSpecs()
        {
            ClearInputSpecs();
            ClearOutputSpecs();

            foreach (var module in Modules)
            {
                module.UpdateInputSpecs();
                module.UpdateOutputSpecs();
            }
        }

        public void SetFromSolutionInputs(
            GDModelScene scene, Solution solution
        )
        {
            foreach (var module in Modules)
            {
                module.SetFromSolutionInputs(scene, solution);
            }
        }

        public void SetToSolutionOutputs(
            GDModelScene scene, Solution solution
        )
        {
            foreach (var module in Modules)
            {
                module.SetToSolutionOutputs(scene, solution);
            }
        }

        protected override void Evaluate(Solution solution)
        {
            var scene = LoadGDModelSceneJson();

            SetFromSolutionInputs(scene, solution);

            scene.ExecuteUpdate();

            SetToSolutionOutputs(scene, solution);
        }

        public GDModelScene CreateSceneFromSolution(Solution solution)
        {
            var scene = LoadGDModelSceneJson();

            SetFromSolutionInputs(scene, solution);

            scene.ExecuteUpdate();

            return scene;
        }

        private void SaveGDModelSceneJson()
        {
            var workspace = new JsonWorkspace();

            workspace.Save(GDModelScene);

            GDModelSceneJson = workspace.ToJson();
        }

        private GDModelScene LoadGDModelSceneJson()
        {
            var workspace = JsonWorkspace.FromJson(GDModelSceneJson);

            var gdModelScene =
                workspace.Load<GDModelScene>(workspace.ItemUids[0]);

            return gdModelScene;
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(GDModelScene)
            .Concat(Modules);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(GDModelScene), GDModelScene);
            writer.AddValues(nameof(Modules), _modules);

            writer.AddValue(
                nameof(BuildingDesignerProblemModule),
                BuildingDesignerProblemModule
            );
            writer.AddValue(
                nameof(ParkingLotDesignerProblemModule),
                ParkingLotDesignerProblemModule
            );
            writer.AddValue(
                nameof(LandUseEvaluatorProblemModule),
                LandUseEvaluatorProblemModule
            );
            writer.AddValue(
                nameof(SetbackProblemModule),
                SetbackProblemModule
            );
            writer.AddValue(
                nameof(SlantPlanesProblemModule),
                SlantPlanesProblemModule
            );
            writer.AddValue(
                nameof(FinanceEvaluatorProblemModule),
                FinanceEvaluatorProblemModule
            );
        }

        #endregion

        #region Properties

        public GDModelScene GDModelScene { get; }

        private string GDModelSceneJson { get; set; }

        public IReadOnlyList<GDModelProblemModule> Modules { get; }

        public BuildingDesignerProblemModule BuildingDesignerProblemModule
        { get; private set; }

        public ParkingLotDesignerProblemModule ParkingLotDesignerProblemModule
        { get; private set; }

        public LandUseEvaluatorProblemModule LandUseEvaluatorProblemModule
        { get; private set; }

        public SetbackProblemModule SetbackProblemModule
        { get; private set; }

        public SlantPlanesProblemModule SlantPlanesProblemModule
        { get; private set; }

        public FinanceEvaluatorProblemModule FinanceEvaluatorProblemModule
        { get; private set; }

        #endregion

        #region Member variables

        private readonly List<GDModelProblemModule> _modules =
            new List<GDModelProblemModule>();

        #endregion
    }
}
