using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Data;
using AvaloniaEdit;

namespace NeBrowser.Controls
{
	public class BindableTextEditor: TextEditor, INotifyPropertyChanged
	{
		/// <summary>
		/// A bindable Text property
		/// </summary>
		public  string BindingText
		{
			get => GetValue(BindingTextProperty);
			set
			{
				Text = value;
				SetValue(BindingTextProperty, value);
			}
		}

		/// <summary>
		/// The bindable text property dependency property
		/// </summary>

		public static readonly DirectProperty<BindableTextEditor,string> BindingTextProperty =
			AvaloniaProperty.RegisterDirect<BindableTextEditor, string>(nameof(BindingText),
				editor => editor.BindingText,
				(editor, s) => editor.BindingText = s,
				default(string),BindingMode.TwoWay);
		
	

		protected override void OnTextChanged(EventArgs e)
		{
			RaisePropertyChanged(nameof(BindingText));
			base.OnTextChanged(e);
		}

		/// <summary>
		/// Raises a property changed event
		/// </summary>
		/// <param name="property">The name of the property that updates</param>
		public void RaisePropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}