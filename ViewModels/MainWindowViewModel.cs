using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using NeBrowser.Enums;
using NeBrowser.Extensions;
using NeBrowser.Helpers;
using NeBrowser.Models;
using NeBrowser.Views;
using Newtonsoft.Json;
using ReactiveUI;
using Serilog;

namespace NeBrowser.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private MainWindow _mainWindow;
		private RequestEnum _selectedRequestEnum = RequestEnum.GET;
		private string _url;
		private string _data;
		private string _requestBodyText;
		private bool _isSending;
		private bool _isText, _isXML, _isJSON, _isRow;
		private readonly ObservableAsPropertyHelper<string> _responseBody;
		private int? _statusCode;
		private bool _isSucceedRequest;
		private CancellationTokenSource _source;
		private Data _selectData = Data.Row;
		private string _highlighting;
		private bool _isFromFile;
		private string _fileData;

		public bool IsFromFile
		{
			get => _isFromFile;
			set => this.RaiseAndSetIfChanged(ref _isFromFile, value);
		}

		public string FileData
		{
			get => _fileData;
			set => this.RaiseAndSetIfChanged(ref _fileData, value);
		}

		public string Highlighting
		{
			get => _highlighting;
			set => this.RaiseAndSetIfChanged(ref _highlighting, value);
		}

		public bool IsText
		{
			get => _isText;
			set
			{
				this.RaiseAndSetIfChanged(ref _isText, value);
				if (value) ConvertDataCommand.Execute().Wait();
			}
		}
		public bool IsRowData
		{
			get => _isRow;
			set
			{
				this.RaiseAndSetIfChanged(ref _isRow, value);
				if (value) ConvertDataCommand.Execute().Wait();
			}
		}

		public bool IsXml
		{
			get => _isXML;
			set
			{
				this.RaiseAndSetIfChanged(ref _isXML, value);
				if (value) 
					ConvertDataCommand.Execute().Wait();
			}
		}

		public bool IsJson
		{
			get => _isJSON;
			set
			{
				this.RaiseAndSetIfChanged(ref _isJSON, value);
				if (value) ConvertDataCommand.Execute().Wait();
			}
		}

		public bool IsSuceedRequest
		{
			get => _isSucceedRequest;
			set
			{
				this.RaiseAndSetIfChanged(ref _isSucceedRequest, value);
				if (value) ConvertDataCommand.Execute().Wait();
			}
		}

		public int? StatusCode
		{
			get => _statusCode;
			set
			{
				this.RaiseAndSetIfChanged(ref _statusCode, value);
				if (value != null)
					IsSuceedRequest = true;
			}
		}

		public bool IsSending
		{
			get => _isSending;
			set => this.RaiseAndSetIfChanged(ref _isSending, value);
		}

		public RequestEnum SelectedRequestEnum
		{
			get => _selectedRequestEnum;
			set => this.RaiseAndSetIfChanged(ref _selectedRequestEnum, value);
		}


		public ReactiveCommand<Unit, Unit> SendCommand { get; }
		public ReactiveCommand<Unit, Unit> AddEmptyParamCommand { get; }
		public ReactiveCommand<Unit, Unit> AddEmptyHeaderCommand { get; }
		public ReactiveCommand<string, Unit> RemoveParamCommand { get; }
		public ReactiveCommand<string, Unit> RemoveHeaderCommand { get; }
		public ReactiveCommand<Unit, Unit> ShowSettingCommand { get; }
		public ReactiveCommand<Unit, Unit> CancelCommand { get; }
		public ReactiveCommand<Unit, Unit> QuitProgramCommand { get; }
		public ReactiveCommand<Unit, Unit> SaveFileCommand { get; }
		public ReactiveCommand<Unit, Unit> SelectFileForBodyCommand { get; }
		public ReactiveCommand<Unit, string> ConvertDataCommand { get; }

		public RequestEnum[] RequestEnums { get; set; } =
			(RequestEnum[]) Enum.GetValues(typeof(RequestEnum));


		public string Url
		{
			get => _url;
			set => this.RaiseAndSetIfChanged(ref _url, value);
		}

		public string RequestBody
		{
			get => _requestBodyText;
			set => this.RaiseAndSetIfChanged(ref _requestBodyText, value);
		}

		public string ResponseBody => _responseBody.Value;

		public ObservableCollectionExtended<Param> QueryParams { get; } =
			new ObservableCollectionExtended<Param>();

		public ObservableCollectionExtended<Param> RequestHeadersParams { get; }
			= new ObservableCollectionExtended<Param>();

		public ObservableCollectionExtended<Param> ResponseHeadersParams
		{
			get;
		} =
			new ObservableCollectionExtended<Param>();

		public MainWindowViewModel(MainWindow window)
		{
			_mainWindow = window;
			_isText = true;
			Log.Information("app start");
			UpdateState();
			SelectFileForBodyCommand =
				ReactiveCommand.CreateFromTask(SelectFileForBody);
			QuitProgramCommand =
				ReactiveCommand.Create(() => Environment.Exit(0));
			SaveFileCommand = ReactiveCommand.CreateFromTask(SaveFile);
			SendCommand = ReactiveCommand.CreateFromTask(SendRequest);
			ConvertDataCommand = ReactiveCommand.Create(ConvertData);
			AddEmptyParamCommand =
				ReactiveCommand.Create(() => AddEmptyParam(QueryParams));
			RemoveParamCommand =
				ReactiveCommand.Create<string>(s =>
					RemoveParam(s, QueryParams));
			RemoveHeaderCommand =
				ReactiveCommand.Create<string>(s =>
					RemoveParam(s, RequestHeadersParams));
			AddEmptyHeaderCommand =
				ReactiveCommand.Create(
					() => AddEmptyParam(RequestHeadersParams));
			ShowSettingCommand = ReactiveCommand.CreateFromTask(ShowSetting);
			CancelCommand = ReactiveCommand.Create(() => _source.Cancel());

			_responseBody =
				ConvertDataCommand.ToProperty(this, x => x.ResponseBody);

			var updateUrl =
				ReactiveCommand.Create<IReadOnlyCollection<Param>>(UpdateUrl);
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

			SendCommand.IsExecuting.Subscribe(e => IsSending = e,
				error => Log.Error($"Uh oh: {error}"));
			SendCommand.ThrownExceptions
				.Merge(updateUrl.ThrownExceptions)
				.Merge(updateParams.ThrownExceptions)
				.Subscribe(error => Log.Error($"Uh oh: {error}"));
			AddEmptyParamCommand.ThrownExceptions
				.Merge(updateUrl.ThrownExceptions)
				.Merge(updateParams.ThrownExceptions)
				.Subscribe(error => Log.Error($"Uh oh: {error}"));
			RemoveParamCommand.ThrownExceptions
				.Merge(updateUrl.ThrownExceptions)
				.Merge(updateParams.ThrownExceptions)
				.Subscribe(error => Log.Error($"Uh oh: {error}"));

			ShowSettingCommand.ThrownExceptions
				.Merge(updateUrl.ThrownExceptions)
				.Merge(updateParams.ThrownExceptions)
				.Subscribe(error => Log.Error($"Uh oh: {error}"));
			ConvertDataCommand.ThrownExceptions.Subscribe(e =>
				Log.Error(e.Message));
		}

		private void UpdateState()
		{
			if (!File.Exists(PathConstant.StatePath)) return;
			try
			{
				var json = File.ReadAllText(PathConstant.StatePath);
				var s = JsonConvert.DeserializeObject<Memento>(json);
				Url = s.Url;
				RequestHeadersParams.Clear();
				RequestHeadersParams.AddRange(s.Headers.Select(p => new Param
				{
					Key = p.Key,
					Value = p.Value
				}));
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
			}
		}

		private void UpdateParams(string url)
		{
			var qObj = HttpUtility.ParseQueryString(new Uri(_url).Query);
			QueryParams.Clear();
			QueryParams.AddRange(qObj.AllKeys.Select(key =>
				new Param {Key = key, Value = qObj[key]}));
		}

		private void UpdateUrl(IReadOnlyCollection<Param> headers)
		{
			var url = new Uri(Url);
			Url = QueryHelpers.AddQueryString(
				$"{url.Scheme}{Uri.SchemeDelimiter}{url.Authority}{url.AbsolutePath}",
				headers.Where(h =>
						!string.IsNullOrEmpty(h.Key) &&
						!string.IsNullOrEmpty(h.Value))
					.ToDictionary(param => param.Key,
						param => param.Value));
		}

		private async Task SendRequest()
		{
			var res = await Send();
			if (res is null) return;
			StatusCode = res.StatusCode;
			ResponseHeadersParams.Clear();
			ResponseHeadersParams.AddRange(res.Headers.Select(h => new Param
			{
				Key = h.Name,
				Value = h.Value
			}));
			var data = (await res.GetStringAsync()).EmptyIfNull();
			SaveContent();
			_data = data;
			ConvertDataCommand.Execute().Wait();
		}

		private async Task SaveContent()
		{
			var m = new Memento
			{
				Url = _url,
				Headers = RequestHeadersParams.Select(p => new Pair
				{
					Key = p.Key,
					Value = p.Value
				})
			};
			var data = JsonConvert.SerializeObject(m);
			await File.WriteAllTextAsync(PathConstant.StatePath, data);
		}

		private async Task ShowSetting()
		{
			var s = Program.ServiceProvider.GetService<Setting>();
			s.DataContext = Program.ServiceProvider
				.GetService<SettingWindowViewModel>();
			await s.ShowDialog(_mainWindow);
		}

		private async Task<IFlurlResponse> Send()
		{
			var method = SelectedRequestEnum switch
			{
				RequestEnum.GET => HttpMethod.Get,
				RequestEnum.POST => HttpMethod.Post,
				RequestEnum.PUT => HttpMethod.Put,
				RequestEnum.OPTIONS => HttpMethod.Options,
				RequestEnum.HEAD => HttpMethod.Head,
				RequestEnum.PATCH => HttpMethod.Patch,
				RequestEnum.DELETE => HttpMethod.Delete,
				RequestEnum.TRACE => HttpMethod.Trace,
				_ => null
			};

			Url url;
			try
			{
				url = new Url(_url.Trim());
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}

			foreach (var header in RequestHeadersParams.Where(h => h.IsUseful))
			{
				url.WithHeader(header.Key, header.Value);
			}

			_source = new CancellationTokenSource();
			return await url.AllowAnyHttpStatus()
				.SendAsync(method, GetData(), _source.Token);
		}

		private HttpContent GetData()
		{
			if (!IsFromFile)
				return string.IsNullOrEmpty(_requestBodyText)
					? null
					: new StringContent(_requestBodyText);
			if (!File.Exists(FileData))
				throw new FileNotFoundException($@"{FileData} not found");
			return new StreamContent(File.OpenRead(FileData));
		}

		private void AddEmptyParam(ICollection<Param> @params)
		{
			@params.Add(new Param());
		}

		private void RemoveParam(string key, ICollection<Param> @params)
		{
			@params.Remove(@params.First(p => p.Key == key));
		}

		private async Task SaveFile()
		{
			var dialog = new SaveFileDialog();
			var result = await dialog.ShowAsync(_mainWindow);
			if (result != null)
			{
				await File.WriteAllTextAsync(result, _responseBody.Value);
			}
		}

		private string ConvertData()
		{
			var s = new string(_data);
			if (IsXml)
			{
				BeautifyHelper.TryBeautifyXml(ref s);
				Highlighting = "XML";
				return s;
			}

			if (IsJson)
			{
				BeautifyHelper.TryBeautifyJson(ref s);
				Highlighting = "JavaScript";
				return s;
			}

			if (IsText)
			{
				
			}

			Highlighting = "Text";
			return s;
		}

		private async Task SelectFileForBody()
		{
			var dialog = new OpenFileDialog();
			var result = await dialog.ShowAsync(_mainWindow);
			if (result != null)
			{
				FileData = result.FirstOrDefault().EmptyIfNull();
			}
		}
	}
}