using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lib_VP.Rectifier
{
    public class DataGrid
    {
        private readonly List<string> _columnNames;
        private readonly Dictionary<string, Queue<double>> _dataColumns = new Dictionary<string, Queue<double>>();

        /// <summary>
        /// </summary>
        /// <param name="columnNames">
        ///     Naming convention:
        ///     1. Value names in mm have no suffix
        ///     2. Value names in pixel have "_pixel" after their mm counterpart
        ///     3. Values that used to calculate weights should end with "_standard"
        /// </param>
        public DataGrid(List<string> columnNames)
        {
            _columnNames = columnNames;
            foreach (var name in columnNames) _dataColumns[name] = new Queue<double>();
        }

        public int MaxRows { get; set; } = 8;

        public int Rows
        {
            get
            {
                if (!EachcolumnHasTheSameHeight)
                    throw new InvalidDataException("All columns should have the same height!");
                return _dataColumns.ElementAt(0).Value.Count;
            }
        }

        public int Cols => _dataColumns.Count;

        private bool EachcolumnHasTheSameHeight
        {
            get
            {
                var heightOfFirstCol = _dataColumns.ElementAt(0).Value.Count;
                return _dataColumns.All(ele => ele.Value.Count == heightOfFirstCol);
            }
        }

        public void AddRow(List<double> newValues)
        {
            if (newValues.Count != Cols)
                throw new InvalidOperationException(
                    "Newly added values should have the same length as columns of DataGrid");

            for (var i = 0; i < newValues.Count; i++) _dataColumns[_columnNames[i]].Enqueue(newValues[i]);

            if (Rows > MaxRows) DequeueRow();
        }

        private void DequeueRow()
        {
            foreach (var column in _dataColumns)
                column.Value.Dequeue();
        }
    }
}