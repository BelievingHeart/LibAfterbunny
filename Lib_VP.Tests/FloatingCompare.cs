using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Lib_VP.Tests
{
    [TestFixture]
    public class FloatingCompare
    {
        internal static double Epslon { get; } = 0.0001;

        internal static bool FloatingEnumerablesAreVeryClose(IEnumerable<double> actual, IEnumerable<double> expected)
        {
            foreach (var twoNumbersAreClose in actual.Zip(expected, (d, d1) => FloatingNumberAreClose(d, d1)))
                if (!twoNumbersAreClose)
                    return false;

            return true;
        }

        internal static bool FloatingNumberAreClose(double actual, double expected)
        {
            return Math.Abs(actual - expected) < Epslon;
        }

        [Test]
        public void _DoublePermutationsAreVeryClose_WhenPermutationsAreClose_ReturnTrue()
        {
            var per1 = new List<double> {1, 2, 3};
            var proportion = 0.1;
            var per2 = new List<double> {1 + Epslon * proportion, 2 + Epslon * proportion, 3 + Epslon * proportion};

            var isPermutationsClose = FloatingEnumerablesAreVeryClose(per1, per2);

            Assert.True(isPermutationsClose);
        }
    }
}