using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Title { get; set; }

        public Uri ImageUri { get; set; }

        public IEnumerable<Categoria> Subcategories { get; set; }

        public IEnumerable<Ejercicio> Ejercicios { get; set; }

        public int TotalNumberOfItems { get; set; }

        public bool HasSubcategories { get; set; }
    }
}
