using System;
using System.ComponentModel;
using Cognex.VisionPro;

namespace Lib_VP.Tests
{
    public class CogToolBlockMock : ICogTool
    {
        public bool RunHasBeenCalled { get; private set; }

        public void SuspendChangedEvent()
        {
            throw new NotImplementedException();
        }

        public void ResumeAndRaiseChangedEvent()
        {
            throw new NotImplementedException();
        }

        public int ChangedEventSuspended { get; }
        public StateFlagsCollection StateFlags { get; }
        public event CogChangedEventHandler Changed;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ISite Site { get; set; }
        public event EventHandler Disposed;

        public ICogRecord CreateCurrentRecord()
        {
            throw new NotImplementedException();
        }

        public ICogRecord CreateLastRunRecord()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            RunHasBeenCalled = true;
        }

        public CogDictionary UserData { get; }
        public string Name { get; set; }
        public ICogRunStatus RunStatus { get; set; }
        public CogDataBindingsCollection DataBindings { get; }
        public event EventHandler Running;
        public event EventHandler Ran;
    }
}