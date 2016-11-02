using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.Database;
using AmeCommon.Model;
using AmeCommon.Tasks;
using Microsoft.Extensions.Options;
using SharpSvn;
using SharpSvn.Security;

namespace AmeCommon.CardsCapture
{
    public class AddProjectToSvnCommand : BackgroundTask
    {
        private readonly IOptions<AmeConfig> config;
        public readonly AmeFotoVideoProject Project;
        private readonly SvnClient svn;
        public string SvnUri => config.Value.SvnRoot + Project.UniqueName;
        public override string Name => $"Tworzenie repozytorium SVN {SvnUri}";
        public override string Label => "svn";

        public AddProjectToSvnCommand(IOptions<AmeConfig> config, AmeFotoVideoProject project)
        {
            this.config = config;
            this.Project = project;
            svn = new SvnClient();
            svn.Authentication.UserNamePasswordHandlers += delegate(object sender, SvnUserNamePasswordEventArgs args)
            {
                args.UserName = config.Value.SvnUser;
                args.Password = config.Value.SvnPassword;
            };
        }

        public AddProjectToSvnCommand(IOptions<AmeConfig> config) : this(config, new AmeFotoVideoProject())
        {
        }

        protected override void Execute()
        {
            var uri = new Uri(config.Value.SvnRoot + Project.UniqueName);
            if (!svn.RemoteCreateDirectory(uri, new SvnCreateDirectoryArgs {LogMessage = Project.UniqueName}))
                throw new ApplicationException($"Nie można utworzyć repozytorium SVN {uri}");
            if (!svn.CheckOut(uri, Project.LocalPathRoot))
                throw new ApplicationException($"Nie można przypisać katalogu {Project.LocalPathRoot} do repozytorium SVN {uri}");
            svn.SetProperty(Project.LocalPathRoot, "svn:ignore", string.Join("\r\n", GetIgnores()));
            svn.Commit(Project.LocalPathRoot, new SvnCommitArgs {Depth = SvnDepth.Children, LogMessage = "Dodanie listy ignorowanych plików"});

            AddProjectFiles();
            AddPodklady();
            AddZdjeciaRaw();
            Project.SvnRepository = uri.ToString();
        }

        private void AddZdjeciaRaw()
        {
            var zdjeciaRaw = Path.Combine(Project.LocalPathRoot, "ZdjeciaRaw");
            if (Directory.Exists(zdjeciaRaw))
            {
                svn.SetProperty(zdjeciaRaw, "svn:ignore", "*.CR2\r\n*.NEF");
                svn.Commit(zdjeciaRaw, new SvnCommitArgs { Depth = SvnDepth.Empty, LogMessage = "Dodanie listy ignorowanych plików" });
            }
        }

        private void AddPodklady()
        {
            string podklady = Path.Combine(Project.LocalPathRoot, "Podkłady");
            svn.Add(podklady, SvnDepth.Infinity);
            svn.Commit(podklady, new SvnCommitArgs { Depth = SvnDepth.Infinity, LogMessage = "Dodanie startowych podkładów"});
        }

        private void AddProjectFiles()
        {
            var filesToAdd = FilesToCommit().ToList();
            foreach (var file in filesToAdd)
                svn.Add(file);
            svn.Commit(filesToAdd, new SvnCommitArgs { Depth = SvnDepth.Empty, LogMessage = "Pliki startowe" });
        }

        private IEnumerable<string> FilesToCommit()
        {
            foreach (var prproj in Directory.GetFiles(Project.LocalPathRoot, "*.prproj"))
                yield return prproj;
            foreach (var json in Directory.GetFiles(Project.LocalPathRoot, "*.json"))
                yield return json;
        }

        private List<string> GetIgnores()
        {
            var ignoreList = Directory.GetDirectories(Project.LocalPathRoot)
                .Select(Path.GetDirectoryName)
                .ToList();
            ignoreList.Remove("ZdjeciaRaw");
            ignoreList.Add("pluraleyes");
            ignoreList.Add("*.bak");
            ignoreList.Add("*.psd");
            ignoreList.Add("*.ncor");
            ignoreList.Add("Adobe Premiere Pro Auto-Save");
            ignoreList.Add("gotowe");
            ignoreList.Add("encore - dvd");
            return ignoreList;
        }

        public override void Dispose()
        {
            svn.Dispose();
        }
    }
}
