using System;
using System.Linq;
using DaiwaRentalGD.Model.BuildingDesign;
using Xunit;

namespace DaiwaRentalGD.Model.Tests.BuildingDesign
{
    public class TestUnitArrangerComponent
    {
        [Fact]
        public void TestConstructor()
        {
            var uac = new UnitArrangerComponent();

            Assert.Null(uac.Building);
            Assert.Null(uac.UnitCatalog);

            Assert.Equal(
                UnitArrangerComponent.DefaultMinNumOfFloors,
                uac.MinNumOfFloors
            );
            Assert.Equal(
                UnitArrangerComponent.DefaultMaxNumOfFloors,
                uac.MaxNumOfFloors
            );
            Assert.Equal(
                UnitArrangerComponent.DefaultMinNumOfFloors,
                uac.NumOfFloors
            );
            Assert.Equal(0.0, uac.NormalizedNumOfFloors);

            Assert.Equal(
                UnitArrangerComponent.DefaultMinNumOfUnitsPerFloor,
                uac.MinNumOfUnitsPerFloor
            );
            Assert.Equal(
                UnitArrangerComponent.DefaultMaxNumOfUnitsPerFloor,
                uac.MaxNumOfUnitsPerFloor
            );
            Assert.Equal(
                UnitArrangerComponent.DefaultMinNumOfUnitsPerFloor,
                uac.NumOfUnitsPerFloor
            );
            Assert.Equal(0.0, uac.NormalizedNumOfUnitsPerFloor);

            Assert.Equal(
                Enumerable.Repeat(
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    uac.NumOfUnitsPerFloor
                ),
                uac.NormalizedEntryIndices
            );
        }

        [Fact]
        public void TestSetNormalizedEntryIndex()
        {
            var uac = new UnitArrangerComponent
            {
                MinNumOfUnitsPerFloor = 1,
                MaxNumOfUnitsPerFloor = 5,
                NumOfUnitsPerFloor = 3
            };

            uac.SetNormalizedEntryIndex(0, 0.1);
            uac.SetNormalizedEntryIndex(1, 0.2);
            uac.SetNormalizedEntryIndex(2, 0.3);

            Assert.Equal(
                new[] { 0.1, 0.2, 0.3 },
                uac.NormalizedEntryIndices
            );
        }

        [Fact]
        public void TestSetNormalizedEntryIndex_OutOfRange()
        {
            var uac = new UnitArrangerComponent
            {
                MinNumOfUnitsPerFloor = 1,
                MaxNumOfUnitsPerFloor = 5,
                NumOfUnitsPerFloor = 3
            };

            uac.SetNormalizedEntryIndex(0, 0.1);
            uac.SetNormalizedEntryIndex(1, 0.2);
            uac.SetNormalizedEntryIndex(2, 0.3);

            Assert.Equal(
                new[] { 0.1, 0.2, 0.3 },
                uac.NormalizedEntryIndices
            );

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.SetNormalizedEntryIndex(0, -0.2); }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.SetNormalizedEntryIndex(0, 1.6); }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.SetNormalizedEntryIndex(-1, 0.2); }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.SetNormalizedEntryIndex(5, 0.6); }
            );

            Assert.Equal(
                new[] { 0.1, 0.2, 0.3 },
                uac.NormalizedEntryIndices
            );
        }

        [Fact]
        public void TestMinNumOfFloors()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent();

            uac.MinNumOfFloors = 1;
            uac.MaxNumOfFloors = 3;
            uac.NumOfFloors = 2;

            Assert.Equal(1, uac.MinNumOfFloors);
            Assert.Equal(2, uac.NumOfFloors);
            Assert.Equal(0.5, uac.NormalizedNumOfFloors, DecimalPlaces);

            uac.MinNumOfFloors = 3;

            Assert.Equal(3, uac.MinNumOfFloors);
            Assert.Equal(3, uac.NumOfFloors);
            Assert.Equal(0.5, uac.NormalizedNumOfFloors, DecimalPlaces);

            uac.MinNumOfFloors = 1;

            Assert.Equal(1, uac.MinNumOfFloors);
            Assert.Equal(3, uac.NumOfFloors);
            Assert.Equal(5.0 / 6.0, uac.NormalizedNumOfFloors, DecimalPlaces);
        }

        [Fact]
        public void TestMinNumOfFloors_OutOfRange()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent();

            uac.MinNumOfFloors = 1;
            uac.MaxNumOfFloors = 3;
            uac.NumOfFloors = 2;

            Assert.Equal(1, uac.MinNumOfFloors);
            Assert.Equal(2, uac.NumOfFloors);
            Assert.Equal(0.5, uac.NormalizedNumOfFloors, DecimalPlaces);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MinNumOfFloors = -1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MinNumOfFloors = 0; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MinNumOfFloors = 5; }
            );

            Assert.Equal(1, uac.MinNumOfFloors);
            Assert.Equal(2, uac.NumOfFloors);
            Assert.Equal(0.5, uac.NormalizedNumOfFloors, DecimalPlaces);
        }

        [Fact]
        public void TestMaxNumOfFloors()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent();

            uac.MinNumOfFloors = 1;
            uac.MaxNumOfFloors = 4;
            uac.NumOfFloors = 3;

            Assert.Equal(4, uac.MaxNumOfFloors);
            Assert.Equal(3, uac.NumOfFloors);
            Assert.Equal(0.625, uac.NormalizedNumOfFloors, DecimalPlaces);

            uac.MaxNumOfFloors = 1;

            Assert.Equal(1, uac.MaxNumOfFloors);
            Assert.Equal(1, uac.NumOfFloors);
            Assert.Equal(0.5, uac.NormalizedNumOfFloors, DecimalPlaces);

            uac.MaxNumOfFloors = 2;

            Assert.Equal(2, uac.MaxNumOfFloors);
            Assert.Equal(1, uac.NumOfFloors);
            Assert.Equal(0.25, uac.NormalizedNumOfFloors, DecimalPlaces);
        }

        [Fact]
        public void TestMaxNumOfFloors_OutOfRange()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent();

            uac.MaxNumOfFloors = 5;
            uac.MinNumOfFloors = 2;
            uac.NumOfFloors = 4;

            Assert.Equal(5, uac.MaxNumOfFloors);
            Assert.Equal(4, uac.NumOfFloors);
            Assert.Equal(0.625, uac.NormalizedNumOfFloors, DecimalPlaces);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MaxNumOfFloors = -1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MaxNumOfFloors = 0; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MaxNumOfFloors = 1; }
            );

            Assert.Equal(5, uac.MaxNumOfFloors);
            Assert.Equal(4, uac.NumOfFloors);
            Assert.Equal(0.625, uac.NormalizedNumOfFloors, DecimalPlaces);
        }

        [Fact]
        public void TestNumOfFloors()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent
            {
                MinNumOfFloors = 1,
                MaxNumOfFloors = 5
            };

            uac.NumOfFloors = 1;
            Assert.Equal(1, uac.NumOfFloors);
            Assert.Equal(0.1, uac.NormalizedNumOfFloors, DecimalPlaces);

            uac.NumOfFloors = 5;
            Assert.Equal(5, uac.NumOfFloors);
            Assert.Equal(0.9, uac.NormalizedNumOfFloors, DecimalPlaces);

            uac.NumOfFloors = 3;
            Assert.Equal(3, uac.NumOfFloors);
            Assert.Equal(0.5, uac.NormalizedNumOfFloors, DecimalPlaces);
        }

        [Fact]
        public void TestNumOfFloors_OutOfRange()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent
            {
                MinNumOfFloors = 2,
                MaxNumOfFloors = 6
            };

            uac.NumOfFloors = 3;
            Assert.Equal(3, uac.NumOfFloors);
            Assert.Equal(0.3, uac.NormalizedNumOfFloors, DecimalPlaces);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NumOfFloors = -1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NumOfFloors = 0; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NumOfFloors = 1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NumOfFloors = 7; }
            );

            Assert.Equal(3, uac.NumOfFloors);
            Assert.Equal(0.3, uac.NormalizedNumOfFloors, DecimalPlaces);
        }

        [Fact]
        public void TestNormalizedNumOfFloors()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent
            {
                MinNumOfFloors = 1,
                MaxNumOfFloors = 5
            };

            uac.NormalizedNumOfFloors = 0.0;
            Assert.Equal(0.0, uac.NormalizedNumOfFloors, DecimalPlaces);
            Assert.Equal(1, uac.NumOfFloors);

            uac.NormalizedNumOfFloors = 1.0;
            Assert.Equal(1.0, uac.NormalizedNumOfFloors, DecimalPlaces);
            Assert.Equal(5, uac.NumOfFloors);

            uac.NormalizedNumOfFloors = 0.45;
            Assert.Equal(0.45, uac.NormalizedNumOfFloors, DecimalPlaces);
            Assert.Equal(3, uac.NumOfFloors);

            uac.MaxNumOfFloors = 4;
            Assert.Equal(0.625, uac.NormalizedNumOfFloors, DecimalPlaces);
            Assert.Equal(3, uac.NumOfFloors);

            uac.MinNumOfFloors = 3;
            Assert.Equal(0.25, uac.NormalizedNumOfFloors, DecimalPlaces);
            Assert.Equal(3, uac.NumOfFloors);
        }

        [Fact]
        public void TestNormalizedNumOfFloors_OutOfRange()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent
            {
                MinNumOfFloors = 1,
                MaxNumOfFloors = 5
            };

            uac.NormalizedNumOfFloors = 0.45;
            Assert.Equal(0.45, uac.NormalizedNumOfFloors, DecimalPlaces);
            Assert.Equal(3, uac.NumOfFloors);

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NormalizedNumOfFloors = -0.2; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NormalizedNumOfFloors = 1.5; }
            );

            Assert.Equal(0.45, uac.NormalizedNumOfFloors, DecimalPlaces);
            Assert.Equal(3, uac.NumOfFloors);
        }

        [Fact]
        public void TestMinNumOfUnitsPerFloor()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent();

            uac.MinNumOfUnitsPerFloor = 1;
            uac.MaxNumOfUnitsPerFloor = 3;
            uac.NumOfUnitsPerFloor = 2;

            Assert.Equal(1, uac.MinNumOfUnitsPerFloor);
            Assert.Equal(2, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.5, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );

            uac.MinNumOfUnitsPerFloor = 3;

            Assert.Equal(3, uac.MinNumOfUnitsPerFloor);
            Assert.Equal(3, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.5, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );

            uac.MinNumOfUnitsPerFloor = 1;

            Assert.Equal(1, uac.MinNumOfUnitsPerFloor);
            Assert.Equal(3, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                5.0 / 6.0, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
        }

        [Fact]
        public void TestMinNumOfUnitsPerFloor_OutOfRange()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent();

            uac.MinNumOfUnitsPerFloor = 1;
            uac.MaxNumOfUnitsPerFloor = 3;
            uac.NumOfUnitsPerFloor = 2;

            Assert.Equal(1, uac.MinNumOfUnitsPerFloor);
            Assert.Equal(2, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.5, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MinNumOfUnitsPerFloor = -1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MinNumOfUnitsPerFloor = 0; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MinNumOfUnitsPerFloor = 5; }
            );

            Assert.Equal(1, uac.MinNumOfUnitsPerFloor);
            Assert.Equal(2, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.5, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
        }

        [Fact]
        public void TestMaxNumOfUnitsPerFloor()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent();

            uac.MinNumOfUnitsPerFloor = 1;
            uac.MaxNumOfUnitsPerFloor = 4;
            uac.NumOfUnitsPerFloor = 3;

            Assert.Equal(4, uac.MaxNumOfUnitsPerFloor);
            Assert.Equal(3, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.625, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );

            uac.MaxNumOfUnitsPerFloor = 1;

            Assert.Equal(1, uac.MaxNumOfUnitsPerFloor);
            Assert.Equal(1, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.5, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );

            uac.MaxNumOfUnitsPerFloor = 2;

            Assert.Equal(2, uac.MaxNumOfUnitsPerFloor);
            Assert.Equal(1, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.25, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
        }

        [Fact]
        public void TestMaxNumOfUnitsPerFloor_OutOfRange()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent();

            uac.MaxNumOfUnitsPerFloor = 5;
            uac.MinNumOfUnitsPerFloor = 2;
            uac.NumOfUnitsPerFloor = 4;

            Assert.Equal(5, uac.MaxNumOfUnitsPerFloor);
            Assert.Equal(4, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.625, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MaxNumOfUnitsPerFloor = -1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MaxNumOfUnitsPerFloor = 0; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.MaxNumOfUnitsPerFloor = 1; }
            );

            Assert.Equal(5, uac.MaxNumOfUnitsPerFloor);
            Assert.Equal(4, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.625, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
        }

        [Fact]
        public void TestNumOfUnitsPerFloor()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent
            {
                MinNumOfUnitsPerFloor = 1,
                MaxNumOfUnitsPerFloor = 5
            };

            uac.NumOfUnitsPerFloor = 1;
            Assert.Equal(1, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.1, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(
                new[]
                { UnitArrangerComponent.DefaultNormalizedEntryIndex },
                uac.NormalizedEntryIndices
            );

            uac.SetNormalizedEntryIndex(0, 0.1);

            uac.NumOfUnitsPerFloor = 5;
            Assert.Equal(5, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.9, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(
                new[]
                {
                    0.1,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex
                },
                uac.NormalizedEntryIndices
            );

            uac.NumOfUnitsPerFloor = 3;
            Assert.Equal(3, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.5, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(
                new[]
                {
                    0.1,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex
                },
                uac.NormalizedEntryIndices
            );
        }

        [Fact]
        public void TestNumOfUnitsPerFloor_OutOfRange()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent
            {
                MinNumOfUnitsPerFloor = 2,
                MaxNumOfUnitsPerFloor = 6
            };

            uac.NumOfUnitsPerFloor = 3;
            Assert.Equal(3, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.3, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            uac.SetNormalizedEntryIndex(0, 0.1);
            Assert.Equal(
                new[]
                {
                    0.1,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex
                },
                uac.NormalizedEntryIndices
            );

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NumOfUnitsPerFloor = -1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NumOfUnitsPerFloor = 0; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NumOfUnitsPerFloor = 1; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NumOfUnitsPerFloor = 7; }
            );

            Assert.Equal(3, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                0.3, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(
                new[]
                {
                    0.1,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex
                },
                uac.NormalizedEntryIndices
            );
        }

        [Fact]
        public void TestNormalizedNumOfUnitsPerFloor()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent
            {
                MinNumOfUnitsPerFloor = 1,
                MaxNumOfUnitsPerFloor = 5
            };

            uac.NormalizedNumOfUnitsPerFloor = 0.0;
            Assert.Equal(
                0.0, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(1, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                new[]
                { UnitArrangerComponent.DefaultNormalizedEntryIndex },
                uac.NormalizedEntryIndices
            );

            uac.SetNormalizedEntryIndex(0, 0.1);

            uac.NormalizedNumOfUnitsPerFloor = 1.0;
            Assert.Equal(
                1.0, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(5, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                new[]
                {
                    0.1,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex
                },
                uac.NormalizedEntryIndices
            );

            uac.NormalizedNumOfUnitsPerFloor = 0.45;
            Assert.Equal(
                0.45, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(3, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                new[]
                {
                    0.1,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex
                },
                uac.NormalizedEntryIndices
            );

            uac.MaxNumOfUnitsPerFloor = 4;
            Assert.Equal(
                0.625, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(3, uac.NumOfUnitsPerFloor);

            uac.MinNumOfUnitsPerFloor = 3;
            Assert.Equal(
                0.25, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(3, uac.NumOfUnitsPerFloor);
        }

        [Fact]
        public void TestNormalizedNumOfUnitsPerFloor_OutOfRange()
        {
            const int DecimalPlaces = 6;

            var uac = new UnitArrangerComponent
            {
                MinNumOfUnitsPerFloor = 1,
                MaxNumOfUnitsPerFloor = 5
            };

            uac.NormalizedNumOfUnitsPerFloor = 0.45;
            Assert.Equal(
                0.45, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(3, uac.NumOfUnitsPerFloor);
            uac.SetNormalizedEntryIndex(0, 0.1);
            Assert.Equal(
                new[]
                {
                    0.1,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex
                },
                uac.NormalizedEntryIndices
            );

            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NormalizedNumOfUnitsPerFloor = -0.2; }
            );
            Assert.ThrowsAny<ArgumentOutOfRangeException>(
                () => { uac.NormalizedNumOfUnitsPerFloor = 1.5; }
            );

            Assert.Equal(
                0.45, uac.NormalizedNumOfUnitsPerFloor, DecimalPlaces
            );
            Assert.Equal(3, uac.NumOfUnitsPerFloor);
            Assert.Equal(
                new[]
                {
                    0.1,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex,
                    UnitArrangerComponent.DefaultNormalizedEntryIndex
                },
                uac.NormalizedEntryIndices
            );
        }
    }
}
