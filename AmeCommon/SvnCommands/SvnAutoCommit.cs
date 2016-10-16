using System.IO;
using System.Threading;
using AmeCommon.Database;
using AmeCommon.Tasks;
using Microsoft.Extensions.Options;
using SharpSvn;
using SharpSvn.Security;

namespace AmeCommon.SvnCommands
{
    public class SvnAutoCommit : BackgroundTask
    {
        private readonly DirectoryInfo directory;
        public override string Name => "Automatyczna synchronizacja katalogu " + directory.FullName;
        private readonly FileSystemWatcher watcher;
        private readonly ManualResetEvent wait = new ManualResetEvent(false);
        private readonly SvnClient svn;

        public SvnAutoCommit(IOptions<AmeConfig> config, DirectoryInfo directory)
        {
            this.directory = directory;
            watcher = new FileSystemWatcher(directory.FullName);
            watcher.Changed += Commit;
            svn = new SvnClient();
            svn.Authentication.UserNamePasswordHandlers += delegate (object sender, SvnUserNamePasswordEventArgs args)
            {
                args.UserName = config.Value.SvnUser;
                args.Password = config.Value.SvnPassword;
            };
        }

        private void Commit(object sender, FileSystemEventArgs e)
        {
            svn.Commit(directory.FullName);
        }

        protected override void Execute()
        {
            wait.WaitOne();
        }

        public override void Dispose()
        {
            wait.Set();
            svn.Dispose();
            watcher.Dispose();
        }
    }
}
