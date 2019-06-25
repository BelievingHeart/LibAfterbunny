using Cognex.VisionPro;
using Cognex.VisionPro.Implementation;
using Lib_Enums;
using NUnit.Framework;

namespace Lib_VP.Tests
{
    [TestFixture]
    public class BlockRunnerTests
    {
        [TestCase(CogToolResultConstants.Accept, RunResult.OK)]
        [TestCase(CogToolResultConstants.Reject, RunResult.NG)]
        [TestCase(CogToolResultConstants.Error, RunResult.NG)]
        public void RunToolBlock_2PossibleResults(CogToolResultConstants blockResult, RunResult expectedRunResult)
        {
            var blockMock = BlockRunnerMockFactory.CreateCogToolMock(blockResult);
            var blockMockTyped = (CogToolBlockMock) blockMock;
            var blockRunner = new BlockRunner(ResultMode.OK_NG);

            blockRunner.RunToolBlock(blockMock);

            Assert.True(blockRunner.LastRunResult == expectedRunResult);
            Assert.True(blockMockTyped.RunHasBeenCalled);
        }

        [TestCase(CogToolResultConstants.Accept, false, RunResult.OK)]
        [TestCase(CogToolResultConstants.Error, true, RunResult.ProductMissing)]
        [TestCase(CogToolResultConstants.Error, false, RunResult.NG)]
        [TestCase(CogToolResultConstants.Reject, false, RunResult.NG)]
        public void RunToolBlock_3PossibleResults(CogToolResultConstants blockResult, bool productIsMissing,
            RunResult expectedRunResult)
        {
            var blockMock = BlockRunnerMockFactory.CreateCogToolMock(blockResult);
            var blockMockTyped = (CogToolBlockMock) blockMock;
            var blockRunner = new BlockRunner(ResultMode.OK_NG_ProductMissing, tool => productIsMissing);

            blockRunner.RunToolBlock(blockMock);

            Assert.True(blockRunner.LastRunResult == expectedRunResult);
            Assert.True(blockMockTyped.RunHasBeenCalled);
        }
    }

    internal static class BlockRunnerMockFactory
    {
        public static ICogTool CreateCogToolMock(CogToolResultConstants resultConstant)
        {
            var output = new CogToolBlockMock
            {
                RunStatus = new CogRunStatus(resultConstant, "test", 0, 0, null)
            };
            return output;
        }
    }
}