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
    /// <see cref="DaiwaRentalGD.Model.GDModelScene.LandUseEvaluator"/>.
    /// </summary>
    [Serializable]
    public class LandUseEvaluatorProblemModule : GDModelProblemModule
    {
        #region Constructors

        public LandUseEvaluatorProblemModule(GDModelProblem problem) :
            base(problem)
        { }

        protected LandUseEvaluatorProblemModule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            BuildingCoverageRatioOutputSpec = reader.GetValue<OutputSpec>(
                nameof(BuildingCoverageRatioOutputSpec)
            );

            FloorAreaRatioOutputSpec =
                reader.GetValue<OutputSpec>(nameof(FloorAreaRatioOutputSpec));
        }

        #endregion

        #region Methods

        public override void UpdateOutputSpecs()
        {
            var gdms = Problem.GDModelScene;
            double SiteSizeX = gdms.Site.SiteComponent.GetBBoxOfBounddary().SizeX;
            double SiteSizeY = gdms.Site.SiteComponent.GetBBoxOfBounddary().SizeY;


            BuildingCoverageRatioOutputSpec.Constraint.Min = 25;
            WalkwayLengthOutputSpec.Constraint.Max = (SiteSizeX + SiteSizeY) * 4 / 5;
            Problem.AddOutputSpec(BuildingCoverageRatioOutputSpec);
            Problem.AddOutputSpec(FloorAreaRatioOutputSpec);
            Problem.AddOutputSpec(NumberOfParkingOutputSpec);
            Problem.AddOutputSpec(WalkwayLengthOutputSpec);
        }

        public override void SetToSolutionOutputs(
            GDModelScene scene, Solution solution
        )
        {
            var lue = scene.LandUseEvaluator;

            solution.SetOutput(
                BuildingCoverageRatioOutputSpec,
                lue.BuildingCoverageRatioComponent.BuildingCoverageRatio
            );

            solution.SetOutput(
                FloorAreaRatioOutputSpec,
                lue.FloorAreaRatioComponent.FloorAreaRatio
            );

            var plrc = scene.ParkingLotDesigner.ParkingLotRequirementsComponent;
            solution.SetOutput(
                NumberOfParkingOutputSpec,
                plrc.NumOfCarParkingSpaces
            );

            int wwc = plrc.WalkwayPaths.Count;
            double wwl = 0.0;
            for(int wwi=0; wwi<wwc; wwi++) {
                wwl += plrc.WalkwayPaths[wwi].Length;
            }
            solution.SetOutput(
                WalkwayLengthOutputSpec,
                wwl
            ) ;

        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems()
            .Append(BuildingCoverageRatioOutputSpec)
            .Append(FloorAreaRatioOutputSpec)
            .Append(WalkwayLengthOutputSpec);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(BuildingCoverageRatioOutputSpec),
                BuildingCoverageRatioOutputSpec
            );

            writer.AddValue(
                nameof(FloorAreaRatioOutputSpec), FloorAreaRatioOutputSpec
            );

            writer.AddValue(
                nameof(WalkwayLengthOutputSpec), WalkwayLengthOutputSpec
            );
        }

        #endregion

        #region Properties

        public OutputSpec BuildingCoverageRatioOutputSpec { get; } =
            new OutputSpec
            {
                Name = BuildingCoverageRatioOutputName,
                DataSpec = new RealDataSpec(),
                ObjectiveType = ObjectiveType.Maximize
            };

        public OutputSpec FloorAreaRatioOutputSpec { get; } =
            new OutputSpec
            {
                Name = FloorAreaRatioOutputName,
                DataSpec = new RealDataSpec(),
                ObjectiveType = ObjectiveType.None
            };

        public OutputSpec NumberOfParkingOutputSpec { get; } =
            new OutputSpec
            {
                Name = NumberOfParkingOutputName,
                DataSpec = new RealDataSpec(),
                ObjectiveType = ObjectiveType.Maximize
            };

        public OutputSpec WalkwayLengthOutputSpec { get; } =
            new OutputSpec
            {
                Name = WalkwayLengthOutputName,
                DataSpec = new RealDataSpec(),
                ObjectiveType = ObjectiveType.Minimize
            };
        #endregion

        #region Constants

        public const string BuildingCoverageRatioOutputName =
            "Building Coverage Ratio";

        public const string FloorAreaRatioOutputName =
            "Floor Area Ratio";

        public const string NumberOfParkingOutputName =
            "Number of Parking Space";

        public const string WalkwayLengthOutputName =
            "Walkway Length";

        #endregion
    }
}
