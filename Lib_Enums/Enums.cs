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
        ProductMissing = 4
    }

    public enum ResultMode
    {
        OK_NG,
        OK_NG_ProductMissing
    }

    public enum FileOrDirectory
    {
        File,
        Directory
    }
}