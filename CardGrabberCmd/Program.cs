using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;
using SharpSvn;

namespace CardGrabberCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = args[0]; //Environment.CurrentDirectory;
            Console.WriteLine("Katalog docelowy: "+dir);
            CreateSvn(dir);

            var drives = DriveInfo.GetDrives().Where(d => File.Exists(d + "ame.xml"));

            Parallel.ForEach(drives, d => ProcessCard(d, dir));

            //FileSystem.CopyDirectory(sourcePath, destinationPath, UIOption.AllDialogs);
            Console.WriteLine("Przetwarzanie zakończone");
            Console.ReadLine();
        }

        private static void ProcessCard(DriveInfo drive, string dir)
        {
            XDocument doc = XDocument.Load(drive + "ame.xml");
            if (doc.Root == null)
                return;
            var cardName = (string)doc.Root.Element("name");

            Console.WriteLine("Przetwarzanie karty: " + cardName);
            foreach (var destDirNode in doc.Root.Elements("dest-dir"))
            {
                var destDir = Path.Combine(dir, (string)destDirNode.Attribute("name"));
                Directory.CreateDirectory(destDir);
                foreach (var sourceDirNode in destDirNode.Elements("source-dir"))
                {
                    var sourcePath = drive + (string)sourceDirNode;
                    FileSystem.MoveDirectory(sourcePath, Path.Combine(destDir, Path.GetFileName((string)sourceDirNode)), UIOption.AllDialogs);
                }

                foreach (var sourceDirNode in destDirNode.Elements("source-cam-foto"))
                {
                    var sourcePath = drive + (string)sourceDirNode;
                    foreach (var subSourceDirectory in Directory.GetDirectories(sourcePath))
                    {
                        foreach (var file in Directory.GetFiles(subSourceDirectory))
                        {
                            File.Move(file, Path.Combine(destDir, Path.GetFileName(file)));
                        }
                        Directory.Delete(subSourceDirectory);
                    }
                }
            }
        }

        private static bool IsRepo(SvnClient svn, string dir)
        {
            Uri uri; Guid id;
            return svn.TryGetRepository(dir, out uri, out id);
        } 


        private static void CreateSvn(string dir)
        {
            SvnClient svn = new SvnClient();

            if (IsRepo(svn, dir))
                return;

            var parent = Path.GetDirectoryName(dir);
            if (IsRepo(svn, parent))
            {
                AddFromParent(svn, dir);
                return;
            }

            var path = "svn://video-bydgoszcz.pl/video/" + Path.GetFileName(dir);
            Uri uri; Guid id;
            if (svn.TryGetRepository(dir, out uri, out id))
                return;

            
            svn.RemoteCreateDirectories(new[] { new Uri(path),  }, new SvnCreateDirectoryArgs
            {
                LogMessage = "Nowy projekt"
            });
            svn.CheckOut(new SvnUriTarget(path), dir);
            svn.SetProperty(dir, "svn:ignore", "A\nB\nC\nCanon\nZdjecia\nZdjeciaRaw\nCardGrabberCmd.exe");
            svn.Commit(dir, new SvnCommitArgs {LogMessage = "add ignores", Depth = SvnDepth.Empty});
            Console.WriteLine("SVN created");
        }

        private static void AddFromParent(SvnClient svn, string dir)
        {
            svn.Add(dir, new SvnAddArgs
            {
                Depth = SvnDepth.Empty
            });
            svn.SetProperty(dir, "svn:ignore", "A\nB\nC\nCanon\nZdjecia\nZdjeciaRaw\nCardGrabberCmd.exe");
            svn.Commit(dir, new SvnCommitArgs {Depth = SvnDepth.Empty, LogMessage = "Nowy projekt"});
            Console.WriteLine("SVN created");
        }
    }
}
