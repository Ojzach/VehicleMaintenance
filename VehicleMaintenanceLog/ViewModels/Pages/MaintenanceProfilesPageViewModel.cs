using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Commands;
using VehicleMaintenanceLog.Models;
using VehicleMaintenanceLog.ViewModels.Models;
using VehicleMaintenanceLog.ViewModels.Windows.DataEntryWindow;

namespace VehicleMaintenanceLog.ViewModels
{
    class MaintenanceProfilesPageViewModel : ViewModelBase
    {

        private ObservableCollection<MaintenanceProfileViewModel> _maintenanceProfiles = new ObservableCollection<MaintenanceProfileViewModel>();
        public ObservableCollection<MaintenanceProfileViewModel> MaintenanceProfiles
        { get => _maintenanceProfiles; set { _maintenanceProfiles = value; OnPropertyChanged("MaintenanceProfiles"); } }

        private MaintenanceProfileViewModel _selectedMaintenanceProfile = null;
        public MaintenanceProfileViewModel SelectedMaintenanceProfile
        {
            get => _selectedMaintenanceProfile; set { _selectedMaintenanceProfile = value; OnPropertyChanged("SelectedMaintenanceProfile"); }
        }

        public MaintenanceProfilesPageViewModel()
        {

            UpdateMaintenanceProfileList();

            profileDataEntryWindow = new DataEntryWindowViewModel(new MaintenanceProfileDataInputViewModel());

            profileDataEntryWindow.DataEntrySubmitted += UpdateMaintenanceProfileList;

            LoadNewProfileWindowCommand = new RelayCommand(execute => profileDataEntryWindow.LoadWindow());
            LoadEditProfileWindowCommand = new RelayCommand(execute => profileDataEntryWindow.LoadWindow(true, SelectedMaintenanceProfile), canExecute => { return SelectedMaintenanceProfile != null; });
            //DeleteSelectedTaskCommand = new RelayCommand(execute => DeleteTask(), canExecute => { return SelectedTask != null; });
        }


        private void UpdateMaintenanceProfileList()
        {
            foreach (MaintenanceProfile profile in SqliteDataAccess.GetAllMaintenanceProfiles())
            {
                MaintenanceProfiles.Add(new MaintenanceProfileViewModel(profile));
            }

            if (MaintenanceProfiles.Count > 0) SelectedMaintenanceProfile = MaintenanceProfiles[0];

        }

        private void UpdateMaintenanceProfileList(object sender, DataEntryEventArgs args)
        {
            MaintenanceProfileViewModel profile = (MaintenanceProfileViewModel)args.SubmittedItem;

            if (MaintenanceProfiles.Contains(profile))
            {
                MaintenanceProfiles.Remove(profile);
                MaintenanceProfiles.Add(profile);

                SelectedMaintenanceProfile = MaintenanceProfiles[MaintenanceProfiles.Count - 1];
            }
            else MaintenanceProfiles.Add(profile);
        }

        public ICommand LoadNewProfileWindowCommand { get; }
        public ICommand LoadEditProfileWindowCommand { get; }
        public ICommand DeleteSelectedProfileCommand { get; }

        private DataEntryWindowViewModel profileDataEntryWindow;
    }
}
