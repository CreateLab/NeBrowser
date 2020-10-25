using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NeBrowser.Enums;
using Avalonia;
using DynamicData;
using DynamicData.Binding;
using Microsoft.AspNetCore.WebUtilities;
using NeBrowser.Models;
using ReactiveUI;

namespace NeBrowser.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private RequestEnum _selectedRequestEnum = RequestEnum.GET;
        private string _url;
        private string _requestBody;
        private string _responseBody;
        private HttpClient _client;
        public ReactiveCommand<Unit, Unit> SendCommand { get; }
        public RequestEnum[] RequestEnums { get; set; }
        private readonly ReadOnlyObservableCollection<Header> _headers;

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

        public ReadOnlyObservableCollection<Header> Headers => _headers;
        public ObservableCollectionExtended<Header> Source { get; }

        public MainWindowViewModel()
        {
            _client = new HttpClient();
            RequestEnums = (RequestEnum[]) Enum.GetValues(typeof(RequestEnum));
            SendCommand = ReactiveCommand.CreateFromTask(SendRequest);
            this.WhenAnyValue(x => x.Url).Throttle(TimeSpan.FromSeconds(1)).Subscribe(_ => UpdateParams());
            Source = new ObservableCollectionExtended<Header>();
            Source.ToObservableChangeSet()
                .Bind(out _headers)
                .Throttle(TimeSpan.FromSeconds(1))
                .ToCollection()
                .Subscribe(c => UpdateURL(c));
        }

        private void UpdateParams()
        {
            if (string.IsNullOrEmpty(Url)) return;
            var qObj = HttpUtility.ParseQueryString((new Uri(_url)).Query);
            Source.Clear();
            foreach (var key in qObj.AllKeys)
            {
                Source.Add(new Header {Key = key, Value = qObj[key]});
            }
        }

        private void UpdateURL(IReadOnlyCollection<Header> readOnlyCollection)
        {
            if (readOnlyCollection.Count == 0) return;
            var url = new Uri(_url);
            var path = $"{url.Scheme}{Uri.SchemeDelimiter}{url.Authority}{url.AbsolutePath}";
            Url = QueryHelpers.AddQueryString(path,
                readOnlyCollection.ToDictionary(header => header.Key, header => header.Value));
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