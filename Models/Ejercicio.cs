using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace PatientControl.Models
{
    [Table("Ejercicios")]
    public class Ejercicio
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        [Ignore]
        public string titulo { get; set; }
        [Ignore]
        public string descripcion { get; set; }
        [Ignore]
        public Uri imageUri { get; set; }

        public string articulacion { get; set; }
        public string tipo { get; set; }
        public string lado { get; set; }
        //[Indexed]
        public int paciente_id { get; set; }
        public string angulo { get; set; }
        public int repeticiones { get; set; }
        public DateTime fechaRealizado { get; set; }

        public double diferencia { get; set; }
    }
}
