using System;

namespace Lib_Enums
{
    public enum TriggerType
    {
        RisingEdge,
        FallingEdge
    }

    public enum LogScenario
    {
        Always,
        ExceptOK
    }

    [Flags]
    public enum RunResult
    {
        OK = 1,
        NG = 2,
        ProductMissing = 4,
        Error = 8
    }

    public enum ResultMode
    {
        OK_NG_Error,
        OK_NG_ProductMissing_Error
    }

    public enum FileOrDirectory
    {
        File,
        Directory
    }
}