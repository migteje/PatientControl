using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using PatientControl.Interfaces;
using PatientControl.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace PatientControl.ViewModels
{
    public class MainPageViewModel: ViewModel, Interfaces.IMainPageViewModel
    {
        public IEnumerable<CategoriaViewModel> RootCategories { get; set; }

        public bool LoadingData { get; set; }

            public IEventAggregator _eventAggregator;
            public INavigationService _navigationService;
            private readonly IDialogService _dialogService;

            private PacienteViewModel _paciente;
            public PacienteViewModel Paciente
            {
                get { return _paciente; }
                set { SetProperty(ref _paciente, value); }
            }
            string _Title = default(string);
            public string Title { get { return _Title; } set { SetProperty(ref _Title, value); } }

            string _username = default(string);
            public string Username { get { return _username; } set { SetProperty(ref _username, value); } }

            public DelegateCommand VerDatosCommand { get; private set; }
            public DelegateCommand EditarCommand { get; private set; }
            public DelegateCommand EliminarCommand { get; private set; }

            public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, PacienteViewModel Paciente, IDialogService dialogService)
            {
                _eventAggregator = eventAggregator;
                _navigationService = navigationService;
                _dialogService = dialogService;

                RootCategories = new List<CategoriaViewModel>()
                {
                    new CategoriaViewModel(new Categoria()
                    { 
                        title = "Ejercicios de Hombro",
                        ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() { paciente_id = Paciente.Id, articulacion = "Hombro", titulo = "Abduccion-Aduccion", descripcion = "Realizar levantamiento lateral del brazo", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() { paciente_id = Paciente.Id, articulacion = "Hombro", titulo = "FlexoExtension", descripcion = "Realizar levantamiento frontal del brazo", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() { paciente_id = Paciente.Id, articulacion = "Hombro", titulo = "FlexExHorizontal", descripcion = "Realizar movimientos laterales", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null),
                    new CategoriaViewModel(new Categoria()
                    { 
                        title = "Ejercicios de Codo", 
                        ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() { paciente_id = Paciente.Id, articulacion = "Codo", titulo = "CodoFlexEx",  descripcion = "Doblar y extender el codo moviendo el antebrazo", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},                            }
                    }, null)
                };
            }

            public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
            {
                this.Title = "PatientControl";
                VerDatosCommand = DelegateCommand.FromAsyncHandler(VerDatosAsync);
                EditarCommand = DelegateCommand.FromAsyncHandler(EditarAsync);
                EliminarCommand = DelegateCommand.FromAsyncHandler(EliminarAsync);
                this.Paciente = (PacienteViewModel) navigationParameter;
                this.Username = Paciente.Nombre;
                foreach (var cat in RootCategories)
                    foreach (var ej in cat.Ejercicios)
                        ej.Paciente_Id = Paciente.Id;
            }

        private async Task VerDatosAsync()
        {
            await GoToNextPageAsync("Datos");
        }

        private async Task EditarAsync()
        {
            await GoToNextPageAsync("Editar");
        }

        private async Task EliminarAsync()
        {
            if (await Paciente.DeleteInfoPaciente(Paciente.Id))
                _dialogService.Show("El paciente se ha eliminado correctamente");
            else
                _dialogService.Show("Ha habido un problema eliminando al paciente");
            await GoToNextPageAsync("Login");
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
                        if (nav.Equals("Datos"))
                            _navigationService.Navigate(App.Experiences.Datos.ToString(), Paciente);
                        if (nav.Equals("Login"))
                            _navigationService.Navigate(App.Experiences.Login.ToString(), null);
                        if (nav.Equals("Editar"))
                            _navigationService.Navigate(App.Experiences.Registro.ToString(), Paciente);

                    };
            return navigateAction;
        }

    }
}
