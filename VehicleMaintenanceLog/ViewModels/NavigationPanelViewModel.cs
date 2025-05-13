using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using VehicleMaintenanceLog.Stores;
using VehicleMaintenanceLog.Commands;

namespace VehicleMaintenanceLog.ViewModels
{
    class NavigationPanelViewModel : ViewModelBase
    {

        private readonly NavigationStore _navigationStore;

        public NavigationPanelViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;

            LoadVehicleOverviewViewCommand = new NavigateCommand(navigationStore, () => new VehicleOverviewPageViewModel());
            LoadVehiclesViewCommand = new NavigateCommand(navigationStore, () => new VehiclesPageViewModel());
            LoadMaintenanceProfilesViewCommand = new NavigateCommand(navigationStore, () => new MaintenanceProfilesPageViewModel());
            LoadTasksViewCommand = new NavigateCommand(navigationStore, () => new TasksPageViewModel());
        }

        public ICommand LoadVehicleOverviewViewCommand { get; }
        public ICommand LoadVehiclesViewCommand { get; }
        public ICommand LoadMaintenanceProfilesViewCommand { get; }
        public ICommand LoadTasksViewCommand { get; }


    }
}
