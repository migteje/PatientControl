using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.Models
{
    public class Categoria
    {
        public int id { get; set; }

        public int parentId { get; set; }

        public string title { get; set; }

        public Uri imageUri { get; set; }

        public IEnumerable<Ejercicio> ejercicios { get; set; }

        public int totalNumberOfItems { get; set; }
    }
}
