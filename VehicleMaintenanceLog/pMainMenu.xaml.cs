using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using VehicleMaintenanceLog.Classes;

namespace VehicleMaintenanceLog
{
    /// <summary>
    /// Interaction logic for pMainMenu.xaml
    /// </summary>
    public partial class pMainMenu : Page
    {
        private ObservableCollection<MaintenanceTaskStatus> entries = new ObservableCollection<MaintenanceTaskStatus>();
        public ObservableCollection<MaintenanceTaskStatus> Entries { get { return entries; } set { entries = value; } }

        private MaintenanceTaskStatus selectedEntry = null;
        public MaintenanceTaskStatus SelectedEntry { get { return selectedEntry; } set { selectedEntry = value; } }


        public pMainMenu()
        {
            DataContext = this;
            InitializeComponent();
        }

        public void LoadPage()
        {
            ClearPage();

            App.VehicleDataInputWindow.NewVehicleCreated += UpdateCBVehicleSelection;

            UpdateCBVehicleSelection();

        }

        private void UpdateCBVehicleSelection()
        {
            cbVehicleSelection.Items.Clear();

            foreach (Vehicle v in SqliteDataAccess.LoadVehicles())
            {
                cbVehicleSelection.Items.Add(new ComboBoxItem() { Content = v.VehicleName, Tag = v.vehicleID });

                if (v.vehicleID == App.selectedVehicleID) cbVehicleSelection.SelectedIndex = cbVehicleSelection.Items.Count - 1;
            }

            if (cbVehicleSelection.Items.Count != 0 && cbVehicleSelection.SelectedIndex == -1) cbVehicleSelection.SelectedIndex = 0;
        }


        public void ClosePage()
        {
            App.VehicleDataInputWindow.NewVehicleCreated -= UpdateCBVehicleSelection;

            ClearPage();
        }

        private void ClearPage()
        {
            cbVehicleSelection.Items.Clear();
            Entries.Clear();
            SelectedEntry = null;
            tbVehicleMileage.Text = "Mileage: 0000";
            tbVehicleManufactureDate.Text = "Manufacture Date: 00/00/0000";
            tbxSetMileage.Text = "";
        }

        private void UpdateMaintenanceStatusMenu(int vID)
        {
            Entries.Clear();

            List<TaskSchedule> schedules = SqliteDataAccess.GetMaintenanceSchedules(SqliteDataAccess.GetVehicleType(vID), vID);
            int vehicleMileage = SqliteDataAccess.GetValue<int>("Vehicle", vID, "Mileage");
            DateTime vehicleManufactureDate = DateTime.FromBinary(SqliteDataAccess.GetValue<Int64>("Vehicle", vID, "ManufactureDate"));

            int mileageToNext = -1;
            DateTime dateOfNext = DateTime.MinValue;

            foreach (TaskSchedule schedule in schedules)
            {
                MaintenanceLogItem previousLog = SqliteDataAccess.GetMostRecentVehicleMaintenanceLog(vID, schedule.TaskID);

                if (previousLog != null) 
                {

                    if (previousLog.tempFixMileage == -1) mileageToNext = schedule.mileageIncrement == -1 ? int.MaxValue : (previousLog.VehicleMileage + schedule.mileageIncrement) - vehicleMileage;
                    else mileageToNext = (previousLog.VehicleMileage + previousLog.tempFixMileage) - vehicleMileage;

                    if (previousLog.tempFixTime == -1) dateOfNext = schedule.timeIncrement == -1 ? DateTime.MaxValue : previousLog.datecompleted.AddMonths(schedule.timeIncrement);
                    else dateOfNext = previousLog.datecompleted.AddMonths(previousLog.tempFixTime);

                    Entries.Add(new MaintenanceTaskStatus(schedule.TaskID, schedule.ScheduleID, previousLog.LogID, schedule.TaskName, (previousLog.tempFixMileage != -1, previousLog.tempFixTime != -1), mileageToNext, dateOfNext));
                }
                else
                {
                    mileageToNext = schedule.mileageIncrement == -1 ? int.MaxValue : schedule.mileageIncrement - vehicleMileage;
                    dateOfNext = schedule.timeIncrement == -1 ? DateTime.MaxValue : vehicleManufactureDate.AddMonths(schedule.timeIncrement);

                    if(mileageToNext != int.MaxValue || dateOfNext != DateTime.MaxValue) Entries.Add(new MaintenanceTaskStatus(schedule.TaskID, schedule.ScheduleID, -1 , schedule.TaskName, (false, false), mileageToNext, dateOfNext));
                }


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
                    sortDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortDirection = currentListSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }

                lvMaintenanceStatus.Items.SortDescriptions.Clear();
                lvMaintenanceStatus.Items.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));
                lvMaintenanceStatus.Items.SortDescriptions.Add(new SortDescription("VehicleMileage", ListSortDirection.Descending));

                currentSelectedColumnHeader = headerClicked;
                currentListSortDirection = sortDirection;
            }


        }

        private void cbVehicleSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            if (cbVehicleSelection.SelectedItem != null)
            {
                App.selectedVehicleID = SelectedVehicleID;
                UpdateMaintenanceStatusMenu(SelectedVehicleID);

                tbVehicleManufactureDate.Text = "Manufacture Date: " + DateTime.FromBinary(SqliteDataAccess.GetValue<Int64>("Vehicle", SelectedVehicleID, column: "ManufactureDate")).ToString("d");
                UpdateMileageTB();
            }
        }

        private void bUpdateMileage_Click(object sender, RoutedEventArgs e)
        {
            if(tbxSetMileage.Text != "" && SelectedVehicleID != -1)
            {
                SqliteDataAccess.SetVehicleMileage(SelectedVehicleID, int.Parse(tbxSetMileage.Text));
                tbxSetMileage.Text = "";
                UpdateMileageTB();
                UpdateMaintenanceStatusMenu(SelectedVehicleID);
            }
        }

        private void bEditVehicle_Click(object sender, RoutedEventArgs e)
        {
            if (cbVehicleSelection.SelectedIndex != -1)
            {
                App.VehicleDataInputWindow.LoadPage(SelectedVehicleID, true);
            }

        }

        private void bDeleteVehicle_Click(object sender, RoutedEventArgs e)
        {
            if (cbVehicleSelection.SelectedIndex != -1)
            {

                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    SqliteDataAccess.DeleteVehicle(SelectedVehicleID);
                    cbVehicleSelection.Items.Remove(cbVehicleSelection.SelectedItem);

                    if(cbVehicleSelection.Items.Count > 0) cbVehicleSelection.SelectedIndex = 0;
                    else
                    {
                        ClearPage();
                        App.selectedVehicleID = -1;     
                    }
                }

            }
        }

        private void UpdateMileageTB() => tbVehicleMileage.Text = "Mileage: " + SqliteDataAccess.GetValue<int>("Vehicle", SelectedVehicleID, column: "Mileage").ToString("N0");

        private int SelectedVehicleID { get { return (int)((ComboBoxItem)cbVehicleSelection.SelectedItem).Tag; } }


        private void TextIsIntegerValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(cc => Char.IsNumber(cc));
            base.OnPreviewTextInput(e);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double remainingSpace = lvMaintenanceStatus.ActualWidth;

            if (remainingSpace > 0)
            {
                float column1weight = 2, column2weight = 1, column3weight = 1;
                float totalWeight = column1weight + column2weight + column3weight;

                (lvMaintenanceStatus.View as GridView).Columns[0].Width = Math.Ceiling(remainingSpace * (column1weight / totalWeight));
                (lvMaintenanceStatus.View as GridView).Columns[1].Width = Math.Ceiling(remainingSpace * (column2weight / totalWeight));
                (lvMaintenanceStatus.View as GridView).Columns[2].Width = Math.Ceiling(remainingSpace * (column3weight / totalWeight));
            }
        }
    }

    public class MaintenanceTaskStatus
    {
        public int taskID = -1;
        public int scheduleID = -1;
        public int previousLogID = -1;
        public (bool miles, bool time) previousLogTempFix = (false, false);

        public string TaskName { get; set; } = "";
        private int mileageToNext = int.MaxValue;
        public string MileageToNext { get { return mileageToNext == int.MaxValue ? "NA" : mileageToNext.ToString(); } }
        private DateTime dateOfNext = DateTime.MaxValue;
        public string DateOfNext { get { return dateOfNext == DateTime.MaxValue ? "NA" : dateOfNext.ToString("d"); } }

        Brush greenBrush = new SolidColorBrush( Color.FromRgb(33, 166, 27) );
        Brush redBrush = new SolidColorBrush( Color.FromRgb(214, 18, 13) );
        Brush yellowBrush = new SolidColorBrush(Color.FromRgb(235, 203, 14) );


        public Brush MileageColor { get {
                if (mileageToNext < 0) return redBrush;
                else if (previousLogTempFix.miles == true) return yellowBrush;
                else if (mileageToNext == int.MaxValue) return Brushes.Black;
                else return greenBrush; } }
        public Brush DateColor { get {
                if (dateOfNext.Ticks - DateTime.Today.Ticks < 0) return redBrush;
                else if (previousLogTempFix.time == true) return yellowBrush;
                else if (dateOfNext == DateTime.MaxValue) return Brushes.Black;
                else return greenBrush; } }

        public MaintenanceTaskStatus(int taskID, int scheduleID, int previousLogID, string taskName, (bool miles, bool time) isTempFix, int mileageToNext, DateTime dateOfNext)
        {
            this.taskID = taskID;
            this.scheduleID = scheduleID;
            this.previousLogID = previousLogID;
            TaskName = taskName;
            this.mileageToNext = mileageToNext;
            this.dateOfNext = dateOfNext;
            previousLogTempFix = isTempFix;
        }
    }

}
