using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Models;
using VehicleMaintenanceLog.ViewModels.Models;
using VehicleMaintenanceLog.Views.Windows.DataEntryWindow;

namespace VehicleMaintenanceLog.ViewModels.Windows.DataEntryWindow
{
    internal class TaskScheduleDataInputViewModel : ViewModelBase, IDataEntryView
    {
        private TaskScheduleDataInputView _inputView;
        public UserControl InputView { get => _inputView; }

        public string InputTitle => _isEditMode ? "Edit Schedule" : "New Schedule";

        public (int w, int h) WindowDimensions => (350, 350);

        private bool _isEditMode;


        private ObservableCollection<MaintenanceTaskViewModel> _tasks = new ObservableCollection<MaintenanceTaskViewModel>();
        public ObservableCollection<MaintenanceTaskViewModel> Tasks { get => _tasks; set {  _tasks = value; OnPropertyChanged("Tasks"); } }
        private MaintenanceTaskViewModel _selectedTask;
        public MaintenanceTaskViewModel SelectedTask { get => _selectedTask; set { _selectedTask = value; OnPropertyChanged("SelectedTask"); if(value != null) NewSchedule.TaskID = value.TaskID; } }

        private TaskScheduleViewModel _newSchedule;
        public TaskScheduleViewModel NewSchedule { get => _newSchedule; set { _newSchedule = value; OnPropertyChanged("NewSchedule"); } }

        private bool _mileageIncrementEnabled, _timeIncrementEnabled;
        public bool MileageIncrementEnabled { get => _mileageIncrementEnabled; set { _mileageIncrementEnabled = value; OnPropertyChanged("MileageIncrementEnabled"); } }
        public bool TimeIncrementEnabled { get => _timeIncrementEnabled; set { _timeIncrementEnabled = value; OnPropertyChanged("TimeIncrementEnabled"); } }


        private MaintenanceProfileViewModel _currentMaintenanceProfile;

        public TaskScheduleDataInputViewModel()
        {
            _inputView = new TaskScheduleDataInputView() { DataContext = this };

        }

        public bool CanSubmit()
        {
            return (_newSchedule != null && _newSchedule.TaskID != -1 && ((MileageIncrementEnabled && _newSchedule.MileageIncrement > 0) || !MileageIncrementEnabled) && ((TimeIncrementEnabled && _newSchedule.TimeIncrement > 0) || !TimeIncrementEnabled));
        }

        public void ClearInputs()
        {
            NewSchedule = new TaskScheduleViewModel(new TaskSchedule());
            MileageIncrementEnabled = false;
            TimeIncrementEnabled = false;
        }

        public void LoadPage(bool editMode, ViewModelBase selectedItemToEdit, object inputData)
        {
            _currentMaintenanceProfile = (MaintenanceProfileViewModel)inputData;

            if (editMode) NewSchedule = (TaskScheduleViewModel)selectedItemToEdit;
            else NewSchedule = new TaskScheduleViewModel(new TaskSchedule());

            NewSchedule.MaintenanceProfileID = _currentMaintenanceProfile.ProfileID;

            _isEditMode = editMode;
            OnPropertyChanged("InputTitle");

            List<int> tasksInProfile = SqliteDataAccess.GetTaskIDsInProfile(_currentMaintenanceProfile.ProfileID);

            Tasks.Clear();
            foreach(MaintenanceTask task in SqliteDataAccess.GetMaintenanceTasks(_currentMaintenanceProfile.VehicleTypeConstraint))
            {
                if (!tasksInProfile.Contains(task.id))
                {
                    Tasks.Add(new MaintenanceTaskViewModel(task));
                }
                else if (editMode && task.id == NewSchedule.TaskID) 
                {
                    Tasks.Add(new MaintenanceTaskViewModel(task));
                    SelectedTask = Tasks[Tasks.Count - 1];
                }

            }

            if(NewSchedule.MileageIncrement != -1) MileageIncrementEnabled = true;
            if(NewSchedule.TimeIncrement != -1) TimeIncrementEnabled = true;
        }

        public object SubmitData()
        {
            if (!MileageIncrementEnabled) NewSchedule.MileageIncrement = -1;
            if (!TimeIncrementEnabled) NewSchedule.TimeIncrement = -1; 

            if (!_isEditMode)
            {
                SqliteDataAccess.CreateItem<TaskSchedule>(NewSchedule.ToTaskSchedule());
                NewSchedule.SetID(SqliteDataAccess.GetNewestItemID<MaintenanceProfile>());
            }
            else SqliteDataAccess.EditItem<TaskSchedule>(NewSchedule.ToTaskSchedule());

            return NewSchedule;
        }
    }
}
