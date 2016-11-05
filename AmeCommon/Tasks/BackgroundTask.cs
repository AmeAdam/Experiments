using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AmeCommon.Tasks
{
    public abstract class BackgroundTask : IDisposable
    {
        public Guid Id { get; private set; }
        public List<string> Logs { get; set; }
        public TaskState State { get; set; }
        public long SyncPoint { get; set; }
        public Task WorkerTask { get; }
        public event Action<BackgroundTask> OnComplete;
        public Exception Error;
        public DateTime StarTime;
        public DateTime EndTime;
        protected CancellationTokenSource CancelationSource = new CancellationTokenSource();
        public ManualResetEvent WaitHandle { get; } = new ManualResetEvent(false);
        protected CancellationToken CancellationToken { get; }
        public virtual string Label => "unknown"; 

        protected BackgroundTask()
        {
            Id = Guid.NewGuid();
            State = TaskState.Waiting;
            CancellationToken = CancelationSource.Token;
            WorkerTask = new Task(ExecuteInternal, CancellationToken);
        }

        public virtual string Name => GetType().Name;

        protected void UpdateSyncPoint()
        {
            SyncPoint = DateTime.UtcNow.Ticks;
        }

        public virtual IEnumerable<BackgroundTask> ChildTasks
        {
            get { yield break; }
        }

        public bool IsCompleted => State == TaskState.Completed || State == TaskState.Error || State == TaskState.Aborted;

        private void ExecuteInternal()
        {
            try
            {
                StarTime = DateTime.Now;
                State = TaskState.InProgress;
                UpdateSyncPoint();
                Execute();
                if (CancellationToken.IsCancellationRequested)
                    State = TaskState.Aborted;
                else if (ChildTasks.Any(t => t.State == TaskState.Error))
                    State = TaskState.Error;
                else
                    State = TaskState.Completed;
                UpdateSyncPoint();
            }
            catch (Exception ex)
            {
                Error = ex;
                State = TaskState.Error;
                UpdateSyncPoint();
            }
            finally
            {
                EndTime = DateTime.Now;
                UpdateSyncPoint();
                WaitHandle.Set();
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

        public virtual void Dispose()
        {
            CancelationSource.Dispose();
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
