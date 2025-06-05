using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Models;

namespace VehicleMaintenanceLog.ViewModels
{
    class MaintenanceStatusListViewModel : ViewModelBase
    {

        private ObservableCollection<MaintenanceTaskStatusViewModel> entries = new ObservableCollection<MaintenanceTaskStatusViewModel>();
        public ObservableCollection<MaintenanceTaskStatusViewModel> Entries { get { return entries; } set { entries = value; } }


        private readonly VehicleOverviewPageViewModel _vehicleOverviewPage;

        public MaintenanceStatusListViewModel(VehicleOverviewPageViewModel vehicleOverviewPage)
        {
            _vehicleOverviewPage = vehicleOverviewPage;
            vehicleOverviewPage.PropertyChanged += UpdateMaintenanceStatusMenu;
        }


        private void UpdateMaintenanceStatusMenu(object sender, EventArgs args)
        {
            Entries.Clear();


            if (_vehicleOverviewPage.SelectedVehicle != null)
            {
                foreach (TaskSchedule schedule in SqliteDataAccess.GetMaintenanceSchedules(_vehicleOverviewPage.SelectedVehicle.ToVehicle()))
                    Entries.Add(new MaintenanceTaskStatusViewModel(_vehicleOverviewPage.SelectedVehicle.ToVehicle(), schedule));
            }
        }
    }
}
