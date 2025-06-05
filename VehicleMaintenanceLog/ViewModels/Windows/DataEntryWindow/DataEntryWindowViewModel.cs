using System;
using System.Windows.Controls;
using System.Windows.Input;
using VehicleMaintenanceLog.Commands;

namespace VehicleMaintenanceLog.ViewModels
{
    public class DataEntryWindowViewModel
    {

        public IDataEntryView DataInputPage { get; }

        public event EventHandler<DataEntryEventArgs> DataEntrySubmitted;

        private DataEntryWindow _window;


        public DataEntryWindowViewModel(IDataEntryView inputPage)
        {
            DataInputPage = inputPage;

            _window = new DataEntryWindow() { DataContext = this, Width = DataInputPage.WindowDimensions.w, Height = DataInputPage.WindowDimensions.h};

            CloseWindowCommand = new RelayCommand(execute => CloseWindow());
            SubmitDataCommand = new RelayCommand(execute => SubmitData(), canExecute => DataInputPage.CanSubmit());
        }

        public void LoadWindow(bool isEditMode = false, ViewModelBase selectedItemToEdit = null, object inputData = null)
        {
            _window.Show();
            DataInputPage.LoadPage(isEditMode, selectedItemToEdit, inputData);
        }

        public ICommand SubmitDataCommand { get; }
        public ICommand CloseWindowCommand { get; }

        private void CloseWindow()
        {
            _window.Hide();
        }
        
        private void SubmitData()
        {
            object submittedData = DataInputPage.SubmitData();

            if (submittedData != null)
            {
                DataEntrySubmitted?.Invoke(this, new DataEntryEventArgs() { SubmittedItem = submittedData });
            }

            DataInputPage.ClearInputs();
            _window.Hide();
        }



    }
    public interface IDataEntryView
    {
        UserControl InputView { get; }
        string InputTitle { get; }
        (int w, int h) WindowDimensions { get; }

        void LoadPage(bool editMode, ViewModelBase selectedItemToEdit, object inputdata);
        bool CanSubmit();
        object SubmitData();
        public void ClearInputs();
    }

    public class DataEntryEventArgs : EventArgs
    {
        public object SubmittedItem;
    }
}
