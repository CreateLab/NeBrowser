using ReactiveUI;

namespace NeBrowser.Models
{
    public class KeyValuePair<T1,T2>:ReactiveObject
    {
        private T1 _item1;
        private T2 _item2;
        protected T1 Item1
        {
            get => _item1;
            set => this.RaiseAndSetIfChanged(ref _item1, value);
        }
        protected T2 Item2  
        {
            get => _item2;
            set => this.RaiseAndSetIfChanged(ref _item2, value);
        }
     }
}