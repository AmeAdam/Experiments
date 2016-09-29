using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AmeCommon.Tasks
{
    public abstract class BackgroundTask
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Logs { get; set; }
        public TaskState State { get; set; }
        public Task WorkerTask { get; }
        public event Action<BackgroundTask> OnComplete;
        public Exception Error;
        public DateTime StarTime;
        public DateTime EndTime;
        protected CancellationTokenSource CancelationSource = new CancellationTokenSource();
        protected CancellationToken CancellationToken { get; }

        protected BackgroundTask()
        {
            State = TaskState.Waiting;
            CancellationToken = CancelationSource.Token;
            WorkerTask = new Task(ExecuteInternal, CancellationToken);
        }


        private void ExecuteInternal()
        {
            try
            {
                StarTime = DateTime.Now;
                State = TaskState.InProgress;
                Execute();
                State = CancellationToken.IsCancellationRequested ? TaskState.Aborted : TaskState.Completed;
            }
            catch (Exception ex)
            {
                Error = ex;
                State = TaskState.Error;
            }
            finally
            {
                EndTime = DateTime.Now;
                OnOnComplete();
            }
        }

        public void Cancel()
        {
            CancelationSource.Cancel();
        }

        protected abstract void Execute();

        public Task ExecuteAsync()
        {
            WorkerTask.Start();
            return WorkerTask;
        }

        protected virtual void OnOnComplete()
        {
            OnComplete?.Invoke(this);
        }
    }

    public enum TaskState
    {
        Waiting,
        InProgress,
        Completed,
        Aborted,
        Error
    }
}
