using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib_VP.Rectifier
{
    public class WeightCalculationUnit
    {
        public WeightCalculationUnit(string name)
        {
            Name = name;
        }

        public void AddWeightCalculationEntity(RectificationEntity pair)
        {
            if (PairDuplicates(pair)) return;
            _entitiesForWeightCalculation.Add(pair);
        }

        public void Calculate()
        {
            _weight = _entitiesForWeightCalculation.Average(pair => pair.CalculateWeight());
            UpdateWeights(_weight);
            _calculated = true;
        }

        private void UpdateWeights(double weight)
        {
            foreach (var entity in _entitiesWhoseWeightWillBeUpdated) entity.Weight = weight;
        }

        private bool PairDuplicates(RectificationEntity pair)
        {
            return _entitiesForWeightCalculation.Any(ele => ele.Name == pair.Name);
        }

        public void AddToWeightToBeUpdatedList(RectificationEntity nonStandardEntity)
        {
            _entitiesWhoseWeightWillBeUpdated.Add(nonStandardEntity);
        }

        #region Fields

        private double _weight;
        private bool _calculated;
        public readonly string Name;
        private readonly List<RectificationEntity> _entitiesForWeightCalculation = new List<RectificationEntity>();
        private readonly List<RectificationEntity> _entitiesWhoseWeightWillBeUpdated = new List<RectificationEntity>();

        #endregion

        #region Properties

        public int Count => _entitiesForWeightCalculation.Count;

        public double Weight
        {
            get
            {
                if (!_calculated)
                    throw new InvalidOperationException("Weight can not be accessed until Calculate is called");
                return _weight;
            }
        }

        #endregion
    }
}