using System;
using System.Collections.Generic;
using System.IO;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AvaloniaApplication1;

public partial class HomePageView : UserControl
{
    private DispatcherTimer _reloadTimer;
    public HomePageView()
    {
        InitializeComponent();
        MainBrowWebView.Url = new Uri("https://i.mi.com");
        MainBrowWebView.NavigationCompleted += MainBrowWebView_NavigationCompleted; ;
        _reloadTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(100)
        };
        _reloadTimer.Tick += (sender, e) =>
        {
            if (MainBrowWebView != null)
            {
                MainBrowWebView.Reload();
            }
        };
    }

    private void MainBrowWebView_NavigationCompleted(object? sender, WebViewCore.Events.WebViewUrlLoadedEventArg e)
    {
        if (e.IsSuccess && sender != null)
        {
            Avalonia.WebView.Windows.Core.WebView2Core? web = sender as Avalonia.WebView.Windows.Core.WebView2Core;

            if (web?.CoreWebView2 == null)
                return;

            web.CoreWebView2.Settings.IsGeneralAutofillEnabled = true;
            web.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            web.CoreWebView2.Settings.IsScriptEnabled = true;

            web.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;

            _reloadTimer.Start();
        }
    }

    private async void CoreWebView2_WebResourceResponseReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceResponseReceivedEventArgs e)
    {
        try
        {
            if (e.Response != null && e.Response.StatusCode == 200)
            {
                
                if (e.Request.Uri.Contains("https://i.mi.com/find/v2/device/status/list?ts="))
                {
                    Stream content = await e.Response.GetContentAsync();
                    StreamReader sr = new StreamReader(content, System.Text.Encoding.UTF8);
                    string json = sr.ReadToEnd();

                    JObject? resObj = JObject.Parse(json);

                    var obj = resObj?.SelectToken("data");

                    var deviceList = obj?.SelectToken("deviceList") as JArray;

                    if (deviceList != null)
                    {
                        foreach (var device in deviceList)
                        {
                            string modelName = $"{device?.SelectToken("$.deviceNameAndImage.0.modelName.zhCN")}";
                            string phone = "";
                            if (!string.IsNullOrEmpty(modelName))
                            {
                                var parsedModel = JObject.Parse(modelName);
                                phone = $"{parsedModel.SelectToken("$.modelName")}";
                            }

                            var componentList = device?.SelectToken("componentList") as JArray;

                            if (componentList != null)
                            {
                                foreach (var component in componentList)
                                {
                                    var locationList = component.SelectToken("locationList") as JArray;
                                    var did = $"{component?.SelectToken("did")}";

                                    if (locationList != null)
                                    {
                                        foreach (var location in locationList)
                                        {
                                            var gpsInfoTransformed = location.SelectToken("gpsInfoTransformed") as JArray;

                                            long clientUpdateTime;
                                            long.TryParse($"{location?.SelectToken("clientUpdateTime")}", out clientUpdateTime);

                                            if (clientUpdateTime > 0)
                                            {
                                                var exists = SqlSugarHelpers.DbMaintenance().Queryable<LocationInfo>().Where(r => r.clientUpdateTime == clientUpdateTime).Any();

                                                System.DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds((long)clientUpdateTime).ToLocalTime().DateTime;

                                                if (gpsInfoTransformed != null && !exists)
                                                {
                                                    var olist = JsonConvert.SerializeObject(gpsInfoTransformed);

                                                    List<LocationInfo>? geos = JsonConvert.DeserializeObject<List<LocationInfo>>(olist);
                                                    if (geos != null)
                                                    {
                                                        foreach (var row in geos)
                                                        {
                                                            row.clientUpdateTime = clientUpdateTime;
                                                            row.phone = phone;
                                                            row.CreateTime = dateTime;
                                                            row.phoneID = did;
                                                            row.WebSource = "https://i.mi.com/";                                                          
                                                        }

                                                        SqlSugarHelpers.DbMaintenance().Storageable(geos).ExecuteCommand();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }               
            }
        }
        catch
        {

        }
    }
}