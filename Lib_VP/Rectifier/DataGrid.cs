using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lib_VP.Rectifier
{
    public class DataGrid : IEnumerable<DataColumn>
    {
        public DataGrid(IEnumerable<string> dataColumnNames, bool rowsFixed = false, int maxRows = int.MaxValue)
        {
            // Duplication checking
            var set = new HashSet<string>();
            var columnNames = dataColumnNames.ToList();
            foreach (var dataColumnName in columnNames) set.Add(dataColumnName);
            if (set.Count < columnNames.Count) throw new DuplicateNameException("dataColumnNames has duplications!");

            foreach (var dataColumnName in columnNames) _dataColumns.Add(new DataColumn(dataColumnName));
            RowsFixed = rowsFixed;
            MaxRows = maxRows;
        }

        public IEnumerable<double> this[string key]
        {
            get
            {
                foreach (var dataColumn in _dataColumns)
                    if (dataColumn.Name == key)
                        return dataColumn;
                throw new KeyNotFoundException($"Unable to find a DataColumn with name:{key}");
            }
            set
            {
                for (var i = 0; i < _dataColumns.Count; i++)
                    if (_dataColumns[i].Name == key)
                    {
                        _dataColumns[i] = new DataColumn(key, value);
                        return;
                    }

                _dataColumns.Add(new DataColumn(key, value));
            }
        }

        public IEnumerator<DataColumn> GetEnumerator()
        {
            return _dataColumns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRow(IEnumerable<double> newRow)
        {
            var list = newRow.ToList();
            if (list.Count != Cols)
                throw new InvalidOperationException(
                    "Newly added values should have the same length as columns of DataGrid");

            for (var i = 0; i < list.Count; i++) _dataColumns[i].Enqueue(list[i]);
        }

        public IEnumerable<DataColumn> GetColumnsFromRegex(string pattern)
        {
            var regex = new Regex(pattern);

            if (_dataColumns.All(column => !regex.IsMatch(column.Name)))
                throw new KeyNotFoundException($"No item in _dataColumn is matched with pattern: {pattern}");
            return _dataColumns.Where(column => regex.IsMatch(column.Name));
        }

        #region Fields

        private readonly List<DataColumn> _dataColumns = new List<DataColumn>();
        private int _maxRows = 8;
        private bool _rowsFixed;

        #endregion

        #region Properties

        public bool RowsFixed
        {
            get => _rowsFixed;
            set
            {
                _rowsFixed = value;
                foreach (var dataColumn in _dataColumns) dataColumn.RowsFixed = value;
            }
        }

        public int MaxRows
        {
            get => _maxRows;
            set
            {
                if (value <= 0) throw new InvalidOperationException("MaxRows can not be equal or less than 0");
                _maxRows = value;
                foreach (var dataColumn in _dataColumns) dataColumn.MaxRows = value;
            }
        }

        public int Cols => _dataColumns.Count;

        public int Rows
        {
            get
            {
                if (!EachcolumnHasTheSameHeight)
                    throw new InvalidDataException("All columns should have the same height!");
                return _dataColumns[0].Rows;
            }
        }

        private bool EachcolumnHasTheSameHeight
        {
            get
            {
                var heightOfFirstCol = _dataColumns[0].Rows;
                return _dataColumns.All(ele => ele.Rows == heightOfFirstCol);
            }
        }

        public bool EnoughDataCollected => RowsFixed && Rows == MaxRows;

        #endregion
    }
}