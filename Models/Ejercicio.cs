﻿using System;
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

        public string titulo { get; set; }

        public string descripcion { get; set; }

        public Uri imageUri { get; set; }

        //[Indexed]
        public int paciente_id { get; set; }

        public string anguloAb { get; set; }
        public string anguloAd { get; set; }
        public int repeticionesAbd { get; set; }
        public string anguloFl { get; set; }
        public string anguloEx { get; set; }
        public int repeticionesFlEx { get; set; }
        public string anguloFlh { get; set; }
        public string anguloExh { get; set; }
        public int repeticionesFlExh { get; set; }
        public string anguloCfl { get; set; }
        public string anguloCex { get; set; }
        public int repeticionesCflEx { get; set; }

        public double diferencia { get; set; }


    }
}
