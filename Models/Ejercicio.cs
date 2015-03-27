using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.Models
{
    public class Ejercicio
    {
        public string NumeroEjercicio { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Uri ImageUri { get; set; }

        public int SubcategoryId { get; set; }

        public string AnguloAb { get; set; }
        public string AnguloAd { get; set; }
        public int RepeticionesAbd { get; set; }
        public string AnguloFl { get; set; }
        public string AnguloEx { get; set; }
        public int RepeticionesFlEx { get; set; }
        public string AnguloFlh { get; set; }
        public string AnguloExh { get; set; }
        public int RepeticionesFlExh { get; set; }
        public string AnguloCfl { get; set; }
        public string AnguloCex { get; set; }
        public int RepeticionesCflEx { get; set; }

        public double Diferencia { get; set; }


    }
}
