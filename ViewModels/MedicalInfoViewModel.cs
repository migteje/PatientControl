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

        //private readonly ICheckoutDataRepository _checkoutDataRepository;
        public DelegateCommand<string> CheckedCommand { get; private set; }

        public MedicalInfoViewModel(IResourceLoader resourceLoader)
            {
                Paciente = new PacienteViewModel(new Paciente());
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

        public string ValidateForm()
        {
            if (Paciente.FechaLesion.Year >= DateTime.Today.Year) 
                return "La fecha no es correcta";
            if (Paciente.Diagnostico == " " || Paciente.Diagnostico == null || Paciente.Diagnostico == "")
                return "El diagnóstico no puede ser nulo";
            if (Paciente.ZonaLesion == null || Paciente.ZonaLesion == "")
                return "Necesita seleccionar la zona de la lesión";
            return "bien";
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
