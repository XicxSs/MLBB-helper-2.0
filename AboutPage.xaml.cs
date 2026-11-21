using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace MlbbHelper;

public partial class AboutPage : ContentPage
{
    public AboutPage() => InitializeComponent();

    async void OnBackTapped(object? sender, EventArgs e) => await Navigation.PopAsync();

    static async Task OpenUrlAsync(string url)
    {
        try
        {
            await Launcher.Default.OpenAsync(url);
        }
        catch
        {
            // Справочник продолжает работать и без внешнего браузера.
        }
    }

    async void OnMlbbDexTapped(object? sender, EventArgs e) =>
        await OpenUrlAsync("https://mlbbdex.com");

    async void OnRoneTapped(object? sender, EventArgs e) =>
        await OpenUrlAsync("https://arena.rone.dev");

    async void OnWikiTapped(object? sender, EventArgs e) =>
        await OpenUrlAsync("https://mobilelegends.fandom.com/wiki/Mobile_Legends:_Bang_Bang");
}
