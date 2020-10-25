namespace NeBrowser.Models
{
    public class Param:KeyValuePair<string,string>
    {
        public string Key
        {
            get => Item1;
            set => Item1 = value;
        }
        public string Value
        {
            get => Item2;
            set => Item2 = value;
        }
        
    }
}