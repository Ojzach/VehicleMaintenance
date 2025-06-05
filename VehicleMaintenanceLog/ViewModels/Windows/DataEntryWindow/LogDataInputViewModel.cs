using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using VehicleMaintenanceLog.Models;
using VehicleMaintenanceLog.Views;
using VehicleMaintenanceLog.Classes;
using System.Windows;


namespace VehicleMaintenanceLog.ViewModels.Windows.DataEntryWindow
{
    class LogDataInputViewModel : ViewModelBase, IDataEntryView
    {
        private readonly LogDataInputView _inputView;
        public UserControl InputView { get => _inputView; }

        public string InputTitle => _isEditMode ? "Edit Log" : "New Log";

        public (int w, int h) WindowDimensions => (450, 450);

        private bool _isEditMode;
        private MaintenanceLogViewModel _logToEdit;



        ObservableCollection<MaintenanceTaskViewModel> _tasks = new ObservableCollection<MaintenanceTaskViewModel>();
        public ObservableCollection<MaintenanceTaskViewModel> Tasks { 
            get => _tasks; 
            set 
            {
                _tasks = value;
                OnPropertyChanged("Tasks");
            }
        }
        private MaintenanceTaskViewModel _selectedTask;
        public MaintenanceTaskViewModel SelectedTask { 
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged("SelectedTask");
            }
        }
        private int _mileageInput;
        public int MileageInput { get => _mileageInput; set { _mileageInput = value; OnPropertyChanged("MileageInput"); } }
        private DateTime _dateInput;
        public DateTime DateInput { get => _dateInput; set { _dateInput = value; OnPropertyChanged("DateInput"); } }
        private bool _tempFixInput;
        public bool IsTempFixInput { get => _tempFixInput; 
            set
            {
                _tempFixInput = value;
                OnPropertyChanged("TempFixInput");
                OnPropertyChanged("TempFixInputVisibility");
            }
        }
        private int _tempFixMileageInput;
        private int _tempFixTimeInput;
        public string TempFixMileageInput {
            get
            {
                if (_tempFixMileageInput != -1) return _tempFixMileageInput.ToString();
                else return "";
            }
            set
            {
                int i;
                if (int.TryParse(value, out i)) _tempFixMileageInput = i;
                else _tempFixMileageInput = -1;

                OnPropertyChanged("TempFixMileageInput");
            }
        
        }
        public string TempFixTimeInput
        {
            get
            {
                if (_tempFixTimeInput != -1) return _tempFixTimeInput.ToString();
                else return "";
            }
            set
            {
                int i;
                if (int.TryParse(value, out i)) _tempFixTimeInput = i;
                else _tempFixTimeInput = -1;

                OnPropertyChanged("TempFixMonthsInput");
            }
        }
        private string _notesInput;
        public string NotesInput { get => _notesInput; set { _notesInput = value; OnPropertyChanged("NotesInput"); } }

        public Visibility TempFixInputVisibility { get => IsTempFixInput ? Visibility.Visible : Visibility.Collapsed; }

        private readonly VehicleOverviewPageViewModel _vehicleOverviewPage;
        public LogDataInputViewModel(VehicleOverviewPageViewModel vehicleOverviewPage)
        {
            _inputView = new LogDataInputView() { DataContext = this };
            _vehicleOverviewPage = vehicleOverviewPage;
        }

        public bool CanSubmit()
        {
            return (SelectedTask != null && MileageInput >= 0 && (IsTempFixInput == false || (_tempFixTimeInput != -1 || _tempFixMileageInput != -1)));
        }

        public void LoadPage(bool editMode, ViewModelBase selectedItemToEdit, object inputData)
        {
            _logToEdit = (MaintenanceLogViewModel)selectedItemToEdit;
            _isEditMode = editMode;
            OnPropertyChanged("InputTitle");

            foreach (MaintenanceTask t in SqliteDataAccess.GetMaintenanceTasks(_vehicleOverviewPage.SelectedVehicle.VehicleMaintenanceProfileID)) Tasks.Add(new MaintenanceTaskViewModel(t));


            if (!editMode) ClearInputs();
            else
            {
                SelectedTask = Tasks?.Where(x => x.TaskID == _logToEdit.LogTaskID).First();
                MileageInput = _logToEdit.LogVehicleMileage;
                DateInput = _logToEdit.LogDateCompleted;
                IsTempFixInput = (_logToEdit.LogTempFixTime != -1 || _logToEdit.LogTempFixMileage != -1);
                TempFixMileageInput = _logToEdit.LogTempFixMileage.ToString();
                TempFixTimeInput = _logToEdit.LogTempFixTime.ToString();
                NotesInput = _logToEdit.LogNotes;
            }
        }

        public object SubmitData()
        {
            if (_vehicleOverviewPage.SelectedVehicle.VehicleMileage < MileageInput)
            {

                MessageBoxResult result = MessageBox.Show("Log Mileage Greater Than Vehicle Mileage. Do You Want To Update Vehicle Mileage?", "", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    _vehicleOverviewPage.SelectedVehicle.UpdateVehicleMileage(MileageInput);
                    _vehicleOverviewPage.SelectedVehicle.VehicleMileage = MileageInput;
                }
                else if (result == MessageBoxResult.Cancel) return false;
            }


            MaintenanceLogItem log = new MaintenanceLogItem(_vehicleOverviewPage.SelectedVehicle.VehicleID, SelectedTask.TaskID, MileageInput, DateInput, _tempFixMileageInput, _tempFixTimeInput, NotesInput);

            if (!_isEditMode)
            {
                SqliteDataAccess.CreateItem<MaintenanceLogItem>(log);
                log.LogID = SqliteDataAccess.GetNewestItemID<MaintenanceLogItem>();
            }
            else
            {
                SqliteDataAccess.EditItem<MaintenanceLogItem>(log);
                log.LogID = _logToEdit.LogID;

            }

            ClearInputs();

            return log;

        }
    
        public void ClearInputs()
        {
            SelectedTask = null;
            MileageInput = _vehicleOverviewPage.SelectedVehicle.VehicleMileage;
            DateInput = DateTime.Today;
            IsTempFixInput = false;
            TempFixMileageInput = "";
            TempFixTimeInput = "";
            NotesInput = "";
        }
    
    }
}
