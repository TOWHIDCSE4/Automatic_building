using System;
using DaiwaRentalGD.Optimization.Solvers;
using O3.Nsga.UI.ViewModels;

namespace DaiwaRentalGD.Gui.ViewModels.Optimization
{
    /// <summary>
    ///// View model for
    ///<see cref="DaiwaRentalGD.Optimization.Solvers.GDModelSolver"/>.
    /// </summary>
    public class GDModelSolverViewModel : Nsga2SolverViewModel
    {
        #region Constructors

        public GDModelSolverViewModel(GDModelSolver solver) : base(solver)
        {
            GDModelSolver = solver ??
                throw new ArgumentNullException(nameof(solver));
        }

        #endregion

        #region Properties

        public GDModelSolver GDModelSolver { get; }

        #endregion
    }
}
