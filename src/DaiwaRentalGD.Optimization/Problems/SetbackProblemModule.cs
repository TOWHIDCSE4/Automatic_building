using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model;
using O3.Foundation;

namespace DaiwaRentalGD.Optimization.Problems
{
    /// <summary>
    /// Problem module that corresponds to
    /// <see cref="DaiwaRentalGD.Model.GDModelScene.Setback"/>.
    /// </summary>
    [Serializable]
    public class SetbackProblemModule : GDModelProblemModule
    {
        #region Constructors

        public SetbackProblemModule(GDModelProblem problem) : base(problem)
        { }

        protected SetbackProblemModule(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        { }

        #endregion

        #region Methods

        public override void SetToSolutionOutputs(
            GDModelScene scene, Solution solution
        )
        {
            var setback = scene.Setback;

            if (!setback.SetbackResolverComponent.IsValid)
            {
                solution.Fail();
            }
        }

        #endregion

        #region Constants

        public const string SetbackInvalidMessage =
            "Invalid setback";

        #endregion
    }
}
