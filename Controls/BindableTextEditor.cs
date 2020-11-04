﻿using System;
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
		public new string BindingText
		{
			get => GetValue(TextProperty);
			set
			{
				Text = value;
				SetValue(TextProperty, value);
			}
		}

		/// <summary>
		/// The bindable text property dependency property
		/// </summary>

		public static readonly DirectProperty<BindableTextEditor,string> TextProperty =
			AvaloniaProperty.RegisterDirect<BindableTextEditor, string>("BindingText",
				editor => editor.BindingText,
				(editor, s) => editor.BindingText = s,
				default(string),BindingMode.TwoWay);
		
	

		protected override void OnTextChanged(EventArgs e)
		{
			RaisePropertyChanged("Text");
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