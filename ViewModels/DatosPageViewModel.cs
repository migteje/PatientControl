using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using PatientControl.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace PatientControl.ViewModels
{
    public class DatosPageViewModel : ViewModel, Interfaces.IDatosPageViewModel
    {

        IEventAggregator _eventAggregator;
        INavigationService _navigationService;

        string _Title = default(string);
        public string Title { get { return _Title; } set { SetProperty(ref _Title, value); } }

        private string _Prueba = default(string);
        public string Prueba { get { return _Prueba; } set { SetProperty(ref _Prueba, value); } }

        private string _Valor = default(string);
        public string Valor { get { return _Valor; } set { SetProperty(ref _Valor, value); } }

        public IEnumerable<CategoriaViewModel> rootCategories;

        public DatosPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            this.Title = "VerDatos";
            rootCategories = navigationParameter as IEnumerable<CategoriaViewModel>;

            Prueba = rootCategories.First().Ejercicios.First().Title;
            Debug.WriteLine(Prueba);
            Valor = rootCategories.First().Ejercicios.First().AnguloMedido;
            Debug.WriteLine(Valor);
        }

    }
}
