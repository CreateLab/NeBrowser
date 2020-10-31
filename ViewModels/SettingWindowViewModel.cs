using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using ReactiveUI;

namespace NeBrowser.ViewModels
{
	public class SettingWindowViewModel:ViewModelBase
	{
		private PaletteHelper _paletteHelper;
		private bool _theme;

		public bool Theme
		{
			get => _theme;
			set
			{
				BaseThemeChanged(value);
				this.RaiseAndSetIfChanged(ref _theme, value);
			}
		}

		public SettingWindowViewModel(PaletteHelper helper)
		{
			_paletteHelper = helper;
		}
		public void BaseThemeChanged(bool value)
		{
			var theme = _paletteHelper.GetTheme();
			var baseThemeMode = value switch {
				false  => BaseThemeMode.Dark,
				true => BaseThemeMode.Light
			};
			theme.SetBaseTheme(baseThemeMode.GetBaseTheme());
			_paletteHelper.SetTheme(theme);
		}
	}
}