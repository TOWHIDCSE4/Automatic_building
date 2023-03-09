using System.Collections.Generic;
using System.Linq;
using DaiwaRentalGD.Optimization.Samples.Foundation;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using O3.Commons.Evolution.TerminationConditions;
using O3.Commons.Pareto;
using O3.Commons.RandomNumberGenerators;
using O3.Commons.Utilities;
using O3.Foundation;
using O3.Nsga;
using O3.Nsga.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Optimization.Samples.TerminationConditions
{
    /// <summary>
    /// Provides sample usage of <see cref="OutputAdjacencyTermCond"/>.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class OutputAdjacencyTermCondSampleTests
    {
        #region Constructors

        public OutputAdjacencyTermCondSampleTests(
            ITestOutputHelper outputHelper
        )
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        #region Tests

        [Theory]
        [MemberData(nameof(TestTerminateDataWithParetoSet))]
        [MemberData(nameof(TestTerminateDataWithEpsilonParetoSet))]
        public void TestTerminate(Nsga2Solver solver)
        {
            OutputHelper.WriteLine(
                "Stats over Generations\n"
            );

            var termCond = solver.TerminationCondition as
                OutputAdjacencyTermCond;

            string rowFormat = "{0},{1},{2},{3},{4}";

            // When the solver starts, write the headers for the stats

            solver.StatusChanged += (sender, e) =>
            {
                if (e.PreviousStatus == SolverStatus.Stopped &&
                    e.CurrentStatus == SolverStatus.Running)
                {
                    OutputHelper.WriteLine(
                        rowFormat,
                        "Generation",
                        "Adjacency Count",
                        "Solution Count",
                        "Saved Archive Size",
                        "Adjacency Ratio"
                    );
                }
            };

            // When the solver is updated (in the case of `Nsga2Solver`,
            // this means when a generation is complete),
            // write information from `OutputAdjacencyTermCond`

            solver.Updated += (sender, e) =>
            {
                OutputHelper.WriteLine(
                    rowFormat,
                    termCond.LastCheckedGeneration,
                    termCond.GetAdjacencyCount(),
                    solver.Solutions.Count(),
                    termCond.SavedArchive.Count(),
                    termCond.GetAdjacencyRatio()
                );
            };

            // Start the solver

            solver.Solve();

            OutputHelper.WriteLine(string.Empty);

            // Write adjacency info after termination

            OutputHelper.WriteLine(
                "Adjacency Ratio Threshold: {0:P2}\n" +
                "Adjacency Ratio at Termination: {1:P2}\n",
                termCond.RatioThreshold,
                termCond.GetAdjacencyRatio()
            );

            // Write basic info of an `Nsga2Solver` instance

            OutputHelper.WriteLine(
                Nsga2SolverInfoStringCreator.Default
                    .GetInfoString(solver)
            );

            OutputHelper.WriteLine(string.Empty);

            // Write inputs and outputs of given solutions in CSV format

            OutputHelper.WriteLine(
                SolutionCsvStringCreator.Default
                    .GetCsvString(solver.Solutions)
            );
        }

        public static IEnumerable<object[]> TestTerminateDataWithParetoSet()
        {
            // Sample output of this test case:
            //
            // Adjacency Ratio Threshold: 92.00 %
            // Adjacency Ratio at Termination: 93.10 %
            //
            // Name: NSGA-II Solver
            // Status: Finished
            // Generation: 25 / 100
            // Phase: GenerationFinished
            // Population: 40 / 40
            // Offspring: 30 / 40
            // Solutions: 638
            // Evaluations: 1000

            var solver = CreateNsga2Solver();

            // `solver` uses `ParetoSet` for solution archive, which does not
            // limit the number of solutions archived and can get slow
            // when `ParetoSet` grows bigger. We try to keep this test case
            // fast by:
            //
            // - Reducing the population size (from 100 to 40)
            // - Set the ratio threshold relatively lower, such as 0.92

            solver.PopulationSize = 40;

            var termCond = new OutputAdjacencyTermCond();

            // Please make sure to set `OutputAdjacencyTermCond` to
            // `Nsga2Solver.TerminationCondition` first before
            // setting its other settings, which rely on information
            // from the solver.

            solver.TerminationCondition = termCond;

            // Output adjacency is checked
            // every 5 generations
            // (`OutputAdjacencyTermCond.GenerationInterval`)
            // and calculated between
            // the current solutions (`Nsga2Solver.Solutions`) and
            // the solutions from 5 generations ago
            // (stored in `OutputAdjacencyTermCond.SavedArchive`).

            termCond.GenerationInterval = 5;

            // Solution A and B are considered adjacent
            // in the dimension of Output 0 (`outputIndex`)
            // if their values of Output 0
            // differ than at most 1.0 (`distanceThreshold`).

            termCond.SetDistanceThreshold(
                outputIndex: 0, distanceThreshold: 1.0
            );

            // Solution A and B are considered adjacent
            // in the dimension of Output 1 (`outputIndex`)
            // if their values of Output 1
            // differ than at most 1.0 (`distanceThreshold`).

            termCond.SetDistanceThreshold(
                outputIndex: 1, distanceThreshold: 1.0
            );

            // 2 Solutions are considered adjacent if all of their outputs
            // differ by at most the corresponding distance thresholds.

            // The termination condition is met when
            // 92% (`OutputAdjacencyTermCond.RatioThreshold`)
            // of the current solutions (`Nsga2Solver.Solutions`)
            // are adjacent to any of the solutions from
            // 5 generations ago (`OutputAdjacencyTermCond.SavedArchive`).

            termCond.RatioThreshold = 0.92;

            yield return new object[]
            {
                solver
            };
        }

        public static IEnumerable<object[]>
            TestTerminateDataWithEpsilonParetoSet()
        {
            // Sample output of this test case:
            //
            // Adjacency Ratio Threshold: 95.00 %
            // Adjacency Ratio at Termination: 95.71 %
            //
            // Name: NSGA-II Solver
            // Status: Finished
            // Generation: 40 / 100
            // Phase: GenerationFinished
            // Population: 100 / 100
            // Offspring: 67 / 100
            // Solutions: 70
            // Evaluations: 3972

            var solver = CreateNsga2Solver();

            // Epsilon-Pareto set is used for `solver`.
            //
            // Based on the following settings, `Nsga2Solver.Solutions` will:
            //
            // - Have objectives (f1 and f2) no greater than 100 and 100
            //   (`.Epsilons[0].ObjectiveBound` and
            //    `.Epsilons[1].ObjectiveBound`)
            //
            // - For every box of 0.5 x 0.5 in size in the objective space,
            //   there is at most one solution so that the solutions are
            //   more uniformly distributed and the number of solutions is
            //   limited (due to `ObjectiveBound` and `Value` settings)

            var epsilonParetoSet = new EpsilonParetoSet
            {
                Problem = solver.Problem
            };

            epsilonParetoSet.Epsilons[0].ObjectiveBound = 100;
            epsilonParetoSet.Epsilons[0].Value = 0.5;
            epsilonParetoSet.Epsilons[0].Type = EpsilonType.Absolute;

            epsilonParetoSet.Epsilons[1].ObjectiveBound = 100;
            epsilonParetoSet.Epsilons[1].Value = 0.5;
            epsilonParetoSet.Epsilons[1].Type = EpsilonType.Absolute;

            solver.SolutionArchive = epsilonParetoSet;

            // Please refer to comments in
            // `TestTerminateByOutputAdjacencyDataWithParetoSet()` on
            // setting up `OutputAdjacencyTermCond`.

            var termCond = new OutputAdjacencyTermCond();

            solver.TerminationCondition = termCond;

            termCond.GenerationInterval = 5;

            termCond.SetDistanceThreshold(
                outputIndex: 0, distanceThreshold: 0.5
            );

            termCond.SetDistanceThreshold(
                outputIndex: 1, distanceThreshold: 0.5
            );

            termCond.RatioThreshold = 0.95;

            yield return new object[]
            {
                solver
            };
        }

        #endregion

        #region Utilities

        private static Nsga2Solver CreateNsga2Solver()
        {
            // Please refer to `Nsga2SolverSampleTests` for
            // more details on `Nsga2Solver` settings.

            var solver = new Nsga2Solver
            {
                Problem = BinhAndKorn2Problem.Default,
                PopulationSize = 100,
                MaxGeneration = 100,
                SolutionArchive = new ParetoSet()
            };

            // DoE (Design of Experiments)

            var doe = new RealUniformDistributionDoe
            {
                Problem = solver.Problem,
                RandomNumberGenerator = new XorshiftRng
                {
                    Seed = 0
                }
            };

            doe.SetInputRangesFromProblem();

            solver.DesignOfExps = doe;

            // Crossover operator

            var cxOp = new BlendCxOp
            {
                Problem = solver.Problem,
                RandomNumberGenerator = new XorshiftRng
                {
                    Seed = 1
                }
            };

            cxOp.SetBlendAlphasFromProblem();

            cxOp.BlendAlphas[0].Alpha = 0.5;
            cxOp.BlendAlphas[0].Probability = 0.8;

            cxOp.BlendAlphas[1].Alpha = 0.5;
            cxOp.BlendAlphas[1].Probability = 0.8;

            solver.CrossoverOp = cxOp;

            // Mutation operator

            var mutOp = new RealUniformOffsetMutOp
            {
                Problem = solver.Problem,
                RandomNumberGenerator = new XorshiftRng
                {
                    Seed = 2
                }
            };

            mutOp.SetOffsetsFromProblem();

            mutOp.RealUniformOffsets[0].UseRelative = true;
            mutOp.RealUniformOffsets[0].RelativeMaxOffset = 0.2;
            mutOp.RealUniformOffsets[0].Probability = 0.1;

            mutOp.RealUniformOffsets[1].UseRelative = true;
            mutOp.RealUniformOffsets[1].RelativeMaxOffset = 0.2;
            mutOp.RealUniformOffsets[1].Probability = 0.1;

            solver.MutationOp = mutOp;

            return solver;
        }

        #endregion

        #endregion

        #region Properties

        public ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
