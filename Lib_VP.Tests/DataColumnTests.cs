using System.Collections.Generic;
using System.Linq;
using Lib_VP.Rectifier;
using NUnit.Framework;

namespace Lib_VP.Tests
{
    [TestFixture]
    public class DataColumnTests
    {
        private DataColumn CreateDataColumn()
        {
            var dataColumn = new DataColumn("test");
            dataColumn.Enqueue(1);
            dataColumn.Enqueue(2);
            dataColumn.Enqueue(3);

            return dataColumn;
        }

        [Test]
        public void Add_ACollectionOfDouble_CorrespondinglyAdded()
        {
            var dataColumn = CreateDataColumn();
            var expected = new List<double> {2, 4, 6};

            var actual = dataColumn.Add(new List<double> {1, 2, 3}).ToList();

            Assert.True(FloatingCompare.FloatingEnumerablesAreVeryClose(actual, expected));
        }

        [Test]
        public void Add_SingleDouble_AllAddUpThatAmount()
        {
            var dataColumn = CreateDataColumn();
            var expected = new List<double> {2, 3, 4};

            var actual = dataColumn.Add(1);

            Assert.True(FloatingCompare.FloatingEnumerablesAreVeryClose(actual, expected));
        }

        [Test]
        public void DividedBy_ACollectionOfDouble_CorrespondinglyDivided()
        {
            var dataColumn = CreateDataColumn();
            var expected = new List<double> {1, 1, 1};

            var actual = dataColumn.DividedBy(new List<double> {1, 2, 3}).ToList();

            Assert.True(FloatingCompare.FloatingEnumerablesAreVeryClose(actual, expected));
        }

        [Test]
        public void DividedBy_SingleDouble_AllDividedByThatAmount()
        {
            var dataColumn = CreateDataColumn();
            var expected = new List<double> {1 / 2.0, 2 / 2.0, 3 / 2.0};


            var actual = dataColumn.DividedBy(2);

            Assert.True(FloatingCompare.FloatingEnumerablesAreVeryClose(actual, expected));
        }

        [Test]
        public void Subtract_ACollectionOfDouble_CorrespondinglySubtracted()
        {
            var dataColumn = CreateDataColumn();
            var expected = new List<double> {0, 0, 0};

            var actual = dataColumn.Subtract(new List<double> {1, 2, 3}).ToList();

            Assert.True(FloatingCompare.FloatingEnumerablesAreVeryClose(actual, expected));
        }

        [Test]
        public void Subtract_SingleDouble_AllSubtractedByThatAmount()
        {
            var dataColumn = CreateDataColumn();
            var expected = new List<double> {0, 1, 2};

            var actual = dataColumn.Subtract(1);

            Assert.True(FloatingCompare.FloatingEnumerablesAreVeryClose(actual, expected));
        }

        [Test]
        public void Times_ACollectionOfDouble_CorrespondinglyMultiplied()
        {
            var dataColumn = CreateDataColumn();
            var expected = new List<double> {1, 4, 9};

            var actual = dataColumn.Times(new List<double> {1, 2, 3}).ToList();

            Assert.True(FloatingCompare.FloatingEnumerablesAreVeryClose(actual, expected));
        }

        [Test]
        public void Times_SingleDouble_AllMultipliedByThatAmount()
        {
            var dataColumn = CreateDataColumn();
            var expected = new List<double> {2, 4, 6};

            var actual = dataColumn.Times(2);

            Assert.True(FloatingCompare.FloatingEnumerablesAreVeryClose(actual, expected));
        }
    }
}