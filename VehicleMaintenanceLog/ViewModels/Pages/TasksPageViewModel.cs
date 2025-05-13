using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using VehicleMaintenanceLog.Classes;
using VehicleMaintenanceLog.Models;
using VehicleMaintenanceLog.Commands;
using VehicleMaintenanceLog.ViewModels.Windows.DataEntryWindow;

namespace VehicleMaintenanceLog.ViewModels
{

    class TasksPageViewModel : ViewModelBase
    {

        private ObservableCollection<MaintenanceTaskViewModel> _taskEntries = new ObservableCollection<MaintenanceTaskViewModel>();
        public ObservableCollection<MaintenanceTaskViewModel> TaskEntries { get => _taskEntries; set => _taskEntries = value; }

        private MaintenanceTaskViewModel _selectedTask;
        public MaintenanceTaskViewModel SelectedTask {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged("SelectedTask");
            }
        }

        public TasksPageViewModel()
        {
            UpdateTaskListView();

            dataEntryWindow = new DataEntryWindowViewModel(new TaskDataInputViewModel());

            dataEntryWindow.DataEntrySubmitted += UpdateTaskListView;

            LoadNewTaskWindowCommand = new RelayCommand(execute => dataEntryWindow.LoadWindow());
            LoadEditTaskWindowCommand = new RelayCommand(execute => dataEntryWindow.LoadWindow(true, SelectedTask), canExecute => { return SelectedTask != null; });
            DeleteSelectedTaskCommand = new RelayCommand(execute => DeleteTask(), canExecute => { return SelectedTask != null; });
        }


        private void UpdateTaskListView()
        {
            TaskEntries.Clear();

            foreach(MaintenanceTask task in SqliteDataAccess.GetAllTasks())
            {
                TaskEntries.Add(new MaintenanceTaskViewModel(task));
            }
        }

        private void UpdateTaskListView(object sender, DataEntryEventArgs args)
        {

            MaintenanceTaskViewModel task = (MaintenanceTaskViewModel)args.SubmittedItem;

            if (TaskEntries.Contains(task))
            {
                TaskEntries.Remove(task);
                TaskEntries.Add(task);
            }
            else TaskEntries.Add(task);
            
        }


        public ICommand LoadNewTaskWindowCommand { get; }
        public ICommand LoadEditTaskWindowCommand { get; }
        public ICommand DeleteSelectedTaskCommand { get; }

        private DataEntryWindowViewModel dataEntryWindow;

        private void DeleteTask()
        {

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SqliteDataAccess.DeleteMaintenanceTask(SelectedTask.ToMaintenanceTask());
                TaskEntries.Remove(SelectedTask);

            }
        }
    }
}
