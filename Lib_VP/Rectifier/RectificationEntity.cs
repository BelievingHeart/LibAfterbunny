using System.Linq;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;

namespace Lib_VP.Rectifier
{
    public class RectificationEntity
    {
        private DataColumn _millimeterColumnUnbiased;
        public DataColumn MillimeterColumn, PixelColumn, MillimeterColumnEstimate, MillimeterColumnOMM;

        /// <summary>
        ///     Possible names: X1, X, Y1WeightedByX
        /// </summary>
        public string Name;

        public RectificationEntity(string name, DataColumn millimeterColumn, DataColumn pixelColumn)
        {
            Name = name;
            MillimeterColumn = millimeterColumn;
            PixelColumn = pixelColumn;
        }

        public double Weight { get; set; }

        public double Bias { get; private set; }

        public double CalculateWeight()
        {
            return MillimeterColumnOMM.Zip(PixelColumn, (miliDist, pixelDist) => miliDist / pixelDist).Average();
        }

        public void EstimateBiasedMillimeterDistances()
        {
            var MillimeterDistancesUnbiased = PixelColumn.Times(Weight);
            var diffs = MillimeterColumnOMM.Subtract(MillimeterDistancesUnbiased);
            Bias = diffs.Average();
            MillimeterColumnEstimate = new DataColumn("MillimeterColumnEstimate",
                MillimeterDistancesUnbiased.Select(dist => dist + Bias));
        }

        public void EditWeightAndBias(ICogTool toolBlock)
        {
            var inputNameWeight = "Weight_" + Name; // For example: Weight_X, Weight_Y1_WeightedByX
            var inputNameBias = "Bias_" + Name;

            EditBlockInput(inputNameWeight, Weight, toolBlock);
            EditBlockInput(inputNameBias, Bias, toolBlock);
        }

        private void EditBlockInput(string inputName, double value, ICogTool toolBlock)
        {
            var blockType = toolBlock as CogToolBlock;
            if (blockType == null) return;

            blockType.Inputs[inputName].Value = value;
        }
    }
}