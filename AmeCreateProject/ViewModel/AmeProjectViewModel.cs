using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AmeCreateProject.Model;
using AmeCreateProject.Utils;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace AmeCreateProject.ViewModel
{
    public class AmeProjectViewModel : BindableBase
    {
        public string ProjectDir { get; set; } = @"E:\Projekty\2015 Przedszkole nr 11\";
        private readonly IRemovableMediaUtils mediaUtils;
        public DelegateCommand NextDayCommand { get; private set; }
        public DelegateCommand PrevDayCommand { get; private set; }
        public DelegateCommand CreateProjectCommand { get; private set; }
        public DelegateCommand ChangeProjectDirCommand { get; private set; }
        

        public AmeProjectViewModel(IRemovableMediaUtils mediaUtils)
        {
            this.mediaUtils = mediaUtils;
            Model = new AmeProjectModel();
            NextDayCommand = new DelegateCommand(NextDay);
            PrevDayCommand = new DelegateCommand(PrevDay);
            CreateProjectCommand = new DelegateCommand(CreateProject);
            ChangeProjectDirCommand = new DelegateCommand(ChangeProjectDir);
            RefreshDirectoryWarning();
        }

        private void ChangeProjectDir()
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = ProjectDir;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ProjectDir = dialog.SelectedPath;
                OnPropertyChanged(() => ProjectDir);
                RefreshDirectoryWarning();
            }
        }

        private void CreateProject()
        {
        }

        private void PrevDay()
        {
            Model.Date = Model.Date.AddDays(-1);
            OnPropertyChanged(() => ProjectDate);
            RefreshDirectoryWarning();
        }

        private void NextDay()
        {
            Model.Date = Model.Date.AddDays(1);
            OnPropertyChanged(() => ProjectDate);
            RefreshDirectoryWarning();
        }

        public AmeProjectModel Model { get; set; }

        public string ProjectName
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value; 
                OnPropertyChanged(() => ProjectName);
            }
        }

        public string WarnignDirectoryExist { get; set; }

        public string ProjectDate
        {
            get { return Model.Date.ToString("yyyy-MM-dd"); }
            set
            {
                DateTime date;
                if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    Model.Date = date;
                OnPropertyChanged(() => ProjectDate);
            }
        }

        private void RefreshDirectoryWarning()
        {
            var existingDirectory = Directory.GetDirectories(ProjectDir, ProjectDate + "*").FirstOrDefault();
            if (existingDirectory == null)
                WarnignDirectoryExist = null;
            else
                WarnignDirectoryExist = Path.GetFileName(existingDirectory);
            OnPropertyChanged(() => WarnignDirectoryExist);
        }
    }
}
