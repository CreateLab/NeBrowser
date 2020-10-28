using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DynamicData;
using DynamicData.Binding;
using Microsoft.AspNetCore.WebUtilities;
using NeBrowser.Enums;
using NeBrowser.Extensions;
using NeBrowser.Helpers;
using NeBrowser.Models;
using ReactiveUI;

namespace NeBrowser.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private RequestEnum _selectedRequestEnum = RequestEnum.GET;
        private string _url;
        private string _requestBody;
        private bool _isSending;
        private readonly ObservableAsPropertyHelper<string> _responseBody;
        private readonly HttpClient _client = new HttpClient();


        public bool IsSending
        {
            get => _isSending;
            set => this.RaiseAndSetIfChanged(ref _isSending, value);
        }
        public ReactiveCommand<Unit, string> SendCommand { get; }
        public ReactiveCommand<Unit, Unit> AddEmptyParamCommand { get; }
        
        public ReactiveCommand<string, Unit> RemoveParamCommand { get; }
        public RequestEnum[] RequestEnums { get; set; } = (RequestEnum[]) Enum.GetValues(typeof(RequestEnum));

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

        public string ResponseBody => _responseBody.Value;

        public ObservableCollectionExtended<Param> QueryParams { get; } = new ObservableCollectionExtended<Param>();

        public MainWindowViewModel()
        {
            SendCommand = ReactiveCommand.CreateFromTask(SendRequest);
            AddEmptyParamCommand = ReactiveCommand.Create(AddEmptyParam);
            RemoveParamCommand = ReactiveCommand.Create<string>(RemoveParam);
            
            _responseBody = SendCommand.ToProperty(this, x => x.ResponseBody);

            var updateUrl = ReactiveCommand.Create<IReadOnlyCollection<Param>>(UpdateUrl);
            var updateParams = ReactiveCommand.Create<string>(UpdateParams);

            QueryParams.ToObservableChangeSet()
                .AutoRefresh() // This magic listens to change notifications of inner objects.
                .ToCollection()
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Where(headers => headers.Count > 0)
                .InvokeCommand(updateUrl);

            this.WhenAnyValue(x => x.Url)
                .Where(url => !string.IsNullOrEmpty(url))
                .InvokeCommand(updateParams);

            SendCommand.ThrownExceptions
                .Merge(updateUrl.ThrownExceptions)
                .Merge(updateParams.ThrownExceptions)
                .Subscribe(error => Console.WriteLine($"Uh oh: {error}"));
            AddEmptyParamCommand.ThrownExceptions
                .Merge(updateUrl.ThrownExceptions)
                .Merge(updateParams.ThrownExceptions)
                .Subscribe(error => Console.WriteLine($"Uh oh: {error}"));
            RemoveParamCommand.ThrownExceptions
                .Merge(updateUrl.ThrownExceptions)
                .Merge(updateParams.ThrownExceptions)
                .Subscribe(error => Console.WriteLine($"Uh oh: {error}"));
            SendCommand.IsExecuting.Subscribe(e => IsSending = e, error => Console.WriteLine($"Uh oh: {error}"));
        }

        private void UpdateParams(string url)
        {
            var qObj = HttpUtility.ParseQueryString(new Uri(_url).Query);
            QueryParams.Clear();
            QueryParams.AddRange(qObj.AllKeys.Select(key => new Param {Key = key, Value = qObj[key]}));
        }

        private void UpdateUrl(IReadOnlyCollection<Param> headers)
        {
            var url = new Uri(Url);
            Url = QueryHelpers.AddQueryString(
                $"{url.Scheme}{Uri.SchemeDelimiter}{url.Authority}{url.AbsolutePath}",
                headers.Where(h => !string.IsNullOrEmpty(h.Key) && !string.IsNullOrEmpty(h.Value))
                    .ToDictionary(header => header.Key, header => header.Value));
        }

        private async Task<string> SendRequest()
        {
            var res = await Send();
            var data = (await res.Content.ReadAsStringAsync()).EmptyIfNull();
            if (BeautifyHelper.TryBeautifyJson(ref data)) return data;
            BeautifyHelper.TryBeautifyXml(ref data);
            return data;
        }


        private Task<HttpResponseMessage> Send() => _selectedRequestEnum switch
        {
            RequestEnum.GET => _client.GetAsync(_url),
            RequestEnum.POST => _client.PostAsync(_url, new ByteArrayContent(Encoding.UTF8.GetBytes(_requestBody))),
            _ => null,
        };

        private void AddEmptyParam()
        {
            QueryParams.Add(new Param());
        }

        private void RemoveParam(string key)
        {
            QueryParams.Remove(QueryParams.First(p => p.Key == key));
        }
    }
}