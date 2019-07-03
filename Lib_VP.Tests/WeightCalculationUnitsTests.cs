using System.Collections.Generic;
using Lib_VP.Rectifier;
using NUnit.Framework;

namespace Lib_VP.Tests
{
    [TestFixture]
    public class WeightCalculationUnitsTests
    {
        private WeightCalculationUnits CreateWeightCalculationUnits()
        {
            return new WeightCalculationUnits();
        }

        [Test]
        public void AddUnitFromRectificationEntity_CorrespondingUnitExists_AddToCorrespondingUnit()
        {
            var units = CreateWeightCalculationUnits();
            var expectedUnitName = "X";

            units.AddUnitFromRectificationEntity(new RectificationEntity("X1"));
            units.AddUnitFromRectificationEntity(new RectificationEntity("X11"));
            units.AddUnitFromRectificationEntity(new RectificationEntity("X"));

            Assert.AreEqual(units.Count, 1);
            WeightCalculationUnit unit = null;
            Assert.DoesNotThrow(() => { unit = units[expectedUnitName]; });
            Assert.AreEqual(3, unit.Count4WeightCalculationEntities);
        }

        [Test]
        public void AddUnitFromRectificationEntity_CorrespondingUnitNotExists_CreateNewUnit()
        {
            var units = CreateWeightCalculationUnits();
            var expectedUnitName = "X";

            units.AddUnitFromRectificationEntity(new RectificationEntity("X1"));

            Assert.AreEqual(units.Count, 1);
            Assert.DoesNotThrow(() =>
            {
                var unit = units[expectedUnitName];
            });
        }

        [Test]
        public void Indexer_KeyExists_ReturnProperUnit()
        {
            var units = CreateWeightCalculationUnits();
            var expectedUnitName = "Hello";

            units.AddUnitFromRectificationEntity(new RectificationEntity("Hello2019"));

            var unit = units[expectedUnitName];
            Assert.AreEqual(expectedUnitName, unit.Name);
        }

        [Test]
        public void Indexer_KeyNotExists_ThrowsKeyNotFoundException()
        {
            var units = CreateWeightCalculationUnits();

            Assert.Catch<KeyNotFoundException>(() =>
            {
                var unit = units["hello"];
            });
        }
    }
}