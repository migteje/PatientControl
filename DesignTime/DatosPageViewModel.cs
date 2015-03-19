using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.DesignTime
{
    public class DatosPageViewModel : ViewModel, Interfaces.IDatosPageViewModel
    {
        public string Title { get; set; }
        public DatosPageViewModel() { }
    }
}
