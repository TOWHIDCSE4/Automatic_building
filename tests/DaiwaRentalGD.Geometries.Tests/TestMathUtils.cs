using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Xunit;

namespace DaiwaRentalGD.Geometries.Tests
{
    public class TestMathUtils
    {
        [Fact]
        public void TestCrossProduct()
        {
            const int DecimalPlaces = 6;

            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 0.0, 0.0, 6.0 }),
                MathUtils.CrossProduct(
                    new DenseVector(new[] { 2.0, 0.0, 0.0 }),
                    new DenseVector(new[] { 1.0, 3.0, 0.0 })
                ),
                DecimalPlaces
            ));

            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { -6.0, 0.0, 0.0 }),
                MathUtils.CrossProduct(
                    new DenseVector(new[] { 0.0, 1.0, 3.0 }),
                    new DenseVector(new[] { 0.0, 2.0, 0.0 })
                ),
                DecimalPlaces
            ));

            Assert.True(Precision.AlmostEqual(
                new DenseVector(new[] { 0.0, 0.0, 0.0 }),
                MathUtils.CrossProduct(
                    new DenseVector(new[] { 2.0, 0.0, 0.0 }),
                    new DenseVector(new[] { 2.0, 0.0, 0.0 })
                ),
                DecimalPlaces
            ));
        }

        [Fact]
        public void TestGetAngle2D()
        {
            const int DecimalPlaces = 6;

            // 0 degrees from +X axis
            Vector<double> v0 = new DenseVector(new[] { 2.0, 0.0, 0.0 });

            // 45 degrees from +X axis
            Vector<double> v1 = new DenseVector(new[] { 1.5, 1.5, 0.0 });

            // 90 degrees from +X axis
            Vector<double> v2 = new DenseVector(new[] { 0.0, 1.0, 0.0 });

            // 150 degrees from +X axis
            Vector<double> v3 =
                new DenseVector(new[] { -Math.Sqrt(3.0), 1.0, 0.0 });

            // 180 degrees from +X axis
            Vector<double> v4 = new DenseVector(new[] { -5.0, 0.0, 0.0 });

            // 225 degrees from +X axis
            Vector<double> v5 = new DenseVector(new[] { -1.0, -1.0, 0.0 });

            // 300 (or -60) degrees from +X axis
            Vector<double> v6 =
                new DenseVector(new[] { 1.0, -Math.Sqrt(3.0), 0.0 });

            Assert.Equal(
                0.0,
                MathUtils.GetAngle2D(v1, v1),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI / 4.0,
                MathUtils.GetAngle2D(v0, v1),
                DecimalPlaces
            );
            Assert.Equal(
                -Math.PI / 4.0,
                MathUtils.GetAngle2D(v1, v0),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI / 4.0,
                MathUtils.GetAngle2D(v1, v2),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI / 180.0 * 105.0,
                MathUtils.GetAngle2D(v1, v3),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI,
                MathUtils.GetAngle2D(v0, v4),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI / 180.0 * 135.0,
                MathUtils.GetAngle2D(v2, v5),
                DecimalPlaces
            );
            Assert.Equal(
                Math.PI,
                MathUtils.GetAngle2D(v1, v5),
                DecimalPlaces
            );
            Assert.Equal(
                -Math.PI / 180.0 * 105.0,
                MathUtils.GetAngle2D(v1, v6),
                DecimalPlaces
            );
        }

        [Theory]
        // maxInclusive == minInclusive
        [InlineData(3, 3, 3, 0.0, 1.0, 0.5)]
        [InlineData(2, 2, 2, 1.2, 2.4, 1.8)]
        // maxInclusive == minInclusive + 1
        [InlineData(3, 3, 4, 0.0, 1.0, 0.25)]
        [InlineData(4, 3, 4, 0.0, 1.0, 0.75)]
        [InlineData(3, 3, 4, 1.0, 2.0, 1.25)]
        [InlineData(4, 3, 4, 1.0, 2.0, 1.75)]
        // maxInclusive == minInclusive + 2
        [InlineData(20, 20, 22, 0.0, 1.0, 1.0 / 6.0)]
        [InlineData(21, 20, 22, 0.0, 1.0, 3.0 / 6.0)]
        [InlineData(22, 20, 22, 0.0, 1.0, 5.0 / 6.0)]
        [InlineData(20, 20, 22, 1.0, 2.0, 1.0 + 1.0 / 6.0)]
        [InlineData(21, 20, 22, 1.0, 2.0, 1.0 + 3.0 / 6.0)]
        [InlineData(22, 20, 22, 1.0, 2.0, 1.0 + 5.0 / 6.0)]
        public void TestMapIntToDouble(
            int value, int minInclusive, int maxInclusive,
            double targetMinInclusive, double targetMaxInclusive,
            double expectedTargetValue
        )
        {
            const int DecimalPlaces = 6;

            double targetValue = MathUtils.MapIntToDouble(
                value, minInclusive, maxInclusive,
                targetMinInclusive, targetMaxInclusive
            );

            Assert.Equal(expectedTargetValue, targetValue, DecimalPlaces);
        }

        [Theory]
        // targetMaxInclusive == targetMinInclusive
        [InlineData(2.0, 2.0, 4.0, 10, 10, 10)]
        [InlineData(2.5, 2.0, 4.0, 10, 10, 10)]
        [InlineData(3.0, 2.0, 4.0, 10, 10, 10)]
        [InlineData(3.5, 2.0, 4.0, 10, 10, 10)]
        [InlineData(4.0, 2.0, 4.0, 10, 10, 10)]
        // targetMaxInclusive == targetMinInclusive + 1
        [InlineData(2.0, 2.0, 4.0, 10, 11, 10)]
        [InlineData(2.5, 2.0, 4.0, 10, 11, 10)]
        [InlineData(3.0 - 1e-6, 2.0, 4.0, 10, 11, 10)]
        [InlineData(3.0 + 1e-6, 2.0, 4.0, 10, 11, 11)]
        [InlineData(3.5, 2.0, 4.0, 10, 11, 11)]
        [InlineData(4.0, 2.0, 4.0, 10, 11, 11)]
        // targetMaxInclusive == targetMinInclusive + 2
        [InlineData(1.0, 1.0, 3.0, 10, 12, 10)]
        [InlineData(1.5, 1.0, 3.0, 10, 12, 10)]
        [InlineData(1.0 + 2.0 / 3.0 - 1e-6, 1.0, 3.0, 10, 12, 10)]
        [InlineData(1.0 + 2.0 / 3.0 + 1e-6, 1.0, 3.0, 10, 12, 11)]
        [InlineData(2.0, 1.0, 3.0, 10, 12, 11)]
        [InlineData(2.0 + 1.0 / 3.0 - 1e-6, 1.0, 3.0, 10, 12, 11)]
        [InlineData(2.0 + 1.0 / 3.0 + 1e-6, 1.0, 3.0, 10, 12, 12)]
        [InlineData(2.5, 1.0, 3.0, 10, 12, 12)]
        [InlineData(3.0, 1.0, 3.0, 10, 12, 12)]
        public void TestMapDoubleToInt(
            double value, double minInclusive, double maxInclusive,
            int targetMinInclusive, int targetMaxInclusive,
            int expectedTargetValue
        )
        {
            int targetValue = MathUtils.MapDoubleToInt(
                value, minInclusive, maxInclusive,
                targetMinInclusive, targetMaxInclusive
            );

            Assert.Equal(expectedTargetValue, targetValue);
        }
    }
}
