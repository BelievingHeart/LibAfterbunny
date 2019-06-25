using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lib_IO
{
    public class CommunicatorAdaptor
    {
        private ICommunicator _communicator;
        private AutoResetEvent _cond = new AutoResetEvent(false);
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public event EventHandler TriggerReceived;

        protected virtual void OnTriggerReceived()
        {
            TriggerReceived?.Invoke(this, EventArgs.Empty);
        }

        private void ExecutionLoops()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cond.WaitOne();
                OnTriggerReceived();
            }
        }

        public void StartListening()
        {
            _communicator.StartListening();
            Task.Factory.StartNew(ExecutionLoops, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public CommunicatorAdaptor(ICommunicator communicator)
        {
            _communicator = communicator;
            _communicator.Triggered += (sender, args) => _cond.Set();
        }
        
        public void EndListening()
        {
            _cancellationTokenSource.Cancel();
            _cond.Set();
            _cancellationTokenSource.Dispose();
        }
    }
}