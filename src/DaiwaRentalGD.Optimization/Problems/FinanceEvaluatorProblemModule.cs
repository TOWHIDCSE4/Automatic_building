using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model;
using O3.Commons.DataSpecs;
using O3.Foundation;
using Workspaces.Core;

namespace DaiwaRentalGD.Optimization.Problems
{
    /// <summary>
    /// Problem module that corresponds to
    /// <see cref="DaiwaRentalGD.Model.GDModelScene.FinanceEvaluator"/>.
    /// </summary>
    [Serializable]
    public class FinanceEvaluatorProblemModule : GDModelProblemModule
    {
        #region Constructors

        public FinanceEvaluatorProblemModule(GDModelProblem problem) :
            base(problem)
        { }

        protected FinanceEvaluatorProblemModule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            TotalCostYenOutputSpec =
                reader.GetValue<OutputSpec>(nameof(TotalCostYenOutputSpec));

            TotalRevenueYenPerYearOutputSpec = reader.GetValue<OutputSpec>(
                nameof(TotalRevenueYenPerYearOutputSpec)
            );

            GrossRorPerYearOutputSpec = reader.GetValue<OutputSpec>(
                nameof(GrossRorPerYearOutputSpec)
            );
        }

        #endregion

        #region Methods

        public override void UpdateOutputSpecs()
        {
            Problem.AddOutputSpec(TotalCostYenOutputSpec);
            Problem.AddOutputSpec(TotalRevenueYenPerYearOutputSpec);
            Problem.AddOutputSpec(GrossRorPerYearOutputSpec);
        }

        public override void SetToSolutionOutputs(
            GDModelScene scene, Solution solution
        )
        {
            var fe = scene.FinanceEvaluator;
            var sfc = fe.SummaryFinanceComponent;

            solution.SetOutput(
                TotalCostYenOutputSpec,
                sfc.TotalCostYen
            );

            solution.SetOutput(
                TotalRevenueYenPerYearOutputSpec,
                sfc.TotalRevenueYenPerYear
            );

            solution.SetOutput(
                GrossRorPerYearOutputSpec,
                sfc.GrossRorPerYear
            );
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(TotalCostYenOutputSpec)
            .Append(TotalRevenueYenPerYearOutputSpec)
            .Append(GrossRorPerYearOutputSpec);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(TotalCostYenOutputSpec), TotalCostYenOutputSpec
            );
            writer.AddValue(
                nameof(TotalRevenueYenPerYearOutputSpec),
                TotalRevenueYenPerYearOutputSpec
            );
            writer.AddValue(
                nameof(GrossRorPerYearOutputSpec), GrossRorPerYearOutputSpec
            );
        }

        #endregion

        #region Properties

        public OutputSpec TotalCostYenOutputSpec { get; } =
            new OutputSpec
            {
                Name = TotalCostYenOutputName,
                DataSpec = new RealDataSpec(),
                ObjectiveType = ObjectiveType.Minimize
            };

        public OutputSpec TotalRevenueYenPerYearOutputSpec { get; } =
            new OutputSpec
            {

                Name = TotalRevenueYenPerYearOutputName,
                DataSpec = new RealDataSpec(),
                ObjectiveType = ObjectiveType.Maximize
            };

        public OutputSpec GrossRorPerYearOutputSpec { get; } =
            new OutputSpec
            {

                Name = GrossRorPerYearOutputName,
                DataSpec = new RealDataSpec(),
                ObjectiveType = ObjectiveType.None
            };

        #endregion

        #region Constants

        public const string TotalCostYenOutputName =
            "Total Cost (Yen)";

        public const string TotalRevenueYenPerYearOutputName =
            "Total Revenue (Yen/Year)";

        public const string GrossRorPerYearOutputName =
            "Gross Rate of Return (Per Year)";

        #endregion
    }
}
