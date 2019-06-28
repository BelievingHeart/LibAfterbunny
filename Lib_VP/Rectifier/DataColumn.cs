using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lib_VP.Rectifier
{
    public class DataColumn : IEnumerable<double>
    {
        #region Fields

        private readonly Queue<double> _data;

        #endregion

        public DataColumn(string name, IEnumerable<double> data = null)
        {
            Name = name;
            if (data == null)
            {
                _data = new Queue<double>();
                return;
            }

            foreach (var d in data) _data.Enqueue(d);
        }

        public double this[int index] => _data.ElementAt(index);


        public IEnumerator<double> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enqueue(double newValue)
        {
            _data.Enqueue(newValue);
            if (RowsFixed && _data.Count > MaxRows) _data.Dequeue();
        }

        public IEnumerable<double> DividedBy(IEnumerable<double> divisors)
        {
            return _data.Zip(divisors, (arg1, arg2) => arg1 / arg2);
        }

        public IEnumerable<double> Times(IEnumerable<double> multipliers)
        {
            return _data.Zip(multipliers, (arg1, arg2) => arg1 * arg2);
        }

        public IEnumerable<double> Add(IEnumerable<double> adder)
        {
            return _data.Zip(adder, (arg1, arg2) => arg1 + arg2);
        }

        public IEnumerable<double> DividedBy(double divisor)
        {
            return _data.Select(num => num / divisor);
        }

        public IEnumerable<double> Times(double multiplier)
        {
            return _data.Select(num => num * multiplier);
        }

        public IEnumerable<double> Add(double adder)
        {
            return _data.Select(num => num + adder);
        }

        #region Properties

        public string Name { set; get; }

        public int Rows => _data.Count;
        public int MaxRows { get; set; }
        public bool RowsFixed { get; set; }

        #endregion
    }
}