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

        public string AnguloAb { get { return _ejercicio.AnguloAb; } set { _ejercicio.AnguloAb = value; } }
        public string AnguloAd { get { return _ejercicio.AnguloAd; } set { _ejercicio.AnguloAd = value; } }
        public int RepeticionesAbd { get { return _ejercicio.RepeticionesAbd; } set { _ejercicio.RepeticionesAbd = value; } }
        public string AnguloFl { get { return _ejercicio.AnguloFl; } set { _ejercicio.AnguloFl = value; } }
        public string AnguloEx { get { return _ejercicio.AnguloEx; } set { _ejercicio.AnguloEx = value; } }
        public int RepeticionesFlEx { get { return _ejercicio.RepeticionesFlEx; } set { _ejercicio.RepeticionesFlEx = value; } }
        public string AnguloFlh { get { return _ejercicio.AnguloFlh; } set { _ejercicio.AnguloFlh = value; } }
        public string AnguloExh { get { return _ejercicio.AnguloExh; } set { _ejercicio.AnguloExh = value; } }
        public int RepeticionesFlExh { get { return _ejercicio.RepeticionesFlExh; } set { _ejercicio.RepeticionesFlExh = value; } }
        public string AnguloCfl { get { return _ejercicio.AnguloCfl; } set { _ejercicio.AnguloCfl = value; } }
        public string AnguloCex { get { return _ejercicio.AnguloCex; } set { _ejercicio.AnguloCex = value; } }
        public int RepeticionesCflEx { get { return _ejercicio.RepeticionesCflEx; } set { _ejercicio.RepeticionesCflEx = value; } }

        public Uri Image { get { return _ejercicio.ImageUri; } }

        public int ItemPosition { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", Title, Description, NumeroEjercicio);
        }

    }
}
