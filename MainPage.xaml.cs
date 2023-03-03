using CommunityToolkit.Mvvm.Input;
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
        this.BindingContext = this;

#if ANDROID
            btnFullScreen.IsVisible = true;
#elif IOS
        btnFullScreen.IsVisible = false;
#endif

        WeakReferenceMessenger.Default.Register<NotifyFullScreenClosed>(this, HandleOrientation);
    }

    //https://youtu.be/dQw4w9WgXcQ
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        deviceOrientationService = new DeviceOrientationService();
        deviceOrientationService.SetDeviceOrientation(displayOrientation: DisplayOrientation.Portrait);

        await Task.Run(async () =>
                            MediaSource = await CreateMediaStream(@"BlIv9l_EQMw"));

        await MainThread.InvokeOnMainThreadAsync((Action)(() => mediaElement.Source = MediaSource));
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

    private async Task<Uri> CreateMediaStream(string VideoId)
    {
        YoutubeClient youtube = new();
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(VideoId);
        var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

        if (streamInfo != null)
        {
            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
            return new Uri(streamInfo.Url);
        }
        else
            return null;
    }

    private async void btnFullScreen_Clicked(object sender, EventArgs e)
    {
        if (MediaSource != null)
        {
            FullScreenPage page = new FullScreenPage(new CurrentVideoState
            {
                Position = mediaElement.Position,
                VideoUri = MediaSource,
            });

            await MopupService.Instance.PushAsync(page);
        }
    }
}