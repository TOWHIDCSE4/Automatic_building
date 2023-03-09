using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Optimization.Problems;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using O3.Commons.RandomNumberGenerators;
using O3.Foundation;

namespace DaiwaRentalGD.Optimization.Solvers
{
    /// <summary>
    /// Solver module that corresponds to
    /// <see cref="DaiwaRentalGD.Optimization.Problems
    /// .GDModelProblem.BuildingDesignerProblemModule"/>.
    /// </summary>
    [Serializable]
    public class BuildingDesignerSolverModule : GDModelSolverModule
    {
        #region Constructors

        public BuildingDesignerSolverModule(GDModelSolver solver) :
            base(solver)
        {
            InitializeDesignOfExps();
            InitializeCrossoverOp();
            InitializeMutationOp();
        }

        protected BuildingDesignerSolverModule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            RealUniformDistributionDoe.ProblemChanged +=
                (sender, e) => UpdateDesignOfExps(e.CurrentProblem);

            BlendCxOp.ProblemChanged +=
                (sender, e) => UpdateCrossoverOp(e.CurrentProblem);

            RealUniformOffsetMutOp.ProblemChanged +=
                (sender, e) => UpdateMutationOp(e.CurrentProblem);
        }

        #endregion

        #region Methods

        private void InitializeDesignOfExps()
        {
            RealUniformDistributionDoe = new RealUniformDistributionDoe
            {
                Name = ModuleName,
                RandomNumberGenerator = new MersenneTwisterRng()
            };

            RealUniformDistributionDoe.ProblemChanged +=
                (sender, e) => UpdateDesignOfExps(e.CurrentProblem);
        }

        private void InitializeCrossoverOp()
        {
            BlendCxOp = new BlendCxOp
            {
                Name = ModuleName,
                RandomNumberGenerator = new MersenneTwisterRng()
            };

            BlendCxOp.ProblemChanged +=
                (sender, e) => UpdateCrossoverOp(e.CurrentProblem);
        }

        private void InitializeMutationOp()
        {
            RealUniformOffsetMutOp = new RealUniformOffsetMutOp
            {
                Name = ModuleName,
                RandomNumberGenerator = new MersenneTwisterRng()
            };

            RealUniformOffsetMutOp.ProblemChanged +=
                (sender, e) => UpdateMutationOp(e.CurrentProblem);
        }

        private void UpdateDesignOfExps(Problem problem)
        {
            var problemModule =
                (problem as GDModelProblem)?.BuildingDesignerProblemModule;

            if (problemModule == null)
            {
                return;
            }

            RealUniformDistributionDoe
                .AddInputRange(problemModule.BuildingXInputSpec);

            RealUniformDistributionDoe
                .AddInputRange(problemModule.BuildingYInputSpec);

            RealUniformDistributionDoe
                .AddInputRange(problemModule.BuildingTNAngleInputSpec);

            RealUniformDistributionDoe
                .AddInputRange(problemModule.NumOfUnitsPerFloorInputSpec);
            RealUniformDistributionDoe
               .AddInputRange(problemModule.NumOfFloorInputSpec);
            RealUniformDistributionDoe
         .AddInputRange(problemModule.UnitTypeInputSpec);
            RealUniformDistributionDoe
        .AddInputRange(problemModule.EntraceTypeInputSpec);

            foreach (var inputSpec in
                problemModule.NormalizedEntryIndexInputSpecs)
            {
                RealUniformDistributionDoe.AddInputRange(inputSpec);
            }
        }

        private void UpdateCrossoverOp(Problem problem)
        {
            var problemModule =
                (problem as GDModelProblem)?.BuildingDesignerProblemModule;

            if (problemModule == null)
            {
                return;
            }

            var buildingXBlendAlpha =
                BlendCxOp.AddBlendAlpha(problemModule.BuildingXInputSpec);
            buildingXBlendAlpha.Probability = DefaultCxOpProbability;

            var buildingYBlendAlpha =
                BlendCxOp.AddBlendAlpha(problemModule.BuildingYInputSpec);
            buildingYBlendAlpha.Probability = DefaultCxOpProbability;

            var buildingTNAngleBlendAlpha =
                BlendCxOp.AddBlendAlpha(
                    problemModule.BuildingTNAngleInputSpec
                );
            buildingTNAngleBlendAlpha.Probability = DefaultCxOpProbability;

            var numOfUnitsPerFloorBlendAlpha =
                BlendCxOp.AddBlendAlpha(
                    problemModule.NumOfUnitsPerFloorInputSpec
                );
            numOfUnitsPerFloorBlendAlpha.Probability = DefaultCxOpProbability;



            var numOfFloorBlendAlpha =
                BlendCxOp.AddBlendAlpha(
                    problemModule.NumOfFloorInputSpec
                );
            numOfUnitsPerFloorBlendAlpha.Probability = DefaultCxOpProbability;



            var unitTypeBlendAlpha =
                BlendCxOp.AddBlendAlpha(
                    problemModule.UnitTypeInputSpec
                );
            var entraceTypeBlendAlpha =
              BlendCxOp.AddBlendAlpha(
                  problemModule.EntraceTypeInputSpec
              );
            numOfUnitsPerFloorBlendAlpha.Probability = DefaultCxOpProbability;

            foreach (var inputSpec in
                problemModule.NormalizedEntryIndexInputSpecs)
            {
                var blendAlpha = BlendCxOp.AddBlendAlpha(inputSpec);
                blendAlpha.Probability = DefaultCxOpProbability;
            }
        }

        private void UpdateMutationOp(Problem problem)
        {
            var problemModule =
                (problem as GDModelProblem)?.BuildingDesignerProblemModule;

            if (problemModule == null)
            {
                return;
            }

            var buildingXOffset =
                RealUniformOffsetMutOp.AddOffset(
                    problemModule.BuildingXInputSpec
                );
            buildingXOffset.Probability = HighMutOpProbability;
            buildingXOffset.RelativeMaxOffset = HighMutOpMaxOffset;

            var buildingYOffset =
                RealUniformOffsetMutOp.AddOffset(
                    problemModule.BuildingYInputSpec
                );
            buildingYOffset.Probability = HighMutOpProbability;
            buildingYOffset.RelativeMaxOffset = HighMutOpMaxOffset;

            var buildingTNAngleOffset =
                RealUniformOffsetMutOp.AddOffset(
                    problemModule.BuildingTNAngleInputSpec
                );
            buildingTNAngleOffset.Probability = DefaultMutOpProbability;


            var numOfUnitsPerFloorOffset =
                RealUniformOffsetMutOp.AddOffset(
                    problemModule.NumOfUnitsPerFloorInputSpec
                );
            numOfUnitsPerFloorOffset.Probability = HighMutOpProbability;
            numOfUnitsPerFloorOffset.UseRelative = false;
            numOfUnitsPerFloorOffset.AbsoluteMaxOffset = 1;


            var unitTypeOffset =
                RealUniformOffsetMutOp.AddOffset(
                    problemModule.UnitTypeInputSpec
                );
            unitTypeOffset.Probability = HighMutOpProbability;
            unitTypeOffset.UseRelative = false;
            unitTypeOffset.AbsoluteMaxOffset = 1;

            var entraceTypeOffset =
              RealUniformOffsetMutOp.AddOffset(
                  problemModule.EntraceTypeInputSpec
              );

            entraceTypeOffset.Probability = DefaultMutOpProbability;
            entraceTypeOffset.UseRelative = false;
            entraceTypeOffset.AbsoluteMaxOffset = 1;


            var numOfFloorOffset =
              RealUniformOffsetMutOp.AddOffset(
                  problemModule.NumOfFloorInputSpec
              );
            numOfFloorOffset.Probability = HighMutOpProbability;
            numOfFloorOffset.UseRelative = false;
            numOfFloorOffset.AbsoluteMaxOffset = 1;

            foreach (var inputSpec in
                problemModule.NormalizedEntryIndexInputSpecs)
            {
                var offset = RealUniformOffsetMutOp.AddOffset(inputSpec);
                offset.Probability = DefaultMutOpProbability;
            }
        }

        #endregion

        #region Properties

        private RealUniformDistributionDoe RealUniformDistributionDoe
        {
            get => DesignOfExps as RealUniformDistributionDoe;
            set => DesignOfExps = value;
        }

        private BlendCxOp BlendCxOp
        {
            get => CrossoverOp as BlendCxOp;
            set => CrossoverOp = value;
        }

        private RealUniformOffsetMutOp RealUniformOffsetMutOp
        {
            get => MutationOp as RealUniformOffsetMutOp;
            set => MutationOp = value;
        }

        #endregion

        #region Constants

        public const string ModuleName = "Building Designer";

        public const double DefaultCxOpProbability = 0.9;

        public const double DefaultMutOpProbability = 0.2;

        public const double HighMutOpProbability = 0.8;
        public const double HighMutOpMaxOffset = 0.2;

        #endregion
    }
}
