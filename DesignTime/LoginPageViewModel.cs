using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.DesignTime
{
    public class LoginPageViewModel: Interfaces.ILoginPageViewModel
    {
        public LoginPageViewModel()
        {
            this.Username = "Desgin user";
        }
        public string Username { get; set; }
    }
}
