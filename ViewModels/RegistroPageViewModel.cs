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
        private bool _isRegistroInfoInvalid;
        private bool _isMedicalInfoInvalid;

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
        public bool IsRegistroInfoInvalid
        {
            get { return _isRegistroInfoInvalid; }
            private set { SetProperty(ref _isRegistroInfoInvalid, value); }
        }

        [RestorableState]
        public bool IsMedicalInfoInvalid
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

            Title = _resourceLoader.GetString("RegistroTitle");
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
            IsRegistroInfoInvalid = RegistroViewModel.ValidateForm() == false;
            IsMedicalInfoInvalid = MedicalViewModel.ValidateForm() == false;

            if (IsRegistroInfoInvalid || IsMedicalInfoInvalid) return;
            
                string errorMessage = string.Empty;

                try
                {
                    if(await ProcessFormAsync())
                    _navigationService.GoBack();
                }
                catch (ModelValidationException mvex)
                {
                    DisplayValidationErrors(mvex.ValidationResult);
                }
                catch (Exception ex)
                {
                    errorMessage = string.Format(CultureInfo.CurrentCulture, _resourceLoader.GetString("GeneralServiceErrorMessage"), Environment.NewLine, ex.Message);
                }

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    _dialogService.Show(_resourceLoader.GetString("ErrorServiceUnreachable") + ". " + errorMessage);
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

        private void DisplayValidationErrors(ModelValidationResult modelValidationResults)
        {
            var errors = new Dictionary<string, ReadOnlyCollection<string>>();

            // Property keys format: address.{Propertyname}
            foreach (var propkey in modelValidationResults.ModelState.Keys)
            {
                string propertyName = propkey.Substring(propkey.IndexOf('.') + 1); // strip off order. prefix

                errors.Add(propertyName, new ReadOnlyCollection<string>(modelValidationResults.ModelState[propkey]));
            }

            if (errors.Count > 0) RegistroViewModel.BindableValidator.SetAllErrors(errors);
        }

    }
}
