
using System.Collections.ObjectModel;
using System.Windows.Input;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Stores;
using VehicleMaintenanceLog.Commands;
using VehicleMaintenanceLog.Models;
using System.Windows;

namespace VehicleMaintenanceLog.ViewModels
{
    class VehicleOverviewPageViewModel : ViewModelBase
    {

        private readonly NavigationStore _navigationStore;
        public ViewModelBase CurrentListTab => _navigationStore.CurrentViewModel;

        public ObservableCollection<VehicleViewModel> Vehicles { get; set; } = new ObservableCollection<VehicleViewModel>();

        private VehicleViewModel selectedVehicle = null;
        public VehicleViewModel SelectedVehicle { 
            get { return selectedVehicle; } 
            set 
            { 
                selectedVehicle = value;
                OnPropertyChanged("SelectedVehicle");
                
            } 
        }


        public VehicleOverviewPageViewModel()
        {
            _navigationStore = new NavigationStore();
            _navigationStore.CurrentViewModelChanged += OnCurrentListTabChanged;
            _navigationStore.CurrentViewModel = new MaintenanceStatusListViewModel(this);
            UpdateVehicleList();


            LoadLogListViewCommand = new NavigateCommand(_navigationStore, () => new VehicleLogsListViewModel(this));
            LoadMaintenanceStatusListViewCommand = new NavigateCommand(_navigationStore, () => new MaintenanceStatusListViewModel(this));

            UpdateMileageShowCommand = new RelayCommand(execute => UpdateMileageShow(), canExecute => { return MileageUpdateVisibility != Visibility.Visible; });
            UpdateMileageSubmitCommand = new RelayCommand(execute => UpdateMileageSubmit(), canExecute => { return UpdateMileageInput >= 0; });
        }

        public void UpdateVehicleList()
        {
            Vehicles.Clear();
            foreach (Vehicle v in SqliteDataAccess.LoadVehicles()) Vehicles.Add(new VehicleViewModel(v));

            if (Vehicles.Count > 0) SelectedVehicle = Vehicles[0];
        }


        public ICommand LoadLogListViewCommand { get; }
        public ICommand LoadMaintenanceStatusListViewCommand { get; }
        public ICommand UpdateMileageShowCommand { get; }
        public ICommand UpdateMileageSubmitCommand { get; }


        private void OnCurrentListTabChanged()
        {
            OnPropertyChanged(nameof(CurrentListTab));
        }


        private Visibility _MileageUpdateVisibilty = Visibility.Collapsed;
        public Visibility MileageUpdateVisibility {
            get => _MileageUpdateVisibilty;
            private set
            {
                _MileageUpdateVisibilty = value;
                OnPropertyChanged("MileageUpdateVisibility");
            }
        }

        private void UpdateMileageShow()
        {
            MileageUpdateVisibility = Visibility.Visible;
        }

        public int UpdateMileageInput { get; set; }
        private void UpdateMileageSubmit()
        {
            SelectedVehicle.UpdateVehicleMileage(UpdateMileageInput);
            MileageUpdateVisibility = Visibility.Collapsed;
            OnPropertyChanged("SelectedVehicle");
        }


    }
}
