using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using AStalker.Enums;
using Avalonia;
using ReactiveUI;

namespace AStalker.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private RequestEnum _selectedRequestEnum = RequestEnum.GET;
        private string _url;
        private string _requestBody;
        private string _responseBody;
        private HttpClient _client;
        public ReactiveCommand<Unit,Unit> SendCommand { get; }
        public RequestEnum[] RequestEnums { get; set; }

        public RequestEnum SelectedRequestEnum
        {
            get => _selectedRequestEnum;
            set => this.RaiseAndSetIfChanged(ref _selectedRequestEnum, value);
        }

        public string Url
        {
            get => _url;
            set => this.RaiseAndSetIfChanged(ref _url, value);
        }

        public string RequestBody
        {
            get => _requestBody;
            set => this.RaiseAndSetIfChanged(ref _requestBody, value);
        }

        public string ResponseBody
        {
            get => _responseBody;
            set => this.RaiseAndSetIfChanged(ref _responseBody, value);
        }

        public MainWindowViewModel()
        {
            _client = new HttpClient();
            RequestEnums = (RequestEnum[]) Enum.GetValues(typeof(RequestEnum));
            SendCommand = ReactiveCommand.CreateFromTask(SendRequest);
        }

        private async Task SendRequest()
        {
            var res = await Send();
            ResponseBody = await res.Content.ReadAsStringAsync();
        }
        private Task<HttpResponseMessage> Send() => _selectedRequestEnum switch
        {
            RequestEnum.GET => _client.GetAsync(_url),
            RequestEnum.POST => _client.PostAsync(_url, new ByteArrayContent(Encoding.UTF8.GetBytes(_requestBody))),
            _ => null,
        };
    }
}