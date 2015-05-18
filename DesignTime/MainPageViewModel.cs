using PatientControl.Models;
using PatientControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.DesignTime
{
    public class MainPageViewModel: Interfaces.IMainPageViewModel
    {
       public IEnumerable<CategoriaViewModel> RootCategories { get; set; }

        public bool LoadingData { get; set; }

        public string Title { get; set; }

        public MainPageViewModel()
        {
            Title = "Bienvenido";
            RootCategories = new List<CategoriaViewModel>()
                {
                    new CategoriaViewModel(new Categoria()
                    { 
                        title = "Categoria 1", 
                        ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() {  titulo = "Ejercicio 1", descripcion = "Description of Ejercicio 1", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  titulo = "Ejercicio 2", descripcion = "Description of Ejercicio 2", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  titulo = "Ejercicio 3", descripcion = "Description of Ejercicio 3", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null),
                    new CategoriaViewModel(new Categoria()
                    { 
                        title = "Categoria 2", 
                        ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() {  titulo = "Ejercicio 1",  descripcion = "Description of Ejercicio 1", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  titulo = "Ejercicio 2",  descripcion = "Description of Ejercicio 2", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null),
                    new CategoriaViewModel(new Categoria()
                    { 
                        title = "Categoria 3", 
                        ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() {  titulo = "Ejercicio 1",  descripcion = "Description of Ejercicio 1", imageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null)
                };
        }
    }
}
