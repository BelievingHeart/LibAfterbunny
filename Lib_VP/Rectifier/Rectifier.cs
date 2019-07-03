using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cognex.VisionPro;

namespace Lib_VP.Rectifier
{
    /// <summary>
    ///     To use Rectifier, the outputs and inputs of the CogToolBlock must be named properly
    ///     Naming Rules by examples:
    ///     For outputs that are measured by millimeter:
    ///     X, Y1, Z_WeightedByX
    ///     For outputs that are measured by pixel:
    ///     X_pixel, Y1_pixel, Z_WeightedByX_pixel
    ///     For outputs that are measured by angle:
    ///     Angle1, Angle1_Pixel(A fake output using the same value of Angle1)
    ///     For inputs:
    ///     Weight_X, Bias_X
    ///     Weight_Y1, Bias_Y1
    ///     Weight_Z_WeightedByX, Bias_Z_WeightedByX
    ///     Weight_Angle1, Bias_Angle1
    /// </summary>
    public class Rectifier
    {
        /// <summary>
        /// </summary>
        /// <param name="pixelPattern">
        ///     Values that represent distances measured by pixel should end with "_Pixel"
        ///     Examples: X1_pixel, Y1_WeightedByX_Pixel
        /// </param>
        /// <param name="nonstandardPattern">
        ///     Values that are not used to calculate weights should have names with "_WeightedBy" after their counterpart
        ///     Examples: Y1_WeightedByX, Y_WeightedByX_Pixel
        /// </param>
        /// <param name="columnNames">
        ///     Naming convention:
        ///     1. Value names in mm have no suffix
        /// </param>
        public Rectifier(IOMMDataParser dataParser, string pixelPattern = @"_Pixel",
            string nonstandardPattern = @"_WeightedBy")
        {
            _dataParser = dataParser;
            _pixelPattern = pixelPattern;
            _nonstandardPattern = nonstandardPattern;
        }


        #region Properties

        public DataGrid DataGrid
        {
            get => _dataGrid;
            set
            {
                _dataGrid = value;
                OnDataGridSet();
            }
        }

        #endregion

        public void Rectify(ICogTool toolBlock)
        {
            if (_dataGrid == null)
                throw new InvalidOperationException("Rectify can not be called until DataGrid is initialized.");
            if (!_dataGrid.EnoughDataCollected)
                throw new InvalidOperationException(
                    $"At least {_dataGrid.MaxRows} lines of data should be collected before calling Rectify");
            var OMM_Data = ParseOMMCSV();
            AssociateWithRectificationEntities(OMM_Data);
            _weightCalculationUnits.CalculateWeights();
            EstimateMillimeterDistances(); // for each RectificationEntity, estimate biased millimeter distances
            EditBlockInputs(toolBlock);
        }

        #region Implementations

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
            var regex = new Regex($"{_nonstandardPattern}(\\w+)");
            var matchResult = regex.Match(name);
            return matchResult.Groups[1].Value;
        }


        private void ExtractMillimeterPixelPairsFromDataGrid()
        {
            var pixelColumns = _dataGrid.GetColumnsFromRegex(_pixelPattern);
            var millimeterColumns = GetCorrespondingMillimeterColumns(pixelColumns);
            _rectificationEntities = pixelColumns.Zip(millimeterColumns,
                (pixelColumn, millimeterColumn) =>
                    new RectificationEntity(millimeterColumn.Name)
                        {PixelColumn = pixelColumn, MillimeterColumn = millimeterColumn}).ToList();
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
            return new DataColumn(millimeterNameExpected, _dataGrid[millimeterNameExpected]);
        }

        private string RemovePixelSuffix(string pixelColumnName)
        {
            return pixelColumnName.Substring(0, pixelColumnName.IndexOf(_pixelPattern, StringComparison.Ordinal));
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

        private void OnDataGridSet()
        {
            ExtractMillimeterPixelPairsFromDataGrid();
            ExtractWeightCalculationUnits();
        }

        private List<DataColumn> ParseOMMCSV()
        {
            return _dataParser.Parse(_dataGrid.MaxRows);
        }

        #endregion

        #region Fields

        private DataGrid _dataGrid;
        private readonly IOMMDataParser _dataParser;
        private readonly string _pixelPattern;
        private readonly string _nonstandardPattern;
        private List<RectificationEntity> _rectificationEntities;
        private readonly WeightCalculationUnits _weightCalculationUnits = new WeightCalculationUnits();

        #endregion
    }
}