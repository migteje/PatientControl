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

        public double AnguloMedido { get; set; }

        public double Diferencia { get; set; }

        public double Repeticiones { get; set; }


    }
}
