using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Commands;
using VehicleMaintenanceLog.Models;
using VehicleMaintenanceLog.ViewModels.Models;
using VehicleMaintenanceLog.ViewModels.Windows.DataEntryWindow;

namespace VehicleMaintenanceLog.ViewModels
{
    public class MaintenanceProfilesPageViewModel : ViewModelBase
    {

        private ObservableCollection<MaintenanceProfileViewModel> _maintenanceProfiles = new ObservableCollection<MaintenanceProfileViewModel>();
        private MaintenanceProfileViewModel _selectedMaintenanceProfile = null;
        public ObservableCollection<MaintenanceProfileViewModel> MaintenanceProfiles
        { get => _maintenanceProfiles; set { _maintenanceProfiles = value; OnPropertyChanged("MaintenanceProfiles"); } }
        public MaintenanceProfileViewModel SelectedMaintenanceProfile
        { get => _selectedMaintenanceProfile; set { _selectedMaintenanceProfile = value; OnPropertyChanged("SelectedMaintenanceProfile"); UpdateScheduleList(); } }

        private ObservableCollection<TaskScheduleViewModel> _maintenanceProfileSchedules = new ObservableCollection<TaskScheduleViewModel>();
        private TaskScheduleViewModel _selectedSchedule = null;
        public ObservableCollection<TaskScheduleViewModel> MaintenanceProfileSchedules
        { get => _maintenanceProfileSchedules; set { _maintenanceProfileSchedules = value; OnPropertyChanged("MaintenanceProfileSchedules"); } }
        public TaskScheduleViewModel SelectedSchedule
        { get => _selectedSchedule; set { _selectedSchedule = value; OnPropertyChanged("SelectedSchedule"); } }

        public MaintenanceProfilesPageViewModel()
        {

            UpdateMaintenanceProfileList();

            profileDataEntryWindow = new DataEntryWindowViewModel(new MaintenanceProfileDataInputViewModel());
            taskScheduleDataEntryWindow = new DataEntryWindowViewModel(new TaskScheduleDataInputViewModel());

            profileDataEntryWindow.DataEntrySubmitted += UpdateMaintenanceProfileList;
            taskScheduleDataEntryWindow.DataEntrySubmitted += UpdateScheduleList;

            LoadNewProfileWindowCommand = new RelayCommand(execute => profileDataEntryWindow.LoadWindow());
            LoadEditProfileWindowCommand = new RelayCommand(execute => profileDataEntryWindow.LoadWindow(true, SelectedMaintenanceProfile), canExecute => { return (SelectedMaintenanceProfile != null && SelectedMaintenanceProfile.ProfileID != 0); });
            DeleteSelectedScheduleCommand = new RelayCommand(execute => DeleteSchedule(), canExecute => { return SelectedSchedule != null; });
            DeleteSelectedProfileCommand = new RelayCommand(execute => DeleteProfile(), canExecute => { return (SelectedMaintenanceProfile != null && SelectedMaintenanceProfile.ProfileID != 0); });

            LoadNewScheduleWindowCommand = new RelayCommand(execute => taskScheduleDataEntryWindow.LoadWindow(inputData: SelectedMaintenanceProfile));
            LoadEditScheduleWindowCommand = new RelayCommand(execute => taskScheduleDataEntryWindow.LoadWindow(true, SelectedSchedule, SelectedMaintenanceProfile), canExecute => { return true; });
        }


        private void UpdateMaintenanceProfileList()
        {
            foreach (MaintenanceProfile profile in SqliteDataAccess.GetAllOfType<MaintenanceProfile>())
            {
                MaintenanceProfiles.Add(new MaintenanceProfileViewModel(profile));
            }

            if (MaintenanceProfiles.Count > 0) SelectedMaintenanceProfile = MaintenanceProfiles[0];

        }
        private void UpdateMaintenanceProfileList(object sender, DataEntryEventArgs args)
        {
            SelectedMaintenanceProfile = MaintenanceProfiles[0];

            MaintenanceProfileViewModel profile = (MaintenanceProfileViewModel)args.SubmittedItem;

            if (MaintenanceProfiles.Contains(profile))
            {
                MaintenanceProfiles.Remove(profile);
                MaintenanceProfiles.Add(profile);

                SelectedMaintenanceProfile = MaintenanceProfiles[MaintenanceProfiles.Count - 1];
            }
            else MaintenanceProfiles.Add(profile);
        }

        private void UpdateScheduleList()
        {
            if (SelectedMaintenanceProfile != null)
            {
                MaintenanceProfileSchedules.Clear();
                SelectedSchedule = null;

                foreach (TaskSchedule schedule in SqliteDataAccess.GetSchedulesInProfile(SelectedMaintenanceProfile.ProfileID))
                {
                    MaintenanceProfileSchedules.Add(new TaskScheduleViewModel(schedule));
                }
            }

        }
        private void UpdateScheduleList(object sender, DataEntryEventArgs args)
        {
            TaskScheduleViewModel schedule = (TaskScheduleViewModel)args.SubmittedItem;

            if (MaintenanceProfileSchedules.Contains(schedule))
            {
                MaintenanceProfileSchedules.Remove(schedule);
                MaintenanceProfileSchedules.Add(schedule);

                SelectedSchedule = MaintenanceProfileSchedules[MaintenanceProfileSchedules.Count - 1];
            }
            else MaintenanceProfileSchedules.Add(schedule);
        }

        private void DeleteSchedule()
        {

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SqliteDataAccess.DeleteItem<TaskSchedule>(SelectedSchedule.ScheduleID);
                MaintenanceProfileSchedules.Remove(SelectedSchedule);

            }
        }

        private void DeleteProfile()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SqliteDataAccess.DeleteItem<MaintenanceProfile>(SelectedMaintenanceProfile.ProfileID);
                MaintenanceProfiles.Remove(SelectedMaintenanceProfile);

                if (MaintenanceProfiles.Count >= 0) SelectedMaintenanceProfile = MaintenanceProfiles[0];
            }
        }

        public ICommand LoadNewProfileWindowCommand { get; }
        public ICommand LoadEditProfileWindowCommand { get; }
        public ICommand DeleteSelectedProfileCommand { get; }

        public ICommand LoadNewScheduleWindowCommand { get; }
        public ICommand LoadEditScheduleWindowCommand { get; }
        public ICommand DeleteSelectedScheduleCommand { get; }

        private DataEntryWindowViewModel profileDataEntryWindow;
        private DataEntryWindowViewModel taskScheduleDataEntryWindow;
    }
}
