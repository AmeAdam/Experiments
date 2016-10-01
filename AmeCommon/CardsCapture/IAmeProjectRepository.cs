using System;
using System.Collections.Generic;
using AmeCommon.Database;
using AmeCommon.Model;

namespace AmeCommon.CardsCapture
{
    public interface IAmeProjectRepository
    {
        void SaveProject(AmeFotoVideoProject project);
        List<AmeFotoVideoProject> AllProjects();
        int CreateProject(DateTime projectDate, string projectName);
        void ChangeRootDirectory();
        AmeLocalSettings LocalSettings { get; set; }
        AmeFotoVideoProject GetProject(string projectPath);
        void RemoveProject(int projectId);
    }
}