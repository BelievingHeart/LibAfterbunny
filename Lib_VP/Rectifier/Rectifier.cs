using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cognex.VisionPro;

namespace Lib_VP.Rectifier
{
    public class Rectifier
    {
        /// <summary>
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="standardFile"></param>
        /// <param name="pixelPattern">
        ///     Values that represent distances measured by pixel should end with "_pixel"
        ///     Examples: X1_pixel, Y1_weightedByXStar_pixel
        /// </param>
        /// <param name="nonstandardPattern">
        ///     Values that are not used to calculate weights should have names with "_weightedBy" after their counterpart
        ///     Examples: Y1_weightedByX, Y_weightedByXStar_pixel
        /// </param>
        /// <param name="columnNames">
        ///     Naming convention:
        ///     1. Value names in mm have no suffix
        /// </param>
        public Rectifier(DataGrid dataGrid, string standardFile, string pixelPattern = @"_pixel",
            string nonstandardPattern = @"_weightedBy")
        {
            _dataGrid = dataGrid;
            _standardFile = standardFile;
            _pixelPattern = pixelPattern;
            _nonstandardPattern = nonstandardPattern;
            ExtractMillimeterPixelPairsFromDataGrid();
            ExtractWeightCalculationUnits();
        }

        private void ExtractWeightCalculationUnits()
        {
            var nonStandardEntities = new List<RectificationEntity>();
            foreach (var entity in _rectificationEntities)
                if (entity.Name.Contains(_nonstandardPattern))
                    nonStandardEntities.Add(entity);
                else
                    _weightCalculationUnits.AddUnitFromRectificationEntity(entity);

            foreach (var nonStandardEntity in nonStandardEntities)
            {
                var weightSourceName = GetRectificationSourceName(nonStandardEntity.Name);
                _weightCalculationUnits[weightSourceName].AddToWeightToBeUpdatedList(nonStandardEntity);
            }
        }

        private string GetRectificationSourceName(string name)
        {
            var regex = new Regex($"{_nonstandardPattern}(\\w+)_");
            var matchResult = regex.Match(name);
            return matchResult.Groups[1].Value;
        }


        private void ExtractMillimeterPixelPairsFromDataGrid()
        {
            var pixelColumns = _dataGrid.GetColumnsFromRegex(_pixelPattern);
            var millimeterColumns = GetCorrespondingMillimeterColumns(pixelColumns);
            _rectificationEntities = (List<RectificationEntity>) pixelColumns.Zip(millimeterColumns,
                (pixelColumn, millimeterColumn) =>
                    new RectificationEntity(millimeterColumn.Name, millimeterColumn, pixelColumn));
        }

        private List<DataColumn> GetCorrespondingMillimeterColumns(IEnumerable<DataColumn> pixelColumns)
        {
            var output = new List<DataColumn>();
            foreach (var pixelColumn in pixelColumns) output.Add(GetCorrespondingMillimeterColumn(pixelColumn));

            return output;
        }

        private DataColumn GetCorrespondingMillimeterColumn(DataColumn pixelColumn)
        {
            var millimeterNameExpected = RemovePixelSuffix(pixelColumn.Name);
            return _dataGrid[millimeterNameExpected];
        }

        private string RemovePixelSuffix(string pixelColumnName)
        {
            return pixelColumnName.Substring(0, pixelColumnName.IndexOf(_pixelPattern, StringComparison.Ordinal) + 1);
        }

        public void Rectify(ICogTool toolBlock)
        {
            if (!_dataGrid.EnoughDataCollected)
                throw new InvalidOperationException(
                    $"At least {_dataGrid.MaxRows} lines of data should be collected before calling Rectify");
            var OMM_Data = ParseOMMCSV();
            AssociateWithRectificationEntities(OMM_Data);
            _weightCalculationUnits.CalculateWeights();
            EstimateMillimeterDistances(); // for each RectificationEntity, estimate biased millimeter distances
            EditBlockInputs(toolBlock);
        }

        private void EditBlockInputs(ICogTool toolBlock)
        {
            foreach (var rectificationEntity in _rectificationEntities)
                rectificationEntity.EditWeightAndBias(toolBlock);
        }

        private void EstimateMillimeterDistances()
        {
            foreach (var rectificationEntity in _rectificationEntities)
                rectificationEntity.EstimateBiasedMillimeterDistances();
        }

        private void AssociateWithRectificationEntities(List<DataColumn> ommData)
        {
            foreach (var column in ommData)
                for (var i = 0; i < _rectificationEntities.Count; i++)
                {
                    if (_rectificationEntities[i].Name.Equals(column.Name, StringComparison.Ordinal))
                    {
                        _rectificationEntities[i].MillimeterColumnOMM = column;
                        break;
                    }

                    if (i == _rectificationEntities.Count - 1)
                        throw new KeyNotFoundException(
                            $"Can not find any RectificationEntity with Name: {column.Name}");
                }
        }

        private List<DataColumn> ParseOMMCSV()
        {
            var output = new List<DataColumn>();
            var lines = File.ReadAllLines(_standardFile);
            var firstLine = lines[0];
            var names = firstLine.Split(',');
            var itemsCount = names.Length;
            var lastNumLines = lines.Reverse().Take(_dataGrid.MaxRows).Reverse();

            for (var i = 0; i < itemsCount - 1; i++) output.Add(new DataColumn(names[i + 1]));

            foreach (var line in lastNumLines)
            {
                var values = line.Split(',');
                for (var i = 0; i < itemsCount - 1; i++) output[i].Enqueue(double.Parse(values[i + 1]));
            }

            return output;
        }


        #region Fields

        private readonly DataGrid _dataGrid;
        private readonly string _standardFile;
        private readonly string _pixelPattern;
        private readonly string _nonstandardPattern;
        private List<RectificationEntity> _rectificationEntities;
        private WeightCalculationUnits _weightCalculationUnits;

        #endregion


        #region Properties

        #endregion
    }
}