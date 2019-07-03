using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Lib_VP.Rectifier;
using NUnit.Framework;

namespace Lib_VP.Tests
{
    [TestFixture]
    public class DataGridTests
    {
        private DataGrid CreateDataGrid(bool rowFixed, int maxRows)
        {
            return new DataGrid(new List<string> {"X1", "World"}, rowFixed, maxRows);
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
        public void GetColumnsFromRegex_WhenRegexMatched_ReturnACollectionOfDataColumns()
        {
            var dataGrid = CreateDataGrid(false, 2);

            var dataColumns = dataGrid.GetColumnsFromRegex(@"[a-z,A-Z]+\d");
            var columnList = dataColumns.ToList();

            Assert.AreEqual(columnList.Count, 1);
            var name = columnList[0].Name;
            Assert.AreEqual(name, "X1");
        }

        [Test]
        public void GetColumnsFromRegex_WhenRegexNotMatched_KeyNotFoundExceptionThrown()
        {
            var dataGrid = CreateDataGrid(false, 2);

            Assert.Catch<KeyNotFoundException>(() =>
            {
                var dataColumns = dataGrid.GetColumnsFromRegex(@"[a-z,A-Z]+\d{2}");
            });
        }

        [Test]
        public void IndexerGet_WhenKeyExists_ReturnTheRightEntry()
        {
            var dataGrid = CreateDataGrid(false, 2);
            dataGrid.AddRow(new List<double> {1, 2});
            dataGrid.AddRow(new List<double> {3, 4});

            var col = dataGrid["World"].ToList();
            Assert.AreEqual(col[1], 4);
        }

        [Test]
        public void IndexerGet_WhenKeyNotExist_KeyNotFoundExceptionThrown()
        {
            var dataGrid = CreateDataGrid(false, 2);
            dataGrid.AddRow(new List<double> {1, 2});

            Assert.Catch<KeyNotFoundException>(() =>
            {
                var col = dataGrid["world"];
            });
        }

        [Test]
        public void IndexerSet_WhenKeyExist_DataOfCorrespondingColumnGetReplaced()
        {
            var dataGrid = CreateDataGrid(false, 2);

            dataGrid["World"] = new List<double> {1, 1, 1, 1};

            Assert.AreEqual(4, dataGrid["World"].ToList().Count);
        }

        [Test]
        public void IndexerSet_WhenKeyNotExist_AddNewOne()
        {
            var dataGrid = CreateDataGrid(false, 2);

            dataGrid["world"] = new List<double>();

            Assert.AreEqual(3, dataGrid.Cols);
        }
    }
}