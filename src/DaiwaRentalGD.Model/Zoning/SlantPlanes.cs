using System;
using System.Runtime.Serialization;
using DaiwaRentalGD.Scene;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Zoning
{
    /// <summary>
    /// A scene object that creates slant planes and
    /// validates their restrictions.
    /// </summary>
    [Serializable]
    public class SlantPlanes : SceneObject
    {
        #region Constructors

        public SlantPlanes() : base()
        {
            InitializeComponents();
        }

        protected SlantPlanes(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _adjacentSiteSlantPlanesComponent =
                reader.GetValue<AdjacentSiteSlantPlanesComponent>(
                    nameof(AdjacentSiteSlantPlanesComponent)
                );

            _absoluteHeightSlantPlanesComponent =
                reader.GetValue<AbsoluteHeightSlantPlanesComponent>(
                    nameof(AbsoluteHeightSlantPlanesComponent)
                );

            _roadSlantPlanesComponent =
                reader.GetValue<RoadSlantPlanesComponent>(
                    nameof(RoadSlantPlanesComponent)
                );

            _northSlantPlanesComponent =
                reader.GetValue<NorthSlantPlanesComponent>(
                    nameof(NorthSlantPlanesComponent)
                );
        }

        #endregion

        #region Methods

        private void InitializeComponents()
        {
            AddComponent(AdjacentSiteSlantPlanesComponent);
            AddComponent(AbsoluteHeightSlantPlanesComponent);
            AddComponent(RoadSlantPlanesComponent);
            AddComponent(NorthSlantPlanesComponent);
        }

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(AdjacentSiteSlantPlanesComponent),
                AdjacentSiteSlantPlanesComponent
            );

            writer.AddValue(
               nameof(AbsoluteHeightSlantPlanesComponent),
               AbsoluteHeightSlantPlanesComponent
           );

            writer.AddValue(
                nameof(RoadSlantPlanesComponent),
                RoadSlantPlanesComponent
            );

            writer.AddValue(
                nameof(NorthSlantPlanesComponent),
                NorthSlantPlanesComponent
            );
        }

        #endregion

        #region Properties

        public AdjacentSiteSlantPlanesComponent
            AdjacentSiteSlantPlanesComponent
        {
            get => _adjacentSiteSlantPlanesComponent;
            set
            {
                ReplaceComponent(_adjacentSiteSlantPlanesComponent, value);
                _adjacentSiteSlantPlanesComponent = value;

                NotifyPropertyChanged();
            }
        }

        public AbsoluteHeightSlantPlanesComponent
          AbsoluteHeightSlantPlanesComponent
        {
            get => _absoluteHeightSlantPlanesComponent;
            set
            {
                ReplaceComponent(_absoluteHeightSlantPlanesComponent, value);
                _absoluteHeightSlantPlanesComponent = value;

                NotifyPropertyChanged();
            }
        }

        public RoadSlantPlanesComponent RoadSlantPlanesComponent
        {
            get => _roadSlantPlanesComponent;
            set
            {
                ReplaceComponent(_roadSlantPlanesComponent, value);
                _roadSlantPlanesComponent = value;

                NotifyPropertyChanged();
            }
        }

        public NorthSlantPlanesComponent NorthSlantPlanesComponent
        {
            get => _northSlantPlanesComponent;
            set
            {
                ReplaceComponent(_northSlantPlanesComponent, value);
                _northSlantPlanesComponent = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private AdjacentSiteSlantPlanesComponent
            _adjacentSiteSlantPlanesComponent =
            new AdjacentSiteSlantPlanesComponent();

        private AbsoluteHeightSlantPlanesComponent _absoluteHeightSlantPlanesComponent =
            new AbsoluteHeightSlantPlanesComponent();

        private RoadSlantPlanesComponent _roadSlantPlanesComponent =
            new RoadSlantPlanesComponent();

        private NorthSlantPlanesComponent _northSlantPlanesComponent =
            new NorthSlantPlanesComponent();

        #endregion
    }
}
