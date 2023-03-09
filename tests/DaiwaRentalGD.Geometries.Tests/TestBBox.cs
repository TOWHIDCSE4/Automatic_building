using System;
using System.Collections.Generic;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestBBox
    {
        #region Constructors

        public TestBBox(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConsturctor()
        {
            BBox bbox = new BBox();

            Assert.Equal(0.0, bbox.MinX);
            Assert.Equal(0.0, bbox.MinY);
            Assert.Equal(0.0, bbox.MinZ);

            Assert.Equal(0.0, bbox.MaxX);
            Assert.Equal(0.0, bbox.MaxY);
            Assert.Equal(0.0, bbox.MaxZ);

            Assert.Equal(0.0, bbox.SizeX);
            Assert.Equal(0.0, bbox.SizeY);
            Assert.Equal(0.0, bbox.SizeZ);
        }

        [Fact]
        public void TestFromMinAndMax()
        {
            const int DecimalPlaces = 6;

            BBox bbox = BBox.FromMinAndMax(
                1.2, 3.4, 5.6,
                7.0, 8.0, 9.0
            );

            Assert.Equal(1.2, bbox.MinX, DecimalPlaces);
            Assert.Equal(3.4, bbox.MinY, DecimalPlaces);
            Assert.Equal(5.6, bbox.MinZ, DecimalPlaces);

            Assert.Equal(7.0, bbox.MaxX, DecimalPlaces);
            Assert.Equal(8.0, bbox.MaxY, DecimalPlaces);
            Assert.Equal(9.0, bbox.MaxZ, DecimalPlaces);

            Assert.Equal(5.8, bbox.SizeX, DecimalPlaces);
            Assert.Equal(4.6, bbox.SizeY, DecimalPlaces);
            Assert.Equal(3.4, bbox.SizeZ, DecimalPlaces);
        }

        [Fact]
        public void TestFromMinAndMax_InvalidArgs()
        {
            Assert.ThrowsAny<ArgumentException>(
                () =>
                {
                    BBox.FromMinAndMax(
                        1.2, 3.4, 5.6,
                        0.7, 8.0, 9.0
                    );
                }
            );

            Assert.ThrowsAny<ArgumentException>(
                () =>
                {
                    BBox.FromMinAndMax(
                        1.2, 34.0, 5.6,
                        0.7, 8.0, 9.0
                    );
                }
            );

            Assert.ThrowsAny<ArgumentException>(
                () =>
                {
                    BBox.FromMinAndMax(
                        1.2, 3.4, 56.0,
                        0.7, 8.0, 9.0
                    );
                }
            );
        }

        [Fact]
        public void TestFromMinAndSize()
        {
            const int DecimalPlaces = 6;

            BBox bbox = BBox.FromMinAndSize(
                1.2, 3.4, 5.6,
                7.0, 8.0, 9.0
            );

            Assert.Equal(1.2, bbox.MinX, DecimalPlaces);
            Assert.Equal(3.4, bbox.MinY, DecimalPlaces);
            Assert.Equal(5.6, bbox.MinZ, DecimalPlaces);

            Assert.Equal(8.2, bbox.MaxX, DecimalPlaces);
            Assert.Equal(11.4, bbox.MaxY, DecimalPlaces);
            Assert.Equal(14.6, bbox.MaxZ, DecimalPlaces);

            Assert.Equal(7.0, bbox.SizeX, DecimalPlaces);
            Assert.Equal(8.0, bbox.SizeY, DecimalPlaces);
            Assert.Equal(9.0, bbox.SizeZ, DecimalPlaces);
        }

        [Fact]
        public void TestFromMinAndSize_InvalidArgs()
        {
            Assert.ThrowsAny<ArgumentException>(
                () =>
                {
                    BBox.FromMinAndSize(
                        1.2, 3.4, 5.6,
                        -1.0, 8.0, 9.0
                    );
                }
            );
            Assert.ThrowsAny<ArgumentException>(
                () =>
                {
                    BBox.FromMinAndSize(
                        1.2, 3.4, 5.6,
                        7.0, -1.0, 9.0
                    );
                }
            );
            Assert.ThrowsAny<ArgumentException>(
                () =>
                {
                    BBox.FromMinAndSize(
                        1.2, 3.4, 5.6,
                        7.0, 8.0, -1.0
                    );
                }
            );
        }

        [Fact]
        public void TestGetBBox()
        {
            BBox bbox = new BBox();

            Assert.Equal(bbox, bbox.GetBBox());
        }

        [Fact]
        public void TestSetMinMax()
        {
            const int DecimalPlaces = 6;

            BBox bbox = new BBox();

            bbox.SetMinXMaxX(1.0, 2.0);

            Assert.Equal(1.0, bbox.MinX, DecimalPlaces);
            Assert.Equal(2.0, bbox.MaxX, DecimalPlaces);

            bbox.SetMinYMaxY(3.0, 4.0);

            Assert.Equal(3.0, bbox.MinY, DecimalPlaces);
            Assert.Equal(4.0, bbox.MaxY, DecimalPlaces);

            bbox.SetMinZMaxZ(5.0, 6.0);

            Assert.Equal(5.0, bbox.MinZ, DecimalPlaces);
            Assert.Equal(6.0, bbox.MaxZ, DecimalPlaces);
        }

        [Fact]
        public void TestSetMinMax_InvalidArgs()
        {
            const int DecimalPlaces = 6;

            BBox bbox = BBox.FromMinAndMax(1.2, 3.4, 5.6, 7.0, 8.0, 9.0);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.SetMinXMaxX(1.0, 0.5); }
            );
            Assert.Equal(1.2, bbox.MinX, DecimalPlaces);
            Assert.Equal(7.0, bbox.MaxX, DecimalPlaces);
            Assert.Equal(5.8, bbox.SizeX, DecimalPlaces);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.SetMinYMaxY(1.0, 0.5); }
            );
            Assert.Equal(3.4, bbox.MinY, DecimalPlaces);
            Assert.Equal(8.0, bbox.MaxY, DecimalPlaces);
            Assert.Equal(4.6, bbox.SizeY, DecimalPlaces);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.SetMinZMaxZ(1.0, 0.5); }
            );
            Assert.Equal(5.6, bbox.MinZ, DecimalPlaces);
            Assert.Equal(9.0, bbox.MaxZ, DecimalPlaces);
            Assert.Equal(3.4, bbox.SizeZ, DecimalPlaces);
        }

        [Fact]
        public void TestGetBBox_IBBoxes()
        {
            const int DecimalPlaces = 6;

            BBox bbox0_0 = BBox.FromMinAndMax(1.0, 2.0, 3.0, 4.0, 5.0, 6.0);
            BBox bbox0_1 = BBox.FromMinAndMax(1.2, -2.0, 3.5, 40.0, 5.0, 5.5);
            BBox bbox0_2 = BBox.FromMinAndMax(2.0, 3.0, 9.0, 2.0, 5.0, 16.0);

            BBox bbox0 = BBox.GetBBox(new[] { bbox0_0, bbox0_1, bbox0_2 });

            Assert.Equal(1.0, bbox0.MinX, DecimalPlaces);
            Assert.Equal(-2.0, bbox0.MinY, DecimalPlaces);
            Assert.Equal(3.0, bbox0.MinZ, DecimalPlaces);

            Assert.Equal(40.0, bbox0.MaxX, DecimalPlaces);
            Assert.Equal(5.0, bbox0.MaxY, DecimalPlaces);
            Assert.Equal(16.0, bbox0.MaxZ, DecimalPlaces);

            BBox bbox1 = BBox.GetBBox(new BBox[] { });

            Assert.Equal(0.0, bbox1.MinX, DecimalPlaces);
            Assert.Equal(0.0, bbox1.MinY, DecimalPlaces);
            Assert.Equal(0.0, bbox1.MinZ, DecimalPlaces);

            Assert.Equal(0.0, bbox1.MaxX, DecimalPlaces);
            Assert.Equal(0.0, bbox1.MaxY, DecimalPlaces);
            Assert.Equal(0.0, bbox1.MaxZ, DecimalPlaces);
        }

        [Fact]
        public void TestMinMaxSize()
        {
            const int DecimalPlaces = 6;

            BBox bbox = new BBox();

            bbox.MinX = -1.0;
            bbox.MaxX = 2.0;
            Assert.Equal(-1.0, bbox.MinX, DecimalPlaces);
            Assert.Equal(2.0, bbox.MaxX, DecimalPlaces);
            Assert.Equal(3.0, bbox.SizeX, DecimalPlaces);

            bbox.SizeX = 4.0;
            Assert.Equal(-1.0, bbox.MinX, DecimalPlaces);
            Assert.Equal(3.0, bbox.MaxX, DecimalPlaces);
            Assert.Equal(4.0, bbox.SizeX, DecimalPlaces);

            bbox.MinY = -10.0;
            bbox.MaxY = 20.0;
            Assert.Equal(-10.0, bbox.MinY, DecimalPlaces);
            Assert.Equal(20.0, bbox.MaxY, DecimalPlaces);
            Assert.Equal(30.0, bbox.SizeY, DecimalPlaces);

            bbox.SizeY = 40.0;
            Assert.Equal(-10.0, bbox.MinY, DecimalPlaces);
            Assert.Equal(30.0, bbox.MaxY, DecimalPlaces);
            Assert.Equal(40.0, bbox.SizeY, DecimalPlaces);

            bbox.MinZ = -100.0;
            bbox.MaxZ = 200.0;
            Assert.Equal(-100.0, bbox.MinZ, DecimalPlaces);
            Assert.Equal(200.0, bbox.MaxZ, DecimalPlaces);
            Assert.Equal(300.0, bbox.SizeZ, DecimalPlaces);

            bbox.SizeZ = 400.0;
            Assert.Equal(-100.0, bbox.MinZ, DecimalPlaces);
            Assert.Equal(300.0, bbox.MaxZ, DecimalPlaces);
            Assert.Equal(400.0, bbox.SizeZ, DecimalPlaces);
        }

        [Fact]
        public void TestGetCenter()
        {
            BBox bbox = BBox.FromMinAndMax(
                1.0, 2.0, 3.0,
                10.0, 20.0, 30.0
            );

            Assert.Equal(
                new Point(5.5, 11.0, 16.5),
                bbox.GetCenter()
            );
        }

        [Fact]
        public void TestOverlapAndOverlapInterior()
        {
            BBox bbox0 = BBox.FromMinAndMax(-1.0, -2.0, -3.0, 1.0, 2.0, 3.0);
            BBox bbox1 = BBox.FromMinAndMax(0.5, 0.6, 0.7, 1.2, 2.3, 3.4);
            BBox bbox2 = BBox.FromMinAndMax(0.5, 0.6, 3.7, 1.2, 2.3, 5.4);
            BBox bbox3 = BBox.FromMinAndMax(1.0, 0.6, 0.7, 1.5, 2.3, 3.4);
            BBox bbox4 = BBox.FromMinAndMax(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

            Assert.True(BBox.DoesOverlap(bbox0, bbox0));
            Assert.True(BBox.DoesOverlapInterior(bbox0, bbox0));

            Assert.True(BBox.DoesOverlap(bbox4, bbox4));
            Assert.False(BBox.DoesOverlapInterior(bbox4, bbox4));

            Assert.True(BBox.DoesOverlap(bbox0, bbox1));
            Assert.True(BBox.DoesOverlapInterior(bbox0, bbox1));
            Assert.True(BBox.DoesOverlap(bbox1, bbox0));
            Assert.True(BBox.DoesOverlapInterior(bbox1, bbox0));

            Assert.False(BBox.DoesOverlap(bbox0, bbox2));
            Assert.False(BBox.DoesOverlapInterior(bbox0, bbox2));
            Assert.False(BBox.DoesOverlap(bbox2, bbox0));
            Assert.False(BBox.DoesOverlapInterior(bbox2, bbox0));

            Assert.True(BBox.DoesOverlap(bbox0, bbox3));
            Assert.False(BBox.DoesOverlapInterior(bbox0, bbox3));
            Assert.True(BBox.DoesOverlap(bbox3, bbox0));
            Assert.False(BBox.DoesOverlapInterior(bbox3, bbox0));

            Assert.True(BBox.DoesOverlap(bbox0, bbox4));
            Assert.True(BBox.DoesOverlapInterior(bbox0, bbox4));
            Assert.True(BBox.DoesOverlap(bbox4, bbox0));
            Assert.True(BBox.DoesOverlapInterior(bbox4, bbox0));
        }

        [Theory]
        [MemberData(nameof(TestGetHashCodeAndEqualsAndOpsData))]
        public void TestGetHashCodeAndEqualsAndOps(
            BBox bbox0, BBox bbox1, bool equals
        )
        {
            Assert.Equal(bbox0, bbox0);
            Assert.Equal(bbox0.GetHashCode(), bbox0.GetHashCode());
            Assert.NotNull(bbox0);

            Assert.True(bbox0 == bbox0);
            Assert.False(bbox0 == null);
            Assert.False(null == bbox0);

            Assert.False(bbox0 != bbox0);
            Assert.True(bbox0 != null);
            Assert.True(null != bbox0);

            if (equals)
            {
                Assert.Equal(bbox0, bbox1);
                Assert.Equal(bbox1, bbox0);
                Assert.Equal(bbox0.GetHashCode(), bbox1.GetHashCode());
                Assert.True(bbox0 == bbox1);
                Assert.True(bbox1 == bbox0);
                Assert.False(bbox0 != bbox1);
                Assert.False(bbox1 != bbox0);
            }
            else
            {
                Assert.NotEqual(bbox0, bbox1);
                Assert.NotEqual(bbox1, bbox0);
                Assert.NotEqual(bbox0.GetHashCode(), bbox1.GetHashCode());
                Assert.False(bbox0 == bbox1);
                Assert.False(bbox1 == bbox0);
                Assert.True(bbox0 != bbox1);
                Assert.True(bbox1 != bbox0);
            }
        }

        public static IEnumerable<object[]> TestGetHashCodeAndEqualsAndOpsData
        {
            get
            {
                BBox bbox0 = new BBox();

                BBox bbox1 = new BBox();

                BBox bbox2 = BBox.FromMinAndMax(
                    0.1, 2.3, 4.5,
                    6.7, 8.9, 10.0
                );

                BBox bbox3 = BBox.FromMinAndMax(
                    0.1, 2.3, 4.5,
                    6.7, 8.9, 10.0
                );

                BBox bbox4 = BBox.FromMinAndMax(
                    1.0 + 0.1, 1.0 + 2.3, 1.0 + 4.5,
                    1.0 + 6.7, 1.0 + 8.9, 1.0 + 10.0
                );

                return new[]
                {
                    new object[] { bbox0, bbox1, true },
                    new object[] { bbox2, bbox3, true },
                    new object[] { bbox2, bbox4, false }
                };
            }
        }

        [Fact]
        public void TestMinMaxSize_InvalidValiues()
        {
            const int DecimalPlaces = 6;

            BBox bbox = BBox.FromMinAndMax(1.2, 3.4, 5.6, 7.0, 8.0, 9.0);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.MinX = 100.0; }
            );
            Assert.Equal(1.2, bbox.MinX, DecimalPlaces);
            Assert.Equal(5.8, bbox.SizeX, DecimalPlaces);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.MinY = 100.0; }
            );
            Assert.Equal(3.4, bbox.MinY, DecimalPlaces);
            Assert.Equal(4.6, bbox.SizeY, DecimalPlaces);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.MinZ = 100.0; }
            );
            Assert.Equal(5.6, bbox.MinZ, DecimalPlaces);
            Assert.Equal(3.4, bbox.SizeZ, DecimalPlaces);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.MaxX = -100.0; }
            );
            Assert.Equal(7.0, bbox.MaxX, DecimalPlaces);
            Assert.Equal(5.8, bbox.SizeX, DecimalPlaces);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.MaxY = -100.0; }
            );
            Assert.Equal(8.0, bbox.MaxY, DecimalPlaces);
            Assert.Equal(4.6, bbox.SizeY, DecimalPlaces);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.MaxZ = -100.0; }
            );
            Assert.Equal(9.0, bbox.MaxZ, DecimalPlaces);
            Assert.Equal(3.4, bbox.SizeZ, DecimalPlaces);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.SizeX = -1.0; }
            );
            Assert.Equal(7.0, bbox.MaxX, DecimalPlaces);
            Assert.Equal(5.8, bbox.SizeX, DecimalPlaces);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.SizeY = -1.0; }
            );
            Assert.Equal(8.0, bbox.MaxY, DecimalPlaces);
            Assert.Equal(4.6, bbox.SizeY, DecimalPlaces);

            Assert.ThrowsAny<ArgumentException>(
                () => { bbox.SizeZ = -1.0; }
            );
            Assert.Equal(9.0, bbox.MaxZ, DecimalPlaces);
            Assert.Equal(3.4, bbox.SizeZ, DecimalPlaces);
        }

        [Fact]
        public void TestSerialization()
        {
            var bbox = new BBox();

            bbox.SetMinXMaxX(0.0, 1.0);
            bbox.SetMinYMaxY(2.0, 3.0);
            bbox.SetMinZMaxZ(4.0, 5.0);

            var bboxItem = new HelperWorkspaceItem<BBox>(bbox);

            var workspace = new JsonWorkspace();
            workspace.Save(bboxItem);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedItem =
                workspaceCopy.Load<HelperWorkspaceItem<BBox>>(
                    bboxItem.ItemInfo.Uid
                );

            Assert.Equal(bbox, deserializedItem.Data);
        }

        #endregion

        #region Properties

        public ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
