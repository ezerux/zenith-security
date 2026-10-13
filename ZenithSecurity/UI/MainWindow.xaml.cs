using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Web.WebView2.Core;
using ZenithSecurity.Security;

namespace ZenithSecurity.UI
{
    public partial class MainWindow : Window
    {
        private readonly SecurityBridge _bridge;
        private readonly DispatcherTimer _clock;

        public MainWindow()
        {
            InitializeComponent();
            _bridge = new SecurityBridge();

            _clock = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _clock.Tick += (_, _) =>
                TbTime.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
            _clock.Start();

            Loaded += async (_, _) => await InitWebViewAsync();
        }

        private async System.Threading.Tasks.Task InitWebViewAsync()
        {
            // Use temp folder for WebView2 user data
            var udp = Path.Combine(Path.GetTempPath(), "ZenithSecurityWebView");
            var env = await CoreWebView2Environment.CreateAsync(userDataFolder: udp);
            await WebView.EnsureCoreWebView2Async(env);

            WebView.CoreWebView2.Settings.IsStatusBarEnabled   = false;
            WebView.CoreWebView2.Settings.AreDevToolsEnabled   = false;
            WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            // Inject bridge — all API calls from JS come here
            WebView.CoreWebView2.WebMessageReceived += OnWebMessage;

            // Add host objects (alternative: use WebMessageReceived for all calls)
            WebView.CoreWebView2.AddHostObjectToScript("zenith", _bridge);

            // Load HTML from embedded resource
            var html = HtmlLoader.GetDashboardHtml();
            WebView.CoreWebView2.NavigateToString(html);

            TbStatus.Text = "● Running";
        }

        private async void OnWebMessage(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var msg = e.TryGetWebMessageAsString();
                if (string.IsNullOrEmpty(msg)) return;

                var req = JsonSerializer.Deserialize<ApiRequest>(msg,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (req == null) return;

                var result = await _bridge.HandleAsync(req.Endpoint, req.Payload);
                var json   = JsonSerializer.Serialize(new { id = req.Id, data = result });
                WebView.CoreWebView2.PostWebMessageAsString(json);
            }
            catch (Exception ex)
            {
                var err = JsonSerializer.Serialize(new { error = ex.Message });
                WebView.CoreWebView2.PostWebMessageAsString(err);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            WebView.CoreWebView2?.Reload();
        }

        protected override void OnClosed(EventArgs e)
        {
            _clock.Stop();
            _bridge.Dispose();
            base.OnClosed(e);
        }
    }

    public class ApiRequest
    {
        public string Id       { get; set; } = "";
        public string Endpoint { get; set; } = "";
        public JsonElement Payload { get; set; }
    }
}
