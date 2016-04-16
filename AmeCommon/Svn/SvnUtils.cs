using AmeCommon.Settings;
using SharpSvn;
using System;
using System.IO;

namespace AmeCommon.Svn
{
    public class SvnUtils : ISvnUtils, IDisposable
    {
        private readonly ISettingsProvider settings;
        private readonly SvnClient svn;

        public SvnUtils(ISettingsProvider settings)
        {
            this.settings = settings;
            svn = new SvnClient();
        }

        private bool IsDirAlreadyInSvn(string dir)
        {
            Uri uri; Guid id;
            return svn.TryGetRepository(dir, out uri, out id);
        }


        public void CreateSvn(string dir)
        {
            var parentDir = Path.GetDirectoryName(dir);
            if (!IsDirAlreadyInSvn(parentDir))
                AddDirToSvn(dir);

            if (!IsDirAlreadyInSvn(dir))
                svn.Add(dir, new SvnAddArgs { Depth = SvnDepth.Empty });

            ApplySvnProperties(dir);

            foreach (var file in Directory.GetFiles(dir, "*.prproj"))
                svn.Add(file);

            svn.Commit(dir, new SvnCommitArgs { LogMessage = "Nowy projekt", Depth = SvnDepth.Children });
        }

        private void AddDirToSvn(string dir)
        { 
            if (IsDirAlreadyInSvn(dir))
                return;

            var svnUri = settings.SvnSettings.RootPath + Path.GetFileName(dir);
            CreateDirectoryOnSvnServer(svnUri);
            svn.CheckOut(new SvnUriTarget(svnUri), dir);
        }

        private void CreateDirectoryOnSvnServer(string uri)
        {
            svn.RemoteCreateDirectories(new[] { new Uri(uri), }, new SvnCreateDirectoryArgs
            {
                LogMessage = "Nowy projekt"
            });
        }

        private void ApplySvnProperties(string dir)
        {
            foreach(var svnProperty in settings.SvnSettings.SvnProperties)
            {
                var path = Path.Combine(dir, svnProperty.Path);
                if (Directory.Exists(path))
                {
                    if (!IsDirAlreadyInSvn(dir))
                        svn.Add(dir);
                    svn.SetProperty(path, svnProperty.Name, svnProperty.Value);
                }
            }
        }

        public void Dispose()
        {
            svn.Dispose();
        }
    }
}
