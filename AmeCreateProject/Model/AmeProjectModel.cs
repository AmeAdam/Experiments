using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AmeCreateProject.Model
{
    public class AmeProjectModel
    {
        public const string DefaultName = "[Nazwa Wydarzenia]";
        public DateTime Date { get; set; } = DateTime.Now.Date;
        public string Name { get; set; } = DefaultName;
        public string DirName => Date.ToString("yyyy-MM-dd") + " " + Name;
        private string projectDir = @"E:\Projekty";
        private List<string> allDirectories;

        public string ProjectDir
        {
            get { return projectDir; }
            set
            {
                projectDir = value;
                UpdateDirectoriesList();
            }
        }

        private void UpdateDirectoriesList()
        {
            allDirectories = Directory.GetDirectories(ProjectDir, Date.ToString("yyyy-MM-dd") + " *").ToList();
        }

        public AmeProjectModel()
        {
            UpdateDirectoriesList();
            SetDirectory(allDirectories.FirstOrDefault());
        }

        public void MoveNext()
        {
            MoveInternal(1);
        }

        public void MovePrevious()
        {
            MoveInternal(-1);
        }

        private bool TrySetDirectoryByIndex(int currentPos, int shift)
        {
            if (currentPos < 0)
                return false;
            var index = currentPos + shift;
            if (index < 0 || index >= allDirectories.Count)
                return false;
            SetDirectory(allDirectories[index]);
            return true;
        }

        private void MoveInternal(int shift)
        {
            var pos = allDirectories.IndexOf(Path.Combine(projectDir, DirName));
        
            if (!TrySetDirectoryByIndex(pos, shift))
            {
                Date = Date.AddDays(shift);
                UpdateDirectoriesList();
                var nextDir = shift >= 0 ? allDirectories.FirstOrDefault() : allDirectories.LastOrDefault();
                SetDirectory(nextDir);
            }
        }

        private void SetDirectory(string newDirectory)
        {
            if (newDirectory == null)
                Name = DefaultName;
            else
            {
                var tempName = Path.GetFileName(newDirectory);
                if (tempName.Length > 11)
                    tempName = tempName.Substring(11);
                Name = tempName;
            }
        }

        public string ProjectDate
        {
            get { return Date.ToString("yyyy-MM-dd"); }
            set
            {
                DateTime date;
                if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    Date = date;
            }
        }
    }
}
