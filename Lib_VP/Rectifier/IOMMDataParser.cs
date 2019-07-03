using System.Collections.Generic;

namespace Lib_VP.Rectifier
{
    public interface IOMMDataParser
    {
        List<DataColumn> Parse(int numLines);
    }
}