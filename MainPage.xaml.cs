using CommunityToolkit.Mvvm.Messaging;
using MElemetModified.Services;
using Mopups.Services;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace MElemetModified;

public partial class MainPage : ContentPage
{

	public Uri? MediaSource { get; set; }
	DeviceOrientationService deviceOrientationService;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = this;
		deviceOrientationService = new();

#if ANDROID
			btnFullScreen.IsVisible = true;
#elif IOS
		btnFullScreen.IsVisible = false;
#endif

		WeakReferenceMessenger.Default.Register<NotifyFullScreenClosed>(this, HandleOrientation);
	}

	/*
	 * https://youtu.be/dQw4w9WgXcQ
	 * here, videoId => dQw4w9WgXcQ 
	*/
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		deviceOrientationService.SetDeviceOrientation(displayOrientation: DisplayOrientation.Portrait);

		await Task.Run(async () =>
							MediaSource = await CreateMediaStream(@"dQw4w9WgXcQ"));

		if (MediaSource != null) await MainThread.InvokeOnMainThreadAsync((Action)(() => mediaElement.Source = MediaSource));
	}

	private void HandleOrientation(object recipient, NotifyFullScreenClosed message)
	{
		deviceOrientationService = new DeviceOrientationService();
		deviceOrientationService.SetDeviceOrientation(displayOrientation: DisplayOrientation.Portrait);
	}

	protected override async void OnDisappearing()
	{
		base.OnDisappearing();
		await Task.Run(() => MediaSource = null);
	}

	static async Task<Uri?> CreateMediaStream(string VideoId)
	{
		YoutubeClient _client = new();

		var streamManifest = await _client.Videos.Streams.GetManifestAsync(VideoId);
		var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

		if (streamInfo != null) return new Uri(streamInfo.Url);
		else
			return null;
	}

	private async void btnFullScreen_Clicked(object sender, EventArgs e)
	{
		if (MediaSource != null)
		{
			FullScreenPage page = new(new CurrentVideoState
			{
				Position = mediaElement.Position,
				VideoUri = MediaSource,
			});

			await MopupService.Instance.PushAsync(page);
		}
	}
}