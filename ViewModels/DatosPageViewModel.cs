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

        private PacienteViewModel _paciente;
        public PacienteViewModel Paciente
        {
            get { return _paciente; }
            set { SetProperty(ref _paciente, value); }
        }
        
        private string _title = default(string);
        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }

        private string _articulacion = default(string);
        public string Articulacion { get { return _articulacion; } set { SetProperty(ref _articulacion, value); } }
        private string _prueba = default(string);
        public string Prueba { get { return _prueba; } set { SetProperty(ref _prueba, value); } }

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

/*      private string _ValorAb = default(string);
        public string ValorAb { get { return _ValorAb; } set { SetProperty(ref _ValorAb, value); } }
        private string _ValorAd = default(string);
        public string ValorAd { get { return _ValorAd; } set { SetProperty(ref _ValorAd, value); } }
        private int _RepeticionAbd = default(int);
        public int RepeticionAbd { get { return _RepeticionAbd; } set { SetProperty(ref _RepeticionAbd, value); } }
        private string _ValorFl = default(string);
        public string ValorFl { get { return _ValorFl; } set { SetProperty(ref _ValorFl, value); } }
        private string _ValorEx = default(string);
        public string ValorEx { get { return _ValorEx; } set { SetProperty(ref _ValorEx, value); } }
        private int _RepeticionFlEx = default(int);
        public int RepeticionFlEx { get { return _RepeticionFlEx; } set { SetProperty(ref _RepeticionFlEx, value); } }
        private string _ValorFlh = default(string);
        public string ValorFlh { get { return _ValorFlh; } set { SetProperty(ref _ValorFlh, value); } }
        private string _ValorExh = default(string);
        public string ValorExh { get { return _ValorExh; } set { SetProperty(ref _ValorExh, value); } }
        private int _RepeticionFlExh = default(int);
        public int RepeticionFlExh { get { return _RepeticionFlExh; } set { SetProperty(ref _RepeticionFlExh, value); } }
        private string _ValorCfl = default(string);
        public string ValorCfl { get { return _ValorCfl; } set { SetProperty(ref _ValorCfl, value); } }
        private string _ValorCex = default(string);
        public string ValorCex { get { return _ValorCex; } set { SetProperty(ref _ValorCex, value); } }
        private int _RepeticionCflEx = default(int);
        public int RepeticionCflEx { get { return _RepeticionCflEx; } set { SetProperty(ref _RepeticionCflEx, value); } }

        public IEnumerable<CategoriaViewModel> rootCategories;*/

        public DatosPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
        }

        public override async void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            this.Paciente = navigationParameter as PacienteViewModel;
            this.Title = "Ver Resultados Paciente";
            this.Ejercicio = new EjercicioViewModel(new Ejercicio());
            Ejercicios = await Ejercicio.ObtenerEjercicios(Paciente.Id.ToString());
            Debug.WriteLine(Ejercicios.ElementAt(1).Angulo);
            /*rootCategories = navigationParameter as IEnumerable<CategoriaViewModel>;
            ValorAb = rootCategories.ElementAt(0).Ejercicios.ElementAt(0).AnguloAb;
            ValorAd = rootCategories.ElementAt(0).Ejercicios.ElementAt(0).AnguloAd;
            RepeticionAbd = rootCategories.ElementAt(0).Ejercicios.ElementAt(0).RepeticionesAbd;
            ValorFl = rootCategories.ElementAt(0).Ejercicios.ElementAt(1).AnguloFl;
            ValorEx = rootCategories.ElementAt(0).Ejercicios.ElementAt(1).AnguloEx;
            RepeticionFlEx = rootCategories.ElementAt(0).Ejercicios.ElementAt(1).RepeticionesFlEx;
            ValorFlh = rootCategories.ElementAt(0).Ejercicios.ElementAt(2).AnguloFlh;
            ValorExh = rootCategories.ElementAt(0).Ejercicios.ElementAt(2).AnguloExh;
            RepeticionFlExh = rootCategories.ElementAt(0).Ejercicios.ElementAt(2).RepeticionesFlExh;
            ValorCfl = rootCategories.ElementAt(1).Ejercicios.ElementAt(0).AnguloCfl;
            ValorCex = rootCategories.ElementAt(1).Ejercicios.ElementAt(0).AnguloCex;
            RepeticionCflEx = rootCategories.ElementAt(1).Ejercicios.ElementAt(0).RepeticionesCflEx;*/
        }

    }
}
