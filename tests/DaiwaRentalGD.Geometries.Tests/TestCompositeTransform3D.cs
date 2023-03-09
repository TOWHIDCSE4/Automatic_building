using System;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestCompositeTransform3D
    {
        #region Constructors

        public TestCompositeTransform3D(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConstructorDefault()
        {
            CompositeTransform3D tf = new CompositeTransform3D();

            Assert.Empty(tf.Transforms);
            Assert.Equal(
                DenseMatrix.CreateIdentity(4),
                tf.Matrix
            );
        }

        [Fact]
        public void TestAdd()
        {
            CompositeTransform3D tf0 = new CompositeTransform3D();

            TrsTransform3D tf0_0 = new TrsTransform3D();
            TrsTransform3D tf0_1 = new TrsTransform3D();
            CompositeTransform3D tf0_2 = new CompositeTransform3D();

            tf0.Add(tf0_0);
            tf0.Add(tf0_1);
            tf0.Add(tf0_2);

            Assert.Equal(
                new ITransform3D[] { tf0_0, tf0_1, tf0_2 },
                tf0.Transforms
            );
        }

        [Fact]
        public void TestAdd_NullTransform()
        {
            CompositeTransform3D tf = new CompositeTransform3D();

            Assert.Throws<ArgumentNullException>(() => { tf.Add(null); });

            Assert.Empty(tf.Transforms);
        }

        [Fact]
        public void TestAdd_ThisCompositeTransform()
        {
            CompositeTransform3D tf = new CompositeTransform3D();

            Assert.ThrowsAny<ArgumentException>(() => { tf.Add(tf); });

            Assert.Empty(tf.Transforms);
        }

        [Fact]
        public void TestInsert()
        {
            CompositeTransform3D tf0 = new CompositeTransform3D();

            TrsTransform3D tf0_0 = new TrsTransform3D();
            TrsTransform3D tf0_1 = new TrsTransform3D();
            CompositeTransform3D tf0_2 = new CompositeTransform3D();

            tf0.Insert(0, tf0_0);
            tf0.Insert(1, tf0_1);
            tf0.Insert(1, tf0_2);

            Assert.Equal(
                new ITransform3D[] { tf0_0, tf0_2, tf0_1 },
                tf0.Transforms
            );
        }

        [Fact]
        public void TestInsert_InvalidIndex()
        {
            CompositeTransform3D tf0 = new CompositeTransform3D();

            TrsTransform3D tf0_0 = new TrsTransform3D();
            TrsTransform3D tf0_1 = new TrsTransform3D();
            CompositeTransform3D tf0_2 = new CompositeTransform3D();

            tf0.Add(tf0_0);
            tf0.Add(tf0_1);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => tf0.Insert(100, tf0_2)
            );

            Assert.Equal(
                new ITransform3D[] { tf0_0, tf0_1 },
                tf0.Transforms
            );
        }

        [Fact]
        public void TestRemove()
        {
            CompositeTransform3D tf0 = new CompositeTransform3D();

            TrsTransform3D tf0_0 = new TrsTransform3D();
            TrsTransform3D tf0_1 = new TrsTransform3D();
            CompositeTransform3D tf0_2 = new CompositeTransform3D();

            tf0.Add(tf0_0);
            tf0.Add(tf0_1);
            tf0.Add(tf0_2);

            tf0.Remove(1);

            Assert.Equal(
                new ITransform3D[] { tf0_0, tf0_2 },
                tf0.Transforms
            );

            tf0.Remove(0);

            Assert.Equal(
                new ITransform3D[] { tf0_2 },
                tf0.Transforms
            );
        }

        [Fact]
        public void TestRemove_InvalidIndex()
        {
            CompositeTransform3D tf0 = new CompositeTransform3D();

            TrsTransform3D tf0_0 = new TrsTransform3D();
            TrsTransform3D tf0_1 = new TrsTransform3D();
            CompositeTransform3D tf0_2 = new CompositeTransform3D();

            tf0.Add(tf0_0);
            tf0.Add(tf0_1);
            tf0.Add(tf0_2);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { tf0.Remove(10); }
            );

            Assert.Equal(
                new ITransform3D[] { tf0_0, tf0_1, tf0_2 },
                tf0.Transforms
            );
        }

        [Fact]
        public void TestTransform()
        {
            const int DecimalPlaces = 6;

            CompositeTransform3D tf = new CompositeTransform3D(
                new ITransform3D[] {

                    new TrsTransform3D { Tz = 1.0 },

                    new CompositeTransform3D(
                        new[] {

                            new TrsTransform3D { Rz = Math.PI / 3.0 },
                            new TrsTransform3D { Rz = Math.PI / 6.0 }
                        }
                    ),

                    new TrsTransform3D { Sy = 10.0 }
            });

            Vector<double> vector3D = new DenseVector(
                new[] { 1.2, 3.4, 5.6 }
            );

            Assert.True(Precision.AlmostEqual(

                new DenseVector(new[] { -3.4, 12.0, 6.6 }),

                tf.Transform(vector3D),

                DecimalPlaces
            ));
        }

        [Fact]
        public void TestTransform_NoTransform()
        {
            CompositeTransform3D tf = new CompositeTransform3D();

            Vector<double> vector3D =
                new DenseVector(new[] { 1.0, 2.0, 3.0 });

            Assert.Equal(vector3D, tf.Transform(vector3D));
        }

        [Theory]
        [MemberData(nameof(TestGetHashCodeAndEqualsAndOpsData))]
        public void TestGetHashCodeAndEqualsAndOps(
            CompositeTransform3D tf0, CompositeTransform3D tf1,
            bool equals
        )
        {
            Assert.Equal(tf0, tf0);
            Assert.Equal(tf0.GetHashCode(), tf0.GetHashCode());
            Assert.NotNull(tf0);

            Assert.True(tf0 == tf0);
            Assert.False(tf0 == null);
            Assert.False(null == tf0);

            Assert.False(tf0 != tf0);
            Assert.True(tf0 != null);
            Assert.True(null != tf0);

            if (equals)
            {
                Assert.Equal(tf0, tf1);
                Assert.Equal(tf1, tf0);
                Assert.Equal(tf0.GetHashCode(), tf1.GetHashCode());
                Assert.True(tf0 == tf1);
                Assert.True(tf1 == tf0);
                Assert.False(tf0 != tf1);
                Assert.False(tf1 != tf0);
            }
            else
            {
                Assert.NotEqual(tf0, tf1);
                Assert.NotEqual(tf1, tf0);
                Assert.NotEqual(tf0.GetHashCode(), tf1.GetHashCode());
                Assert.False(tf0 == tf1);
                Assert.False(tf1 == tf0);
                Assert.True(tf0 != tf1);
                Assert.True(tf1 != tf0);
            }
        }

        public static IEnumerable<object[]> TestGetHashCodeAndEqualsAndOpsData
        {
            get
            {
                CompositeTransform3D tf0 = new CompositeTransform3D();

                CompositeTransform3D tf1 = new CompositeTransform3D();

                CompositeTransform3D tf2 = new CompositeTransform3D(
                    new ITransform3D[]
                    {
                        new CompositeTransform3D(
                            new[]
                            {
                                new TrsTransform3D { Sy = 5.6 }
                            }
                        ),
                        new TrsTransform3D { Tx = 1.2 },
                        new TrsTransform3D { Rz = 3.4 }
                    }
                );

                CompositeTransform3D tf3 = new CompositeTransform3D(
                    new ITransform3D[]
                    {
                        new CompositeTransform3D(
                            new[]
                            {
                                new TrsTransform3D { Sy = 5.6 }
                            }
                        ),
                        new TrsTransform3D { Tx = 1.2 },
                        new TrsTransform3D { Rz = 3.4 }
                    }
                );

                CompositeTransform3D tf4 = new CompositeTransform3D(
                    new ITransform3D[]
                    {
                        new CompositeTransform3D(),
                        new TrsTransform3D { Tx = 0.12 },
                        new TrsTransform3D { Rz = 3.4 }
                    }
                );

                return new[]
                {
                    new object[] { tf0, tf1, true },
                    new object[] { tf2, tf3, true },
                    new object[] { tf2, tf4, false }
                };
            }
        }

        [Fact]
        public void TestMatrix()
        {
            const int DecimalPlaces = 6;

            CompositeTransform3D tf = new CompositeTransform3D(
                new ITransform3D[] {

                    new TrsTransform3D { Tz = 1.0 },

                    new CompositeTransform3D(
                        new[] {

                            new TrsTransform3D { Rz = Math.PI / 3.0 },
                            new TrsTransform3D { Rx = Math.PI / 6.0 }
                        }
                    ),

                    new TrsTransform3D { Sy = 10.0 }
            });

            Assert.True(Precision.AlmostEqual(

                TrsTransform3D.GetScaleMatrix(1.0, 10.0, 1.0) *
                TrsTransform3D.GetRotationMatrix(Math.PI / 6.0, 0.0, 0.0) *
                TrsTransform3D.GetRotationMatrix(0.0, 0.0, Math.PI / 3.0) *
                TrsTransform3D.GetTranslateMatrix(0.0, 0.0, 1.0),

                tf.Matrix,

                DecimalPlaces
            ));
        }

        [Fact]
        public void TestSerialization()
        {
            var transform = new CompositeTransform3D(
                new ITransform3D[] {
                    new TrsTransform3D { Tz = 1.0 },
                    new CompositeTransform3D(
                        new[] {

                            new TrsTransform3D { Rz = Math.PI / 3.0 },
                            new TrsTransform3D { Rz = Math.PI / 6.0 }
                        }
                    ),
                    new TrsTransform3D { Sy = 10.0 }
            });

            var transformItem =
                new HelperWorkspaceItem<CompositeTransform3D>(transform);

            var workspace = new JsonWorkspace();
            workspace.Save(transformItem);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedItem = workspaceCopy
                .Load<HelperWorkspaceItem<CompositeTransform3D>>(
                    transformItem.ItemInfo.Uid
                );

            Assert.Equal(transform, deserializedItem.Data);
        }

        #endregion

        #region Properties

        private ITestOutputHelper OutputHelper { get; }

        #endregion
    }
}
