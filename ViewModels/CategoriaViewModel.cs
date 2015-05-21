using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using PatientControl.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PatientControl.ViewModels
{
    public class CategoriaViewModel
    {
        private readonly Categoria _category;
        private INavigationService _navigationService;

        public CategoriaViewModel(Categoria category, INavigationService navigationService)
        {
            _category = category;
            _navigationService = navigationService;
            
            if (category != null && category.ejercicios != null)
            {
                int position = 0;
                Ejercicios = new ReadOnlyCollection<EjercicioViewModel>(category.ejercicios
                                                                            .Select(p => new EjercicioViewModel(p) {ItemPosition = position++ })
                                                                            .ToList());
            }
        }

        public int CategoryId { get { return _category.id; } }

        public int ParentCategoryId { get { return _category.parentId; } }

        public string Title { get { return _category.title; } set { _category.title = value; }}

        public IReadOnlyCollection<EjercicioViewModel> Ejercicios { get; set; }

        public int TotalNumberOfItems { get { return _category.totalNumberOfItems; } }

        public ICommand CategoryNavigationCommand { get; private set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
