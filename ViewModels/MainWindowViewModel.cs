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
using NeBrowser.ViewModels;
using ReactiveUI;

public class Header : ReactiveObject
{
    private string _key;
    public string Key
    {
        get => _key;
        set => this.RaiseAndSetIfChanged(ref _key, value);
    }

    private string _value;
    public string Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
}

public class MainWindowViewModel : ViewModelBase
{
    private RequestEnum _selectedRequestEnum = RequestEnum.GET;
    private string _url;
    private string _requestBody;

    private readonly ObservableAsPropertyHelper<string> _responseBody;
    private readonly HttpClient _client = new HttpClient();

    public ReactiveCommand<Unit, string> SendCommand { get; }
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

    public ObservableCollectionExtended<Header> Headers { get; } = new ObservableCollectionExtended<Header>();

    public MainWindowViewModel()
    {
        SendCommand = ReactiveCommand.CreateFromTask(SendRequest);
        _responseBody = SendCommand.ToProperty(this, x => x.ResponseBody);

        var updateUrl = ReactiveCommand.Create<IReadOnlyCollection<Header>>(UpdateUrl);
        var updateParams = ReactiveCommand.Create<string>(UpdateParams);

        Headers.ToObservableChangeSet()
            .AutoRefresh() // This magic listens to change notifications of inner objects.
            .ToCollection()
            .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
            .Where(headers => headers.Count > 0)
            .InvokeCommand(updateUrl);

        this.WhenAnyValue(x => x.Url)
            .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
            .Where(url => !string.IsNullOrEmpty(url))
            .InvokeCommand(updateParams);

        SendCommand.ThrownExceptions
            .Merge(updateUrl.ThrownExceptions)
            .Merge(updateParams.ThrownExceptions)
            .Subscribe(error => Console.WriteLine($"Uh oh: {error}"));
    }

    private void UpdateParams(string url)
    {
        var qObj = HttpUtility.ParseQueryString(new Uri(_url).Query);
        Headers.Clear();
        Headers.AddRange(qObj.AllKeys.Select(key => new Header {Key = key, Value = qObj[key]}));
    }

    private void UpdateUrl(IReadOnlyCollection<Header> headers)
    {
        var url = new Uri(Url);
        Url = QueryHelpers.AddQueryString(
            $"{url.Scheme}{Uri.SchemeDelimiter}{url.Authority}{url.AbsolutePath}",
            headers.ToDictionary(header => header.Key, header => header.Value));
    }

    private async Task<string> SendRequest()
    {
        var res = await Send();
        return await res.Content.ReadAsStringAsync();
    }

    private Task<HttpResponseMessage> Send() => _selectedRequestEnum switch
    {
        RequestEnum.GET => _client.GetAsync(_url),
        RequestEnum.POST => _client.PostAsync(_url, new ByteArrayContent(Encoding.UTF8.GetBytes(_requestBody))),
        _ => null,
    };
}