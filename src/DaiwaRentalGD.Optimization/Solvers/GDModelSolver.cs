using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Optimization.Problems;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using O3.Nsga;
using Workspaces.Core;
using O3.Commons.Pareto;

namespace DaiwaRentalGD.Optimization.Solvers
{
    /// <summary>
    /// Solver for solving
    /// <see cref="DaiwaRentalGD.Optimization.Problems.GDModelProblem"/>.
    /// </summary>
    [Serializable]
    public class GDModelSolver : Nsga2Solver
    {
        #region Constructors

        public GDModelSolver() : base()
        {
            PopulationSize = DefaultPopulationSize;

            MaxGeneration = DefaultMaxGeneration;

            Modules = _modules.AsReadOnly();

            InitializeComponents();

            InitializeModules();

            UpdateComponents();
        }

        protected GDModelSolver(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            Modules = _modules.AsReadOnly();

            var reader = new WorkspaceItemReader(this, info, context);

            _modules.AddRange(
                reader.GetValues<GDModelSolverModule>(nameof(Modules))
            );

            BuildingDesignerSolverModule =
                reader.GetValue<BuildingDesignerSolverModule>(
                    nameof(BuildingDesignerSolverModule)
                );

            ParkingLotDesignerSolverModule =
                reader.GetValue<ParkingLotDesignerSolverModule>(
                    nameof(ParkingLotDesignerSolverModule)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            DesignOfExps = new CompositeDoe();

            CrossoverOp = new CompositeCxOp();

            MutationOp = new CompositeMutOp();
        }

        private void InitializeModules()
        {
            BuildingDesignerSolverModule =
                new BuildingDesignerSolverModule(this);

            _modules.Add(BuildingDesignerSolverModule);

            ParkingLotDesignerSolverModule =
                new ParkingLotDesignerSolverModule(this);

            _modules.Add(ParkingLotDesignerSolverModule);
        }

        private void UpdateComponents()
        {
            UpdateDesignOfExps();
            UpdateCrossoverOp();
            UpdateMutationOp();
            //SolutionArchive = new ParetoSet();
        }

        private void UpdateDesignOfExps()
        {
            CompositeDoe.ClearChildDoes();

            foreach (var module in Modules)
            {
                if (module.DesignOfExps != null)
                {
                    CompositeDoe.AddChildDoe(module.DesignOfExps);
                }
            }
        }

        private void UpdateCrossoverOp()
        {
            CompositeCxOp.ClearChildCxOps();

            foreach (var module in Modules)
            {
                if (module.CrossoverOp != null)
                {
                    CompositeCxOp.AddChildCxOp(module.CrossoverOp);
                }
            }
        }

        private void UpdateMutationOp()
        {
            CompositeMutOp.ClearChildMutOps();

            foreach (var module in Modules)
            {
                if (module.MutationOp != null)
                {
                    CompositeMutOp.AddChildMutOp(module.MutationOp);
                }
            }
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Concat(Modules);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValues(nameof(Modules), _modules);

            writer.AddValue(
                nameof(BuildingDesignerSolverModule),
                BuildingDesignerSolverModule
            );

            writer.AddValue(
                nameof(ParkingLotDesignerSolverModule),
                ParkingLotDesignerSolverModule
            );
        }

        public struct MutAdjust
        {
            public string name;
            public double probRate;
            public double minProbRate;
            public double offsetRate;
            public double minOffsetRate;
        }

        protected override void Update()
        {
            base.Update();

            // Trigger OnProgress.
            OnProgress();

            /* 突然変位確率と、最大変位幅の調整 */
            MutAdjust[] ma = new MutAdjust[] {
                new MutAdjust { name="Building X",  probRate=0.9, minProbRate = 0.2, offsetRate= 0.9, minOffsetRate = 0.1 },
                new MutAdjust { name="Building Y",  probRate=0.9, minProbRate = 0.2, offsetRate= 0.9, minOffsetRate = 0.1 },
                new MutAdjust { name="Walkway Road Edge Param",  probRate=1.0, minProbRate = 0.2, offsetRate= 0.9, minOffsetRate = 0.1 },
                new MutAdjust { name="Driveway Road Edge Param", probRate=1.0, minProbRate = 0.2, offsetRate= 0.9, minOffsetRate = 0.1 }
            };

            var mutOp = MutationOp;
            foreach (var mo in CompositeMutOp.ChildMutOps)
            {
                string s = mo.Name;
                var ruomo = mo as RealUniformOffsetMutOp;
                foreach (var moc in ruomo.RealUniformOffsets)
                {
                    string nm = moc.InputSpec.Name;
                    foreach (var ss in ma)
                    {
                        if (nm.StartsWith(ss.name))
                        {
                            moc.Probability = moc.Probability > ss.minProbRate ? moc.Probability * ss.probRate : ss.minProbRate;
                            moc.RelativeMaxOffset = moc.RelativeMaxOffset > ss.minOffsetRate ? moc.RelativeMaxOffset * ss.offsetRate : ss.minOffsetRate;
                        }
                    }
                }
            }            
        }
        private void OnProgress()
        {
            int total = MaxGeneration;
            int current = CurrentGeneration;
            int percent = 95 * current / total;
            var e = new ProgressEventArgs(percent);
            ProgressEventHandler?.Invoke(this, e);
        }

        #endregion

        #region Properties
        public event EventHandler<ProgressEventArgs> ProgressEventHandler;

        public GDModelProblem GDModelProblem
        {
            get => Problem as GDModelProblem;
            set => Problem = value;
        }

        public CompositeDoe CompositeDoe
        {
            get => DesignOfExps as CompositeDoe;
            set => DesignOfExps = value;
        }

        public CompositeCxOp CompositeCxOp
        {
            get => CrossoverOp as CompositeCxOp;
            set => CrossoverOp = value;
        }

        public CompositeMutOp CompositeMutOp
        {
            get => MutationOp as CompositeMutOp;
            set => MutationOp = value;
        }

        public IReadOnlyList<GDModelSolverModule> Modules { get; }

        public BuildingDesignerSolverModule BuildingDesignerSolverModule
        { get; private set; }

        public ParkingLotDesignerSolverModule ParkingLotDesignerSolverModule
        { get; private set; }

        #endregion

        #region Member variables

        private readonly List<GDModelSolverModule> _modules =
            new List<GDModelSolverModule>();

        #endregion

        #region Constants

        public new const int DefaultPopulationSize = 20;

        public new const int DefaultMaxGeneration = 20;

        #endregion
    }

    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(int percent)
        {
            Percent = percent;
        }
        public int Percent { get; set; }
    }
}
