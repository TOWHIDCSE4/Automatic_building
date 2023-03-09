using System.Collections.Generic;
using System.Linq;
using DaiwaRentalGD.Optimization.Samples.Foundation;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using O3.Commons.Evolution.TerminationConditions;
using O3.Commons.Metrics;
using O3.Commons.Pareto;
using O3.Commons.RandomNumberGenerators;
using O3.Commons.TerminationConditions;
using O3.Foundation;
using O3.Nsga;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Optimization.Samples.Metrics
{
    /// <summary>
    /// Provides sample usage of <see cref="CoverageMetricEvaluator"/>.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class CoverageMetricEvaluatorSampleTests
    {
        #region Constructors

        public CoverageMetricEvaluatorSampleTests(
            ITestOutputHelper outputHelper
        )
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        #region Tests

        [Theory]
        [MemberData(nameof(TestGetCoverageDataOptimizedVsRandom))]
        [MemberData(nameof(TestGetCoverageDataMoreEvaluations))]
        [MemberData(nameof(TestGetCoverageDataDifferentTermConds))]
        public void TestGetCoverage(
            string solutionsName0, string solutionsName1,
            IReadOnlyList<Solution> solutions0,
            IReadOnlyList<Solution> solutions1
        )
        {
            double coverage01 = CoverageMetricEvaluator.Default
                .GetCoverage(solutions0, solutions1);

            double coverage10 = CoverageMetricEvaluator.Default
                .GetCoverage(solutions1, solutions0);

            OutputHelper.WriteLine(
                "S0 = {0}\n" +
                "S1 = {1}\n" +
                "\n" +
                "Coverage(S0, S1) = {2:P3}\n" +
                "Coverage(S1, S0) = {3:P3}",
                solutionsName0,
                solutionsName1,
                coverage01,
                coverage10
            );
        }

        public static IEnumerable<object[]>
            TestGetCoverageDataOptimizedVsRandom()
        {
            // Sample output of this test case:
            //
            // S0 = 302 Solutions from NSGA-II (Evaluation Count = 600)
            // S1 = 600 Solutions from Random DoE
            //
            // Coverage(S0, S1) = 99.667 %
            // Coverage(S1, S0) = 1.987 %
            //
            // The results should clearly show that optimized solutions
            // overwhelmingly outperform random solutions.

            // Optimized solutions

            var solver = CreateNsga2Solver();

            var termCond = new EvaluationCountTermCond
            {
                MaxEvaluationCount = 600
            };

            solver.TerminationCondition = termCond;

            solver.Solve();

            var solutions0 = solver.Solutions.ToList();

            string solutionsName0 = string.Format(
                "{0} Solutions from " +
                "NSGA-II (Evaluation Count = {1})",
                solutions0.Count,
                solver.NumOfEvaluations
            );

            // Random solutions

            var solutions1 = CreateRandomSolutions(
                solver.Problem, solver.NumOfEvaluations
            );

            string solutionsName1 = string.Format(
                "{0} Solutions from Random DoE",
                solutions1.Count
            );

            yield return new object[]
            {
                solutionsName0,
                solutionsName1,
                solutions0,
                solutions1
            };
        }

        public static IEnumerable<object[]>
            TestGetCoverageDataMoreEvaluations()
        {
            // Sample output of this test case:
            //
            // S0 = 120 Solutions from NSGA-II (Evaluation Count = 321)
            // S1 = 220 Solutions from NSGA-II (Evaluation Count = 500)
            //
            // Coverage(S0, S1) = 70.455 %
            // Coverage(S1, S0) = 100.000 %
            //
            // This `Nsga2Solver` uses Pareto Set as solution archive.
            // 100% shows that the additional evaluations in S1 produced
            // solutions that are at least as good as those in S0.

            // Optimized Solution Set 0

            var solver0 = CreateNsga2Solver();

            var termCond0 = new EvaluationCountTermCond
            {
                MaxEvaluationCount = 321
            };

            solver0.TerminationCondition = termCond0;

            solver0.Solve();

            var solutions0 = solver0.Solutions.ToList();

            string solutionsName0 = string.Format(
                "{0} Solutions from " +
                "NSGA-II (Evaluation Count = {1})",
                solutions0.Count,
                solver0.NumOfEvaluations
            );

            // Optimized Solution Set 1

            // `solver1` is exactly the same as `solver0`
            // (including same random number generators)
            // except for termination condition settings

            var solver1 = CreateNsga2Solver();

            var termCond1 = new EvaluationCountTermCond
            {
                MaxEvaluationCount = 500
            };

            solver1.TerminationCondition = termCond1;

            solver1.Solve();

            var solutions1 = solver1.Solutions.ToList();

            string solutionsName1 = string.Format(
                "{0} Solutions from " +
                "NSGA-II (Evaluation Count = {1})",
                solutions1.Count,
                solver1.NumOfEvaluations
            );

            yield return new object[]
            {
                solutionsName0,
                solutionsName1,
                solutions0,
                solutions1
            };
        }

        public static IEnumerable<object[]>
            TestGetCoverageDataDifferentTermConds()
        {
            // Sample output of this test case:
            //
            // S0 = 1062 Solutions from NSGA-II
            //      (Evaluation Count = 1776, Adjacency Ratio = 95.104 %)
            // S1 = 220 Solutions from NSGA-II (Evaluation Count = 500)
            //
            // Coverage(S0, S1) = 42.727 %
            // Coverage(S1, S0) = 3.861 %
            //
            // This `Nsga2Solver` uses Pareto Set as solution archive.
            // S0 were calculated with 1776 evaluations, which was determined
            // by the output adjacency termination condition.
            //
            // Loosely speaking, ~43% solutions from S1 are worse than
            // at least one solution from S0, and ~4% solutions from S0 are
            // worse than at least one solution from S1. This shows that
            // S0 is the preferred solution set overall.

            // Optimized Solution Set 0

            var solver0 = CreateNsga2Solver();

            // Change one of the random seeds used in the solver
            // so that `solver0` and `solver1` (below)
            // do not overlap in the progress of optimization.

            ((solver0.MutationOp as RealUniformOffsetMutOp)
                .RandomNumberGenerator as XorshiftRng).Seed = 123;

            var termCond0 = new OutputAdjacencyTermCond();

            solver0.TerminationCondition = termCond0;

            termCond0.GenerationInterval = 5;
            termCond0.SetDistanceThreshold(
                outputIndex: 0, distanceThreshold: 1.0
            );
            termCond0.SetDistanceThreshold(
                outputIndex: 1, distanceThreshold: 1.0
            );
            termCond0.RatioThreshold = 0.95;

            solver0.Solve();

            var solutions0 = solver0.Solutions.ToList();

            string solutionsName0 = string.Format(
                "{0} Solutions from " +
                "NSGA-II (Evaluation Count = {1}, Adjacency Ratio = {2:P3})",
                solutions0.Count,
                solver0.NumOfEvaluations,
                termCond0.GetAdjacencyRatio()
            );

            // Optimized Solution Set 1

            var solver1 = CreateNsga2Solver();

            var termCond1 = new EvaluationCountTermCond
            {
                MaxEvaluationCount = 500
            };

            solver1.TerminationCondition = termCond1;

            solver1.Solve();

            var solutions1 = solver1.Solutions.ToList();

            string solutionsName1 = string.Format(
                "{0} Solutions from " +
                "NSGA-II (Evaluation Count = {1})",
                solutions1.Count,
                solver1.NumOfEvaluations
            );

            yield return new object[]
            {
                solutionsName0,
                solutionsName1,
                solutions0,
                solutions1
            };
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

        private static IReadOnlyList<Solution> CreateRandomSolutions(
            Problem problem, int numOfSolutions
        )
        {
            // Create random solutions using a DoE which
            // initializes the inputs of solutions with
            // uniformly distributed random real numbers.
            //
            // Please refer to `Nsga2SolverSampleTests` for
            // more details on `RealUniformDistributionDoe` settings.

            var doe = new RealUniformDistributionDoe
            {
                Problem = problem,
                RandomNumberGenerator = new XorshiftRng
                {
                    Seed = 3
                }
            };

            doe.SetInputRangesFromProblem();

            var solutions = Enumerable.Range(0, numOfSolutions)
                .Select(_ => new Solution(problem)).ToList();

            doe.Update(solutions);

            // A DoE only sets the inputs of a solution.
            // The outputs are evaluated (updated) by calling
            // `Solution.Update()`.

            foreach (var solution in solutions)
            {
                solution.Update();
            }

            return solutions;
        }

        #endregion

        #endregion

        #region Properties

        public ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
