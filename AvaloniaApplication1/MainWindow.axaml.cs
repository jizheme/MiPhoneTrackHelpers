using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AvaloniaApplication1
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Content = new TrackPageView(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            window.Show(this);
        }

        private void MenuItem_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Content = new ConfigPageView(),
                CanResize = false,
                Width= 600,
                Height= 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            window.Show(this);
        }
    }
}