using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Data;
using AvaloniaEdit;
using Serilog;

namespace NeBrowser.Controls
{
	public class BindableTextEditor : TextEditor, INotifyPropertyChanged
	{
		/// <summary>
		/// A bindable Text property
		/// </summary>
		private string _text;

		private bool _isAvailableRowSize;

		public bool IsAvailableRowSize
		{
			get => _isAvailableRowSize;
			set => SetAndRaise(BindingIsAvailableRowSizeProperty, ref _isAvailableRowSize, value);
		}
		public string BindingText
		{
			get => _text;
			set
			{
				if (value != null )
				{
					var pos = value.IndexOf("\n", StringComparison.Ordinal);
					if (pos < 2000 || value.Length < 2000)
						try
						{
							TextArea.Document.Text = value;
							IsAvailableRowSize = true;
						}
						catch (Exception e)
						{
							Log.Error(e.Message);
							TextArea.Document.Text = string.Empty;
							IsAvailableRowSize = false;
						}
						
					else
					{
						TextArea.Document.Text = string.Empty;
						IsAvailableRowSize = false;
						Log.Information("First \\n so far from start");
					}
				}
					
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
		public static readonly DirectProperty<BindableTextEditor, bool>
			BindingIsAvailableRowSizeProperty =
				AvaloniaProperty.RegisterDirect<BindableTextEditor, bool>(
					nameof(IsAvailableRowSize),
					editor => editor.IsAvailableRowSize,
					(editor, b) => editor.IsAvailableRowSize = b,
					true, BindingMode.TwoWay);
	}
}