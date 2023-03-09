using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Workspaces.Json;
using Xunit;
using Xunit.Abstractions;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestTrsTransform3D
    {
        #region Constructors

        public TestTrsTransform3D(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        #endregion

        #region Methods

        [Fact]
        public void TestConstructorAllComponents()
        {
            TrsTransform3D tf = new TrsTransform3D(
                1.0, 2.0, 3.0,
                0.4, 0.5, 0.6,
                7.0, 8.0, 9.0
            );

            Assert.Equal(1.0, tf.Tx);
            Assert.Equal(2.0, tf.Ty);
            Assert.Equal(3.0, tf.Tz);
            Assert.Equal(0.4, tf.Rx);
            Assert.Equal(0.5, tf.Ry);
            Assert.Equal(0.6, tf.Rz);
            Assert.Equal(7.0, tf.Sx);
            Assert.Equal(8.0, tf.Sy);
            Assert.Equal(9.0, tf.Sz);
        }

        [Fact]
        public void TestConstructorDefault()
        {
            TrsTransform3D tf = new TrsTransform3D();

            Assert.Equal(0.0, tf.Tx);
            Assert.Equal(0.0, tf.Ty);
            Assert.Equal(0.0, tf.Tz);
            Assert.Equal(0.0, tf.Rx);
            Assert.Equal(0.0, tf.Ry);
            Assert.Equal(0.0, tf.Rz);
            Assert.Equal(1.0, tf.Sx);
            Assert.Equal(1.0, tf.Sy);
            Assert.Equal(1.0, tf.Sz);
        }

        [Fact]
        public void TestConstructorCopy()
        {
            TrsTransform3D tf0 = new TrsTransform3D(
                1.0, 2.0, 3.0,
                0.4, 0.5, 0.6,
                7.0, 8.0, 9.0
            );

            TrsTransform3D tf1 = new TrsTransform3D(tf0);

            Assert.Equal(tf0.Tx, tf1.Tx);
            Assert.Equal(tf0.Ty, tf1.Ty);
            Assert.Equal(tf0.Tz, tf1.Tz);
            Assert.Equal(tf0.Rx, tf1.Rx);
            Assert.Equal(tf0.Ry, tf1.Ry);
            Assert.Equal(tf0.Rz, tf1.Rz);
            Assert.Equal(tf0.Sx, tf1.Sx);
            Assert.Equal(tf0.Sy, tf1.Sy);
            Assert.Equal(tf0.Sz, tf1.Sz);
        }

        [Fact]
        public void TestGetTranslateMatrix()
        {
            Assert.Equal(

                new DenseMatrix(
                    4, 4,
                    new[]
                    {
                        1.0, 0.0, 0.0, 0.0,
                        0.0, 1.0, 0.0, 0.0,
                        0.0, 0.0, 1.0, 0.0,
                        1.0, 2.0, 3.0, 1.0
                    }
                ),

                TrsTransform3D.GetTranslateMatrix(1.0, 2.0, 3.0)
            );
        }

        [Fact]
        public void TestGetRotationMatrix()
        {
            const int DecimalPlaces = 6;

            double rx = Math.PI / 4.0;

            Assert.True(Precision.AlmostEqual(

                new DenseMatrix(
                    4, 4,
                    new[]
                    {
                        1.0, 0.0, 0.0, 0.0,
                        0.0, Math.Cos(rx), Math.Sin(rx), 0.0,
                        0.0, -Math.Sin(rx), Math.Cos(rx), 0.0,
                        0.0, 0.0, 0.0, 1.0
                    }
                ),

                TrsTransform3D.GetRotationMatrix(rx, 0.0, 0.0),

                DecimalPlaces
            ));

            double ry = Math.PI / 6.0;

            Assert.True(Precision.AlmostEqual(

                new DenseMatrix(
                    4, 4,
                    new[]
                    {
                        Math.Cos(ry), 0.0, Math.Sin(ry), 0.0,
                        0.0, 1.0, 0.0, 0.0,
                        -Math.Sin(ry), 0.0, Math.Cos(ry), 0.0,
                        0.0, 0.0, 0.0, 1.0
                    }
                ),

                TrsTransform3D.GetRotationMatrix(0.0, ry, 0.0),

                DecimalPlaces
            ));

            double rz = Math.PI / 3.0;

            Assert.True(Precision.AlmostEqual(

                new DenseMatrix(
                    4, 4,
                    new[]
                    {
                        Math.Cos(rz), Math.Sin(rz), 0.0, 0.0,
                        -Math.Sin(rz), Math.Cos(rz), 0.0, 0.0,
                        0.0, 0.0, 1.0, 0.0,
                        0.0, 0.0, 0.0, 1.0
                    }
                ),

                TrsTransform3D.GetRotationMatrix(0.0, 0.0, rz),

                DecimalPlaces
            ));

            Assert.True(Precision.AlmostEqual(

                TrsTransform3D.GetRotationMatrix(rx, 0.0, 0.0) *
                TrsTransform3D.GetRotationMatrix(0.0, ry, 0.0) *
                TrsTransform3D.GetRotationMatrix(0.0, 0.0, rz),

                TrsTransform3D.GetRotationMatrix(rx, ry, rz),

                DecimalPlaces
            ));
        }

        [Fact]
        public void TestGetScaleMatrix()
        {
            double sx = 2.0;
            double sy = 3.0;
            double sz = 4.0;

            Assert.Equal(

                new DenseMatrix(
                    4, 4,
                    new[]
                    {
                        sx, 0.0, 0.0, 0.0,
                        0.0, sy, 0.0, 0.0,
                        0.0, 0.0, sz, 0.0,
                        0.0, 0.0, 0.0, 1.0
                    }
                ),

                TrsTransform3D.GetScaleMatrix(sx, sy, sz)
            );
        }

        [Fact]
        public void TestMatrix()
        {
            const int DecimalPlaces = 6;

            TrsTransform3D tf = new TrsTransform3D(
                1.0, 2.0, 3.0,
                0.4, 0.5, 0.6,
                7.0, 8.0, 9.0
            );

            Assert.True(Precision.AlmostEqual(

                TrsTransform3D.GetTranslateMatrix(1.0, 2.0, 3.0) *
                TrsTransform3D.GetRotationMatrix(0.4, 0.5, 0.6) *
                TrsTransform3D.GetScaleMatrix(7.0, 8.0, 9.0),

                tf.Matrix,

                DecimalPlaces
            ));
        }

        [Fact]
        public void TestTransform()
        {
            const int DecimalPlaces = 6;

            Vector<double> vector3D =
                new DenseVector(new[] { 1.0, 2.0, 3.0 });

            ITransform3D tf0 = new TrsTransform3D
            { Tx = 0.1, Ty = 0.2, Tz = 0.3 };

            Assert.True(Precision.AlmostEqual(

                new DenseVector(new[] { 1.1, 2.2, 3.3 }),

                tf0.Transform(vector3D),

                DecimalPlaces
            ));

            ITransform3D tf1 = new TrsTransform3D
            { Rx = Math.PI / 2.0, Ry = 0.0, Rz = Math.PI };

            Assert.True(Precision.AlmostEqual(

                new DenseVector(new[] { -1.0, -3.0, -2.0 }),

                tf1.Transform(vector3D),

                DecimalPlaces
            ));

            ITransform3D tf2 = new TrsTransform3D
            { Sx = 10.0, Sy = 20.0, Sz = 30.0 };

            Assert.True(Precision.AlmostEqual(

                new DenseVector(new[] { 10.0, 40.0, 90.0 }),

                tf2.Transform(vector3D),

                DecimalPlaces
            ));

            TrsTransform3D tf3 = new TrsTransform3D(
                1.0, 2.0, 3.0,
                0.4, 0.5, 0.6,
                7.0, 8.0, 9.0
            );

            Vector<double> vector4D = new DenseVector(
                new[] { vector3D[0], vector3D[1], vector3D[2], 1.0 }
            );

            Assert.True(Precision.AlmostEqual(

                (tf3.Matrix * vector4D).SubVector(0, 3),
                tf3.Transform(vector3D),
                DecimalPlaces
            ));
        }

        [Theory]
        [InlineData(
            0.0, 0.0, 0.0, 0.1, 0.2, 0.3, 4.0, 5.0, 6.0,
            0.0, 0.0, 0.0, 0.1, 0.2, 0.3, 4.0, 5.0, 6.0,
            true
        )]
        [InlineData(
            0.0, 0.0, 0.0, 0.1, 0.2, 0.3, 4.0, 5.0, 6.0,
            0.0, 1.0, 2.0, 0.1, 0.2, 0.3, 4.6, 5.7, 6.8,
            false
        )]
        public void TestGetHashCodeAndEqualsAndOps(

            double tx0, double ty0, double tz0,
            double rx0, double ry0, double rz0,
            double sx0, double sy0, double sz0,

            double tx1, double ty1, double tz1,
            double rx1, double ry1, double rz1,
            double sx1, double sy1, double sz1,

            bool equals
        )
        {
            TrsTransform3D tf0 = new TrsTransform3D
            {
                Tx = tx0,
                Ty = ty0,
                Tz = tz0,
                Rx = rx0,
                Ry = ry0,
                Rz = rz0,
                Sx = sx0,
                Sy = sy0,
                Sz = sz0
            };
            TrsTransform3D tf1 = new TrsTransform3D
            {
                Tx = tx1,
                Ty = ty1,
                Tz = tz1,
                Rx = rx1,
                Ry = ry1,
                Rz = rz1,
                Sx = sx1,
                Sy = sy1,
                Sz = sz1
            };

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

        [Fact]
        public void TestSerialization()
        {
            var transform = new TrsTransform3D(
                1.0, 2.0, 3.0,
                0.4, 0.5, 0.6,
                7.0, 8.0, 9.0
            );

            var transformItem =
                new HelperWorkspaceItem<TrsTransform3D>(transform);

            var workspace = new JsonWorkspace();
            workspace.Save(transformItem);

            OutputHelper.WriteLine(workspace.ToJson());

            var workspaceCopy = JsonWorkspace.FromJson(workspace.ToJson());

            var deserializedItem =
                workspaceCopy.Load<HelperWorkspaceItem<TrsTransform3D>>(
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
