using System;
using System.Collections.Generic;
using Lib_VP.Rectifier;
using NUnit.Framework;

namespace Lib_VP.Tests
{
    [TestFixture]
    public class DataGridTests
    {
        private DataGrid CreateDataGrid()
        {
            return new DataGrid(new List<string> {"hello", "world"});
        }

        [Test]
        public void AddRow_WhenCountOfNewValuesAndColsDoNotMatch_InvalidOperationExceptionThrown()
        {
            var dataGrid = CreateDataGrid();

            Assert.Catch<InvalidOperationException>(() => { dataGrid.AddRow(new List<double> {1, 2, 3}); });
        }

        [Test]
        public void AddRow_WhenMaxRowNotReached_RowsIncrementByOne()
        {
            var dataGrid = CreateDataGrid();

            dataGrid.AddRow(new List<double> {1, 2});

            Assert.AreEqual(dataGrid.Rows, 1);
        }

        [Test]
        public void AddRow_WhenMaxRowReached_RowsRemainsMaxRows()
        {
            var dataGrid = CreateDataGrid();
            var maxRows = 8;
            dataGrid.MaxRows = maxRows;
            for (var i = 0; i < maxRows + 1; i++) dataGrid.AddRow(new List<double> {1, 2});

            Assert.AreEqual(dataGrid.Rows, maxRows);
        }
    }
}