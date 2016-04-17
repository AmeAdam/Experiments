using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using AmeCreateProject.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AmeCommon.Svn;
using AmeCommon.MediaTasks.MoveFilesCommands;

namespace AmeCreateProject.ViewModel
{
    public class AmeProjectViewModel : BindableBase
    {
        private AmeProjectModel Model { get; set; }
        private ISvnUtils svn;
        public string ProjectDir => Model.ProjectDir + " | " + Model.Drive.AvailableFreeSpace / (1024 * 1024 * 1024) + "GB wolnego miejsca";

        public DelegateCommand NextDayCommand { get; private set; }
        public DelegateCommand PrevDayCommand { get; private set; }
        public DelegateCommand CreateProjectCommand { get; private set; }
        public DelegateCommand ChangeProjectDirCommand { get; private set; }
        public ObservableCollection<MediaViewModel> MediasList { get; private set; }
        private volatile bool inProgress;
      
        public AmeProjectViewModel(AmeProjectModel model, ISvnUtils svn)
        {
            Model = model;
            this.svn = svn;
            NextDayCommand = new DelegateCommand(NextDay, () => !inProgress);
            PrevDayCommand = new DelegateCommand(PrevDay, () => !inProgress);
            CreateProjectCommand = new DelegateCommand(CreateProject, () => !inProgress);
            ChangeProjectDirCommand = new DelegateCommand(ChangeProjectDir, () => !inProgress);
            MediasList = new ObservableCollection<MediaViewModel>(Model.MediaList.Select(m => new MediaViewModel(m)));
        }

        private void ChangeProjectDir()
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = ProjectDir;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Model.ProjectDir = dialog.SelectedPath;
                OnPropertyChanged(() => ProjectDir);
                OnPropertyChanged(() => DiskSize);
                Model.SaveSettings();
            }
        }

        public long DiskSize => Model.Drive.AvailableFreeSpace / (1024 * 1024 * 1024);

        private void CreateProject()
        {
            inProgress = true;
            Task.Factory.StartNew(ExecuteSelectedTasks);

        }

        private void ExecuteSelectedTasks()
        {
            try
            {
                var destDir = new DestinationDirectory(Model.DestinationDir);

                var taskHandlers = MediasList
                    .Where(m => m.IsActive)
                    .Select(m =>
                    {
                        return m.Tag.ExecuteAllTasksAsync(destDir);
                    });
                Task.WaitAll(taskHandlers.ToArray());
                svn.CreateSvn(Model.DestinationDir);
            }
            catch (Exception)
            {

            }
            finally
            {
                inProgress = false;
            }
        }

        private void PrevDay()
        {
            Model.MovePrevious();
            OnPropertyChanged(() => ProjectDate);
            OnPropertyChanged(() => ProjectName);
        }

        private void NextDay()
        {
            Model.MoveNext();
            OnPropertyChanged(() => ProjectDate);
            OnPropertyChanged(() => ProjectName);
        }

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
    }
}
