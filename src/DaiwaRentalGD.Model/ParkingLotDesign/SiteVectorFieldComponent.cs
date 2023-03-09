using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.SiteDesign;
using DaiwaRentalGD.Scene;
using MathNet.Numerics.LinearAlgebra;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.ParkingLotDesign
{
    /// <summary>
    /// A component that defines a vector field on a site plane.
    /// </summary>
    [Serializable]
    public class SiteVectorFieldComponent : Component
    {
        #region Constructors

        public SiteVectorFieldComponent() : base()
        { }

        protected SiteVectorFieldComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            SiteVectorField =
                reader.GetValue<SiteVectorField>(nameof(SiteVectorField));

            _site = reader.GetValue<Site>(nameof(Site));
        }

        #endregion

        #region Methods

        public Tuple<Vector<double>, Vector<double>>
            GetVectorPair(Point point)
        {
            return SiteVectorField.GetVectorPair(Site, point);
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(Site);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(nameof(SiteVectorField), SiteVectorField);
            writer.AddValue(nameof(Site), _site);
        }

        #endregion

        #region Properties

        public SiteVectorField SiteVectorField { get; } =
            new SiteVectorField();

        public Site Site
        {
            get => _site;
            set
            {
                _site = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private Site _site;

        #endregion
    }
}
