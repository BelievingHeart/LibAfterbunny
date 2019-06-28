using System;
using System.Collections.Generic;
using System.Data;
using Lib_VP.Rectifier;
using NUnit.Framework;

namespace Lib_VP.Tests
{
    [TestFixture]
    public class DataGridTests
    {
        private DataGrid CreateDataGrid(bool rowFixed, int maxRows)
        {
            return new DataGrid(new List<string> {"Hello", "World"}, rowFixed, maxRows);
        }

        [Test]
        public void AddRow_CountOfNewValuesNotEqualTOCols_InvalidOperationExceptionThrown()
        {
            var dataGrid = CreateDataGrid(false, 2);

            Assert.Catch<InvalidOperationException>(() => dataGrid.AddRow(new List<double> {1, 2, 3}));
        }

        [Test]
        public void AddRow_UnlimitedRowsAndMaxRowsNotReached_RowsIncrementByOne()
        {
            var dataGrid = CreateDataGrid(false, int.MaxValue);
            dataGrid.AddRow(new List<double> {1, 2});

            Assert.AreEqual(dataGrid.Rows, 1);
        }

        [Test]
        public void AddRow_UnlimitedRowsAndMaxRowsReached_RowsGrows()
        {
            var dataGrid = CreateDataGrid(false, 2);
            dataGrid.AddRow(new List<double> {1, 2});
            dataGrid.AddRow(new List<double> {1, 2});
            dataGrid.AddRow(new List<double> {1, 2});

            Assert.AreEqual(dataGrid.Rows, 3);
        }

        [Test]
        public void AddRow_UnlimitedRowsAndMaxRowsReached_RowsTruncated()
        {
            var dataGrid = CreateDataGrid(true, 2);
            dataGrid.AddRow(new List<double> {1, 2});
            dataGrid.AddRow(new List<double> {1, 2});
            dataGrid.AddRow(new List<double> {1, 2});

            Assert.AreEqual(dataGrid.Rows, 2);
        }

        [Test]
        public void Constructor_WhenColumnNamesDuplicates_DuplicateNameExceptionThrown()
        {
            Assert.Catch<DuplicateNameException>(() =>
            {
                var dataGrid = new DataGrid(new List<string> {"hello", "hello"});
            });
        }

        [Test]
        public void Indexer_WhenKeyExists_ReturnTheRightEntry()
        {
            var dataGrid = CreateDataGrid(false, 2);
            dataGrid.AddRow(new List<double> {1, 2});
            dataGrid.AddRow(new List<double> {3, 4});

            var col = dataGrid["World"];
            Assert.AreEqual(col[1], 4);
        }

        [Test]
        public void Indexer_WhenKeyNotExist_KeyNotFoundExceptionThrown()
        {
            var dataGrid = CreateDataGrid(false, 2);
            dataGrid.AddRow(new List<double> {1, 2});

            Assert.Catch<KeyNotFoundException>(() =>
            {
                var col = dataGrid["world"];
            });
        }
    }
}