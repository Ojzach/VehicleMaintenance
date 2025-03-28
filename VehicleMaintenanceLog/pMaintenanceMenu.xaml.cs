using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VehicleMaintenanceLog.Classes;

namespace VehicleMaintenanceLog
{
    /// <summary>
    /// Interaction logic for pMaintenanceTaskMenu.xaml
    /// </summary>
    public partial class pMaintenanceMenu : Page
    {

        private ObservableCollection<TaskSchedule> scheduleEntries = new ObservableCollection<TaskSchedule>();
        public ObservableCollection<TaskSchedule> ScheduleEntries { get { return scheduleEntries; } set { scheduleEntries = value; } }

        private ObservableCollection<MaintenanceLogItem> logEntries = new ObservableCollection<MaintenanceLogItem>();
        public ObservableCollection<MaintenanceLogItem> LogEntries { get { return logEntries; } set { logEntries = value; } }


        MaintenanceMenuStatus currentPageStatus = MaintenanceMenuStatus.Empty;

        public pMaintenanceMenu()
        {
            DataContext = this;
            InitializeComponent();

            foreach (VehicleType type in Enum.GetValues(typeof(VehicleType))) cbVehicleTypeSelection.Items.Add(type);

        }

        public void LoadPage(MaintenanceMenuStatus selectedMenu, int vID = -1)
        {
            ClearPage();

            currentPageStatus = selectedMenu;
            if (currentPageStatus == MaintenanceMenuStatus.TaskMenu) LoadTaskMenu();
            else if (currentPageStatus == MaintenanceMenuStatus.LogMenu) LoadLogMenu();

            //Sets the Selected VehicleType and Vehicle to the vehicle selected in the previous menu
            cbVehicleTypeSelection.SelectedIndex = vID == -1 ? 0 : (int)SqliteDataAccess.GetVehicleType(vID);
            foreach (ComboBoxItem item in cbVehicleSelection.Items) if ((int)item.Tag == vID) cbVehicleSelection.SelectedItem = item;

            
            App.VehicleDataInputWindow.NewVehicleCreated += UpdateVehicleComboBox;
           
        }

        public void ClosePage()
        {
            App.ScheduleDataInputWindow.UpdatedSchedules -= UpdateSchedule;
            App.TaskDataInputWindow.NewTaskCreated -= UpdateSchedule;
            App.VehicleDataInputWindow.NewVehicleCreated -= UpdateVehicleComboBox;
            App.LogDataInputWindow.NewLogCreated -= UpdateLog;

            if (cbVehicleSelection.SelectedIndex != -1) App.selectedVehicleID = SelectedVehicleID;

            showHiddenSchedules = false;

            ClearPage();
        }

        public void ClearPage()
        {
            gvMaintenanceMenu.Columns.Clear();
            ScheduleEntries.Clear();
            LogEntries.Clear();
            cbVehicleTypeSelection.SelectedIndex = -1;
            currentPageStatus = 0;
            lvMaintenanceMenu.Items.SortDescriptions.Clear();
            currentSelectedColumnHeader = null;
            currentListSortDirection = ListSortDirection.Ascending;

            bCreateLog.Visibility = Visibility.Collapsed;
            bEditSchedule.Visibility = Visibility.Collapsed;
            bCreateSchedule.Visibility = Visibility.Collapsed;
            bEditLog.Visibility = Visibility.Collapsed;
            bCreateTask.Visibility = Visibility.Collapsed;
            gShowHiddenCheckBox.Visibility = Visibility.Collapsed;
        }

        private void cbVehicleType_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateVehicleComboBox();
        private void UpdateVehicleComboBox()
        {
            cbVehicleSelection.Items.Clear();

            if (cbVehicleTypeSelection.SelectedItem != null)
            {

                if (currentPageStatus == MaintenanceMenuStatus.TaskMenu) cbVehicleSelection.Items.Add(new ComboBoxItem() { Content = "Type Default", Tag = -1 });

                foreach (Vehicle v in SqliteDataAccess.LoadVehicles())
                {
                    if (v.vehicleType == SelectedVehicleType)
                    {
                        cbVehicleSelection.Items.Add(new ComboBoxItem() { Content = v.VehicleName, Tag = v.vehicleID });
                    }
                }

                if (cbVehicleSelection.Items.Count > 0) cbVehicleSelection.SelectedIndex = 0;
            }
        }


        private void cbVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbVehicleSelection.SelectedItem != null)
            {

                if (currentPageStatus == MaintenanceMenuStatus.TaskMenu)
                {
                    //Selected Index 0 is the default which means there is no selected vehicle
                    int selectedVehicle = cbVehicleSelection.SelectedIndex == 0 ? -1 : SelectedVehicleID;

                    UpdateTasks(SelectedVehicleType, selectedVehicle);
                }
                else if (currentPageStatus == MaintenanceMenuStatus.LogMenu) UpdateLog();

            }

        }

        GridViewColumnHeader currentSelectedColumnHeader;
        ListSortDirection currentListSortDirection = ListSortDirection.Ascending;
        private void lvMaintenanceMenuHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;


            if (headerClicked != null)
            {
                ListSortDirection sortDirection = ListSortDirection.Ascending;

                string propertyName = ((Binding)headerClicked.Column.DisplayMemberBinding).Path.Path;

                if (currentSelectedColumnHeader == null || currentSelectedColumnHeader != headerClicked)
                {
                    if(propertyName == "VehicleMileage" || propertyName == "DateCompleted") sortDirection = ListSortDirection.Descending;
                    else sortDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortDirection = currentListSortDirection == ListSortDirection.Ascending? ListSortDirection.Descending : ListSortDirection.Ascending;
                }

                lvMaintenanceMenu.Items.SortDescriptions.Clear();
                lvMaintenanceMenu.Items.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));
                if(currentPageStatus == MaintenanceMenuStatus.LogMenu) lvMaintenanceMenu.Items.SortDescriptions.Add(new SortDescription("VehicleMileage", ListSortDirection.Descending));

                currentSelectedColumnHeader = headerClicked;
                currentListSortDirection = sortDirection;
            }


        }


        private void bDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (lvMaintenanceMenu.SelectedIndex != -1)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    if (currentPageStatus == MaintenanceMenuStatus.TaskMenu)
                    {
                        
                        if(cbVehicleSelection.SelectedIndex == 0 || ((TaskSchedule)lvMaintenanceMenu.SelectedItem).vehichleAssignedTo != -1)
                        {
                            SqliteDataAccess.DeleteSchedule(((TaskSchedule)lvMaintenanceMenu.SelectedItem).ScheduleID);
                        }
                        else
                        {
                            SqliteDataAccess.CreateTaskSchedule(new TaskSchedule(((TaskSchedule)lvMaintenanceMenu.SelectedItem).TaskID, SelectedVehicleID, -1, -1, "Not Applicable To This Vehicle"));
                            
                        }

                        UpdateTasks(SelectedVehicleType, SelectedVehicleID);
                    }
                    else if (currentPageStatus == MaintenanceMenuStatus.LogMenu)
                    {
                        SqliteDataAccess.DeleteMaintenanceLog(((MaintenanceLogItem)lvMaintenanceMenu.SelectedItem).LogID);
                        LogEntries.RemoveAt(lvMaintenanceMenu.SelectedIndex);
                    }

                }
            }
        }


        #region TaskMenu

        private GridViewColumn[] taskMenuColumns = new GridViewColumn[5] {
            new GridViewColumn { Width = 0, DisplayMemberBinding = new Binding("ScheduleID") },
            new GridViewColumn { Header = "Task", Width = 150, DisplayMemberBinding = new Binding("TaskName") },
            new GridViewColumn { Header = "How Often Miles", Width = 100, DisplayMemberBinding = new Binding("MileageIncrement") },
            new GridViewColumn { Header = "How Often Time", Width = 100, DisplayMemberBinding = new Binding("TimeIncrement") },
            new GridViewColumn { Header = "Notes", Width = 300, DisplayMemberBinding = new Binding("ScheduleNotes") }
        };
        private bool showHiddenSchedules = false;

        public void LoadTaskMenu()
        {
            App.ScheduleDataInputWindow.UpdatedSchedules += UpdateSchedule;
            App.TaskDataInputWindow.NewTaskCreated += UpdateSchedule;

            lvMaintenanceMenu.ItemsSource = ScheduleEntries;
            
            foreach(GridViewColumn column in taskMenuColumns) gvMaintenanceMenu.Columns.Add(column);

            bEditSchedule.Visibility = Visibility.Visible;
            bCreateSchedule.Visibility = Visibility.Visible;
            bCreateTask.Visibility = Visibility.Visible;
            gShowHiddenCheckBox.Visibility = Visibility.Visible;
        }

        private void UpdateSchedule() => UpdateTasks(SelectedVehicleType, SelectedVehicleID);

        private void UpdateTasks(VehicleType type, int vehicleID = -1)
        {
            ScheduleEntries.Clear();

            foreach (TaskSchedule item in SqliteDataAccess.GetMaintenanceSchedules(type, vehicleID))
            { 
                
                if(showHiddenSchedules == false)
                {
                    if (item.timeIncrement != -1 || item.mileageIncrement != -1)
                    {
                        ScheduleEntries.Add(item);
                    }
                }
                else ScheduleEntries.Add(item);

            }
        }

        private void bEditSchedule_Click(object sender, RoutedEventArgs e)
        {
            if(lvMaintenanceMenu.SelectedItem != null) App.ScheduleDataInputWindow.LoadPage(SelectedVehicleID, true, ((TaskSchedule)lvMaintenanceMenu.SelectedItem).ScheduleID);
        }

        private void bCreateSchedule_Click(object sender, RoutedEventArgs e)
        {
            App.ScheduleDataInputWindow.LoadPage(SelectedVehicleID, editItemID: (int)SelectedVehicleType);
        }

        private void bCreateTask_Click(object sender, RoutedEventArgs e)
        {
            App.TaskDataInputWindow.LoadPage((int)SelectedVehicleType);
        }

        private void chbShowHidden_Checked(object sender, RoutedEventArgs e)
        {
            showHiddenSchedules = true;
            UpdateTasks(SelectedVehicleType, SelectedVehicleID);
        }

        private void chbShowHidden_Unchecked(object sender, RoutedEventArgs e)
        {
            showHiddenSchedules = false;
            UpdateTasks(SelectedVehicleType, SelectedVehicleID);
        }

        #endregion


        #region LogMenu

        private GridViewColumn[] logMenuColumns = new GridViewColumn[6] {
            new GridViewColumn { Width = 0, DisplayMemberBinding = new Binding("LogID") },
            new GridViewColumn { Header = "Task", Width = 150, DisplayMemberBinding = new Binding("TaskName") },
            new GridViewColumn { Header = "Mileage", Width = 100, DisplayMemberBinding = new Binding("VehicleMileage") },
            new GridViewColumn { Header = "Date Completed", Width = 100, DisplayMemberBinding = new Binding("DateCompleted") },
            new GridViewColumn { Header = "Temp Fix", Width = 100, DisplayMemberBinding = new Binding("TempFixTxt")},
            new GridViewColumn { Header = "Log Notes", Width = 150, DisplayMemberBinding = new Binding("LogNotes") }
        };
        public void LoadLogMenu()
        {
            App.LogDataInputWindow.NewLogCreated += UpdateLog;

            lvMaintenanceMenu.ItemsSource = LogEntries;
            foreach (GridViewColumn column in logMenuColumns) gvMaintenanceMenu.Columns.Add(column);

            bCreateLog.Visibility = Visibility.Visible;
            bEditLog.Visibility = Visibility.Visible;
        }

        private void UpdateLog()
        {
            LogEntries.Clear();

            foreach (MaintenanceLogItem item in SqliteDataAccess.GetEntireVehicleMaintenanceLog(SelectedVehicleID))
            {
                if (!LogEntries.Contains(item)) LogEntries.Add(item);
            }
        }

        private void bCreateLog_Click(object sender, RoutedEventArgs e)
        {
            if(cbVehicleSelection.SelectedIndex != -1) App.LogDataInputWindow.LoadPage(SelectedVehicleID);
        }

        private void bEditLog_Click(object sender, RoutedEventArgs e)
        {
            if(lvMaintenanceMenu.SelectedItem != null)
            {
                App.LogDataInputWindow.LoadPage(SelectedVehicleID, true, ((MaintenanceLogItem)lvMaintenanceMenu.SelectedItem).LogID);
            }
        }

        #endregion




        private int SelectedVehicleID { get { return (int)((ComboBoxItem)cbVehicleSelection.SelectedItem).Tag; } }
        private VehicleType SelectedVehicleType { get { return (VehicleType)cbVehicleTypeSelection.SelectedItem;  } }


    }

    public enum MaintenanceMenuStatus
    {
        Empty = 0,
        TaskMenu = 1,
        LogMenu = 2,
    }
}
