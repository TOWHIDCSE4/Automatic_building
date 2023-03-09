using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Optimization.Problems;
using O3.Commons.DesignsOfExps;
using O3.Commons.Evolution.Crossover;
using O3.Commons.Evolution.Mutation;
using Workspaces.Core;

namespace DaiwaRentalGD.Optimization.Solvers
{
    /// <summary>
    /// Base class defining a module for
    /// a <see cref="DaiwaRentalGD.Optimization.Solvers.GDModelSolver"/>.
    /// </summary>
    [Serializable]
    public abstract class GDModelSolverModule : IWorkspaceItem
    {
        #region Constructors

        protected GDModelSolverModule(GDModelSolver solver)
        {
            ItemInfo = new WorkspaceItemInfo();

            Solver = solver ??
                throw new ArgumentNullException(nameof(solver));
        }

        protected GDModelSolverModule(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();

            Solver = reader.GetValue<GDModelSolver>(nameof(Solver));

            DesignOfExps =
                reader.GetValue<DesignOfExps>(nameof(DesignOfExps));

            CrossoverOp =
                reader.GetValue<CrossoverOp>(nameof(CrossoverOp));

            MutationOp =
                reader.GetValue<MutationOp>(nameof(MutationOp));
        }

        #endregion

        #region Methods

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>()
            .Append(Solver)
            .Append(DesignOfExps)
            .Append(CrossoverOp)
            .Append(MutationOp);

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();

            writer.AddValue(nameof(Solver), Solver);
            writer.AddValue(nameof(DesignOfExps), DesignOfExps);
            writer.AddValue(nameof(CrossoverOp), CrossoverOp);
            writer.AddValue(nameof(MutationOp), MutationOp);
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        public GDModelSolver Solver { get; }

        public GDModelProblem Problem => Solver.GDModelProblem;

        public virtual DesignOfExps DesignOfExps { get; protected set; }

        public virtual CrossoverOp CrossoverOp { get; protected set; }

        public virtual MutationOp MutationOp { get; protected set; }

        #endregion
    }
}
