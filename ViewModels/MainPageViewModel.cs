using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
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
            private PacienteViewModel _paciente;
            public PacienteViewModel Paciente
            {
                get { return _paciente; }
                set { SetProperty(ref _paciente, value); }
            }

            public DelegateCommand VerDatosCommand { get; private set; }

            public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, PacienteViewModel Paciente)
            {
                _eventAggregator = eventAggregator;
                _navigationService = navigationService;

                RootCategories = new List<CategoriaViewModel>()
                {
                    new CategoriaViewModel(new Categoria()
                    { 
                        title = "Ejercicios de Hombro",
                        ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() { paciente_id = Paciente.Id, titulo = "Abduccion-Aduccion", descripcion = "Realizar levantamiento lateral del brazo", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() { paciente_id = Paciente.Id, titulo = "FlexoExtension", descripcion = "Realizar levantamiento frontal del brazo", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() { paciente_id = Paciente.Id, titulo = "FlexExHorizontal", descripcion = "Realizar movimientos laterales", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null),
                    new CategoriaViewModel(new Categoria()
                    { 
                        title = "Ejercicios de Codo", 
                        ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() { paciente_id = Paciente.Id, titulo = "CodoFlexEx",  descripcion = "Doblar y extender el codo moviendo el antebrazo", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},                            }
                    }, null)
                };
            }

            public override async void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
            {
                this.Title = "PatientControl";
                VerDatosCommand = DelegateCommand.FromAsyncHandler(VerDatosAsync);
                this.Paciente = (PacienteViewModel) navigationParameter;
                this.Username = Paciente.Nombre;
                foreach (var cat in RootCategories)
                    foreach (var ej in cat.Ejercicios)
                        ej.Paciente_Id = Paciente.Id;
            }

            string _Title = default(string);
            public string Title { get { return _Title; } set { SetProperty(ref _Title, value); } }

            string _username = default(string);
            public string Username { get { return _username; } set { SetProperty(ref _username, value); } }

        private async Task VerDatosAsync()
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
                       _navigationService.Navigate(App.Experiences.Datos.ToString(), RootCategories);
                    };
            return navigateAction;
        }

    }
}
