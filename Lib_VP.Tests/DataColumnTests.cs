using System;
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
            dataColumn.Enqueue(1);
            dataColumn.Enqueue(1);

            return dataColumn;
        }

        [Test]
        public void Add_ACollectionOfDouble_CorrespondinglyAdded()
        {
            var dataColumn = CreateDataColumn();

            var sum = dataColumn.Add(new List<double> {0, 1, 2}).ToList();

            var flag = true;
            for (var i = 0; i < 3; i++)
                if (Math.Abs(sum[i] - i - 1) > 0.001)
                    flag = false;

            Assert.True(flag);
        }

        [Test]
        public void Add_SingleDouble_AllAddUpThatAmount()
        {
            var dataColumn = CreateDataColumn();

            var sum = dataColumn.Add(1);

            Assert.True(sum.All(ele => Math.Abs(ele - 2) < 0.001));
        }

        [Test]
        public void DividedBy_ACollectionOfDouble_CorrespondinglyDivided()
        {
            var dataColumn = CreateDataColumn();

            var result = dataColumn.DividedBy(new List<double> {1, 2, 3}).ToList();

            var flag = true;
            for (var i = 0; i < 3; i++)
                if (Math.Abs(result[i] - 1.0 / (i + 1)) > 0.001)
                    flag = false;

            Assert.True(flag);
        }

        [Test]
        public void DividedBy_SingleDouble_AllDividedByThatAmount()
        {
            var dataColumn = CreateDataColumn();

            var result = dataColumn.DividedBy(2);

            Assert.True(result.All(ele => Math.Abs(ele - 0.5) < 0.001));
        }

        [Test]
        public void Times_ACollectionOfDouble_CorrespondinglyMultiplied()
        {
            var dataColumn = CreateDataColumn();

            var result = dataColumn.Times(new List<double> {1, 2, 3}).ToList();

            var flag = true;
            for (var i = 0; i < 3; i++)
                if (Math.Abs(result[i] - (i + 1)) > 0.001)
                    flag = false;

            Assert.True(flag);
        }

        [Test]
        public void Times_SingleDouble_AllMultipliedByThatAmount()
        {
            var dataColumn = CreateDataColumn();

            var result = dataColumn.Times(2);

            Assert.True(result.All(ele => Math.Abs(ele - 2) < 0.001));
        }
    }
}