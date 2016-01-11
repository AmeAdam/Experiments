using System;
using System.Globalization;
using AmeCreateProject.Model;
using AmeCreateProject.Utils;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace AmeCreateProject.ViewModel
{
    public class AmeProjectViewModel : BindableBase
    {
        private readonly IRemovableMediaUtils mediaUtils;
        public DelegateCommand NextDayCommand { get; private set; }
        public DelegateCommand PrevDayCommand { get; private set; }
        public DelegateCommand CreateProjectCommand { get; private set; }

        public AmeProjectViewModel(IRemovableMediaUtils mediaUtils)
        {
            this.mediaUtils = mediaUtils;
            Model = new AmeProjectModel();
            NextDayCommand = new DelegateCommand(NextDay);
            PrevDayCommand = new DelegateCommand(PrevDay);
            CreateProjectCommand = new DelegateCommand(CreateProject);
        }

        private void CreateProject()
        {
        }

        private void PrevDay()
        {
            Model.Date = Model.Date.AddDays(-1);
            OnPropertyChanged(() => ProjectDate);
        }

        private void NextDay()
        {
            Model.Date = Model.Date.AddDays(1);
            OnPropertyChanged(() => ProjectDate);
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
