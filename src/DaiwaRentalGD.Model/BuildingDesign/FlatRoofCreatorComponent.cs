using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.BuildingDesign
{
    /// <summary>
    /// Component that creates flat roof for a building.
    /// </summary>
    [Serializable]
    public class FlatRoofCreatorComponent : RoofCreatorComponent
    {
        #region Constructors

        public FlatRoofCreatorComponent() : base()
        { }

        protected FlatRoofCreatorComponent(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
            var reader = new WorkspaceItemReader(this, info, context);

            _flatRoofComponentTemplate = reader.GetValue<FlatRoofComponent>(
                nameof(FlatRoofComponentTemplate)
            );
        }

        #endregion

        #region Methods

        public Roof CreateRoofFromTemplate()
        {
            var roof = new Roof();

            FlatRoofComponent frc = new FlatRoofComponent
            {
                Height = FlatRoofComponentTemplate.Height,
                EaveLength = FlatRoofComponentTemplate.EaveLength
            };

            roof.RoofComponent = frc;

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
            base.GetReferencedItems().Append(FlatRoofComponentTemplate);

        public override void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddValue(
                nameof(FlatRoofComponentTemplate),
                _flatRoofComponentTemplate
            );
        }

        #endregion

        #region Properties

        public FlatRoofComponent FlatRoofComponentTemplate
        {
            get => _flatRoofComponentTemplate;
            set
            {
                _flatRoofComponentTemplate =
                    value ?? throw new ArgumentNullException(nameof(value));

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Member variables

        private FlatRoofComponent _flatRoofComponentTemplate =
            new FlatRoofComponent();

        #endregion
    }
}
