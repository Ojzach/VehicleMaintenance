using VehicleMaintenanceLog.Stores;

namespace VehicleMaintenanceLog.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;


        public ViewModelBase _currentNavigationPanelViewModel;
        public ViewModelBase CurrentNavigationPanelViewModel => _currentNavigationPanelViewModel;
        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _currentNavigationPanelViewModel = new NavigationPanelViewModel(navigationStore);

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }


        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
