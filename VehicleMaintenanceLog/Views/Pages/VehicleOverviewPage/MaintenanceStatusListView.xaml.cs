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
    /// Interaction logic for MaintenanceStatusListView.xaml
    /// </summary>
    public partial class MaintenanceStatusListView : UserControl
    {
        public MaintenanceStatusListView()
        {
            InitializeComponent();
        }


        GridViewColumnHeader currentSelectedColumnHeader;
        ListSortDirection currentListSortDirection = ListSortDirection.Ascending;
        private void lvMaintenanceMenuHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null)
            {
                ListSortDirection sortDirection = ListSortDirection.Ascending;

                string propertyName = headerClicked.Column.Header.ToString() switch
                {
                    "Miles To Next" => "SortByMileage",
                    "Complete By" => "SortByDate",
                    _ => "TaskName"
                };


                if (currentSelectedColumnHeader != headerClicked || currentListSortDirection == ListSortDirection.Descending)
                    sortDirection = ListSortDirection.Ascending;
                else sortDirection = ListSortDirection.Descending;

                lvMaintenanceStatus.Items.SortDescriptions.Clear();
                lvMaintenanceStatus.Items.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));
                lvMaintenanceStatus.Items.SortDescriptions.Add(new SortDescription("SortByMileage", ListSortDirection.Ascending));

                currentSelectedColumnHeader = headerClicked;
                currentListSortDirection = sortDirection;
            }


        }
    }
}
