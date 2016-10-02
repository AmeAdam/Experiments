using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly AmeFotoVideoProject project;
        private readonly SvnClient svn;

        public AddProjectToSvnCommand(IOptions<AmeConfig> config, AmeFotoVideoProject project)
        {
            this.config = config;
            this.project = project;
            svn = new SvnClient();
            svn.Authentication.UserNamePasswordHandlers += delegate(object sender, SvnUserNamePasswordEventArgs args)
            {
                args.UserName = config.Value.SvnUser;
                args.Password = config.Value.SvnPassword;
            };
        }

        protected override void Execute()
        {
            var uri = new Uri(config.Value.SvnRoot + project.UniqueName);
            if (!svn.RemoteCreateDirectory(uri, new SvnCreateDirectoryArgs {LogMessage = project.UniqueName}))
                throw new ApplicationException($"Nie można utworzyć repozytorium SVN {uri}");
            if (!svn.CheckOut(uri, project.LocalPathRoot))
                throw new ApplicationException($"Nie można przypisać katalogu {project.LocalPathRoot} do repozytorium SVN {uri}");
            svn.SetProperty(project.LocalPathRoot, "svn:ignore", string.Join("\r\n", GetIgnores()));
            svn.Commit(project.LocalPathRoot, new SvnCommitArgs {Depth = SvnDepth.Children, LogMessage = "Dodanie listy ignorowanych plików"});

            AddProjectFiles();
            AddPodklady();
            AddZdjeciaRaw();
        }

        private void AddZdjeciaRaw()
        {
            var zdjeciaRaw = Path.Combine(project.LocalPathRoot, "ZdjeciaRaw");
            if (Directory.Exists(zdjeciaRaw))
            {
                svn.SetProperty(zdjeciaRaw, "svn:ignore", "*.CR2\r\n*.NEF");
                svn.Commit(zdjeciaRaw, new SvnCommitArgs { Depth = SvnDepth.Empty, LogMessage = "Dodanie listy ignorowanych plików" });
            }
        }

        private void AddPodklady()
        {
            string podklady = Path.Combine(project.LocalPathRoot, "Podkłady");
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
            foreach (var prproj in Directory.GetFiles(project.LocalPathRoot, "*.prproj"))
                yield return prproj;
            foreach (var json in Directory.GetFiles(project.LocalPathRoot, "*.json"))
                yield return json;
        }

        private List<string> GetIgnores()
        {
            var ignoreList = Directory.GetDirectories(project.LocalPathRoot)
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
            ignoreList.Add("pluraleyes");
            ignoreList.Add("pluraleyes");
            return ignoreList;
        }

        public override void Dispose()
        {
            svn.Dispose();
        }
    }
}
