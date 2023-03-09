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
    /// .GDModelProblem.ParkingLotDesignerProblemModule"/>.
    /// </summary>
    [Serializable]
    public class ParkingLotDesignerSolverModule : GDModelSolverModule
    {
        #region Constructors

        public ParkingLotDesignerSolverModule(GDModelSolver solver) :
            base(solver)
        {
            InitializeDesignOfExps();
            InitializeCrossoverOp();
            InitializeMutationOp();
        }

        protected ParkingLotDesignerSolverModule(
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

        public void UpdateDesignOfExps(Problem problem)
        {
            var problemModule =
                (problem as GDModelProblem)?.ParkingLotDesignerProblemModule;

            if (problemModule == null)
            {
                return;
            }

            var doe = RealUniformDistributionDoe;

            doe.AddInputRange(problemModule.NumOfWalkwayEntrancesInputSpec);

            for (int walkwayIndex = 0;
                walkwayIndex <
                problemModule.WalkwayRoadEdgeIndexIndexInputSpecs.Count;
                ++walkwayIndex)
            {
                var wreiiInputSpec =
                    problemModule
                    .WalkwayRoadEdgeIndexIndexInputSpecs[walkwayIndex];

                var wrespInputSpec =
                    problemModule
                    .WalkwayRoadEdgeSafeParamInputSpecs[walkwayIndex];

                doe.AddInputRange(wreiiInputSpec);
                doe.AddInputRange(wrespInputSpec);
            }

            doe.AddInputRange(problemModule.NumOfDrivewayEntrancesInputSpec);

            for (int drivewayIndex = 0;
                drivewayIndex <
                problemModule.DrivewayRoadEdgeIndexIndexInputSpecs.Count;
                ++drivewayIndex)
            {
                var dreiiInputSpec =
                    problemModule
                    .DrivewayRoadEdgeIndexIndexInputSpecs[drivewayIndex];

                var drespInputSpec =
                    problemModule
                    .DrivewayRoadEdgeSafeParamInputSpecs[drivewayIndex];

                doe.AddInputRange(dreiiInputSpec);
                doe.AddInputRange(drespInputSpec);
            }

            doe.AddInputRange(problemModule.OverlapWithDrivewaysInputSpec);

            doe.AddInputRange(problemModule.AllowDrivewayTurningInputSpec);

            doe.AddInputRange(problemModule.ParkingAtRoadsideOrDrivewayInputSpec);
        }

        public void UpdateCrossoverOp(Problem problem)
        {
            var problemModule =
                (problem as GDModelProblem)?.ParkingLotDesignerProblemModule;

            if (problemModule == null)
            {
                return;
            }

            var numOfWalkwayEntrancesBlendAlpha =
                BlendCxOp.AddBlendAlpha(
                    problemModule.NumOfWalkwayEntrancesInputSpec
                );
            numOfWalkwayEntrancesBlendAlpha.Probability =
                DefaultCxOpProbability;

            for (int walkwayIndex = 0;
                walkwayIndex <
                problemModule.WalkwayRoadEdgeIndexIndexInputSpecs.Count;
                ++walkwayIndex)
            {
                var wreiiInputSpec =
                    problemModule
                    .WalkwayRoadEdgeIndexIndexInputSpecs[walkwayIndex];

                var wreiiBlendAlpha = BlendCxOp.AddBlendAlpha(wreiiInputSpec);
                wreiiBlendAlpha.Probability = DefaultCxOpProbability;

                var wrespInputSpec =
                    problemModule
                    .WalkwayRoadEdgeSafeParamInputSpecs[walkwayIndex];

                var wrespBlendAlpha = BlendCxOp.AddBlendAlpha(wrespInputSpec);
                wrespBlendAlpha.Probability = DefaultCxOpProbability;
            }

            var NumOfDrivewayEntrancesBlendAlpha =
                BlendCxOp.AddBlendAlpha(
                    problemModule.NumOfDrivewayEntrancesInputSpec
                );
            NumOfDrivewayEntrancesBlendAlpha.Probability =
                DefaultCxOpProbability;

            for (int drivewayIndex = 0;
                drivewayIndex <
                problemModule.DrivewayRoadEdgeIndexIndexInputSpecs.Count;
                ++drivewayIndex)
            {
                var dreiiInputSpec =
                    problemModule
                    .DrivewayRoadEdgeIndexIndexInputSpecs[drivewayIndex];

                var dreiiBlendAlpha = BlendCxOp.AddBlendAlpha(dreiiInputSpec);
                dreiiBlendAlpha.Probability = DefaultCxOpProbability;

                var drespInputSpec =
                    problemModule
                    .DrivewayRoadEdgeSafeParamInputSpecs[drivewayIndex];

                var wrespBlendAlpha = BlendCxOp.AddBlendAlpha(drespInputSpec);
                wrespBlendAlpha.Probability = DefaultCxOpProbability;
            }

            var OverlapWithDrivewaysBlendAlpha =
                BlendCxOp.AddBlendAlpha(
                    problemModule.OverlapWithDrivewaysInputSpec
                );
            OverlapWithDrivewaysBlendAlpha.Probability =
                DefaultCxOpProbability;

            var AllowDrivewayTurningBlendAlpha =
                BlendCxOp.AddBlendAlpha(
                    problemModule.AllowDrivewayTurningInputSpec
                );
            AllowDrivewayTurningBlendAlpha.Probability =
                DefaultCxOpProbability;

            var parkingAtRoadsideOrDrivewayBlendAlpha =
                BlendCxOp.AddBlendAlpha(
                    problemModule.ParkingAtRoadsideOrDrivewayInputSpec
                );
            parkingAtRoadsideOrDrivewayBlendAlpha.Probability =
                DefaultCxOpProbability;

        }

        public void UpdateMutationOp(Problem problem)
        {
            var problemModule =
                (problem as GDModelProblem)?.ParkingLotDesignerProblemModule;

            if (problemModule == null)
            {
                return;
            }

            var numOfWalkwayEntrancesOffset =
                RealUniformOffsetMutOp.AddOffset(
                    problemModule.NumOfWalkwayEntrancesInputSpec
                );
            
            numOfWalkwayEntrancesOffset.Probability =
                DefaultMutOpProbability;

            for (int walkwayIndex = 0;
                walkwayIndex <
                problemModule.WalkwayRoadEdgeIndexIndexInputSpecs.Count;
                ++walkwayIndex)
            {
                var wreiiInputSpec =
                    problemModule
                    .WalkwayRoadEdgeIndexIndexInputSpecs[walkwayIndex];

                var wreiiOffset =
                    RealUniformOffsetMutOp.AddOffset(wreiiInputSpec);
                wreiiOffset.Probability = DefaultMutOpProbability;

                var wrespInputSpec =
                    problemModule
                    .WalkwayRoadEdgeSafeParamInputSpecs[walkwayIndex];

                var wrespOffset =
                    RealUniformOffsetMutOp.AddOffset(wrespInputSpec);
                wrespOffset.Probability = HighMutOpProbability;
                wrespOffset.RelativeMaxOffset = HighMutOpMaxOffset;
            }

            var numOfDrivewayEntrancesOffset =
                RealUniformOffsetMutOp.AddOffset(
                    problemModule.NumOfDrivewayEntrancesInputSpec
                );
            numOfDrivewayEntrancesOffset.Probability =
                DefaultMutOpProbability;

            for (int drivewayIndex = 0;
                drivewayIndex <
                problemModule.DrivewayRoadEdgeIndexIndexInputSpecs.Count;
                ++drivewayIndex)
            {
                var dreiiInputSpec =
                    problemModule
                    .DrivewayRoadEdgeIndexIndexInputSpecs[drivewayIndex];

                var dreiiOffset =
                    RealUniformOffsetMutOp.AddOffset(dreiiInputSpec);
                dreiiOffset.Probability = DefaultMutOpProbability;

                var drespInputSpec =
                    problemModule
                    .DrivewayRoadEdgeSafeParamInputSpecs[drivewayIndex];

                var drespOffset =
                    RealUniformOffsetMutOp.AddOffset(drespInputSpec);
                drespOffset.Probability = HighMutOpProbability;
                drespOffset.RelativeMaxOffset = HighMutOpMaxOffset;
            }

            var OverlapWithDrivewaysOffset =
                RealUniformOffsetMutOp.AddOffset(
                    problemModule.OverlapWithDrivewaysInputSpec
                );
            OverlapWithDrivewaysOffset.Probability =
                    DefaultMutOpProbability;

            var AllowDrivewayTurningOffset =
                    RealUniformOffsetMutOp.AddOffset(
                        problemModule.AllowDrivewayTurningInputSpec
                    );
            AllowDrivewayTurningOffset.Probability =
                    DefaultMutOpProbability;
 
            var parkingAtRoadsideOrDrivewayOffset =
                    RealUniformOffsetMutOp.AddOffset(
                        problemModule.ParkingAtRoadsideOrDrivewayInputSpec
                    );
            parkingAtRoadsideOrDrivewayOffset.Probability =
                    DefaultMutOpProbability;
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

        public const string ModuleName = "Parking Lot Designer";

        public const double DefaultCxOpProbability = 0.9;

        public const double DefaultMutOpProbability = 0.2;

        public const double HighMutOpProbability = 0.8;
        public const double HighMutOpMaxOffset = 0.5;

        #endregion
    }
}
