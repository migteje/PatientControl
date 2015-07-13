using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using PatientControl.Controls;
using PatientControl.Interfaces;
using PatientControl.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace PatientControl.ViewModels
{
    public class RegistroPageViewModel: ViewModel
    {
        string _Title = default(string);
        public string Title { get { return _Title; } set { SetProperty(ref _Title, value); } }

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegistroUserControlViewModel _registroViewModel;
        private readonly IMedicalInfoViewModel _medicalViewModel;
        private string _isRegistroInfoInvalid;
        private string _isMedicalInfoInvalid;

        public IRegistroUserControlViewModel RegistroViewModel
        {
            get { return _registroViewModel; }
        }
        public IMedicalInfoViewModel MedicalViewModel
        {
            get { return _medicalViewModel; }
        }
        public ICommand SaveCommand { get; private set; }
        private readonly IResourceLoader _resourceLoader;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        [RestorableState]
        public string IsRegistroInfoInvalid
        {
            get { return _isRegistroInfoInvalid; }
            private set { SetProperty(ref _isRegistroInfoInvalid, value); }
        }

        private PacienteViewModel _paciente;

        public PacienteViewModel Paciente
        {
            get { return _paciente; }
            set { SetProperty(ref _paciente, value); }
        }

        [RestorableState]
        public string IsMedicalInfoInvalid
        {
            get { return _isMedicalInfoInvalid; }
            private set { SetProperty(ref _isMedicalInfoInvalid, value); }
        }

        public RegistroPageViewModel(IRegistroUserControlViewModel registroViewModel, IMedicalInfoViewModel medicalViewModel, IResourceLoader resourceLoader, IDialogService dialogService, INavigationService navigationService, IEventAggregator eventAggregator)
            {
                _registroViewModel = registroViewModel;
                _medicalViewModel = medicalViewModel;
                _resourceLoader = resourceLoader;
                _dialogService = dialogService;
                _eventAggregator = eventAggregator;
                _navigationService = navigationService;

                SaveCommand = DelegateCommand.FromAsyncHandler(SaveAsync);
            }

        public override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            if (viewModelState == null) return;

            _paciente = (PacienteViewModel)navigationParameter;
            if (Paciente.Id == 0)
                Title = _resourceLoader.GetString("RegistroTitle");
            else Title = _resourceLoader.GetString("EditarTitle");
            if (navigationMode == NavigationMode.New)
            {
                viewModelState["RegistroViewModel"] = new Dictionary<string, object>();
                viewModelState["MedicalViewModel"] = new Dictionary<string, object>();
            }
            
            RegistroViewModel.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
            MedicalViewModel.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            if (viewModelState == null || viewModelState.Count == 0) return;

            RegistroViewModel.OnNavigatedFrom(viewModelState["RegistroViewModel"] as Dictionary<string, object>, suspending);
            MedicalViewModel.OnNavigatedFrom(viewModelState["MedicalViewModel"] as Dictionary<string, object>, suspending);

            base.OnNavigatedFrom(viewModelState, suspending);
        }

        private async Task SaveAsync()
        {
            IsRegistroInfoInvalid = RegistroViewModel.ValidateForm();
            IsMedicalInfoInvalid = MedicalViewModel.ValidateForm();

            if (!IsRegistroInfoInvalid.Equals("bien") || !IsMedicalInfoInvalid.Equals("bien"))
            {
            //    _eventAggregator.GetEvent<Events.BadRegister>().Subscribe(HandleBadRegister);
                HandleBadRegister("Los siguientes campos necesitan revisión: ");
                return;
            }
                string errorMessage = string.Empty;
                try
                {
                    if(await ProcessFormAsync())
                    _navigationService.GoBack();
                }
                catch (Exception ex)
                {
                    errorMessage = string.Format(CultureInfo.CurrentCulture, _resourceLoader.GetString("GeneralServiceErrorMessage"), Environment.NewLine, ex.Message);
                    _dialogService.Show(errorMessage);
                }
        }

        private async Task<bool> ProcessFormAsync()
        {
            
            try
            {
                if (await RegistroViewModel.ProcessFormAsync())
                {
                    await MedicalViewModel.ProcessFormAsync();
                    return true;
                }
                return false;
            }
            catch (ModelValidationException)
            {
                return false;
            }
        }

        private void HandleBadRegister(string value)
        {
            if (IsRegistroInfoInvalid.Equals("bien"))
            _dialogService.Show(value + IsMedicalInfoInvalid);
            if (IsMedicalInfoInvalid.Equals("bien"))
            _dialogService.Show(value + IsRegistroInfoInvalid);
                if (!IsRegistroInfoInvalid.Equals("bien") & !IsMedicalInfoInvalid.Equals("bien"))
                    _dialogService.Show(value + IsRegistroInfoInvalid + " " + IsMedicalInfoInvalid);
        }
    }
}
