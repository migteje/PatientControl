namespace PatientControl.Controls
{
    public class ComboBoxItemValue
    {
        public string Id { get; set; }
        public string Value { get; set; }
        
        public override string ToString()
        {
            return Value;
        }
    }
}
