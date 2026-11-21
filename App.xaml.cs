using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace MlbbHelper;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var navigation = new NavigationPage(new MainPage())
        {
            BarBackgroundColor = Color.FromArgb("#08172B"),
            BarTextColor = Color.FromArgb("#EDF7FF")
        };

        return new Window(navigation);
    }
}
