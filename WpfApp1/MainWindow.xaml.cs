using System.IO;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using TwoCaptcha.Captcha;

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly WebView2 _webView = new();

    private readonly string _url =
        "https://allinonebizprd.b2clogin.com/allinonebizprd.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_signin_signup&client_id=95bad8fc-79af-4bde-95c1-93c69c388434&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fallinone.biz.com.br%2Fallinone&scope=openid&response_type=id_token&prompt=login";

    private readonly string _filePath = @"C:\Users\dev2\RiderProjects\WpfApp1\WpfApp1\";

    public MainWindow()
    {
        InitializeComponent();
        InitializeWebView();
    }


    private async void InitializeWebView()
    {
        try
        {
            Content = _webView; 

            try
            {
                await _webView.EnsureCoreWebView2Async();
                _webView.CoreWebView2.NavigationCompleted += WebView_NavigationCompleted!;
                _webView.Source = new Uri(_url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    private async void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        if (e.IsSuccess)
        {
            await GetCaptchaImage();
        }
        else
        {
            MessageBox.Show($"Error: {e.WebErrorStatus}");
        }
    }

    private async Task GetCaptchaImage()
    {
        if (_webView.CoreWebView2 != null)
        {
            try
            {
                string script = "document.getElementById('captchaControlChallengeCode-img').src;";
                string imageUrl = await _webView.CoreWebView2.ExecuteScriptAsync(script);

                imageUrl = imageUrl.Trim('"');

                await SaveBase64Image(imageUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }

    private async Task SaveBase64Image(string base64Image)
    {
        if (base64Image == "null" || string.IsNullOrEmpty(base64Image))
        {
            await GetCaptchaImage();
            return;
        }

        try
        {
            if (!base64Image.Contains("data:image"))
                return;

            string base64Data = base64Image.Split(',')[1];

            byte[] imageBytes = Convert.FromBase64String(base64Data);

            string fileName = $"{Guid.NewGuid()}.jpg";
            string filePath = _filePath + fileName;

            await File.WriteAllBytesAsync(filePath, imageBytes);

            while (!File.Exists(filePath))
            {
            }

            await GetCaptchaText(filePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }


    private async Task GetCaptchaText(string imageUrl)
    {
        var solver = new TwoCaptcha.TwoCaptcha("10a46e5aa54259554509bd92af9fd99d");
        Normal captcha = new Normal(imageUrl);
        try
        {
            await solver.Solve(captcha);
            MessageBox.Show("Captcha solved: " + captcha.Code);
            File.Delete(imageUrl);
        }
        catch (AggregateException e)
        {
            Console.WriteLine("Error occurred: " + e.InnerExceptions.First().Message);
        }
    }
}