using NUnit.Framework;
using System.IO;
using AmeCommon.MediaTasks;
using AmeCommon.MediaTasks.TaskHandlers;

namespace AmeTests.MovingFilesFromCards
{
    [TestFixture]
    public class MoveDirectoryContentText
    {
        private string sourceDirectory;
        private string targetDirectory;

        [SetUp]
        public void SetUp()
        {
            sourceDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            targetDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(sourceDirectory);
            Directory.CreateDirectory(targetDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(sourceDirectory, true);
            Directory.Delete(targetDirectory, true);
        }

        private string AddFile(string content, params string[] fileRelativePath)
        {
            var filePath = Path.Combine(sourceDirectory, string.Join("\\", fileRelativePath));
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "");
            File.WriteAllText(filePath, content);
            return filePath;
        }

        [Test]
        public void MoveFiles()
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            var srcFile2 = AddFile(@"def", "file2.temp");

            var command = new MoveDirectoryContent(sourceDirectory, "Tempy");
            command.Execute(new DestinationDirectoryHandler(targetDirectory));

            Assert.IsFalse(File.Exists(srcFile1));
            Assert.IsFalse(File.Exists(srcFile2));

            var targetFile1 = Path.Combine(targetDirectory, "Tempy", "file1.temp");
            var targetFile2 = Path.Combine(targetDirectory, "Tempy", "file2.temp");

            Assert.IsTrue(File.Exists(targetFile1));
            Assert.IsTrue(File.Exists(targetFile2));

            Assert.AreEqual("abc", File.ReadAllText(targetFile1));
            Assert.AreEqual("def", File.ReadAllText(targetFile2));
        }

        [Test]
        public void SameFileExist()
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            Directory.CreateDirectory(Path.Combine(targetDirectory, "Tempy"));
            var targetFile1 = Path.Combine(targetDirectory, "Tempy", "file1.temp");
            File.WriteAllText(targetFile1, @"abc");

            var command = new MoveDirectoryContent(sourceDirectory, "Tempy");
            command.Execute(new DestinationDirectoryHandler(targetDirectory));

            Assert.IsFalse(File.Exists(srcFile1));
            Assert.IsTrue(File.Exists(targetFile1));

            Assert.AreEqual("abc", File.ReadAllText(targetFile1));
        }

        [Test]
        public void FileWithTheSameNameExistButDiffrentContent()
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            Directory.CreateDirectory(Path.Combine(targetDirectory, "Tempy"));
            var targetFile1 = Path.Combine(targetDirectory, "Tempy", "file1.temp");
            var targetFile2 = Path.Combine(targetDirectory, "Tempy", "file1_1.temp");
            File.WriteAllText(targetFile1, @"def");

            var command = new MoveDirectoryContent(sourceDirectory, "Tempy");
            command.Execute(new DestinationDirectoryHandler(targetDirectory));

            Assert.IsFalse(File.Exists(srcFile1));
            Assert.IsTrue(File.Exists(targetFile1));
            Assert.IsTrue(File.Exists(targetFile2));

            Assert.AreEqual("def", File.ReadAllText(targetFile1));
            Assert.AreEqual("abc", File.ReadAllText(targetFile2));
        }

        [Test]
        public void MoveFilesAndDirectories()
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            AddFile(@"def", "A", "file2.temp");
            AddFile(@"ghi", "B", "file3.temp");
            AddFile(@"jkl", "B", "C", "file4.temp");

            var command = new MoveDirectoryContent(sourceDirectory, "Tempy");
            command.Execute(new DestinationDirectoryHandler(targetDirectory));

            Assert.IsFalse(File.Exists(srcFile1));
            Assert.IsFalse(Directory.Exists(Path.Combine(sourceDirectory, "A")));
            Assert.IsFalse(Directory.Exists(Path.Combine(sourceDirectory, "B")));

            var targetFile1 = Path.Combine(targetDirectory, "Tempy", "file1.temp");
            var targetFile2 = Path.Combine(targetDirectory, "Tempy\\A", "file2.temp");
            var targetFile3 = Path.Combine(targetDirectory, "Tempy\\B", "file3.temp");
            var targetFile4 = Path.Combine(targetDirectory, "Tempy\\B\\C", "file4.temp");

            Assert.IsTrue(File.Exists(targetFile1));
            Assert.IsTrue(File.Exists(targetFile2));
            Assert.IsTrue(File.Exists(targetFile3));
            Assert.IsTrue(File.Exists(targetFile4));

            Assert.AreEqual("abc", File.ReadAllText(targetFile1));
            Assert.AreEqual("def", File.ReadAllText(targetFile2));
            Assert.AreEqual("ghi", File.ReadAllText(targetFile3));
            Assert.AreEqual("jkl", File.ReadAllText(targetFile4));
        }
    }
}
