using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace PatientControl.ViewModels
{
    public class LoginPageViewModel : ViewModel, Interfaces.ILoginPageViewModel
    {
        string _Username = default(string);
        [RestorableState]
        public string Username { get { return _Username; } set { SetProperty(ref _Username, value); } }

        Interfaces.IDialogService _dialogService;
        IEventAggregator _eventAggregator;
        INavigationService _navigationService;

        public DelegateCommand LogInCommand { get; private set; }

        public LoginPageViewModel(Interfaces.IDialogService dialogService, INavigationService navigationService, IEventAggregator eventAggregator)
            {
                _dialogService = dialogService;
                _eventAggregator = eventAggregator;
                _navigationService = navigationService;

                LogInCommand = DelegateCommand.FromAsyncHandler(LogInAsync);
            }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            _eventAggregator.GetEvent<Events.Logout>().Subscribe(HandleLogout);
        }

        private void HandleLogout(string value)
        {
            _dialogService.Show(value);
        }

        private async Task LogInAsync()
        {
            await GoToNextPageAsync();
        }

        private async Task GoToNextPageAsync()
        {
            // Set up navigate action depending on the application's state
            var navigateAction = await ResolveNavigationActionAsync();

            // Execute the navigate action
            navigateAction();
        }

        private async Task<Action> ResolveNavigationActionAsync()
        {
            Action navigateAction = null;
            var navigationServiceReference = _navigationService;
            navigateAction = async () =>
                    {
                       _navigationService.Navigate(App.Experiences.Main.ToString(), null);
                    };
            return navigateAction;
        }

    }
}
