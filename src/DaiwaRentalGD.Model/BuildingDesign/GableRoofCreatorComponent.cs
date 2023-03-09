using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Component that creates gable roof for a building.
    /// </summary>
    [Serializable]
    public class GableRoofCreatorComponent : RoofCreatorComponent
    {
        #region Constructors

        public GableRoofCreatorComponent() : base()
        { }

        protected GableRoofCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _gableRoofComponentTemplate =
                reader.GetValue<GableRoofComponent>(
                    nameof(GableRoofComponentTemplate)
                );
        }

        #endregion

        #region Methods

        public Roof CreateRoofFromTemplate()
        {
            var roof = new Roof();

            GableRoofComponent grc = new GableRoofComponent
            {
                EaveHeight = GableRoofComponentTemplate.EaveHeight,
                EaveLength = GableRoofComponentTemplate.EaveLength,
                SlopeAngle = GableRoofComponentTemplate.SlopeAngle
            };

            roof.RoofComponent = grc;

            return roof;
        }

        public override void CreateRoofs()
        {
            if (Building == null)
            {
                return;
            }

            BuildingComponent bc = Building.BuildingComponent;

            bc.ClearRoofs();

            if (bc.NumOfFloors == 0)
            {
                return;
            }

            for (int stack = 0; stack < bc.NumOfUnitsPerFloor; ++stack)
            {
                var roof = CreateRoofFromTemplate();

                bc.SetRoof(bc.NumOfFloors - 1, stack, roof);

                var unit = bc.GetUnit(bc.NumOfFloors - 1, stack);

                roof.RoofComponent.Unit = unit;

                roof.TransformComponent.Transform =
                    GetRoofTransform(bc.NumOfFloors - 1, stack);
            }
        }

        public override IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            base.GetReferencedItems().Append(GableRoofComponentTemplate);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(GableRoofComponentTemplate),
                _gableRoofComponentTemplate
            );
        }

        #endregion

        #region Properties

        public GableRoofComponent GableRoofComponentTemplate
        {
            get => _gableRoofComponentTemplate;
            set
            {
                _gableRoofComponentTemplate =
                    value ?? throw new ArgumentNullException(nameof(value));

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private GableRoofComponent _gableRoofComponentTemplate =
            new GableRoofComponent();

        #endregion
    }
}
