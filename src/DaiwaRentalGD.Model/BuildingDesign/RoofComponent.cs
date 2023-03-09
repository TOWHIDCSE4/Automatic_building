using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// A component that describes the roof over a unit.
    /// </summary>
    [Serializable]
    public class RoofComponent : Component
    {
        #region Constructors

        public RoofComponent() : base()
        { }

        protected RoofComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _unit = reader.GetValue<Unit>(nameof(Unit));
        }

        #endregion

        #region Methods

        public virtual Polygon GetPlan()
        {
            return new Polygon();
        }

        public virtual Mesh GetMesh()
        {
            return new Mesh();
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(Unit);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(Unit), _unit);
        }

        #endregion

        #region Properties

        public Unit Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private Unit _unit;

        #endregion
    }
}
