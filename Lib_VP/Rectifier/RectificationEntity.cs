using System;
using System.Linq;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;

namespace Lib_VP.Rectifier
{
    public class RectificationEntity
    {
        #region Implementations

        private void EditBlockInput(string inputName, double value, ICogTool toolBlock)
        {
            if (toolBlock is CogToolBlock)
            {
                var blockTyped = (CogToolBlock) toolBlock;
                blockTyped.Inputs[inputName].Value = value;
            }
            else
            {
                Console.WriteLine($"Input [{inputName}] has changed to {value}");
            }
        }

        #endregion

        #region Fields

        private DataColumn _millimeterColumnUnbiased;

        /// <summary>
        ///     Possible names: X1, X, Y1_WeightedByX
        /// </summary>
        public string Name;

        #endregion


        #region Properties

        public double Weight { get; set; }
        public DataColumn MillimeterColumn { get; set; }
        public DataColumn PixelColumn { get; set; }
        public DataColumn MillimeterColumnEstimate { get; set; }
        public DataColumn MillimeterColumnOMM { get; set; }

        public DataColumn OMMvsEstimateDiff { get; private set; }

        public double Bias { get; private set; }

        #endregion

        #region APIs

        public RectificationEntity(string name)
        {
            Name = name;
        }

        public double CalculateWeight()
        {
            if (MillimeterColumnOMM == null)
                throw new InvalidOperationException("MillimeterColumnOMM can not be null when calling CalculateWeight");
            if (PixelColumn == null)
                throw new InvalidOperationException("PixelColumn can not be null when calling CalculateWeight");
            return MillimeterColumnOMM.Zip(PixelColumn, (miliDist, pixelDist) => miliDist / pixelDist).Average();
        }

        public void EstimateBiasedMillimeterDistances()
        {
            if (MillimeterColumnOMM == null)
                throw new InvalidOperationException("MillimeterColumnOMM can not be null when calling CalculateWeight");
            if (PixelColumn == null)
                throw new InvalidOperationException("PixelColumn can not be null when calling CalculateWeight");

            var millimeterDistancesUnbiased = PixelColumn.Times(Weight);
            OMMvsEstimateDiff = new DataColumn("Diff", MillimeterColumnOMM.Subtract(millimeterDistancesUnbiased));
            Bias = OMMvsEstimateDiff.Average();
            MillimeterColumnEstimate = new DataColumn("MillimeterColumnEstimate",
                millimeterDistancesUnbiased.Select(dist => dist + Bias));
        }

        public void EditWeightAndBias(ICogTool toolBlock)
        {
            var inputNameWeight = "Weight_" + Name; // For example: Weight_X, Weight_Y1_WeightedByX
            var inputNameBias = "Bias_" + Name;

            EditBlockInput(inputNameWeight, Weight, toolBlock);
            EditBlockInput(inputNameBias, Bias, toolBlock);
        }

        #endregion
    }
}