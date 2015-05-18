using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace PatientControl.Models
{
    [Table("Pacientes")]
    public class Paciente
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [NotNull]
        public string nombre { get; set; }
        [NotNull]
        public string apellidos { get; set; }
        public string direccion { get; set; }
        public string direccionOpcional { get; set; }
        public string provincia { get; set; }
        public string localidad { get; set; }
        public string codigoPostal { get; set; }
        public string telefono { get; set; }
        [NotNull]
        public string zonaLesion { get; set; }
        public string diagnostico { get; set; }
        [NotNull]
        public string articValorar { get; set; }
        public DateTimeOffset fechaLesion { get; set; }
    }
}
