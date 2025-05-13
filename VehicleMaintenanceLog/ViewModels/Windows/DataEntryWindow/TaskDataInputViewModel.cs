using System;
using System.Windows.Controls;
using VehicleMaintenanceLog.Views.Windows.DataEntryWindow;
using VehicleMaintenanceLog.Models;
using System.Collections.ObjectModel;
using VehicleMaintenanceLog.Classes;

namespace VehicleMaintenanceLog.ViewModels.Windows.DataEntryWindow
{
    class TaskDataInputViewModel : ViewModelBase, IDataEntryView
    {
        private TaskDataInputView _inputView;
        public UserControl InputView { get => _inputView; }

        public string InputTitle => _isEditMode ? "Edit Task" : "New Task";

        public (int w, int h) WindowDimensions => (350, 350);

        private bool _isEditMode;


        private ObservableCollection<VehicleType> _vehicleTypes = new ObservableCollection<VehicleType>();
        public ObservableCollection<VehicleType> VehicleTypes { get => _vehicleTypes; set { _vehicleTypes = value; OnPropertyChanged("VehicleTypes"); } }


        private MaintenanceTaskViewModel _newTask;
        public MaintenanceTaskViewModel NewTask { 
            get => _newTask; 
            set {
                _newTask = value;
                OnPropertyChanged("NewTask");
            } 
        }


        public TaskDataInputViewModel()
        {
            _inputView = new TaskDataInputView() { DataContext = this };

            foreach (VehicleType type in Enum.GetValues(typeof(VehicleType))) VehicleTypes.Add(type);
        }

        public bool CanSubmit()
        {
            return (NewTask != null && NewTask.TaskName != "");
        }

        public void LoadPage(bool editMode, ViewModelBase selectedItemToEdit)
        {
            if (editMode) NewTask = (MaintenanceTaskViewModel)selectedItemToEdit;
            else NewTask = new MaintenanceTaskViewModel(new MaintenanceTask());
            
            _isEditMode = editMode;
            OnPropertyChanged("InputTitle");
        }

        public object SubmitData()
        {
            if (!_isEditMode)
            {
                SqliteDataAccess.CreateMaintenanceTask(NewTask.ToMaintenanceTask());
                NewTask.SetID(SqliteDataAccess.GetNewestTaskID());
            }
            else SqliteDataAccess.EditTask(NewTask.ToMaintenanceTask());

            return NewTask;
        }

        public void ClearInputs() => NewTask = new MaintenanceTaskViewModel(new MaintenanceTask());
    }
}
