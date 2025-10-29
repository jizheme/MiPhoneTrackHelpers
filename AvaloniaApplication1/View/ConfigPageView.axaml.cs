using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace AvaloniaApplication1;

public partial class ConfigPageView : UserControl
{
    public ConfigPageView()
    {
        InitializeComponent();
        slider1.Value = ConfigManager.GetMapHoursLen();
        slider2.Value = ConfigManager.GetMapAccuracy();
        slider3.Value = ConfigManager.GetSelectAccuracy();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ConfigManager.SaveMapHoursLen((int)slider1.Value);
        ConfigManager.SaveMapAccuracy((int)slider2.Value);
        ConfigManager.SaveSelectAccuracy((int)slider3.Value);

        var window = this.FindAncestorOfType<Window>();
        window?.Close();
    }
}