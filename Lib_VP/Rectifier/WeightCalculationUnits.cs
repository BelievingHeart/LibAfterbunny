using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lib_VP.Rectifier
{
    public class WeightCalculationUnits
    {
        #region Properties

        public int Count
        {
            get
            {
                if (_units.Count != _unitNames.Count)
                    throw new InvalidOperationException("Count of units and count of unitNames must be equal");
                return _units.Count;
            }
        }

        #endregion

        #region Fields

        private readonly HashSet<string> _unitNames = new HashSet<string>();
        private readonly List<WeightCalculationUnit> _units = new List<WeightCalculationUnit>();

        #endregion

        #region APIs

        public void AddUnitFromRectificationEntity(RectificationEntity rectificationEntity)
        {
            var nameWithoutTrailingNumbers = TrimTrailingNumbers(rectificationEntity.Name);
            if (!UnitWithThisNameExists(nameWithoutTrailingNumbers)) CreateNewUnit(nameWithoutTrailingNumbers);
            this[nameWithoutTrailingNumbers].AddWeightCalculationEntity(rectificationEntity);
            this[nameWithoutTrailingNumbers].AddToWeightToBeUpdatedList(rectificationEntity);
        }

        public WeightCalculationUnit this[string unitName]
        {
            get
            {
                foreach (var unit in _units)
                    if (string.Equals(unit.Name, unitName, StringComparison.Ordinal))
                        return unit;

                throw new KeyNotFoundException($"Can not find any unit with Name: {unitName}");
            }
        }

        public void CalculateWeights()
        {
            foreach (var calculationUnit in _units) calculationUnit.Calculate();
        }

        #endregion

        #region Implementations

        private void CreateNewUnit(string nameWithoutTrailingNumbers)
        {
            _unitNames.Add(nameWithoutTrailingNumbers);
            _units.Add(new WeightCalculationUnit(nameWithoutTrailingNumbers));
        }

        private string TrimTrailingNumbers(string pairName)
        {
            var regex = new Regex(@"(_?[a-z, A-Z]+)\d*");
            var matchResult = regex.Match(pairName);
            return matchResult.Groups[1].Value;
        }

        private bool UnitWithThisNameExists(string nameWithoutTrailingNumbers)
        {
            return _unitNames.Contains(nameWithoutTrailingNumbers);
        }

        #endregion
    }
}