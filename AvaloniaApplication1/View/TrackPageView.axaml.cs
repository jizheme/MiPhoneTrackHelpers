using System;
using System.Linq;
using System.Text;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication1;

public partial class TrackPageView : UserControl
{
    public TrackPageView()
    {
        InitializeComponent();
        MainTrackWebView.Url = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Assets\\baidumapview.html");
        LoadData();
    }

    private void LoadData()
    {
        DateTime time = DateTime.Now.AddHours(-ConfigManager.GetMapHoursLen());
        var phoneStrings = SqlSugarHelpers.DbMaintenance()
            .Queryable<LocationInfo>()
            .Where(a => a.accuracy <= ConfigManager.GetSelectAccuracy() && a.CreateTime >= time)
            .Select(it => new { it.phone, it.phoneID })
            .Distinct()
            .ToList();

        Listbox1.ItemsSource = phoneStrings
            .Select(phone => new PhoneListItemTemplate(phone.phone, phone.phoneID))
            .ToList();
    }

    private void StackPanel_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is not StackPanel rowContainer)
            return;

        if (rowContainer.DataContext is not PhoneListItemTemplate itemData)
            return;

        DateTime time = DateTime.Now.AddHours(-ConfigManager.GetMapHoursLen());
        var phone = itemData.name;
        var phoneID = itemData.phoneID;
        var task = SqlSugarHelpers.DbMaintenance().Queryable<LocationInfo>()
            .Where(a => a.phone == phone
            && a.phoneID == phoneID
            && a.accuracy <= ConfigManager.GetSelectAccuracy()
            && a.coordinateType == "baidu"
            && a.CreateTime >= time
            )
            .OrderByDescending(b => b.CreateTime)
            .ToList().Select(a => new
            {
                lat = a.latitude,
                lng = a.longitude,
                time = a.CreateTime
            }).ToList();
        if (task != null && task?.Count > 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("map.clearOverlays();");
            var topItem = task.First();

            builder.Append($"var point = new BMapGL.Point({topItem.lng},{topItem.lat});");
            builder.Append($"map.centerAndZoom(point, {ConfigManager.GetMapAccuracy()});");
            int i = 0;
            foreach (var item in task)
            {
                builder.Append($"var marker{i} = new BMapGL.Marker(new BMapGL.Point({item.lng},{item.lat}),{{enableDragging: true}});map.addOverlay(marker{i});var sContent{i} ='{item.time}';" +
                    $"var infoWindow{i} = new BMapGL.InfoWindow(sContent{i});" +
                    $"marker{i}.addEventListener('click', function () {{ this.openInfoWindow(infoWindow{i});}});");
                i++;
            }
            MainTrackWebView.ExecuteScriptAsync(builder.ToString());
        }
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }
}