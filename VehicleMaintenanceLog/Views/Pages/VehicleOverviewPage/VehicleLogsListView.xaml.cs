using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VehicleMaintenanceLog.Views
{
    /// <summary>
    /// Interaction logic for VehicleLogsListView.xaml
    /// </summary>
    public partial class VehicleLogsListView : UserControl
    {
        public VehicleLogsListView()
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

                string propertyName = ((Binding)headerClicked.Column.DisplayMemberBinding).Path.Path;

                if (currentSelectedColumnHeader == null || currentSelectedColumnHeader != headerClicked)
                {
                    if (propertyName == "VehicleMileage" || propertyName == "DateCompleted") sortDirection = ListSortDirection.Descending;
                    else sortDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortDirection = currentListSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }

                lvMaintenanceMenu.Items.SortDescriptions.Clear();
                lvMaintenanceMenu.Items.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));
                lvMaintenanceMenu.Items.SortDescriptions.Add(new SortDescription("VehicleMileage", ListSortDirection.Descending));

                currentSelectedColumnHeader = headerClicked;
                currentListSortDirection = sortDirection;
            }


        }
    }
}
