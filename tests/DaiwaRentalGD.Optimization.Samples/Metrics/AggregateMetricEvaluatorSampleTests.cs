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
using O3.Foundation;
using O3.Nsga;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Optimization.Samples.Metrics
{
    /// <summary>
    /// Provides sample usage of <see cref="AggregateMetricEvaluator"/>.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class AggregateMetricEvaluatorSampleTests
    {
        #region Constructors

        public AggregateMetricEvaluatorSampleTests(
            ITestOutputHelper outputHelper
        )
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        #region Tests

        [Theory]
        [MemberData(nameof(TestGetMetricsDataOptimizedVsRandom))]
        [MemberData(nameof(TestGetCoverageDataMoreEvaluations))]
        public void TestGetMetrics(
            string solutionsName0, string solutionsName1,
            IReadOnlyList<Solution> solutions0,
            IReadOnlyList<Solution> solutions1
        )
        {
            OutputHelper.WriteLine(
                "S0 = {0}\n" +
                "S1 = {1}\n",
                solutionsName0,
                solutionsName1
            );

            string rowFormatString = "{0,-20}{1,-20}{2,-20}";

            var problem = solutions0.First().Problem;

            var metricEvaluator = AggregateMetricEvaluator.Default;

            for (int inputIndex = 0; inputIndex < problem.InputSpecs.Count;
                ++inputIndex)
            {
                var inputSpec = problem.InputSpecs[inputIndex];

                OutputHelper.WriteLine(
                    rowFormatString,
                    string.Format("Input: {0}", inputSpec.Name),
                    "S0",
                    "S1"
                );

                OutputHelper.WriteLine(string.Empty);

                // Write all types of aggregate metrics specified by
                // `AggregateMetricType` enum.

                foreach (var metricType in
                    Enum.GetValues(typeof(AggregateMetricType))
                    .OfType<AggregateMetricType>())
                {
                    OutputHelper.WriteLine(

                        rowFormatString,

                        metricType,

                        metricEvaluator
                        .GetInputMetric(metricType, solutions0, inputIndex)
                        .ToString("N4"),

                        metricEvaluator
                        .GetInputMetric(metricType, solutions1, inputIndex)
                        .ToString("N4")
                    );
                }

                OutputHelper.WriteLine(string.Empty);
            }

            for (int outputIndex = 0; outputIndex < problem.OutputSpecs.Count;
                ++outputIndex)
            {
                var outputSpec = problem.OutputSpecs[outputIndex];

                OutputHelper.WriteLine(
                    rowFormatString,
                    string.Format("Output: {0}", outputSpec.Name),
                    "S0",
                    "S1"
                );

                OutputHelper.WriteLine(string.Empty);

                // Write all types of aggregate metrics specified by
                // `AggregateMetricType` enum.

                foreach (var metricType in
                    Enum.GetValues(typeof(AggregateMetricType))
                    .OfType<AggregateMetricType>())
                {
                    OutputHelper.WriteLine(

                        rowFormatString,

                        metricType,

                        metricEvaluator
                        .GetOutputMetric(metricType, solutions0, outputIndex)
                        .ToString("N4"),

                        metricEvaluator
                        .GetOutputMetric(metricType, solutions1, outputIndex)
                        .ToString("N4")
                    );
                }

                OutputHelper.WriteLine(string.Empty);
            }
        }

        public static IEnumerable<object[]>
            TestGetMetricsDataOptimizedVsRandom()
        {
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
            // (including same random sources)
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
