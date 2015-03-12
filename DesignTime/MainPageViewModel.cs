using PatientControl.Models;
using PatientControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.DesignTime
{
    class MainPageViewModel: Interfaces.IMainPageViewModel
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
                        Title = "Categoria 1", 
                        Ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() {  Title = "Ejercicio 1", Description = "Description of Ejercicio 1", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  Title = "Ejercicio 2", Description = "Description of Ejercicio 2", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  Title = "Ejercicio 3", Description = "Description of Ejercicio 3", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null),
                    new CategoriaViewModel(new Categoria()
                    { 
                        Title = "Categoria 2", 
                        Ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() {  Title = "Ejercicio 1",  Description = "Description of Ejercicio 1", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                                new Ejercicio() {  Title = "Ejercicio 2",  Description = "Description of Ejercicio 2", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null),
                    new CategoriaViewModel(new Categoria()
                    { 
                        Title = "Categoria 3", 
                        Ejercicios = new List<Ejercicio>()
                            {
                                new Ejercicio() {  Title = "Ejercicio 1",  Description = "Description of Ejercicio 1", ImageUri = new Uri("ms-appx:///Assets/StoreLogo.scale-180.png")},
                            }
                    }, null)
                };
        }
    }
}
