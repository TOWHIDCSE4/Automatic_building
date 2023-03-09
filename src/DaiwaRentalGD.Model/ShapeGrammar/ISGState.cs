using System.Collections.Generic;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ShapeGrammar
{
    /// <summary>
    /// The interface for the state that a shape grammar manipulates.
    /// </summary>
    public interface ISGState : IWorkspaceItem
    {
        #region Properties

        IList<ISGMarker> Markers { get; }

        #endregion
    }
}
