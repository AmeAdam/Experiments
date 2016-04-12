using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace AmeCommon.MediaTasks.TaskHandlers
{
    public class MoveDcimCanon : BaseMediaTask
    {
        private readonly string absoluteTargetMov;
        private readonly string absoluteTargetCr2;
        private readonly string absoluteTargetJpg;

        public MoveDcimCanon(DirectoryInfo destinationDirectory, DriveInfo sourceDisk, string movTarget, string cr2Target, string jpgTarget)
            : base(destinationDirectory, sourceDisk, "DCIM")
        {
            absoluteTargetMov = GetTargetPath(movTarget);
            absoluteTargetCr2 = GetTargetPath(cr2Target);
            absoluteTargetJpg = GetTargetPath(jpgTarget);
        }

        public override void Execute()
        {
            Directory.CreateDirectory(absoluteTargetMov);
            Directory.CreateDirectory(absoluteTargetCr2);
            Directory.CreateDirectory(absoluteTargetJpg);

            foreach (var absoluteSource in Directory.GetDirectories(RootSourceDirectory, "*CANON"))
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