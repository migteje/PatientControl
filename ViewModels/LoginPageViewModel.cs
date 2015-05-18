using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using PatientControl.Interfaces;
using PatientControl.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace PatientControl.ViewModels
{
    public class LoginPageViewModel : ViewModel, Interfaces.ILoginPageViewModel
    {
        string _Username = default(string);
        [RestorableState]
        public string Username { get { return _Username; } set { SetProperty(ref _Username, value); } }

        public IDialogService _dialogService;
        public IEventAggregator _eventAggregator;
        public INavigationService _navigationService;
        private PacienteViewModel _paciente;
        public PacienteViewModel Paciente
        {
            get { return _paciente; }
            set { SetProperty(ref _paciente, value); }
        }


        public DelegateCommand LogInCommand { get; private set; }
        public DelegateCommand RegistrarCommand { get; private set; }

        public LoginPageViewModel(Interfaces.IDialogService dialogService, INavigationService navigationService, IEventAggregator eventAggregator)
            {
                _dialogService = dialogService;
                _eventAggregator = eventAggregator;
                _navigationService = navigationService;
                Paciente = new PacienteViewModel(new Paciente());

                LogInCommand = DelegateCommand.FromAsyncHandler(LogInAsync);
                RegistrarCommand = DelegateCommand.FromAsyncHandler(Registrar);
            }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            _eventAggregator.GetEvent<Events.Logout>().Subscribe(HandleLogout);
        }

        private void HandleLogout(string value)
        {
            _dialogService.Show(value);
        }


        private async Task Registrar()
        {
            await GoToNextPageAsync("registro");
        }

        private async Task LogInAsync()
        {
            await GoToNextPageAsync("login");
        }

        private async Task GoToNextPageAsync(string page)
        {
            // Set up navigate action depending on the application's state
            var navigateAction = await ResolveNavigationActionAsync(page);

            // Execute the navigate action
            navigateAction();
        }

        private async Task<Action> ResolveNavigationActionAsync(string nav)
        {
            Action navigateAction = null;
            var navigationServiceReference = _navigationService;
            navigateAction = async () =>
                    {
                        if (nav.Equals("login"))
                        {
                            if (await Paciente.DoesDbExist("Paciente.db") && await Paciente.ObtenerPacienteInfo(Username))
                            {
                                _navigationService.Navigate(App.Experiences.Main.ToString(), Paciente);
                            }
                            
                        }
                        else
                            _navigationService.Navigate(App.Experiences.Registro.ToString(), Paciente);
                    };
            return navigateAction;
        }
    }
}
