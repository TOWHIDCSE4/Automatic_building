using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaiwaRentalGD.Model;
using DaiwaRentalGD.Model.Samples;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Optimization.Problems;
using O3.Foundation;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Optimization.Samples.Foundation
{
    /// <summary>
    /// Provides sample usage of <see cref="Problem"/>.
    /// This class is not intended for unit testing.
    /// For demo purposes, information is written to xUnit.net outputs.
    /// </summary>
    public class ProblemSampleTests
    {
        #region Constructors

        public ProblemSampleTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        #region Tests

        [Theory]
        [MemberData(nameof(TestShowProblemInfoData))]
        public void TestShowProblemInfo(Problem problem)
        {
            OutputHelper.WriteLine(GetProblemInfoText(problem));
        }

        public static IEnumerable<object[]> TestShowProblemInfoData()
        {
            // Default instance of `BinhAndKorn2Problem`

            yield return new object[]
            {
                BinhAndKorn2Problem.Default
            };

            // A `GDModelProblem` instance

            var gdModelScene = new GDModelScene
            {
                SiteDesigner = new SiteDesigner
                {
                    // Without a valid site
                    // (a site with at least 1 road edge),
                    // problem initialization will fail.
                    // Thus we initialize the scene with a valid site.
                    SiteCreatorComponent = new Sample0SiteCreatorComponent()
                }
            };

            gdModelScene.ExecuteUpdate();

            var gdModelProblem = new GDModelProblem(gdModelScene);

            yield return new object[]
            {
                gdModelProblem
            };
        }

        #endregion

        #region Utilities

        private static string GetProblemInfoText(Problem problem)
        {
            var builder = new StringBuilder();

            builder.AppendFormat(
                "Problem Name: {0}\n\n",
                problem.Name
            );

            builder.AppendFormat(
                "Num of Input Specs: {0}\n\n",
                problem.InputSpecs.Count
            );

            builder.AppendFormat(

                "Num of Output Specs: {0}\n" +
                "Num of Objectives: {1}\n" +
                "Num of Constraints: {2}\n\n",

                problem.OutputSpecs.Count,

                problem.OutputSpecs.Count(
                    outputSpec => outputSpec.ObjectiveType !=
                        ObjectiveType.None
                ),

                problem.OutputSpecs.Count(
                    outputSpec => outputSpec.Constraint.Relation !=
                        ConstraintRelation.None
                )
            );

            // Input specs

            builder.AppendLine("Input Specs:\n");

            for (int inputIndex = 0; inputIndex < problem.InputSpecs.Count;
                ++inputIndex)
            {
                var inputSpecText = GetInputSpecText(problem, inputIndex);

                builder.AppendLine(inputSpecText);
            }

            // Output specs

            builder.AppendLine("Output Specs:\n");

            for (int outputIndex = 0; outputIndex < problem.OutputSpecs.Count;
                ++outputIndex)
            {
                var outputSpecText = GetOutputSpecText(problem, outputIndex);

                builder.AppendLine(outputSpecText);
            }

            string infoText = builder.ToString();
            return infoText;
        }

        private static string GetInputSpecText(
            Problem problem, int inputIndex
        )
        {
            var builder = new StringBuilder();

            string rowFormat = "{0,-20}{1}\n";

            builder.AppendFormat(
                rowFormat,
                "Index:", inputIndex
            );

            var inputSpec = problem.InputSpecs[inputIndex];

            builder.AppendFormat(
                rowFormat,
                "Name:", inputSpec.Name
            );
            builder.AppendFormat(
                rowFormat,
                "Data Spec:", inputSpec.DataSpec?.Name
            );
            builder.AppendFormat(
                rowFormat,
                "Use Constant?", inputSpec.UseConstant
            );

            return builder.ToString();
        }

        private static string GetOutputSpecText(
            Problem problem, int outputIndex
        )
        {
            var builder = new StringBuilder();

            string rowFormat = "{0,-20}{1}\n";

            builder.AppendFormat(
                rowFormat,
                "Index:", outputIndex
            );

            var outputSpec = problem.OutputSpecs[outputIndex];

            builder.AppendFormat(
                rowFormat,
                "Name:", outputSpec.Name
            );
            builder.AppendFormat(
                rowFormat,
                "Data Spec:", outputSpec.DataSpec?.Name
            );
            builder.AppendFormat(
                rowFormat,
                "Objective Type:", outputSpec.ObjectiveType
            );
            builder.AppendFormat(
                rowFormat,
                "Constraint:",
                string.Format(
                    "{0} (Min = {1:N4}, Max = {2:N4})",
                    outputSpec.Constraint.Relation,
                    outputSpec.Constraint.Min,
                    outputSpec.Constraint.Max
                )
            );

            return builder.ToString();
        }

        #endregion

        #endregion

        #region Properties

        public ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
