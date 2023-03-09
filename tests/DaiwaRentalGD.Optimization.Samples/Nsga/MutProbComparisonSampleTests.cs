using System;
using System.Collections.Generic;
using System.Linq;
using DaiwaRentalGD.Optimization.Samples.Foundation;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using O3.Commons.Metrics;
using O3.Commons.Pareto;
using O3.Commons.RandomNumberGenerators;
using O3.Commons.TerminationConditions;
using O3.Nsga;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Optimization.Samples.Nsga
{
    /// <summary>
    /// Provides samples on how to set up mutation probabilities in
    /// different ways and and compare their performance.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class MutProbComparisonSampleTests
    {
        #region Constructors

        public MutProbComparisonSampleTests(
            ITestOutputHelper outputHelper
        )
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        #region Tests

        [Theory]
        [MemberData(nameof(TestCompareMutProbsData))]
        public void TestCompareMutProbs(
            Nsga2Solver solver0, Nsga2Solver solver1
        )
        {
            // To get mutation information at each generation
            // from the outputs, uncomment the following few lines:

            // solver0.Updated += SolverUpdatedWriteMutationInfoHandler;
            // solver1.Updated += SolverUpdatedWriteMutationInfoHandler;
            // 
            // OutputHelper.WriteLine(string.Empty);

            solver0.Solve();
            solver1.Solve();

            var solutions0 = solver0.Solutions.ToList();
            var solutions1 = solver1.Solutions.ToList();

            // Coverage is a metric to compare the performance of
            // two solution sets relatively.
            //
            // Simply put, if Coverage(S0, S1) > Coverage(S1, S0),
            // then S1 has more solutions equally good or worse than
            // solutions in S0. This means the overall performance of S0
            // is better than over S0.
            //
            // Please refer to `CoverageMetricEvaluatorSampleTests`
            // for more examples.

            double coverage01 = CoverageMetricEvaluator.Default
                .GetCoverage(solutions0, solutions1);

            double coverage10 = CoverageMetricEvaluator.Default
                .GetCoverage(solutions1, solutions0);

            OutputHelper.WriteLine(
                "S0 = {0} solutions from {1}\n" +
                "S1 = {2} solutions from {3}\n" +
                "\n" +
                "Coverage(S0, S1) = {4:P3}\n" +
                "Coverage(S1, S0) = {5:P3}",
                solutions0.Count, solver0.Name,
                solutions1.Count, solver1.Name,
                coverage01,
                coverage10
            );
        }

        public static IEnumerable<object[]> TestCompareMutProbsData()
        {
            yield return new object[]
            {
                // Solver with constant mutation probability of 0.1

                CreateConstantMutProbSolver(mutProb: 0.1),

                // Solver with varying mutation probability that
                // starts from 0.15 and end at 0.1 if max generation
                // is reached (which might not happen due to
                // termination condition).
                // Mutation probability decreases linearly
                // at the end of every generation.
                //
                // The motivation of a decreasing mutation probability
                // over time is that the optimization gradually transitions
                // from exploration phase,
                // which favors larger solution space for diversity,
                // to exploitation phase,
                // which favors smaller solution space for refinement

                CreateVaryingMutProbSolver(
                    startMutProb: 0.15,
                    endMutProb: 0.1
                )
            };

            yield return new object[]
            {
                // Solver with constant mutation probability of 0.1

                CreateConstantMutProbSolver(mutProb: 0.1),

                // Solver with constant mutation probability of 0.12

                CreateConstantMutProbSolver(mutProb: 0.12),
            };
        }

        private static Nsga2Solver CreateConstantMutProbSolver(double mutProb)
        {
            var solver = CreateNsga2Solver();

            solver.Name = string.Format(
                "NSGA-II (Constant Mut Prob = {0:P3})",
                mutProb
            );

            // Set mutation probabilities of all inputs to `mutProb`.

            var mutOp = solver.MutationOp as RealUniformOffsetMutOp;

            foreach (var offset in mutOp.RealUniformOffsets)
            {
                offset.Probability = mutProb;
            }

            return solver;
        }

        private static Nsga2Solver CreateVaryingMutProbSolver(
            double startMutProb, double endMutProb
        )
        {
            var solver = CreateNsga2Solver();

            solver.Name = string.Format(
                "NSGA-II " +
                "(Varying Mut Prob, Start = {0:P3}, End = {1:P3})",
                startMutProb, endMutProb
            );

            var mutOp = solver.MutationOp as RealUniformOffsetMutOp;

            // Initially, set mutation probabilities of all inputs to
            // `startMutProb`.

            foreach (var offset in mutOp.RealUniformOffsets)
            {
                offset.Probability = startMutProb;
            }

            solver.Updated += (sender, e) =>
            {
                // `Nsga2Solver.Updated` is triggered at the end of
                // a generation. Please refer to `Nsga2SolverSampleTests`
                // for more information.

                // `progress` represents the normalized progress of
                // the solver based on current generation.

                double progress =
                    (double)solver.CurrentGeneration / solver.MaxGeneration;

                // Mutation probability is a linear interpolation
                // between `startProb` and `endProb` with `progress`
                // being the interpolation parameter.

                double mutProb =
                    (1.0 - progress) * startMutProb + progress * endMutProb;

                // Set the updated mutation probability for all inputs.

                foreach (var offset in mutOp.RealUniformOffsets)
                {
                    offset.Probability = mutProb;
                }
            };

            return solver;
        }

        private void SolverUpdatedWriteMutationInfoHandler(
            object sender, EventArgs e
        )
        {
            var solver = sender as Nsga2Solver;
            var mutOp = solver.MutationOp as RealUniformOffsetMutOp;

            // Get mutation probability for each input

            var mutProbs = mutOp.RealUniformOffsets.Select(
                offset => offset.Probability
            ).ToList();

            // Information including solver name, progress and
            // mutation probabilities

            string infoText = string.Format(
                "{0} [Gen {1} / {2}]: [{3}]",
                solver.Name,
                solver.CurrentGeneration,
                solver.MaxGeneration,
                string.Join(
                    ", ",
                    Enumerable.Range(0, mutProbs.Count)
                    .Select(
                        inputIndex => string.Format(
                            "[{0}] = {1:P3}",
                            inputIndex, mutProbs[inputIndex]
                        )
                    )
                )
            );

            OutputHelper.WriteLine(infoText);
        }

        #endregion

        #region Utilities

        private static Nsga2Solver CreateNsga2Solver()
        {
            // Please refer to `Nsga2SolverSampleTests` class for
            // more details on solver settings.

            var solver = new Nsga2Solver
            {
                Problem = BinhAndKorn2Problem.Default,
                PopulationSize = 40,
                MaxGeneration = 100
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

            // Solution archive

            var solutionArchive = new EpsilonParetoSet();

            solver.SolutionArchive = solutionArchive;

            solutionArchive.Epsilons[0].Type = EpsilonType.Absolute;
            solutionArchive.Epsilons[0].ObjectiveBound = 100.0;
            solutionArchive.Epsilons[0].Value = 0.5;

            solutionArchive.Epsilons[1].Type = EpsilonType.Absolute;
            solutionArchive.Epsilons[1].ObjectiveBound = 100.0;
            solutionArchive.Epsilons[1].Value = 0.5;

            // Termination condition. For a fair comparison,
            // all test solvers are only allowed 1000 solution evaluations.

            var termCond = new EvaluationCountTermCond
            {
                MaxEvaluationCount = 1000
            };

            solver.TerminationCondition = termCond;

            return solver;
        }

        #endregion

        #endregion

        #region Properties

        public ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
