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
        AmeFotoVideoProject CreateProject(DateTime projectDate, string projectName);
        void ChangeRootDirectory();
        AmeLocalSettings LocalSettings { get; set; }
        AmeFotoVideoProject GetProject(string projectPath);
    }
}