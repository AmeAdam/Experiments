using System.Collections.Generic;

namespace AmeCommon.Tasks
{
    public class TasksManager : ITasksManager
    {
        private readonly object sync = new object();
        private readonly List<BackgroundTask> pendingTasks = new List<BackgroundTask>();

        public IList<BackgroundTask> GetPendingTasks()
        {
            lock (sync)
            {
                return new List<BackgroundTask>(pendingTasks);
            }
        }

        public void StartTask(BackgroundTask task)
        {
            lock (sync)
            {
                task.OnComplete += TaskCompleteHandler;
                pendingTasks.Add(task);
                task.ExecuteAsync();
            }
        }

        private void TaskCompleteHandler(BackgroundTask task)
        {
        }
    }
}
