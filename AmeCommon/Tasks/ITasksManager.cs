using System;
using System.Collections.Generic;

namespace AmeCommon.Tasks
{
    public interface ITasksManager
    {
        IList<BackgroundTask> GetPendingTasks();
        IList<BackgroundTask> GetAllTasks();
        void StartTask(BackgroundTask task);
        BackgroundTask GetTask(Guid id);
    }
}