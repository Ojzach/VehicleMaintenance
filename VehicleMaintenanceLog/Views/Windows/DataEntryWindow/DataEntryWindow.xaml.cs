using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace VehicleMaintenanceLog
{
    /// <summary>
    /// Interaction logic for pAddNewLog.xaml
    /// </summary>
    public partial class DataEntryWindow : Window
    {

        public DataEntryWindow()
        {
            InitializeComponent();
            
        }

        private void bdrDragBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void TextIsIntegerValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(cc => Char.IsNumber(cc));
            base.OnPreviewTextInput(e);
        }


    }


}
