using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model;
using O3.Foundation;

namespace DaiwaRentalGD.Optimization.Problems
{
    /// <summary>
    /// Problem module that corresponds to
    /// <see cref="DaiwaRentalGD.Model.GDModelScene.SlantPlanes"/>.
    /// </summary>
    [Serializable]
    public class SlantPlanesProblemModule : GDModelProblemModule
    {
        #region Constructors

        public SlantPlanesProblemModule(GDModelProblem problem) :
            base(problem)
        { }

        protected SlantPlanesProblemModule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public override void SetToSolutionOutputs(
            GDModelScene scene, Solution solution
        )
        {
            var slantPlanes = scene.SlantPlanes;

            if (!slantPlanes.AdjacentSiteSlantPlanesComponent.IsValid)
            {
                solution.Fail();
            }

            if (!slantPlanes.RoadSlantPlanesComponent.IsValid)
            {
                solution.Fail();
            }

            if (!slantPlanes.NorthSlantPlanesComponent.IsValid)
            {
                solution.Fail();
            }
        }

        #endregion

        #region Constants

        public const string AdjacentSiteSlantPlanesInvalidMessage =
            "Adjacent site slant planes not valid";

        public const string RoadSlantPlanesInvalidMessage =
            "Road slant planes not valid";

        public const string NorthSlantPlanesInvalidMessage =
            "North slant planes not valid";

        #endregion
    }
}
