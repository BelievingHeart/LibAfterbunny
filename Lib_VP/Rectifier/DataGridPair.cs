namespace Lib_VP.Rectifier
{
    public class DataGridPair
    {
        public DataColumn MillimeterColumn, PixelColumn;
        public string PairName;

        public DataGridPair(string pairName, DataColumn millimeterColumn, DataColumn pixelColumn)
        {
            PairName = pairName;
            MillimeterColumn = millimeterColumn;
            PixelColumn = pixelColumn;
        }
    }
}