using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib_VP.Rectifier
{
    public class Rectifier
    {
        /// <summary>
        /// </summary>
        /// <param name="columnNames">
        ///     Naming convention:
        ///     1. Value names in mm have no suffix
        /// </param>
        /// <param name="pixelSuffix">
        ///     Values that represent distances measured by pixel should end with "_pixel"
        ///     Examples: X1_pixel, Y1_weightedByXStar_pixel
        /// </param>
        /// <param name="nonstandardSuffix">
        ///     Values that are not used to calculate weights should have names with "_weightedBy" after their counterpart
        ///     Examples: Y1_weightedByX, Y_weightedByXStar_pixel
        /// </param>
        public Rectifier(List<string> columnNames, string standardFile, string pixelSuffix = "_pixel",
            string nonstandardSuffix = "_weightedBy")
        {
            _columnNames = columnNames;
            _standardFile = standardFile;
            foreach (var name in columnNames) _dataColumns[name] = new Queue<double>();
            GroupNames(columnNames, pixelSuffix, nonstandardSuffix);
        }

        private void GroupNames(List<string> columnNames, string pixelSuffix, string nonstandardSuffix)
        {
            var pixelNames = new List<string>();
            foreach (var columnName in columnNames)
                if (columnName.Contains(pixelSuffix))
                    pixelNames.Add(columnName);
            // Make sure every pixelName has a corresponding millimeterName
            // And add it to the millimeterName list
            var millimeterNames = new List<string>();
            foreach (var pixelName in pixelNames)
            {
                var millimeterNamesExpected =
                    pixelName.Substring(0, pixelName.IndexOf(pixelSuffix, StringComparison.Ordinal) + 1);
                if (columnNames.All(name => string.Equals(name, millimeterNamesExpected)))
                    throw new Exception(
                        $"Unable to find a corresponding millimeterName for name: {millimeterNamesExpected}");
                millimeterNames.Add(millimeterNamesExpected);
            }

            // Init nameMaps
            for (var i = 0; i < millimeterNames.Count; i++)
            {
                _allNameMaps.Add(new NameMap {Millmeter = millimeterNames[i], Pixel = pixelNames[i]});
                if (!pixelNames[i].Contains(nonstandardSuffix))
                    _nameMapsStandardOnly.Add(new NameMap {Millmeter = millimeterNames[i], Pixel = pixelNames[i]});
            }

            // Init angleNames
            foreach (var columnName in columnNames)
            {
                if (millimeterNames.Any(millimeterName => columnName.Contains(millimeterName))) continue;
                _angleNames.Add(columnName);
            }
        }


//        public IEnumerable<double> CalcWeights()
//        {
//            if(Rows != MaxRows)
//                throw new InvalidOperationException("Rows must reach MaxRows before Calling CalcWeight");
//            
//        }

        private void DequeueRow()
        {
            foreach (var column in _dataColumns)
                column.Value.Dequeue();
        }

        #region Fields

        private readonly List<string> _columnNames;
        private readonly string _standardFile;
        private readonly Dictionary<string, Queue<double>> _dataColumns = new Dictionary<string, Queue<double>>();
        private List<NameMap> _allNameMaps, _nameMapsStandardOnly;
        private List<string> _angleNames;

        #endregion


        #region Properties

        #endregion
    }

    internal class NameMap
    {
        public string Millmeter { get; set; }
        public string Pixel { get; set; }
    }
}