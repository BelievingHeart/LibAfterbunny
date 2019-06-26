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
        [TestCase(CogToolResultConstants.Error, RunResult.Error)]
        public void RunToolBlock_3PossibleResults(CogToolResultConstants blockResult, RunResult expectedRunResult)
        {
            var blockMock = BlockRunnerMockFactory.CreateCogToolMock(blockResult);
            var blockMockTyped = (CogToolBlockMock) blockMock;
            var blockRunner = new BlockRunner(ResultMode.OK_NG_Error);

            blockRunner.RunToolBlock(blockMock);

            Assert.True(blockRunner.LastRunResult == expectedRunResult);
            Assert.True(blockMockTyped.RunHasBeenCalled);
        }

        [TestCase(CogToolResultConstants.Accept, RunResult.OK)]
        [TestCase(CogToolResultConstants.Error, RunResult.Error)]
        [TestCase(CogToolResultConstants.Reject, RunResult.NG)]
        public void RunToolBlock_4PossibleResultsAndProductPresent(CogToolResultConstants blockResult,
            RunResult expectedRunResult)
        {
            var blockMock = BlockRunnerMockFactory.CreateCogToolMock(blockResult);
            var blockMockTyped = (CogToolBlockMock) blockMock;
            var blockRunner = new BlockRunner(ResultMode.OK_NG_ProductMissing_Error, tool => false);

            blockRunner.RunToolBlock(blockMock);

            Assert.True(blockRunner.LastRunResult == expectedRunResult);
            Assert.True(blockMockTyped.RunHasBeenCalled);
        }

        [TestCase(CogToolResultConstants.Accept, RunResult.OK)]
        [TestCase(CogToolResultConstants.Error, RunResult.ProductMissing)]
        [TestCase(CogToolResultConstants.Reject, RunResult.ProductMissing)]
        public void RunToolBlock_4PossibleResultsAndProductMissing(CogToolResultConstants blockResult,
            RunResult expectedRunResult)
        {
            var blockMock = BlockRunnerMockFactory.CreateCogToolMock(blockResult);
            var blockMockTyped = (CogToolBlockMock) blockMock;
            var blockRunner = new BlockRunner(ResultMode.OK_NG_ProductMissing_Error, tool => true);

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