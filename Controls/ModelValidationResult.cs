using System.Collections.Generic;

namespace PatientControl.Controls
{
    public class ModelValidationResult
    {
        public ModelValidationResult()
        {
            ModelState = new Dictionary<string, List<string>>();
        }

        public Dictionary<string,List<string>> ModelState { get; private set; }
    }
}
