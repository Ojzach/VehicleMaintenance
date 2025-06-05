using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VehicleMaintenanceLog.Views
{
    /// <summary>
    /// Interaction logic for MaintenanceProfilesView.xaml
    /// </summary>
    public partial class MaintenanceProfilesView : UserControl
    {
        public MaintenanceProfilesView()
        {
            InitializeComponent();
        }

        GridViewColumnHeader currentSelectedColumnHeader;
        ListSortDirection currentListSortDirection = ListSortDirection.Ascending;
        private void lvHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null)
            {
                ListSortDirection sortDirection = ListSortDirection.Ascending;

                string propertyName = headerClicked.Column.Header.ToString() switch
                {
                    "Mileage Increment" => "MileageIncrement",
                    "Time Increment" => "TimeIncrement",
                    "Notes" => "ScheduleNotes",
                    _ => "TaskName"
                };


                if (currentSelectedColumnHeader != headerClicked || currentListSortDirection == ListSortDirection.Descending)
                    sortDirection = ListSortDirection.Ascending;
                else sortDirection = ListSortDirection.Descending;

                lvProfileSchedules.Items.SortDescriptions.Clear();
                lvProfileSchedules.Items.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));

                currentSelectedColumnHeader = headerClicked;
                currentListSortDirection = sortDirection;
            }


        }
    }
}
