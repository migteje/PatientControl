using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.StoreApps;
using PatientControl.Controls;
using PatientControl.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace PatientControl.Interfaces
{
    public interface IMedicalInfoViewModel
    {
        [RestorableState]
        PacienteViewModel Paciente { get; set; }
        IReadOnlyCollection<ComboBoxItemValue> Articulaciones { get; }
        void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewState);
        void OnNavigatedFrom(Dictionary<string, object> viewState, bool suspending);
        Task ProcessFormAsync();
        bool ValidateForm();
        event PropertyChangedEventHandler PropertyChanged;
    }
}
