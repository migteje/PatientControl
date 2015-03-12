using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using PatientControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace PatientControl.ViewModels
{
    class MainPageViewModel: ViewModel, Interfaces.IMainPageViewModel
    {
        public IEnumerable<CategoriaViewModel> RootCategories { get; set; }

        public bool LoadingData { get; set; }

            IEventAggregator _eventAggregator;
            INavigationService _navigationService;

            public DelegateCommand VerDatosCommand { get; private set; }

            public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            {
                _eventAggregator = eventAggregator;
                _navigationService = navigationService;

                RootCategories = new List<CategoriaViewModel>()
                {
                    new CategoriaViewModel(new Categoria()
                    { 
                        Title = "Ejercicios de Hombro", 
                        Ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() {  Title = "Abduccion", Description = "Realizar levantamiento lateral del brazo", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  Title = "FlexoExtension", Description = "Realizar levantamiento frontal del brazo", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  Title = "FlexExHorizontal", Description = "Realizar movimientos laterales", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null),
                    new CategoriaViewModel(new Categoria()
                    { 
                        Title = "Ejercicios de Codo", 
                        Ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() {  Title = "FlexoExtension",  Description = "Doblar y extender el codo moviendo el antebrazo", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  Title = "abduccion",  Description = "Rotar el antebrazo respecto al codo", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null),
                    new CategoriaViewModel(new Categoria()
                    { 
                        Title = "Ejercicios de Muñeca", 
                        Ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() {  Title = "FlexoExtension",  Description = "Movimiento lateral de la mano", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null)
                };
            }

            public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
            {
                this.Title = "PatientControl";
                VerDatosCommand = DelegateCommand.FromAsyncHandler(VerDatosAsync);
            }

            string _Title = default(string);
            public string Title { get { return _Title; } set { SetProperty(ref _Title, value); } }

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
                       //_navigationService.Navigate(App.Experiences.Ejercicio.ToString(), null);
                    };
            return navigateAction;
        }

    }
}
