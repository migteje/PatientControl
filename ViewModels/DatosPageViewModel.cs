using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using PatientControl.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PatientControl.ViewModels
{
    public class DatosPageViewModel : ViewModel, Interfaces.IDatosPageViewModel
    {

        IEventAggregator _eventAggregator;
        INavigationService _navigationService;

        public DelegateCommand<string> SelectCommand { get; private set; }

        private PacienteViewModel _paciente;
        public PacienteViewModel Paciente
        {
            get { return _paciente; }
            set { SetProperty(ref _paciente, value); }
        }
        
        private string _title = default(string);
        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }

        private IReadOnlyCollection<EjercicioViewModel> _ejercicios;
        public IReadOnlyCollection<EjercicioViewModel> Ejercicios
        {
            get { return _ejercicios; }
            private set { SetProperty(ref _ejercicios, value); }
        }

        private EjercicioViewModel _ejercicio;
        public EjercicioViewModel Ejercicio
        {
            get { return _ejercicio; }
            private set { SetProperty(ref _ejercicio, value); }
        }

        public DatosPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            this.SelectCommand = new DelegateCommand<string>(Select);
        }

        public override async void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            this.Paciente = navigationParameter as PacienteViewModel;
            this.Title = "Ver Resultados Paciente";
            this.Ejercicio = new EjercicioViewModel(new Ejercicio());
            Ejercicios = await Ejercicio.ObtenerEjercicios(Paciente.Id.ToString(),"todas");
        }


        private async void Select(object parameter)
        {
            Ejercicios = await Ejercicio.ObtenerEjercicios(Paciente.Id.ToString(), (String) parameter);
        }
    }
}
