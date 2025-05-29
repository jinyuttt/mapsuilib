using Avalonia.Controls;
using AvaloniaMapsuiLib;

namespace Map.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MapTool mapTool= this.FindControl<MapTool>("map");
        mapTool.InitMap();
    }
}