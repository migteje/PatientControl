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
    public class RegistroUserControlViewModel: ViewModel, Interfaces.IRegistroUserControlViewModel
    {
        private PacienteViewModel _paciente;
        private readonly BindableValidator _bindableValidator;
        public BindableValidator BindableValidator
        {
            get
            {
                return _bindableValidator;
            }
        }

        public PacienteViewModel Paciente
        {
            get { return _paciente; }
            set { SetProperty(ref _paciente, value); }
        }
        private IReadOnlyCollection<ComboBoxItemValue> _provincias;
        public IReadOnlyCollection<ComboBoxItemValue> Provincias
        {
            get { return _provincias; }
            private set { SetProperty(ref _provincias, value); }
        }

        //private readonly ICheckoutDataRepository _checkoutDataRepository;
        private readonly IResourceLoader _resourceLoader;
        private readonly IDialogService _dialogService;

        public RegistroUserControlViewModel(IResourceLoader resourceLoader, IDialogService dialogService)
            {
                _paciente = new PacienteViewModel(new Paciente());
                _bindableValidator = new BindableValidator(this.Paciente);
                _resourceLoader = resourceLoader;
                _dialogService = dialogService;
            }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
             // La coleccion de provincias necesita datos antes de mostrarse
            PoblarProvincias();

            if (viewModelState != null)
            {
                base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
            }

            // Restore the errors collection manually
            var errorsCollection = RetrieveEntityStateValue<IDictionary<string, ReadOnlyCollection<string>>>("errorsCollection", viewModelState);
            Debug.WriteLine(viewModelState.Count());
            if (errorsCollection != null)
            {
                _bindableValidator.SetAllErrors(errorsCollection);
            }

            if (navigationMode == NavigationMode.New)
            {
                _paciente = (PacienteViewModel) navigationParameter;
            }
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewState, bool suspending)
        {
            base.OnNavigatedFrom(viewState, suspending);

            // Store the errors collection manually
            if (viewState != null)
            {
                AddEntityStateValue("errorsCollection", _bindableValidator.GetAllErrors(), viewState);
            }
        }

        public void PoblarProvincias()
        {
            string errorMessage = string.Empty;
            try
            {
                var provincias = new List<string> { "A coruna","Alava","Albacete","Alicante","Almeria","Asturias",
                    "Avila","Badajoz","Baleares","Barcelona","Burgos","Caceres","Cadiz","Cantabria","Castellon",
                    "Ceuta","Ciudad Real","Cordoba","Cuenca","Girona","Granada","Guadalajara","Guipuzcoa","Huelva",
                    "Huesca","Jaen","La Rioja","Las Palmas","Leon","Lleida","Lugo","Madrid","Malaga","Melilla",
                    "Murcia","Navarra","Ourense","Palencia","Pontevedra","Salamanca","Tenerife","Segovia","Sevilla",
                    "Soria","Tarragona","Teruel","Toledo","Valencia","Valladolid","Vizcaya","Zamora","Zaragoza"};

                var items = new List<ComboBoxItemValue> { new ComboBoxItemValue() { Id = string.Empty, Value = _resourceLoader.GetString("Provincia") } };
                items.AddRange(provincias.Select(provincia => new ComboBoxItemValue() { Id = provincia, Value = provincia }));
                Provincias = new ReadOnlyCollection<ComboBoxItemValue>(items);

                // Select the first item on the list
                // But disable validation first, because we don't want to fire validation at this point
                _bindableValidator.IsValidationEnabled = false;
                _paciente.Provincia = Provincias.First().Id;
                _bindableValidator.IsValidationEnabled = true;
            }
            catch (Exception ex)
            {
                errorMessage = string.Format(CultureInfo.CurrentCulture, _resourceLoader.GetString("GeneralServiceErrorMessage"),
                                             Environment.NewLine, ex.Message);
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                _dialogService.Show(_resourceLoader.GetString("ErrorServiceUnreachable") + ". " + errorMessage);
            }
        }

        public bool ValidateForm()
        {
            //return _bindableValidator.ValidateProperties();
            return true;
        }

        public async Task<bool> ProcessFormAsync()
        {
           if (!await Paciente.DoesDbExist("Paciente.db")) Paciente.CreateDatabase();
           if (await Paciente.CoincidePaciente()){
               _dialogService.Show("El Paciente ya existe, revise los datos de nuevo");
               return false;
           }
           else{
               await Paciente.InsertarNuevoPaciente();
               return true;
           }
        }

    }
}
