using ReactiveUI;

namespace NeBrowser.Models
{
	public class StateParam:KeyValuePair<string,string>
	{
		private bool _isUseful;

		public bool IsUseful
		{
			get => _isUseful;
			set => this.RaiseAndSetIfChanged(ref _isUseful,value);
		}
	}
}