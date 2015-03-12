using PatientControl.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.ViewModels
{
    public class EjercicioViewModel
    {
        private readonly Ejercicio _ejercicio;

        public EjercicioViewModel(Ejercicio ejercicio)
        {
            if (ejercicio == null) throw new ArgumentNullException("ejercicio");
            _ejercicio = ejercicio;
        }

        public string Title { get { return _ejercicio.Title; } }

        public string Description { get { return _ejercicio.Description; } }

        public string NumeroEjercicio { get { return _ejercicio.NumeroEjercicio; } }

        public Uri Image { get { return _ejercicio.ImageUri; } }

        public int ItemPosition { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", Title, Description, NumeroEjercicio);
        }

    }
}
