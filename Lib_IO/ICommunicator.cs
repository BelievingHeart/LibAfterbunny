using System;
using System.Threading.Tasks;
using Lib_Enums;

namespace Lib_IO
{
    public interface ICommunicator
    {
        event EventHandler Triggered;
        Task ReportPLCAsync(RunResult runResult);
        void ReportPLC(RunResult runResult);
        void StartListening();
    }
}