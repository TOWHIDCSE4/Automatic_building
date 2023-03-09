using System.Collections.Generic;
using System.Linq;
using DaiwaRentalGD.Optimization.Samples.Foundation;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using O3.Commons.Pareto;
using O3.Commons.RandomNumberGenerators;
using O3.Commons.Utilities;
using O3.Foundation;
using O3.Nsga;
using O3.Nsga.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Optimization.Samples.SolutionArchives
{
    /// <summary>
    /// Provides sample usage of <see cref="ParetoSet"/> and
    /// <see cref="EpsilonParetoSet"/>. Compares uses of different
    /// solution archives, including the ones mentioned and
    /// no solution archive.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class SolutionArchiveComparisonSampleTests
    {
        #region Constructors

        public SolutionArchiveComparisonSampleTests(
            ITestOutputHelper outputHelper
        )
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        #region Tests

        [Theory]
        [MemberData(nameof(TestCompareSolutionArchivesData))]
        public void TestCompareSolutionArchives(
            IEnumerable<Nsga2Solver> solvers
        )
        {
            foreach (var solver in solvers)
            {
                OutputHelper.WriteLine(
                    "Running {0}, Solution Archive = {1}\n",
                    solver.Name,
                    solver.SolutionArchive?.Name ??
                    "n/a (Population)"
                );

                solver.Solve();

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
        }

        public static IEnumerable<object[]>
            TestCompareSolutionArchivesData()
        {
            var solverWithNoArchive = CreateSolverWithNoArchive();

            var solverUsingEpsilonParetoSet =
                CreateSolverUsingEpsilonParetoSet();

            var solverUsingParetoSet = CreateSolverUsingParetoSet();

            yield return new object[]
            {
                new[]
                {
                    solverWithNoArchive,
                    solverUsingEpsilonParetoSet,
                    solverUsingParetoSet
                }
            };
        }

        #endregion

        #region Utilities

        private static Nsga2Solver CreateSolverWithNoArchive()
        {
            var solver = CreateNsga2Solver();

            // If `SolutionArchive` is not set,
            // `solver.Solutions` will return `solver.Population`,
            // which contains the solutions in the latest population.

            solver.SolutionArchive = null;

            return solver;
        }

        private static Nsga2Solver CreateSolverUsingParetoSet()
        {
            var solver = CreateNsga2Solver();

            // If `SolutionArchive` is set,
            // `solver.Solutions` will return
            // `solver.SolutionArchive.Solutions`.
            //
            // In this case, `solver.Solutions` returns
            // all non-dominated solutions generated since
            // the beginning of the optimization (i.e. the Pareto Set).
            //
            // At the end of each generation,
            // the solver will update the solution archive with
            // the latest population:
            //
            // - If a solution from `Population` is not dominated by
            //   any solution in `ParetoSet`, it will be added `ParetoSet`.
            //
            // - If a solution already in the `ParetoSet` is dominated by
            //   any solution in `Population`, it will be removed from
            //   `ParetoSet`.

            solver.SolutionArchive = new ParetoSet();

            return solver;
        }

        private static Nsga2Solver CreateSolverUsingEpsilonParetoSet()
        {
            var solver = CreateNsga2Solver();

            // When `ParetoSet` gets very large, the update will slow down
            // due to the number of dominance-based comparisons needed.
            // `EpsilonParetoSet` provides a way to control the density
            // (and thus the number) of archived solutions, but it requires
            // some user-specified parameters.

            var epsilonParetoSet = new EpsilonParetoSet();

            // Set `EpsilonParetoSet` to `Nsga2Solver.SolutionArchive` before
            // setting its parameters. `EpsilonParetoSet` relies on
            // certain properties from the solver to initialize itself,
            // e.g. knowing the number of objectives on `Solver.Problem`.

            solver.SolutionArchive = epsilonParetoSet;

            // The user-specified parameters are the epsilon objects.
            // For each objective on the problem, there is an epsilon object.
            //
            // The index of an epsilon corresponds to the objective index.
            // Please note that the objective index can be different
            // from the output index. Output index is counted for
            // all outputs, while objective index is only counted for
            // outputs that are objectives, and starts from 0 with
            // the objective that has the smallest output index.

            Assert.Equal(

                // The number of epsilon objects
                epsilonParetoSet.Epsilons.Count,

                // The number of objectives
                solver.Problem.OutputSpecs.Count(
                    outputSpec =>
                        outputSpec.ObjectiveType != ObjectiveType.None
                )
            );

            // Each epsilon has the following properties:
            //
            // - Type: For this project we only need `Absolute` type.
            //
            // - Objective bound: The worst value allowed for the objective.
            //   For `Epsilons[0]` below, its corresponding objective is
            //   to be minimized. Setting it to 60.0 means that
            //   solutions with Objective 0 above 60.0 will never be
            //   added to the solution archive.
            //
            // - Value: It defines something like a resolution for a
            //   given objective. There is an epsilon value associated
            //   with each objective.
            //
            // In the case of 2 objectives (like in Binh and Korn 2 problem),
            // the 2 epsilon values divide the 2-D objective space into
            // non-overlapping boxes of size 0.5 x 0.5.
            // Epsilon-Pareto Set ensures that there will be
            // at most one solution in each box. This makes the archived
            // solutions more uniformly distributed. With objective bounds and
            // epsilon values, the number of solutions is also under control.

            epsilonParetoSet.Epsilons[0].Type = EpsilonType.Absolute;
            epsilonParetoSet.Epsilons[0].ObjectiveBound = 60.0f;
            epsilonParetoSet.Epsilons[0].Value = 0.5;

            epsilonParetoSet.Epsilons[1].Type = EpsilonType.Absolute;
            epsilonParetoSet.Epsilons[1].ObjectiveBound = 50.0f;
            epsilonParetoSet.Epsilons[1].Value = 0.5;

            return solver;
        }

        private static Nsga2Solver CreateNsga2Solver()
        {
            // Please refer to `Nsga2SolverSampleTests` for
            // more details on `Nsga2Solver` settings.

            var solver = new Nsga2Solver
            {
                Problem = BinhAndKorn2Problem.Default,
                PopulationSize = 40,
                MaxGeneration = 40
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
