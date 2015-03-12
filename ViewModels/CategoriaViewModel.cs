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
            //CategoryNavigationCommand = new DelegateCommand(NavigateToCategory);
            
            if (category != null && category.Ejercicios != null)
            {
                int position = 0;
                Ejercicios = new ReadOnlyCollection<EjercicioViewModel>(category.Ejercicios
                                                                            .Select(p => new EjercicioViewModel(p) { ItemPosition = position++ })
                                                                            .ToList());
            }
        }

        public int CategoryId { get { return _category.Id; } }

        public int ParentCategoryId { get { return _category.ParentId; } }

        public string Title { get { return _category.Title; } }

        public IReadOnlyCollection<EjercicioViewModel> Ejercicios { get; private set; }

        public int TotalNumberOfItems { get { return _category.TotalNumberOfItems; } }

        public ICommand CategoryNavigationCommand { get; private set; }

        public Uri Image { get { return _category.ImageUri;} }

        public override string ToString()
        {
            return Title;
        }

       /* private void NavigateToCategory()
        {
            if (_category.HasSubcategories)
            {
                _navigationService.Navigate("Categoria", CategoryId);
            }
            else
            {
                _navigationService.Navigate("GroupDetail", CategoryId);
            }
        }*/
    }
}
