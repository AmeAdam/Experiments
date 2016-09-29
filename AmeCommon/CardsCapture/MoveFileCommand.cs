using System;
using System.IO;
using System.Threading;
using AmeCommon.Model;

namespace AmeCommon.CardsCapture
{
    public class MoveFileCommand
    {
        public DriveInfo SourceDrive { get; set; }
        public FileInfo SourceFile { get; set; }
        public DirectoryInfo DestinationRoot { get; set; }
        public MediaFile File { get; set; }
        public bool Completed { get; set; }
        public FileInfo DestinationFile => File.GetDestinationFile(DestinationRoot);

        public void Execute(CancellationToken cancellationToken)
        {
            var buffer = new byte[128 * 1024];
            using (var sourceStream = SourceFile.OpenRead())
            {
                var checkSum = new ChecksumFileCalculator();
                DestinationFile.Directory?.Create();

                using (var destinationStream = DestinationFile.Create())
                {
                    int readCount;
                    while ((readCount = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            throw new ApplicationException("Operacja przerwana przez użytkownika!");
                        destinationStream.Write(buffer, 0, readCount);
                        checkSum.AppendBuffer(buffer, readCount);
                    }
                }
                File.CheckSum = checkSum.GetChecksum();
            }
            Completed = true;
        }

        public void DeleteSourceFile()
        {
            SourceFile.Delete();
        }
    }
}