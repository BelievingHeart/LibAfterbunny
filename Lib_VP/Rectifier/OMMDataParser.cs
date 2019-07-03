using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lib_VP.Rectifier
{
    public class OMMDataParser : IOMMDataParser
    {
        private readonly List<string> _columnsToIgnore;
        private readonly string _ommFile;

        public OMMDataParser(string ommFile, params string[] columnsToIgnore)
        {
            _ommFile = ommFile;
            _columnsToIgnore = columnsToIgnore.ToList();
        }

        public List<DataColumn> Parse(int numLines)
        {
            var output = new List<DataColumn>();
            var lines = File.ReadAllLines(_ommFile);
            var firstLine = lines[0];
            var names = firstLine.Split(',');
            var itemsCount = names.Length;
            var lastNumLines = lines.Reverse().Take(numLines).Reverse();

            for (var i = 0; i < itemsCount - 1; i++) output.Add(new DataColumn(names[i + 1]));

            foreach (var line in lastNumLines)
            {
                var values = line.Split(',');
                for (var i = 0; i < itemsCount - 1; i++)
                {
                    var parseSucceed = double.TryParse(values[i + 1], out var value);
                    output[i].Enqueue(parseSucceed ? value : 0);
                }
            }

            return output.Where(column => !_columnsToIgnore.Contains(column.Name)).ToList();
        }
    }
}