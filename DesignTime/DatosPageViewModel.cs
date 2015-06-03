using Microsoft.Practices.Prism.Mvvm;
using PatientControl.Models;
using PatientControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.DesignTime
{
    public class DatosPageViewModel : ViewModel, Interfaces.IDatosPageViewModel
    {
        public string Title { get; set; }
        private IReadOnlyCollection<Ejercicio> _ejercicios;
        public IReadOnlyCollection<Ejercicio> Ejercicios
        {
            get { return _ejercicios; }
            private set { SetProperty(ref _ejercicios, value); }
        }

        private PacienteViewModel _paciente;
        public PacienteViewModel Paciente
        {
            get { return _paciente; }
            set { SetProperty(ref _paciente, value); }
        }

        private EjercicioViewModel _ejercicio;
        public EjercicioViewModel Ejercicio
        {
            get { return _ejercicio; }
            private set { SetProperty(ref _ejercicio, value); }
        }
        public DatosPageViewModel() {
            this.Title = "Ver Resultados Paciente";
            this.Ejercicio = new EjercicioViewModel(new Ejercicio());
            Ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() {  titulo = "Ejercicio 1", descripcion = "Description of Ejercicio 1", articulacion = "hombro", tipo = "Abduccion Hombro Izquierdo", angulo = "80" , fechaRealizado = new DateTime().ToUniversalTime() ,imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  titulo = "Ejercicio 2", descripcion = "Description of Ejercicio 2",  articulacion = "hombro", tipo = "Abduccion Hombro Izquierdo" , angulo = "50" , imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  titulo = "Ejercicio 3", descripcion = "Description of Ejercicio 3",  articulacion = "hombro",tipo = "Abduccion Hombro Izquierdo" , angulo = "40" , imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            };
            Paciente = new PacienteViewModel(new Paciente());
            Paciente.Nombre = "Pedro";
        }
    }
}
