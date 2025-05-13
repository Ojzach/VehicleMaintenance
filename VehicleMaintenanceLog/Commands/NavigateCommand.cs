using System;
using VehicleMaintenanceLog.Stores;
using VehicleMaintenanceLog.ViewModels;

namespace VehicleMaintenanceLog.Commands
{
    class NavigateCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<ViewModelBase> _createViewModel;

        public NavigateCommand(NavigationStore navigationStore, Func<ViewModelBase> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public override void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = _createViewModel();
        }

        public override bool CanExecute(object parameter)
        {
            return _navigationStore.CurrentViewModel.GetType() != _createViewModel().GetType() && base.CanExecute(parameter);
        }
    }
}
