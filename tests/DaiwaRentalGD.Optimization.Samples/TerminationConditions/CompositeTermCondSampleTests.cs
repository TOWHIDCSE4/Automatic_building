using System;
using System.Collections.Generic;
using DaiwaRentalGD.Optimization.Samples.Foundation;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using O3.Commons.Evolution.TerminationConditions;
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
    /// Provides sample usage of <see cref="CompositeTermCond"/>.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class CompositeTermCondSampleTests
    {
        #region Constructors

        public CompositeTermCondSampleTests(ITestOutputHelper outputHelper)
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

            var termCond = solver.TerminationCondition as CompositeTermCond;

            OutputHelper.WriteLine(
                "Termination conditions met: {0} / {1}\n",
                termCond.GetNumOfMetConditions(),
                termCond.Children.Count
            );

            foreach (var child in termCond.Children)
            {
                OutputHelper.WriteLine("- {0}", child.Name);

                switch (child)
                {
                    case TimeLimitTermCond timeLimitTermCond:

                        OutputHelper.WriteLine(
                            "  Max duration: {0:N2} seconds ({1:c})\n" +
                            "  Actual duration: {2:N2} seconds ({3:c})\n" +
                            "  Is condition met? {4}\n",
                            timeLimitTermCond.MaxDuration.TotalSeconds,
                            timeLimitTermCond.MaxDuration,
                            timeLimitTermCond.TotalDuration.TotalSeconds,
                            timeLimitTermCond.TotalDuration,
                            timeLimitTermCond.ShouldTerminate
                        );

                        break;

                    case GenerationCountTermCond genCountTermCond:

                        OutputHelper.WriteLine(
                            "  Max generation: {0}\n" +
                            "  Current generation: {1}\n" +
                            "  Is condition met? {2}\n",
                            genCountTermCond.MaxGeneration,
                            solver.CurrentGeneration,
                            genCountTermCond.ShouldTerminate
                        );

                        break;

                    case EvaluationCountTermCond evalCountTermCond:

                        OutputHelper.WriteLine(
                            "  Max evaluation count: {0}\n" +
                            "  Num of evaluations: {1}\n" +
                            "  Is condition met? {2}\n",
                            evalCountTermCond.MaxEvaluationCount,
                            solver.NumOfEvaluations,
                            evalCountTermCond.ShouldTerminate
                        );

                        break;
                }
            }

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
            // Termination conditions met: 2 / 3
            //
            // - Termination Condition
            //   Max duration: 1.23 seconds(00:00:01.2300000)
            //   Actual duration: 1.11 seconds(00:00:01.1115297)
            //   Is condition met? False
            //
            // - Evaluation Count Term Cond
            //   Max evaluation count: 1000
            //   Num of evaluations: 1000
            //   Is condition met? True
            //
            // - Generation Count Term Cond
            //   Max generation: 2
            //   Current generation: 10
            //   Is condition met? True
            //
            // Name: NSGA - II Solver
            // Status: Finished
            // Generation: 10 / 100
            // Phase: OffspringUpdated
            // Population: 100 / 100
            // Offspring: 89 / 100
            // Solutions: 360
            // Evaluations: 1000

            var solver = CreateNsga2Solver();

            // `termCond` consists of 3 child termination conditions.
            //
            // `solver` should terminate when 2 (`MinNumOfMetConditions`)
            // of the 3 child conditions
            // are met.

            var termCond = new CompositeTermCond
            {
                MinNumOfMetConditions = 2
            };

            solver.TerminationCondition = termCond;

            // Child 0

            termCond.Add(
                new TimeLimitTermCond
                {
                    MaxDuration = TimeSpan.FromSeconds(1.23)
                }
            );

            // Child 1

            termCond.Add(
                new EvaluationCountTermCond
                {
                    MaxEvaluationCount = 1000
                }
            );

            // Child 2

            termCond.Add(
                new GenerationCountTermCond
                {
                    MaxGeneration = 2
                }
            );

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
