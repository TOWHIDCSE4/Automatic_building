using System;
using System.Collections.Generic;
using DaiwaRentalGD.Optimization.Samples.Foundation;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using O3.Commons.Pareto;
using O3.Commons.RandomNumberGenerators;
using O3.Commons.TerminationConditions;
using O3.Commons.Utilities;
using O3.Nsga;
using O3.Nsga.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Optimization.Samples.TerminationConditions
{
    /// <summary>
    /// Provides sample usage of <see cref="TimeLimitTermCond"/>.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class TimeLimitTermCondSampleTests
    {
        #region Constructors

        public TimeLimitTermCondSampleTests(ITestOutputHelper outputHelper)
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

            var termCond = solver.TerminationCondition as TimeLimitTermCond;

            OutputHelper.WriteLine(
                "Max duration: {0:N2} seconds ({1:c})\n" +
                "Actual duration: {2:N2} seconds ({3:c})",
                termCond.MaxDuration.TotalSeconds,
                termCond.MaxDuration,
                termCond.TotalDuration.TotalSeconds,
                termCond.TotalDuration
            );

            OutputHelper.WriteLine(string.Empty);

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

        public static IEnumerable<object[]> TestTerminateData()
        {
            // Sample output of this test case:
            //
            // Max duration: 1.23 seconds (00:00:01.2300000)
            // Actual duration: 1.28 seconds (00:00:01.2784540)
            //
            // Name: NSGA-II Solver
            // Status: Finished
            // Generation: 10 / 100
            // Phase: GenerationFinished
            // Population: 100 / 100
            // Offspring: 70 / 100
            // Solutions: 426
            // Evaluations: 1060
            //
            // The solver works in a non-preemptive way with
            // `TimeLimitTermCond` by querying it and deciding whether
            // it should finish or not. Thus the actual duration will
            // be slightly longer than the maximum duration,
            // depending on the query frequency.

            var solver = CreateNsga2Solver();

            var termCond = new TimeLimitTermCond
            {
                MaxDuration = TimeSpan.FromSeconds(1.23)
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
