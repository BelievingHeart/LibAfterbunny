using System;
using NUnit.Framework;

namespace Lib_VP.Tests
{
    [TestFixture]
    public class RectifierTests
    {
        private Rectifier.Rectifier CreateRectifier()
        {
            return new Rectifier.Rectifier(new OMMDataParserStub());
        }

        [Test]
        public void Rectify_WhenCollectedDataNotEnough_ThrowsInvalidOperationException()
        {
            var rectifier = CreateRectifier();

            Assert.Catch<InvalidOperationException>(() => rectifier.Rectify(null));
        }

        [Test]
        public void Rectify_WhenDataGirdIsNull_ThrowsInvalidOperationException()
        {
            var rectifier = CreateRectifier();

            Assert.Catch<InvalidOperationException>(() => rectifier.Rectify(null));
        }
    }
}