using Microsoft.Practices.Prism.Mvvm;
using PatientControl.Models;
using PatientControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.DesignTime
{
    public class EjercicioPageViewModel : ViewModel, Interfaces.IEjercicioPageViewModel
    {
        public string Title { get; set; }
        public string Angulo { get;  set; }

        public EjercicioPageViewModel()
        {
            Title = "DiseñoEjercicio";
            Angulo = "0";
        }

    }
}
