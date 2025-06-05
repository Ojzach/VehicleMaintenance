using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Commands;
using VehicleMaintenanceLog.Models;
using VehicleMaintenanceLog.ViewModels.Windows.DataEntryWindow;
using VehicleMaintenanceLog.Views;

namespace VehicleMaintenanceLog.ViewModels
{
    class VehicleLogsListViewModel : ViewModelBase
    {
        private ObservableCollection<MaintenanceLogViewModel> _logEntries = new ObservableCollection<MaintenanceLogViewModel>();
        public ObservableCollection<MaintenanceLogViewModel> LogEntries { get { return _logEntries; } set { _logEntries = value; } }

        private MaintenanceLogViewModel _selectedLog = null;
        public MaintenanceLogViewModel SelectedLog
        {
            get { return _selectedLog; }
            set
            {
                _selectedLog = value;
                OnPropertyChanged("SelectedLog");
            }
        }

        private DataEntryWindowViewModel _LogDataEntryWindow;

        private readonly VehicleOverviewPageViewModel _vehicleOverviewPage;
        public VehicleLogsListViewModel(VehicleOverviewPageViewModel vehicleOverviewPage)
        {
            _vehicleOverviewPage = vehicleOverviewPage;
            vehicleOverviewPage.PropertyChanged += UpdateMaintenanceStatusMenu;

            _LogDataEntryWindow = new DataEntryWindowViewModel(new LogDataInputViewModel(vehicleOverviewPage));

            _LogDataEntryWindow.DataEntrySubmitted += UpdateMaintenanceStatusMenu;

            LoadNewLogWindowCommand = new RelayCommand(execute => _LogDataEntryWindow.LoadWindow());
            LoadEditLogWindowCommand = new RelayCommand(execute => _LogDataEntryWindow.LoadWindow(isEditMode: true, selectedItemToEdit: SelectedLog), canExecute => { return SelectedLog != null; });
            DeleteSelectedLogCommand = new RelayCommand(execute => DeleteLog(), canExecute => { return SelectedLog != null; });
        }

        private void UpdateMaintenanceStatusMenu(object sender, EventArgs args)
        {
            LogEntries.Clear();

            if (_vehicleOverviewPage.SelectedVehicle != null)
            {
                foreach (MaintenanceLogItem item in SqliteDataAccess.GetEntireVehicleMaintenanceLog(_vehicleOverviewPage.SelectedVehicle.ToVehicle().id))
                {
                    if (!LogEntries.Contains(new MaintenanceLogViewModel(item))) LogEntries.Add(new MaintenanceLogViewModel(item));
                }
            }

        }

        private void UpdateMaintenanceStatusMenu(object sender, DataEntryEventArgs args)
        {
            MaintenanceLogViewModel log = new MaintenanceLogViewModel((MaintenanceLogItem)args.SubmittedItem);

            if (LogEntries.Contains(log))
            {
                LogEntries.Remove(log);
                LogEntries.Add(log);
            }
            else LogEntries.Add(log);
        }



        public ICommand LoadNewLogWindowCommand { get; }
        public ICommand LoadEditLogWindowCommand { get; }
        public ICommand DeleteSelectedLogCommand { get; }


        private void DeleteLog()
        {

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SqliteDataAccess.DeleteItem<MaintenanceLogItem>(SelectedLog.LogID);
                LogEntries.Remove(SelectedLog);

            }
        }

    }
}
