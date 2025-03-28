using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VehicleMaintenanceLog
{
    /// <summary>
    /// Interaction logic for pAddNewLog.xaml
    /// </summary>
    public partial class wDataEntryWindow : Window
    {

        public event Action NewVehicleCreated;
        public event Action NewLogCreated;
        public event Action NewTaskCreated;
        public event Action UpdatedSchedules;

        public IDataEntryBoxPage dataInputPage;

        private bool editMode = false;

        public wDataEntryWindow(IDataEntryBoxPage openedInputMenu)
        {
            InitializeComponent();

            dataInputPage = openedInputMenu;
            fDataInput.Content = dataInputPage.InputPage;
            
            this.Height = dataInputPage.WindowDimensions.h;
            this.Width = dataInputPage.WindowDimensions.w;
        }

        public void LoadPage(int vID = -1, bool editMode = false, int editItemID = -1)
        {
            this.editMode = editMode;
            Show();
            dataInputPage.LoadPage(editMode, vID, editItemID);

            txtTitle.Text = dataInputPage.InputTitle;
        }

        private void bCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            dataInputPage.ClosePage();
            Hide();
        }

        private void bSubmitLog_Click(object sender, RoutedEventArgs e)
        {
            if(dataInputPage.SubmitData(editMode))
            {
                dataInputPage.ClearData();

                if (dataInputPage.InputPage is pVehicleDataInput) NewVehicleCreated?.Invoke();
                else if (dataInputPage.InputPage is pLogDataInput) NewLogCreated?.Invoke();
                else if (dataInputPage.InputPage is pTaskDataInput) NewTaskCreated?.Invoke();
                else if (dataInputPage.InputPage is pScheduleDataInput) UpdatedSchedules?.Invoke();


                if (editMode) bCloseWindow_Click(null, null);
            }
        }

        private void bdrDragBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void TextIsIntegerValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(cc => Char.IsNumber(cc));
            base.OnPreviewTextInput(e);
        }

        public bool IsClosed { get; private set; }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }


    }

    public interface IDataEntryBoxPage
    {
        Page InputPage { get; }
        string InputTitle { get; }
        (int w, int h) WindowDimensions { get;  }

        void LoadPage(bool editMode = false, int input = -1, int editItemID = -1);
        void ClosePage();
        void ClearData();
        bool SubmitData(bool editMode = false);
    }
}
