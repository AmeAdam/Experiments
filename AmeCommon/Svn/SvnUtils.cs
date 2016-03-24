using AmeCommon.Settings;
using SharpSvn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeCommon.Svn
{
    internal class SvnUtils : IDisposable
    {
        ISettingsProvider settings;
        SvnClient svn;

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


        private void CreateSvn(string dir)
        {
            SvnClient svn = new SvnClient();

            if (IsDirAlreadyInSvn(dir))
                return;

            if (IsDirAlreadyInSvn(Path.GetDirectoryName(dir)))
            {
                CommitDirectory(dir);
                return;
            }

            AddDirToSvn(dir);
        }

        private void AddDirToSvn(string dir)
        { 
            var path = settings.SvnSettings.RootPath + Path.GetFileName(dir);
            Uri uri; Guid id;
            if (svn.TryGetRepository(dir, out uri, out id))
                return;


            svn.RemoteCreateDirectories(new[] { new Uri(path), }, new SvnCreateDirectoryArgs
            {
                LogMessage = "Nowy projekt"
            });
            svn.CheckOut(new SvnUriTarget(path), dir);
            svn.SetProperty(dir, "svn:ignore", "A\nB\nC\nCanon\nZdjecia");

            var rawPath = Path.Combine(dir, "ZdjeciaRaw");
            Directory.CreateDirectory(rawPath);

            svn.Add(rawPath);
            svn.SetProperty(rawPath, "svn:ignore", "*.CR2");

            svn.Commit(dir, new SvnCommitArgs { LogMessage = "add ignores", Depth = SvnDepth.Children });
            Console.WriteLine("SVN created");
        }

        private void CommitDirectory(string dir)
        {
            svn.Add(dir, new SvnAddArgs
            {
                Depth = SvnDepth.Empty
            });
            svn.SetProperty(dir, "svn:ignore", "A\nB\nC\nCanon\nZdjecia\nZdjeciaRaw");
            svn.Commit(dir, new SvnCommitArgs { Depth = SvnDepth.Empty, LogMessage = "Nowy projekt" });
            Console.WriteLine("SVN created");
        }

        public void Dispose()
        {
            svn.Dispose();
        }
    }
}
