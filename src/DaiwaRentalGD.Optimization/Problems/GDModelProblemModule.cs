using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model;
using O3.Foundation;
using Workspaces.Core;

namespace DaiwaRentalGD.Optimization.Problems
{
    /// <summary>
    /// Base class for defining a module of a GD model problem.
    /// </summary>
    [Serializable]
    public abstract class GDModelProblemModule : IWorkspaceItem
    {
        #region Constructors

        protected GDModelProblemModule(GDModelProblem problem)
        {
            ItemInfo = new WorkspaceItemInfo();

            Problem = problem ??
                throw new ArgumentNullException(nameof(problem));
        }

        protected GDModelProblemModule(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            Problem = reader.GetValue<GDModelProblem>(nameof(Problem));
        }

        #endregion

        #region Methods

        public virtual void UpdateInputSpecs()
        { }

        public virtual void UpdateOutputSpecs()
        { }

        public virtual void SetFromSolutionInputs(
            GDModelScene scene, Solution solution
        )
        { }

        public virtual void SetToSolutionOutputs(
            GDModelScene scene, Solution solution
        )
        { }

        public virtual IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            new[] { Problem };

        public virtual void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Problem), Problem);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public GDModelProblem Problem { get; }

        #endregion

        #region Constants

        public const int InvalidInputIndex = -1;

        #endregion
    }
}
