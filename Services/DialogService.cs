using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.Services
{
    public class DialogService : Interfaces.IDialogService
    {
        public async void Show(string content)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(content);
            await dialog.ShowAsync();
        }
    }
}
