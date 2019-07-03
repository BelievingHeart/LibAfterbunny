using System;
using Lib_VP.Rectifier;
using NUnit.Framework;

namespace Lib_VP.IntegrationTests
{
    [TestFixture]
    public class OMMDataParserTests
    {
        [Test]
        public void parse_NeverFail()
        {
            var parser = new OMMDataParser(@"C:\Users\rocke\Desktop\samples.csv", "Result");

            var columns = parser.Parse(2);

            foreach (var column in columns) Console.WriteLine($"{column.Name}: {column[0]}, {column[1]}");
        }
    }
}