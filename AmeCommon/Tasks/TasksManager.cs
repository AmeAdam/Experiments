using System.Collections.Generic;
using System.Linq;

namespace AmeCommon.Tasks
{
    public class TasksManager : ITasksManager
    {
        private readonly object sync = new object();
        private readonly List<BackgroundTask> allTasks = new List<BackgroundTask>();

        public IList<BackgroundTask> GetPendingTasks()
        {
            lock (sync)
            {
                return allTasks.Where(t => !t.IsCompleted).ToList();
            }
        }

        public IList<BackgroundTask> GetAllTasks()
        {
            lock (sync)
            {
                return new List<BackgroundTask>(allTasks);
            }
        }

        public void StartTask(BackgroundTask task)
        {
            lock (sync)
            {
                task.OnComplete += TaskCompleteHandler;
                allTasks.Add(task);
                task.ExecuteAsync();
            }
        }

        private void TaskCompleteHandler(BackgroundTask task)
        {
        }
    }
}
