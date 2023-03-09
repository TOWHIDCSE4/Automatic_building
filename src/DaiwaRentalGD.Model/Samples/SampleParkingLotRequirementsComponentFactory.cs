using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DaiwaRentalGD.Model.ParkingLotDesign;
using Workspaces.Core;

namespace DaiwaRentalGD.Model.Samples
{
    /// <summary>
    /// A factory class that creates a sample
    /// <see cref="DaiwaRentalGD.Model.ParkingLotDesign
    /// .ParkingLotRequirementsComponent"/>.
    /// </summary>
    [Serializable]
    public class SampleParkingLotRequirementsComponentFactory :
        IWorkspaceItem
    {
        #region Constructors

        public SampleParkingLotRequirementsComponentFactory()
        {
            ItemInfo = new WorkspaceItemInfo();
        }

        protected SampleParkingLotRequirementsComponentFactory(
            SerializationInfo info, StreamingContext context
        )
        {
            var reader = new WorkspaceItemReader(this, info, context);

            ItemInfo = reader.GetItemInfo();
        }

        #endregion

        #region Methods

        public ParkingLotRequirementsComponent Create()
        {
            var plrc = new ParkingLotRequirementsComponent();

            plrc.AddUnitRequirements(
                new UnitParkingRequirements(0)
                {
                    CarParkingSpaceMin = 0.0,
                    CarParkingSpaceMax = 0.5,
                    BicycleParkingSpaceMin = 0.5,
                    BicycleParkingSpaceMax = 1.0
                }
            );

            plrc.AddUnitRequirements(
                new UnitParkingRequirements(1)
                {
                    CarParkingSpaceMin = 0.0,
                    CarParkingSpaceMax = 0.5,
                    BicycleParkingSpaceMin = 0.5,
                    BicycleParkingSpaceMax = 1.0
                }
            );

            plrc.AddUnitRequirements(
                new UnitParkingRequirements(2)
                {
                    CarParkingSpaceMin = 0.5,
                    CarParkingSpaceMax = 1.0,
                    BicycleParkingSpaceMin = 1.0,
                    BicycleParkingSpaceMax = 1.5
                }
            );

            plrc.AddUnitRequirements(
                new UnitParkingRequirements(3)
                {
                    CarParkingSpaceMin = 1.0,
                    CarParkingSpaceMax = 2.0,
                    BicycleParkingSpaceMin = 1.0,
                    BicycleParkingSpaceMax = 2.0
                }
            );

            plrc.UseOverrideCarParkingSpaceMinTotal = false;
            plrc.UseOverrideCarParkingSpaceMaxTotal = false;
            plrc.UseOverrideBicycleParkingSpaceMinTotal = false;
            plrc.UseOverrideBicycleParkingSpaceMaxTotal = false;

            return plrc;
        }

        public IEnumerable<IWorkspaceItem> GetReferencedItems() =>
            Enumerable.Empty<IWorkspaceItem>();

        public void GetObjectData(
            SerializationInfo info, StreamingContext context
        )
        {
            var writer = new WorkspaceItemWriter(this, info, context);

            writer.AddItemInfo();
        }

        #endregion

        #region Properties

        public WorkspaceItemInfo ItemInfo { get; }

        #endregion
    }
}
