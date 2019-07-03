using System;
using Lib_VP.Rectifier;
using NUnit.Framework;

namespace Lib_VP.Tests
{
    [TestFixture]
    public class RectificationEntityTests
    {
        private RectificationEntity CreateRectificationEntity(string name)
        {
            return new RectificationEntity(name);
        }

        [Test]
        public void CalculateWeight_MillimeterColumnOMMNull_ThrowsInvalidOperationException()
        {
            var rectificationEntity = CreateRectificationEntity("X1");
            rectificationEntity.PixelColumn = MeasuredData.X1_Pixel;

            Assert.Catch<InvalidOperationException>(() => rectificationEntity.CalculateWeight());
        }

        [Test]
        public void CalculateWeight_PixelColumnNull_ThrowsInvalidOperationException()
        {
            var rectificationEntity = CreateRectificationEntity("X1");
            rectificationEntity.MillimeterColumnOMM = OMMData.X1;

            Assert.Catch<InvalidOperationException>(() => rectificationEntity.CalculateWeight());
        }

        [Test]
        public void CalculateWeight_WhenTwoRequiredColumnsValid_ReturnProperWeight()
        {
            var rectificationEntity = CreateRectificationEntity("Angle");
            rectificationEntity.PixelColumn = MeasuredData.Angle;
            rectificationEntity.MillimeterColumnOMM = OMMData.Angle;
            var expected = Weights.Angle;

            var actual = rectificationEntity.CalculateWeight();

            Assert.True(FloatingCompare.FloatingNumberAreClose(actual, expected));
        }

        [Test]
        public void EstimateBiasedMillimeterDistances_MillimeterColumnOMMNull_ThrowsInvalidOperationException()
        {
            var rectificationEntity = CreateRectificationEntity("X1");
            rectificationEntity.PixelColumn = MeasuredData.X1_Pixel;
            rectificationEntity.Weight = Weights.X;

            Assert.Catch<InvalidOperationException>(() => rectificationEntity.EstimateBiasedMillimeterDistances());
        }

        [Test]
        public void EstimateBiasedMillimeterDistances_PixelColumnNull_ThrowsInvalidOperationException()
        {
            var rectificationEntity = CreateRectificationEntity("X1");
            rectificationEntity.MillimeterColumnOMM = OMMData.X1;
            rectificationEntity.Weight = Weights.X;

            Assert.Catch<InvalidOperationException>(() => rectificationEntity.EstimateBiasedMillimeterDistances());
        }

        [Test]
        public void EstimateBiasedMillimeterDistances_WhenTwoRequiredColumnsValid_CalcBiasProperly()
        {
            var rectificationEntity = CreateRectificationEntity("X1");
            rectificationEntity.PixelColumn = MeasuredData.X1_Pixel;
            rectificationEntity.MillimeterColumnOMM = OMMData.X1;
            rectificationEntity.Weight = Weights.X;
            var expectedBias = Biases.X1;
            var expectedEstimate = Estimates.X1;

            rectificationEntity.EstimateBiasedMillimeterDistances();
            var actualBias = rectificationEntity.Bias;
            var actualEstimate = rectificationEntity.MillimeterColumnEstimate;

            Assert.True(FloatingCompare.FloatingNumberAreClose(actualBias, expectedBias));
            Assert.True(FloatingCompare.FloatingEnumerablesAreVeryClose(actualEstimate, expectedEstimate));
        }
    }
}