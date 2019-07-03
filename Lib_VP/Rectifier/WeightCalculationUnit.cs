using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib_VP.Rectifier
{
    public class WeightCalculationUnit
    {
        #region APIs

        public WeightCalculationUnit(string name)
        {
            Name = name;
        }

        public void AddWeightCalculationEntity(RectificationEntity entity)
        {
            if (WeightCalculationEntityDuplicates(entity)) return;
            _entitiesForWeightCalculation.Add(entity);
        }

        public void Calculate()
        {
            if (_entitiesForWeightCalculation == null)
                throw new InvalidOperationException(
                    "AddWeightCalculationEntity should be called before calling Calcuate");
            Weight = _entitiesForWeightCalculation.Average(pair => pair.CalculateWeight());
            UpdateWeights(Weight);
        }

        public void AddToWeightToBeUpdatedList(RectificationEntity entity)
        {
            _entitiesWhoseWeightWillBeUpdated.Add(entity);
        }

        #endregion

        #region Implementations

        private void UpdateWeights(double weight)
        {
            foreach (var entity in _entitiesWhoseWeightWillBeUpdated) entity.Weight = weight;
        }

        private bool WeightCalculationEntityDuplicates(RectificationEntity pair)
        {
            return _entitiesForWeightCalculation.Any(ele => ele.Name == pair.Name);
        }

        #endregion

        #region Fields

        public readonly string Name;
        private readonly List<RectificationEntity> _entitiesForWeightCalculation = new List<RectificationEntity>();
        private readonly List<RectificationEntity> _entitiesWhoseWeightWillBeUpdated = new List<RectificationEntity>();

        #endregion

        #region Properties

        public int Count4WeightCalculationEntities => _entitiesForWeightCalculation.Count;
        public int Count4WeightUpdatingEntities => _entitiesWhoseWeightWillBeUpdated.Count;

        public double Weight { get; private set; }

        #endregion
    }
}