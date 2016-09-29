using System.Collections.Generic;

namespace AmeCommon.Tasks
{
    public interface ITasksManager
    {
        IList<BackgroundTask> GetPendingTasks();
        void StartTask(BackgroundTask task);
    }
}