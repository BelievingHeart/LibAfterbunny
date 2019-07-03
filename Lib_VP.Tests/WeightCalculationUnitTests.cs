using System;
using Lib_VP.Rectifier;
using NUnit.Framework;

namespace Lib_VP.Tests
{
    [TestFixture]
    public class WeightCalculationUnitTests
    {
        private WeightCalculationUnit CreateWeightCalculationUnit()
        {
            return new WeightCalculationUnit("Test");
        }

        [Test]
        public void AddToWeightToBeUpdatedList_Always_AddSucceed()
        {
            var unit = CreateWeightCalculationUnit();

            unit.AddToWeightToBeUpdatedList(new RectificationEntity("hello"));
            unit.AddToWeightToBeUpdatedList(new RectificationEntity("hello"));

            Assert.AreEqual(unit.Count4WeightUpdatingEntities, 2);
        }

        [Test]
        public void AddWeightCalculationEntity_WhenNameDuplicates_AddCanceled()
        {
            var unit = CreateWeightCalculationUnit();

            unit.AddWeightCalculationEntity(new RectificationEntity("hello"));
            unit.AddWeightCalculationEntity(new RectificationEntity("hello"));

            Assert.AreEqual(unit.Count4WeightCalculationEntities, 1);
        }

        [Test]
        public void AddWeightCalculationEntity_WhenNameNotDuplicates_AddSucceed()
        {
            var unit = CreateWeightCalculationUnit();

            unit.AddWeightCalculationEntity(new RectificationEntity("hello"));
            unit.AddWeightCalculationEntity(new RectificationEntity("world"));

            Assert.AreEqual(unit.Count4WeightCalculationEntities, 2);
        }

        [Test]
        public void Calculate_UsingMultipleCalculationEntities_CalculateProperWeight()
        {
            var unit = CreateWeightCalculationUnit();
            unit.AddWeightCalculationEntity(new RectificationEntity("X1")
                {PixelColumn = MeasuredData.X1_Pixel, MillimeterColumnOMM = OMMData.X1});
            unit.AddWeightCalculationEntity(new RectificationEntity("X2")
                {PixelColumn = MeasuredData.X2_Pixel, MillimeterColumnOMM = OMMData.X2});
            var weightExpected = Weights.X;

            unit.Calculate();
            var weightActual = unit.Weight;

            Assert.True(FloatingCompare.FloatingNumberAreClose(weightActual, weightExpected));
        }

        [Test]
        public void Calculate_UsingSingleCalculationEntity_CalculateProperWeight()
        {
            var unit = CreateWeightCalculationUnit();
            unit.AddWeightCalculationEntity(new RectificationEntity("Angle")
                {PixelColumn = MeasuredData.Angle, MillimeterColumnOMM = OMMData.Angle});
            var weightExpected = Weights.Angle;

            unit.Calculate();
            var weightActual = unit.Weight;

            Assert.True(FloatingCompare.FloatingNumberAreClose(weightActual, weightExpected));
        }

        [Test]
        public void Calculate_WhenCalculationEntitiesNull_ThrowsInvalidOperationException()
        {
            var unit = CreateWeightCalculationUnit();

            Assert.Catch<InvalidOperationException>(() => unit.Calculate());
        }
    }
}