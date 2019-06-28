using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Lib_VP.Rectifier
{
    public class DataGrid
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

        public DataColumn this[string key]
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
                for (var index = 0; index < _dataColumns.Count; index++)
                {
                    if (_dataColumns[index].Name != key) continue;
                    _dataColumns[index] = value;
                    break;
                }
            }
        }

        public void AddRow(IEnumerable<double> newRow)
        {
            var list = newRow.ToList();
            if (list.Count != Cols)
                throw new InvalidOperationException(
                    "Newly added values should have the same length as columns of DataGrid");

            for (var i = 0; i < list.Count; i++) _dataColumns[i].Enqueue(list[i]);
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

        #endregion
    }
}