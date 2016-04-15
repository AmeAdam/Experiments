using System;
using System.IO;
using AmeCommon.MediaTasks;
using AmeCommon.MediaTasks.MoveFilesCommands;
using NUnit.Framework;

namespace AmeTests
{
    [TestFixture]
    public class MoveFilesTest
    {
        private string sourceDirectory;
        private string targetDirectory;
        private const string TargetFolderName = "Tempy";

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

        [TestCase("move-directory-content")]
        [TestCase("move-directory-content-flat")]
        public void MoveFiles(string commandName)
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            var srcFile2 = AddFile(@"def", "file2.temp");

            var command = CreateTaskHandler(commandName);
            command.Execute(new DestinationDirectory(targetDirectory));

            Assert.IsFalse(File.Exists(srcFile1));
            Assert.IsFalse(File.Exists(srcFile2));

            var targetFile1 = Path.Combine(targetDirectory, TargetFolderName, "file1.temp");
            var targetFile2 = Path.Combine(targetDirectory, TargetFolderName, "file2.temp");

            Assert.IsTrue(File.Exists(targetFile1));
            Assert.IsTrue(File.Exists(targetFile2));

            Assert.AreEqual("abc", File.ReadAllText(targetFile1));
            Assert.AreEqual("def", File.ReadAllText(targetFile2));
        }

        [Test]
        public void MoveOnlyFiles()
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            var srcFile2 = AddFile(@"def", "file2.abcd");

            var command = new MoveFiles(SourceDirectory.GetFilesList(Path.Combine(sourceDirectory, "*.*")), TargetFolderName);
            command.Execute(new DestinationDirectory(targetDirectory));

            Assert.IsFalse(File.Exists(srcFile1));
            Assert.IsFalse(File.Exists(srcFile2));

            var targetFile1 = Path.Combine(targetDirectory, TargetFolderName, "file1.temp");
            var targetFile2 = Path.Combine(targetDirectory, TargetFolderName, "file2.abcd");

            Assert.IsTrue(File.Exists(targetFile1));
            Assert.IsTrue(File.Exists(targetFile2));

            Assert.AreEqual("abc", File.ReadAllText(targetFile1));
            Assert.AreEqual("def", File.ReadAllText(targetFile2));
        }

        [Test]
        public void MoveOnlyJpgFiles()
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            var srcFile2 = AddFile(@"def", "file2.jpg");

            var command = new MoveFiles(SourceDirectory.GetFilesList(Path.Combine(sourceDirectory, "*.jpg")), TargetFolderName);
            command.Execute(new DestinationDirectory(targetDirectory));

            Assert.IsTrue(File.Exists(srcFile1));
            Assert.IsFalse(File.Exists(srcFile2));

            var targetFile1 = Path.Combine(targetDirectory, TargetFolderName, "file1.temp");
            var targetFile2 = Path.Combine(targetDirectory, TargetFolderName, "file2.jpg");

            Assert.IsFalse(File.Exists(targetFile1));
            Assert.IsTrue(File.Exists(targetFile2));

            Assert.AreEqual("def", File.ReadAllText(targetFile2));
        }

        [TestCase("move-directory-content")]
        [TestCase("move-directory-content-flat")]
        public void FileWithTheSameNameExistWithSameContent(string commandName)
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            Directory.CreateDirectory(Path.Combine(targetDirectory, TargetFolderName));
            var targetFile1 = Path.Combine(targetDirectory, TargetFolderName, "file1.temp");
            File.WriteAllText(targetFile1, @"abc");

            var command = CreateTaskHandler(commandName);
            command.Execute(new DestinationDirectory(targetDirectory));

            Assert.IsFalse(File.Exists(srcFile1));
            Assert.IsTrue(File.Exists(targetFile1));

            Assert.AreEqual("abc", File.ReadAllText(targetFile1));
        }

        [TestCase("move-directory-content")]
        [TestCase("move-directory-content-flat")]
        public void FileWithTheSameNameExistButDiffrentContent(string commandName)
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            Directory.CreateDirectory(Path.Combine(targetDirectory, TargetFolderName));
            var targetFile1 = Path.Combine(targetDirectory, TargetFolderName, "file1.temp");
            var targetFile2 = Path.Combine(targetDirectory, TargetFolderName, "file1_1.temp");
            File.WriteAllText(targetFile1, @"def");

            var command = CreateTaskHandler(commandName);
            command.Execute(new DestinationDirectory(targetDirectory));

            Assert.IsFalse(File.Exists(srcFile1));
            Assert.IsTrue(File.Exists(targetFile1));
            Assert.IsTrue(File.Exists(targetFile2));

            Assert.AreEqual("def", File.ReadAllText(targetFile1));
            Assert.AreEqual("abc", File.ReadAllText(targetFile2));
        }

        [TestCase("move-directory-content")]
        public void MoveFilesAndDirectories(string commandName)
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            AddFile(@"def", "A", "file2.temp");
            AddFile(@"ghi", "B", "file3.temp");
            AddFile(@"jkl", "B", "C", "file4.temp");

            var command = CreateTaskHandler(commandName);
            command.Execute(new DestinationDirectory(targetDirectory));

            Assert.IsFalse(File.Exists(srcFile1));
            Assert.IsFalse(Directory.Exists(Path.Combine(sourceDirectory, "A")));
            Assert.IsFalse(Directory.Exists(Path.Combine(sourceDirectory, "B")));

            var targetFile1 = Path.Combine(targetDirectory, TargetFolderName, "file1.temp");
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

        [TestCase("move-directory-content-flat")]
        public void MoveFilesAndDirectoriesFlat(string commandName)
        {
            var srcFile1 = AddFile(@"abc", "file1.temp");
            AddFile(@"def", "A", "file2.temp");
            AddFile(@"ghi", "B", "file3.temp");
            AddFile(@"jkl", "B", "C", "file4.temp");

            var command = CreateTaskHandler(commandName);
            command.Execute(new DestinationDirectory(targetDirectory));

            Assert.IsFalse(File.Exists(srcFile1));
            Assert.IsFalse(Directory.Exists(Path.Combine(sourceDirectory, "A")));
            Assert.IsFalse(Directory.Exists(Path.Combine(sourceDirectory, "B")));

            var targetFile1 = Path.Combine(targetDirectory, TargetFolderName, "file1.temp");
            var targetFile2 = Path.Combine(targetDirectory, TargetFolderName, "file2.temp");
            var targetFile3 = Path.Combine(targetDirectory, TargetFolderName, "file3.temp");
            var targetFile4 = Path.Combine(targetDirectory, TargetFolderName, "file4.temp");

            Assert.IsTrue(File.Exists(targetFile1));
            Assert.IsTrue(File.Exists(targetFile2));
            Assert.IsTrue(File.Exists(targetFile3));
            Assert.IsTrue(File.Exists(targetFile4));

            Assert.AreEqual("abc", File.ReadAllText(targetFile1));
            Assert.AreEqual("def", File.ReadAllText(targetFile2));
            Assert.AreEqual("ghi", File.ReadAllText(targetFile3));
            Assert.AreEqual("jkl", File.ReadAllText(targetFile4));
        }

        private IIOCommand CreateTaskHandler(string taskName)
        {
            switch (taskName)
            {
                case "move-directory-content":
                    return new MoveDirectoryContent(SourceDirectory.Create(sourceDirectory), TargetFolderName);
                case "move-directory-content-flat":
                    return new MoveDirectoryContentFlat(SourceDirectory.Create(sourceDirectory), TargetFolderName);
                default:
                    throw new ApplicationException("Not supported command: " + taskName);
            }
        }
    }
}
