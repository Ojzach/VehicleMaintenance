using System.Collections.ObjectModel;
using VehicleMaintenanceLog.Models;
using VehicleMaintenanceLog.Classes;
using System.Windows.Input;
using System.Windows;
using VehicleMaintenanceLog.Commands;
using VehicleMaintenanceLog.ViewModels.Windows.DataEntryWindow;

namespace VehicleMaintenanceLog.ViewModels
{
    class VehiclesPageViewModel : ViewModelBase
    {

        private ObservableCollection<VehicleViewModel> _vehicleEntries = new ObservableCollection<VehicleViewModel>();
        public ObservableCollection<VehicleViewModel> VehicleEntries { get => _vehicleEntries; set => _vehicleEntries = value; }

        private VehicleViewModel _selectedVehicle;
        public VehicleViewModel SelectedVehicle { 
            get => _selectedVehicle; 
            set
            {
                _selectedVehicle = value;
                OnPropertyChanged(nameof(SelectedVehicle));
            } 
        }


        public ICommand LoadNewVehicleWindowCommand { get; }
        public ICommand LoadEditVehicleWindowCommand { get; }
        public ICommand DeleteSelectedVehicleCommand { get; }

        private DataEntryWindowViewModel dataEntryWindow;

        public VehiclesPageViewModel()
        {
            UpdateVehicleEntries();

            dataEntryWindow = new DataEntryWindowViewModel(new VehicleDataInputViewModel());

            dataEntryWindow.DataEntrySubmitted += UpdateVehicleEntries;

            LoadNewVehicleWindowCommand = new RelayCommand(execute => dataEntryWindow.LoadWindow());
            LoadEditVehicleWindowCommand = new RelayCommand(execute => dataEntryWindow.LoadWindow(true, SelectedVehicle), canExecute => { return SelectedVehicle != null; });
            DeleteSelectedVehicleCommand = new RelayCommand(execute => DeleteVehicle(), canExecute => { return SelectedVehicle != null; });
        
        }


        private void UpdateVehicleEntries()
        {
            VehicleEntries.Clear();

            
            foreach (Vehicle v in SqliteDataAccess.GetAllOfType<Vehicle>())
            {
                VehicleEntries.Add(new VehicleViewModel(v));
            }
        }

        private void UpdateVehicleEntries(object sender, DataEntryEventArgs args)
        {
            VehicleViewModel vehicle = (VehicleViewModel)args.SubmittedItem;

            if (VehicleEntries.Contains(vehicle))
            {
                VehicleEntries.Remove(vehicle);
                VehicleEntries.Add(vehicle);
            }
            else VehicleEntries.Add(vehicle);
        }



        private void DeleteVehicle()
        {

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SqliteDataAccess.DeleteItem<Vehicle>(SelectedVehicle.VehicleID);
                VehicleEntries.Remove(SelectedVehicle);

            }
        }

    }
}
