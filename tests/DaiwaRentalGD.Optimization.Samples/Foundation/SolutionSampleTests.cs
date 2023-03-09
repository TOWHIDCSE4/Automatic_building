using System;
using System.Collections.Generic;
using System.Text;
using O3.Foundation;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Optimization.Samples.Foundation
{
    /// <summary>
    /// Provides sample usage of <see cref="Solution"/>.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class SolutionSampleTests
    {
        #region Constructors

        public SolutionSampleTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        #region Tests

        [Theory]
        [MemberData(nameof(TestShowSolutionInfoData))]
        public void TestShowSolutionInfo(
            Problem problem, Action<Solution> setSolutionInputs
        )
        {
            var solution = new Solution(problem)
            {
                Name = "Sample Solution"
            };

            OutputHelper.WriteLine(
                "Solution instantiated:\n" +
                "\n" +
                "{0}",
                GetSolutionInfoText(solution)
            );

            setSolutionInputs(solution);

            OutputHelper.WriteLine(
                "Solution.Inputs set:\n" +
                "\n" +
                "{0}",
                GetSolutionInfoText(solution)
            );

            // * About `Solution.Update()` *
            //
            // When this method is called, `Solution.State` is first set to
            // `Expired` indicating the solution outputs are outdated
            // (and thus not meaningful) and an evaluation is needed.
            //
            // The evaluation logic is defined in the problem for which
            // the solution is created, or more specifically, in
            // `Solution.Problem.Evaluate()`.
            //
            // Please refer to comments `SampleProblemTests` class for
            // more information on how `Problem` and `Solution` work together.

            solution.Update();

            OutputHelper.WriteLine(
                "Solution.Update() called:\n" +
                "\n" +
                "{0}",
                GetSolutionInfoText(solution)
            );
        }

        public static IEnumerable<object[]> TestShowSolutionInfoData()
        {
            yield return new object[]
            {
                BinhAndKorn2Problem.Default,

                new Action<Solution>((solution) =>
                {
                    solution.SetInput(0, 5.0);
                    solution.SetInput(1, 6.0);
                })
            };
        }

        #endregion

        #region Utilities

        private static string GetSolutionInfoText(Solution solution)
        {
            var builder = new StringBuilder();

            builder.AppendFormat(
                "Solution Name: {0}\n",
                solution.Name
            );
            builder.AppendFormat(
                "Problem Name: {0}\n",
                solution.Problem.Name
            );
            builder.AppendFormat(
                "Solution State: {0}\n",
                solution.State
            );

            builder.AppendLine();

            string rowFormat = "{0,-20}{1,-20}{2,-20}\n";

            var problem = solution.Problem;

            for (int inputIndex = 0; inputIndex < solution.Inputs.Count;
                ++inputIndex)
            {
                var inputSpec = problem.InputSpecs[inputIndex];

                // * Raw inputs and outputs *
                //
                // `Solution.Inputs` and `Solution.Outputs` store the
                // raw values of inputs and outputs as `object` types.
                // `Solution.GetInput<T>()` and `Solution.GetOutput<T>()`
                // are used to access typed input and output values
                // by converting these raw values at runtime using
                // the data specs set on input specs and output specs.

                var input = solution.Inputs[inputIndex];

                builder.AppendFormat(
                    rowFormat,
                    string.Format("Input {0}", inputIndex),
                    inputSpec.Name,
                    input
                );
            }

            builder.AppendLine();

            for (int outputIndex = 0; outputIndex < solution.Outputs.Count;
                ++outputIndex)
            {
                var outputSpec = problem.OutputSpecs[outputIndex];

                var output = solution.Outputs[outputIndex];

                builder.AppendFormat(
                    rowFormat,
                    string.Format("Ouptut {0}", outputIndex),
                    outputSpec.Name,
                    output
                );
            }

            string infoText = builder.ToString();
            return infoText;
        }

        #endregion

        #endregion

        #region Properties

        public ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
