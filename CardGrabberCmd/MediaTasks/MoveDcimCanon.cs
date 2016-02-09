using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace CardGrabberCmd.MediaTasks
{
    public class MoveDcimCanon : BaseMediaTask
    {
        private readonly Media parent;
        private const string RelativeSource = @"DCIM";
        private readonly string relativeTargetMov;
        private readonly string relativeTargetCr2;
        private readonly string relativeTargetJpg;

        public MoveDcimCanon(Media parent, XElement settings)
        {
            this.parent = parent;
            relativeTargetMov = (string)settings.Element(XName.Get("target-mov", Media.AmeNamespace));
            relativeTargetCr2 = (string)settings.Element(XName.Get("target-cr2", Media.AmeNamespace));
            relativeTargetJpg = (string)settings.Element(XName.Get("target-jpg", Media.AmeNamespace));
        }

        public override void Execute()
        {
            var absoluteSourceRoot = Path.Combine(parent.SourceDisk.Name, RelativeSource);
            var absoluteTargetMov = Path.Combine(parent.DestinationFolder.FullName, relativeTargetMov);
            var absoluteTargetCr2 = Path.Combine(parent.DestinationFolder.FullName, relativeTargetCr2);
            var absoluteTargetJpg = Path.Combine(parent.DestinationFolder.FullName, relativeTargetJpg);

            Directory.CreateDirectory(absoluteTargetMov);
            Directory.CreateDirectory(absoluteTargetCr2);
            Directory.CreateDirectory(absoluteTargetJpg);

            foreach (var absoluteSource in Directory.GetDirectories(absoluteSourceRoot, "*CANON"))
            {
                MoveAllFiles(absoluteSource, absoluteTargetJpg, "*.jpg");
                MoveAllFiles(absoluteSource, absoluteTargetJpg, "*.jpeg");
                MoveAllFiles(absoluteSource, absoluteTargetMov, "*.MOV");
                ProcessCr2Files(absoluteSource, absoluteTargetJpg);
            }
        }

        private void ProcessCr2Files(string absoluteSource, string absoluteTragetJpg)
        {
            foreach (var cr2File in Directory.GetFiles(absoluteSource, "*.CR2"))
            {
                var destCr2Path = Path.Combine(absoluteTragetJpg, Path.GetFileName(cr2File) ?? "");
                File.Move(cr2File, destCr2Path);

                var bmpDec = BitmapDecoder.Create(new Uri(destCr2Path), BitmapCreateOptions.DelayCreation,
                    BitmapCacheOption.None);
                var bmpEnc = new JpegBitmapEncoder();
                bmpEnc.QualityLevel = 80;
                bmpEnc.Frames.Add(bmpDec.Frames[0]);
                var destJpgPath = Path.Combine(absoluteTragetJpg, Path.GetFileNameWithoutExtension(destCr2Path) + ".jpg");
                using (var ms = File.Create(destJpgPath))
                {
                    bmpEnc.Save(ms);
                }
            }
        }
    }
}