using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Data;
using AvaloniaEdit;

namespace NeBrowser.Controls
{
	public class BindableTextEditor : TextEditor, INotifyPropertyChanged
	{
		/// <summary>
		/// A bindable Text property
		/// </summary>
		private string _text;

		public string BindingText
		{
			get => _text;
			set
			{
				Text = value;
				SetAndRaise(BindingTextProperty, ref _text, value);
			}
		}

		/// <summary>
		/// The bindable text property dependency property
		/// </summary>
		public static readonly DirectProperty<BindableTextEditor, string>
			BindingTextProperty =
				AvaloniaProperty.RegisterDirect<BindableTextEditor, string>(
					nameof(BindingText),
					editor => editor.BindingText,
					(editor, s) => editor.BindingText = s,
					default(string), BindingMode.TwoWay);
	}
}