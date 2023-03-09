using System.Collections.Generic;
using DaiwaRentalGD.Optimization.Samples.Foundation;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using O3.Commons.Evolution.TerminationConditions;
using O3.Commons.Pareto;
using O3.Commons.RandomNumberGenerators;
using O3.Commons.Utilities;
using O3.Nsga;
using O3.Nsga.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Optimization.Samples.TerminationConditions
{
    /// <summary>
    /// Provides sample usage of <see cref="GenerationCountTermCond"/>.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class GenerationCountTermCondSampleTests
    {
        #region Constructors

        public GenerationCountTermCondSampleTests(
            ITestOutputHelper outputHelper
        )
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        #region Tests

        [Theory]
        [MemberData(nameof(TestTerminateData))]
        public void TestTerminate(Nsga2Solver solver)
        {
            solver.Solve();

            var termCond = solver.TerminationCondition as
                GenerationCountTermCond;

            OutputHelper.WriteLine(
                "Max generation from termination condition: {0}\n" +
                "Max generation from solver: {1}\n" +
                "Actual generation: {2}",
                termCond.MaxGeneration,
                solver.MaxGeneration,
                solver.CurrentGeneration
            );

            OutputHelper.WriteLine(string.Empty);

            // Write basic info of an `Nsga2Solver` instance

            OutputHelper.WriteLine(
                Nsga2SolverInfoStringCreator.Default
                    .GetInfoString(solver)
            );

            // Write inputs and outputs of given solutions in CSV format

            OutputHelper.WriteLine(
                SolutionCsvStringCreator.Default
                    .GetCsvString(solver.Solutions)
            );
        }

        public static IEnumerable<object[]> TestTerminateData()
        {
            // Sample output of this test case:
            //
            // Max generation from termination condition: 12
            // Max generation from solver: 100
            // Actual generation: 12
            //
            // Name: NSGA-II Solver
            // Status: Finished
            // Generation: 12 / 100
            // Phase: GenerationFinished
            // Population: 100 / 100
            // Offspring: 79 / 100
            // Solutions: 570
            // Evaluations: 1257
            //
            // `Nsga2Solver` can finish in the middle of a generation.
            // In this case, it is triggered by the `GenerationCountTermCond`.
            // `Nsga2Solver.Solutions` will return all valid solutions
            // at the finishing time.
            //
            // Also, because mutation rates and crossover rates are typically
            // less than 1.0, not all solutions in the population are
            // evaluated - They can be copied from the previous generation
            // without being changed and thus not necessary to be evaluated.
            // So even `MaxEvaluationCount` is set to multiples of
            // population size, the solver does not necessarily finish
            // at the end of a complete generation.

            var solver = CreateNsga2Solver();

            var termCond = new GenerationCountTermCond
            {
                MaxGeneration = 12
            };

            solver.TerminationCondition = termCond;

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
