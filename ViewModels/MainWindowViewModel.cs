using System;
using System.Collections.Generic;
using System.Text;
using AStalker.Enums;
using ReactiveUI;

namespace AStalker.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public RequestEnum[] RequestEnums { get; set; }
        
        private RequestEnum _selectedRequestEnum = RequestEnum.GET;
        public RequestEnum SelectedRequestEnum
        {
            get => _selectedRequestEnum;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedRequestEnum, value);
            }
        }

        public MainWindowViewModel()
        {
            RequestEnums = (RequestEnum[]) Enum.GetValues(typeof(RequestEnum));
        }
    }
}