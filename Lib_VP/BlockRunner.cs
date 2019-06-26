using System;
using System.Collections.Generic;
using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.ToolBlock;
using Lib_Enums;
using Lib_Logger;

namespace Lib_VP
{
    public class BlockRunner
    {
        public BlockRunner(ResultMode resultMode = ResultMode.OK_NG_ProductMissing_Error,
            Func<ICogTool, bool> productMissingCondition = null)
        {
            _resultMode = resultMode;

            // If no productMissingCondition is given, set a default one
            if (productMissingCondition == null)
                _productMissingCondition = block =>
                {
                    var toolBlock = (CogToolBlock) block;
                    CogPMAlignTool pmAlignTool = null;
                    foreach (ICogTool tool in toolBlock.Tools)
                    {
                        if (!(tool is CogPMAlignTool alignTool)) continue;
                        pmAlignTool = alignTool;
                        break;
                    }

                    if (pmAlignTool == null)
                        throw new NullReferenceException(
                            "Make sure there is a PMAlignTool in your block for telling whether the product is missing");

                    return pmAlignTool.Results.Count == 0;
                };
            else
                _productMissingCondition = productMissingCondition;
        }

        #region Events

        public event Action<ICogTool, RunResult> Ran;

        #endregion

        private void OnRan(ICogTool tool)
        {
            Ran?.Invoke(tool, LastRunResult);
        }

        public void RunToolBlock(ICogTool tool)
        {
            tool.Run();
            var cogRunResult = tool.RunStatus.Result;

            LastRunResult = GetLastRunResult(cogRunResult, tool);

            OnRan(tool);
            foreach (var logger in Loggers) logger.Log(tool, LastRunResult);
        }

        private RunResult GetLastRunResult(CogToolResultConstants cogRunResult, ICogTool tool)
        {
            if (_resultMode == ResultMode.OK_NG_Error)
                switch (cogRunResult)
                {
                    case CogToolResultConstants.Accept:
                        return RunResult.OK;
                    case CogToolResultConstants.Error:
                        return RunResult.Error;
                    default:
                        return RunResult.NG;
                }

            if (cogRunResult == CogToolResultConstants.Accept) return RunResult.OK;
            if (_productMissingCondition(tool)) return RunResult.ProductMissing;
            return cogRunResult == CogToolResultConstants.Error ? RunResult.Error : RunResult.NG;
        }

        #region Fields

        private readonly Func<ICogTool, bool> _productMissingCondition;
        private readonly ResultMode _resultMode;
        public List<IToolBlockLogger> Loggers { get; } = new List<IToolBlockLogger>();
        public RunResult LastRunResult { get; private set; }

        #endregion
    }
}