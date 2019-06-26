using System;
using Cognex.VisionPro;
using Lib_Enums;

namespace Lib_VP
{
    public interface IToolBlockLogger
    {
        event EventHandler Logged;
        void Log(ICogTool tool, RunResult runResult);
    }
}