using System.Collections.Generic;
using Lib_VP.Rectifier;

namespace Lib_VP.Tests
{
    public class OMMDataParserStub : IOMMDataParser
    {
        public List<DataColumn> Parse(int numLines)
        {
            return new List<DataColumn>
                {OMMData.X1, OMMData.X2, OMMData.Y1_WeightedByX, OMMData.Y2_WeightedByX, OMMData.Angle};
        }
    }
}