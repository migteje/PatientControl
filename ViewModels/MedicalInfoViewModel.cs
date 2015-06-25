using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using PatientControl.Controls;
using PatientControl.Interfaces;
using PatientControl.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace PatientControl.ViewModels
{
    public class MedicalInfoViewModel: ViewModel, Interfaces.IMedicalInfoViewModel
    {
        private PacienteViewModel _paciente;

        public PacienteViewModel Paciente
        {
            get { return _paciente; }
            set { SetProperty(ref _paciente, value); }
        }
        private IReadOnlyCollection<ComboBoxItemValue> _articulaciones;
        public IReadOnlyCollection<ComboBoxItemValue> Articulaciones
        {
            get { return _articulaciones; }
            private set { SetProperty(ref _articulaciones, value); }
        }

        //private readonly ICheckoutDataRepository _checkoutDataRepository;
        private readonly IResourceLoader _resourceLoader;
        private readonly IDialogService _dialogService;
        public DelegateCommand<string> CheckedCommand { get; private set; }

        public MedicalInfoViewModel(IResourceLoader resourceLoader, IDialogService dialogService)
            {
                _paciente = new PacienteViewModel(new Paciente());
                _resourceLoader = resourceLoader;
                _dialogService = dialogService;
                this.CheckedCommand = new DelegateCommand<string>(Checked);
            }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            if (viewModelState != null)
            {
                base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
            }

            if (navigationMode == NavigationMode.New)
            {
                _paciente = (PacienteViewModel) navigationParameter;
            }
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewState, bool suspending)
        {
            base.OnNavigatedFrom(viewState, suspending);
        }

        public bool ValidateForm()
        {
            return true;
        }

        public async Task ProcessFormAsync()
        {
               await Paciente.UpdateMedicalInfoPaciente(Paciente.Nombre);
        }

        private void Checked(object parameter)
        {
            Paciente.ZonaLesion = parameter.ToString();
        }

    }
}
