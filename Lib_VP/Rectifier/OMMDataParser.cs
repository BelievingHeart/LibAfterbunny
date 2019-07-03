using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lib_VP.Rectifier
{
    public class OMMDataParser : IOMMDataParser
    {
        private readonly string _ommFile;

        public OMMDataParser(string ommFile)
        {
            _ommFile = ommFile;
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
                for (var i = 0; i < itemsCount - 1; i++) output[i].Enqueue(double.Parse(values[i + 1]));
            }

            return output;
        }
    }
}