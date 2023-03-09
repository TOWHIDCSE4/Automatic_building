using System;
using System.Collections.Generic;
using System.Linq;
using DaiwaRentalGD.Geometries;
using DaiwaRentalGD.Model.BuildingDesign;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestUnitComponent
    {
        [Fact]
        public void TestConstructor_Default()
        {
            UnitComponent unitComp = new UnitComponent();

            BBox bbox = unitComp.GetBBox();
            Assert.Equal(
                new BBox { MaxZ = unitComp.RoomHeight },
                bbox
            );

            Assert.Equal(
                UnitComponent.DefaultNumOfBedrooms,
                unitComp.NumOfBedrooms
            );
            Assert.Empty(unitComp.RoomPlans);
            Assert.Equal(
                UnitComponent.DefaultRoomHeight,
                unitComp.RoomHeight
            );
        }

        [Fact]
        public void TestConstructor_Copy()
        {
            UnitComponent uc0 = new UnitComponent
            {
                NumOfBedrooms = 1,
                RoomHeight = 3.5
            };

            uc0.AddRoomPlan(
                new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(6.0, 0.0, 0.0),
                        new Point(6.0, 8.0, 0.0),
                        new Point(0.0, 8.0, 0.0)
                    }
                )
            );

            UnitComponent uc1 = new UnitComponent(uc0);

            Assert.Equal(uc0.NumOfBedrooms, uc1.NumOfBedrooms);
            Assert.Equal(uc0.RoomHeight, uc1.RoomHeight);

            Assert.Equal(uc0.RoomPlans, uc1.RoomPlans);
            foreach (
                var plans in uc0.RoomPlans.Zip(
uc1.RoomPlans,
                    (p0, p1) => new Tuple<Polygon, Polygon>(p0, p1)
                )
            )
            {
                Assert.NotSame(plans.Item1, plans.Item2);
            }
        }

        [Fact]
        public void TestConvertPlanPToPlan()
        {
            Polygon planP = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(100.0, 0.0, 0.0),
                    new Point(200.0, 300.0, 0.0),
                    new Point(0.0, 200.0, 0.0)
                }
            );

            Polygon plan = UnitComponent.ConvertPlanPToPlan(planP);

            Assert.Equal(
                new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(91.0, 0.0, 0.0),
                        new Point(182.0, 273.0, 0.0),
                        new Point(0.0, 182.0, 0.0)
                    }
                ),
                plan
            );
        }

        [Fact]
        public void TestConvertPlanToPlanP()
        {
            Polygon plan = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(91.0, 0.0, 0.0),
                    new Point(182.0, 273.0, 0.0),
                    new Point(0.0, 182.0, 0.0)
                }
            );

            Polygon planP = UnitComponent.ConvertPlanToPlanP(plan);

            Assert.Equal(
                new Polygon(
                    new[]
                    {
                        new Point(0.0, 0.0, 0.0),
                        new Point(100.0, 0.0, 0.0),
                        new Point(200.0, 300.0, 0.0),
                        new Point(0.0, 200.0, 0.0)
                    }
                ),
                planP
            );
        }

        [Fact]
        public void TestAddRoomPlan()
        {
            Polygon roomPlan = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(5.0, 0.0, 0.0),
                    new Point(5.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            );

            UnitComponent unitComp = new UnitComponent();

            unitComp.AddRoomPlan(roomPlan);

            Assert.Equal(
                new List<Polygon> { roomPlan },
                unitComp.RoomPlans
            );
        }

        [Fact]
        public void TestAddRoomPlanP()
        {
            Polygon roomPlanP = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(5.0, 0.0, 0.0),
                    new Point(5.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            );

            UnitComponent unitComp = new UnitComponent();

            unitComp.AddRoomPlanP(roomPlanP);

            Assert.Equal(
                new List<Polygon>
                {
                    UnitComponent.ConvertPlanPToPlan(roomPlanP)
                },
                unitComp.RoomPlans
            );
        }

        [Fact]
        public void TestRemoveRoomPlan()
        {
            Polygon roomPlan0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(5.0, 0.0, 0.0),
                    new Point(5.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            );
            Polygon roomPlan1 = new Polygon(
                new[]
                {
                    new Point(5.0, 2.0, 0.0),
                    new Point(8.0, 2.0, 0.0),
                    new Point(8.0, 6.0, 0.0),
                    new Point(5.0, 6.0, 0.0)
                }
            );

            UnitComponent unitComp = new UnitComponent();

            unitComp.AddRoomPlan(roomPlan0);
            unitComp.AddRoomPlan(roomPlan1);

            unitComp.RemoveRoomPlan(0);

            Assert.Equal(
                new List<Polygon> { roomPlan1 },
                unitComp.RoomPlans
            );
        }

        [Fact]
        public void TestGetBBox()
        {
            Polygon roomPlan0 = new Polygon(
                new[]
                {
                    new Point(0.0, 0.0, 0.0),
                    new Point(5.0, 0.0, 0.0),
                    new Point(5.0, 10.0, 0.0),
                    new Point(0.0, 10.0, 0.0)
                }
            );
            Polygon roomPlan1 = new Polygon(
                new[]
                {
                    new Point(5.0, 2.0, 0.0),
                    new Point(8.0, 2.0, 0.0),
                    new Point(8.0, 6.0, 0.0),
                    new Point(5.0, 6.0, 0.0)
                }
            );

            UnitComponent unitComp = new UnitComponent
            {
                RoomHeight = 3.2
            };

            unitComp.AddRoomPlan(roomPlan0);
            unitComp.AddRoomPlan(roomPlan1);

            BBox bbox = unitComp.GetBBox();

            Assert.Equal(
                BBox.FromMinAndMax(
                    0.0, -2.0, 0.0,
                    8.0, 12.0, 3.2
                ),
                bbox
            );
        }

        [Fact]
        public void TestHouseholdType()
        {
            UnitComponent uc = new UnitComponent();

            uc.NumOfBedrooms = 0;
            Assert.Equal(HouseholdType.SinglePerson, uc.HouseholdType);

            uc.NumOfBedrooms = 1;
            Assert.Equal(HouseholdType.SinglePerson, uc.HouseholdType);

            uc.NumOfBedrooms = 2;
            Assert.Equal(HouseholdType.Family, uc.HouseholdType);

            uc.NumOfBedrooms = 3;
            Assert.Equal(HouseholdType.Family, uc.HouseholdType);
        }

        [Fact]
        public void TestPlanName()
        {
            UnitComponent uc = new UnitComponent();

            uc.NumOfBedrooms = 0;
            Assert.Equal("1R/1K/1DK", uc.PlanName);

            uc.NumOfBedrooms = 1;
            Assert.Equal("1LDK", uc.PlanName);

            uc.NumOfBedrooms = 2;
            Assert.Equal("2LDK", uc.PlanName);

            uc.NumOfBedrooms = 3;
            Assert.Equal("3LDK", uc.PlanName);
        }
    }
}
